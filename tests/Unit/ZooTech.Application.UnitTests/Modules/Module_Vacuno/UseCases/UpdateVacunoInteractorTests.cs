using FluentAssertions;
using Moq;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Modules.Module_Vacuno.Exceptions;
using ZooTech.Application.UnitTests.Modules.Module_Vacuno.Support;
using ZooTech.Domain.Configuration;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Entities;
using ZooTech.Tests.Shared.Factories;

namespace ZooTech.Application.UnitTests.Modules.Module_Vacuno.UseCases;

public sealed class UpdateVacunoInteractorTests
{
    private readonly VacunoMutationTestContext _context = new();

    [Fact]
    public async Task HandleAsync_WhenCommandIsValid_UpdatesEditableFieldsAndPreservesCodigo()
    {
        var existing = VacunoTestDataFactory.ExistingVacuno();
        var command = VacunoTestDataFactory.UpdateCommand() with { Nombre = "Nombre Editado" };
        _context.Repository.Setup(x => x.GetByIdAsync(existing.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);
        _context.Repository
            .Setup(x => x.UpdateAsync(It.IsAny<Vacuno>(), It.IsAny<decimal?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Vacuno vacuno, decimal? _, string? _, CancellationToken _) => vacuno);

        var result = await _context.UpdateInteractor().HandleAsync(command, CancellationToken.None);

        result.Data.Codigo.Should().Be(VacunoTestDataFactory.ExistingCodigo);
        result.Data.Nombre.Should().Be(command.Nombre);
        _context.UnitOfWork.Verify(x => x.ExecuteInTransactionAsync(
            It.IsAny<Func<CancellationToken, Task<Vacuno>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<Func<Vacuno, CancellationToken, Task<Vacuno>>?>()), Times.Once);
        _context.Cache.Verify(x => x.RemoveByPrefixAsync("vacunos:listar"), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WhenVacunoDoesNotExist_DoesNotStartTransaction()
    {
        var command = VacunoTestDataFactory.UpdateCommand(99);
        _context.Repository.Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Vacuno?)null);

        var act = () => _context.UpdateInteractor().HandleAsync(command, CancellationToken.None);

        await act.Should().ThrowAsync<VacunoNotFoundException>();
        _context.UnitOfWork.Verify(x => x.ExecuteInTransactionAsync(
            It.IsAny<Func<CancellationToken, Task<Vacuno>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<Func<Vacuno, CancellationToken, Task<Vacuno>>?>()), Times.Never);
        _context.Cache.Verify(x => x.RemoveByPrefixAsync(It.IsAny<string>()), Times.Never);
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public async Task HandleAsync_WhenImmutableFieldChanges_RejectsUpdate(
        bool changeFechaNacimiento,
        bool changeTipoAdquisicion)
    {
        var existing = VacunoTestDataFactory.ExistingVacuno();
        var command = VacunoTestDataFactory.UpdateCommand() with
        {
            FechaNacimiento = changeFechaNacimiento
                ? existing.FechaNacimiento.AddDays(1)
                : existing.FechaNacimiento,
            TipoAdquisicionCode = changeTipoAdquisicion
                ? "COMPRA"
                : existing.TipoAdquisicionCode
        };
        _context.Repository.Setup(x => x.GetByIdAsync(existing.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var act = () => _context.UpdateInteractor().HandleAsync(command, CancellationToken.None);

        await act.Should().ThrowAsync<ImmutableFieldException>();
        _context.UnitOfWork.Verify(x => x.ExecuteInTransactionAsync(
            It.IsAny<Func<CancellationToken, Task<Vacuno>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<Func<Vacuno, CancellationToken, Task<Vacuno>>?>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_WhenTenantObservationLimitIsExceeded_ReturnsStructuredValidationError()
    {
        var existing = VacunoTestDataFactory.ExistingVacuno();
        var command = VacunoTestDataFactory.UpdateCommand() with { Observaciones = new string('X', 11) };
        _context.Repository.Setup(x => x.GetByIdAsync(existing.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);
        _context.Settings.Setup(x => x.GetSettingAsync(Settings.Vacunos.VacunosObservacionesMaxLength))
            .ReturnsAsync(10);

        var act = () => _context.UpdateInteractor().HandleAsync(command, CancellationToken.None);

        var exception = await act.Should().ThrowAsync<ValidationException>();
        exception.Which.FieldErrors.Should().ContainSingle(error => error.Field == "observaciones");
        _context.Cache.Verify(x => x.RemoveByPrefixAsync(It.IsAny<string>()), Times.Never);
    }
}
