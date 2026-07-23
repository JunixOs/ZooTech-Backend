using FluentAssertions;
using Moq;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Modules.Module_Vacuno.Exceptions;
using ZooTech.Application.UnitTests.Modules.Module_Vacuno.Support;
using ZooTech.Domain.Configuration;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Entities;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Models;
using ZooTech.Tests.Shared.Factories;

namespace ZooTech.Application.UnitTests.Modules.Module_Vacuno.UseCases;

public sealed class CreateVacunoInteractorTests
{
    private readonly VacunoMutationTestContext _context = new();

    [Fact]
    public async Task HandleAsync_WhenCommandIsValid_PersistsInsideTransactionAndClearsListCache()
    {
        var command = VacunoTestDataFactory.CreateCommand();
        _context.Repository.Setup(x => x.ExistsCodigoAsync(command.Codigo, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _context.Repository
            .Setup(x => x.AddAsync(
                It.IsAny<Vacuno>(),
                It.IsAny<decimal?>(),
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<DateOnly?>(),
                It.IsAny<VacunoPhotoMetadata?>()))
            .ReturnsAsync((
                Vacuno vacuno,
                decimal? _,
                string? _,
                CancellationToken _,
                DateOnly? _,
                VacunoPhotoMetadata? _) =>
                VacunoTestDataFactory.PersistedFrom(vacuno, 10));

        var result = await _context.CreateInteractor().HandleAsync(command, CancellationToken.None);

        result.Data.Id.Should().Be(10);
        result.Data.Codigo.Should().Be(command.Codigo);
        _context.UnitOfWork.Verify(x => x.ExecuteInTransactionAsync(
            It.IsAny<Func<CancellationToken, Task<Vacuno>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<Func<Vacuno, CancellationToken, Task<Vacuno>>?>()), Times.Once);
        _context.Repository.Verify(x => x.AddAsync(
            It.Is<Vacuno>(v => v.Codigo == command.Codigo),
            command.PrecioCompra,
            command.AptoPara,
            It.IsAny<CancellationToken>(),
            command.FechaEspecificacion,
            null), Times.Once);
        _context.Cache.Verify(x => x.RemoveByPrefixAsync("vacunos:listar"), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WhenCodigoAlreadyExists_RejectsBeforeTransaction()
    {
        var command = VacunoTestDataFactory.CreateCommand();
        _context.Repository.Setup(x => x.ExistsCodigoAsync(command.Codigo, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var act = () => _context.CreateInteractor().HandleAsync(command, CancellationToken.None);

        await act.Should().ThrowAsync<VacunoAlreadyExistsException>();
        _context.UnitOfWork.Verify(x => x.ExecuteInTransactionAsync(
            It.IsAny<Func<CancellationToken, Task<Vacuno>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<Func<Vacuno, CancellationToken, Task<Vacuno>>?>()), Times.Never);
        _context.Cache.Verify(x => x.RemoveByPrefixAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_WhenTenantCodigoLimitIsExceeded_ReturnsStructuredValidationError()
    {
        var command = VacunoTestDataFactory.CreateCommand() with { Codigo = new string('A', 16) };
        _context.Settings.Setup(x => x.GetSettingAsync(Settings.Vacunos.VacunosCodigoMaxLength))
            .ReturnsAsync(15);

        var act = () => _context.CreateInteractor().HandleAsync(command, CancellationToken.None);

        var exception = await act.Should().ThrowAsync<ValidationException>();
        exception.Which.FieldErrors.Should().ContainSingle(error => error.Field == "codigo");
        _context.Repository.Verify(x => x.ExistsCodigoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
