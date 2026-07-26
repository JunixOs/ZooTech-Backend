using NSubstitute;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Modules.Module_Celo.UseCases.UpdateCelo;
using ZooTech.Domain.Module_Celo.Entities;
using ZooTech.Domain.Module_Celo.Interfaces;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.UnitTests.Modules.Module_Celo.UseCases.UpdateCelo;

public sealed class UpdateCeloInteractorTests
{
    private readonly ICeloRepository _repository;
    private readonly UpdateCeloInteractor _interactor;

    public UpdateCeloInteractorTests()
    {
        _repository = Substitute.For<ICeloRepository>();
        _interactor = new UpdateCeloInteractor(_repository);
    }

    [Fact]
    public async Task HandleAsync_Should_Update_Celo_When_Exists()
    {
        var celo = CreateExistingCelo();
        var command = new UpdateCeloCommand
        {
            Id = celo.Id,
            Observaciones = "Nueva observación",
            CaracteristicaCodes = new List<string> { "NUEVA_CARACTERISTICA" }
        };

        _repository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(celo);
        _repository.UpdateAsync(Arg.Any<Celo>(), Arg.Any<CancellationToken>())
            .Returns(c => c.Arg<Celo>());

        var output = await _interactor.HandleAsync(command, CancellationToken.None);

        Assert.Equal(celo.Id, output.Id);
        Assert.Equal("Nueva observación", output.Observaciones);
        Assert.Single(celo.CaracteristicaCodes);
        Assert.Contains("NUEVA_CARACTERISTICA", celo.CaracteristicaCodes);
        await _repository.Received(1).UpdateAsync(Arg.Any<Celo>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_Should_Update_Only_Observaciones()
    {
        var celo = CreateExistingCelo();
        celo.CaracteristicaCodes.Add("EXISTENTE");
        var command = new UpdateCeloCommand
        {
            Id = celo.Id,
            Observaciones = "Solo observaciones",
            CaracteristicaCodes = new List<string> { "EXISTENTE" }
        };

        _repository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(celo);
        _repository.UpdateAsync(Arg.Any<Celo>(), Arg.Any<CancellationToken>())
            .Returns(c => c.Arg<Celo>());

        var output = await _interactor.HandleAsync(command, CancellationToken.None);

        Assert.Equal("Solo observaciones", output.Observaciones);
        await _repository.Received(1).UpdateAsync(Arg.Any<Celo>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_Should_Clear_Caracteristicas_When_Empty_List()
    {
        var celo = CreateExistingCelo();
        celo.CaracteristicaCodes.Add("CARACTERISTICA_VIEJA");
        var command = new UpdateCeloCommand
        {
            Id = celo.Id,
            CaracteristicaCodes = new List<string>()
        };

        _repository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(celo);
        _repository.UpdateAsync(Arg.Any<Celo>(), Arg.Any<CancellationToken>())
            .Returns(c => c.Arg<Celo>());

        await _interactor.HandleAsync(command, CancellationToken.None);

        Assert.Empty(celo.CaracteristicaCodes);
        await _repository.Received(1).UpdateAsync(Arg.Any<Celo>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_Should_Throw_NotFoundException_When_Celo_Does_Not_Exist()
    {
        var command = new UpdateCeloCommand
        {
            Id = 99,
            Observaciones = "Observación"
        };

        _repository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns((Celo?)null);

        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => _interactor.HandleAsync(command, CancellationToken.None));

        Assert.Equal(ScopeName.Application, exception.ScopeName);
        Assert.Equal(ModuleName.Celo, exception.ModuleName);
        Assert.Contains("99", exception.Message);
        await _repository.DidNotReceive().UpdateAsync(Arg.Any<Celo>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_Should_Throw_ArgumentException_When_Observaciones_Exceed_MaxLength()
    {
        var celo = CreateExistingCelo();
        var command = new UpdateCeloCommand
        {
            Id = celo.Id,
            Observaciones = new string('a', 151)
        };

        _repository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(celo);

        await Assert.ThrowsAsync<ArgumentException>(
            () => _interactor.HandleAsync(command, CancellationToken.None));

        await _repository.DidNotReceive().UpdateAsync(Arg.Any<Celo>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_Should_Throw_InvalidOperationException_When_Celo_Is_Deleted()
    {
        var celo = CreateDeletedCelo();
        var command = new UpdateCeloCommand
        {
            Id = celo.Id,
            Observaciones = "Intento de editar eliminado"
        };

        _repository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(celo);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _interactor.HandleAsync(command, CancellationToken.None));

        await _repository.DidNotReceive().UpdateAsync(Arg.Any<Celo>(), Arg.Any<CancellationToken>());
    }

    private static Celo CreateExistingCelo()
    {
        return Celo.Rehydrate(
            id: 1,
            codigo: "C260722103000",
            fechaHora: DateTime.UtcNow.AddHours(-1),
            vacunoId: 1,
            vacunoCodigo: "V001",
            nombreVacuno: "Vacuna 1",
            encargadoUsuarioId: 2,
            observaciones: "Observación original",
            estadoRegistroCode: "ACTIVO",
            caracteristicaCodes: new List<string>(),
            createdAt: DateTime.UtcNow.AddDays(-1),
            updatedAt: DateTime.UtcNow.AddDays(-1),
            deletedAt: null,
            motivoEliminacion: null,
            createdBy: 2,
            updatedBy: 2,
            deletedBy: null);
    }

    private static Celo CreateDeletedCelo()
    {
        return Celo.Rehydrate(
            id: 2,
            codigo: "C260722103001",
            fechaHora: DateTime.UtcNow.AddHours(-1),
            vacunoId: 1,
            vacunoCodigo: "V001",
            nombreVacuno: "Vacuna 1",
            encargadoUsuarioId: 2,
            observaciones: "Observación original",
            estadoRegistroCode: "ACTIVO",
            caracteristicaCodes: new List<string>(),
            createdAt: DateTime.UtcNow.AddDays(-1),
            updatedAt: DateTime.UtcNow.AddDays(-1),
            deletedAt: DateTime.UtcNow,
            motivoEliminacion: "Eliminado por prueba",
            createdBy: 2,
            updatedBy: 2,
            deletedBy: 2);
    }
}
