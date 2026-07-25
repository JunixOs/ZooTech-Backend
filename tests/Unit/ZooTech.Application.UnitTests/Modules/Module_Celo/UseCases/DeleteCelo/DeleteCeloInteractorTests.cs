using NSubstitute;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Modules.Module_Celo.UseCases.DeleteCelo;
using ZooTech.Application.UnitTests.Modules.Module_Celo.UseCases;
using ZooTech.Domain.Module_Celo.Entities;
using ZooTech.Domain.Module_Celo.Interfaces;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.UnitTests.Modules.Module_Celo.UseCases.DeleteCelo;

public sealed class DeleteCeloInteractorTests
{
    private readonly ICeloRepository _repository;
    private readonly DeleteCeloInteractor _interactor;

    public DeleteCeloInteractorTests()
    {
        _repository = Substitute.For<ICeloRepository>();
        _interactor = new DeleteCeloInteractor(new FakeGanaderiaUnitOfWork(_repository));
    }

    [Fact]
    public async Task Handle_Should_SoftDelete_And_ReturnEmptyOutput_When_Celo_Exists()
    {
        var celo = CreateValidCelo(id: 1);
        var command = new DeleteCeloCommand { Id = 1, MotivoEliminacion = " Duplicado " };

        _repository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(celo);
        _repository.UpdateAsync(Arg.Any<Celo>(), Arg.Any<CancellationToken>())
            .Returns(c => c.Arg<Celo>());

        var output = await _interactor.Handle(command, CancellationToken.None);

        Assert.Equal(Application.Common.Models.EmptyOutput.Value, output);
        await _repository.Received(1).UpdateAsync(
            Arg.Is<Celo>(c => c.IsDeleted && c.MotivoEliminacion == "Duplicado"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ThrowNotFoundException_When_Celo_Does_Not_Exist()
    {
        var command = new DeleteCeloCommand { Id = 42, MotivoEliminacion = "Duplicado" };

        _repository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns((Celo?)null);

        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => _interactor.Handle(command, CancellationToken.None));

        Assert.Equal(ScopeName.Application, exception.ScopeName);
        Assert.Equal(ModuleName.Celo, exception.ModuleName);
        Assert.Contains("42", exception.Message);
        await _repository.DidNotReceive().UpdateAsync(Arg.Any<Celo>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ThrowInvalidOperationException_When_Celo_Already_Deleted()
    {
        var celo = CreateValidCelo(id: 1);
        celo.SoftDelete("Motivo previo", null, DateTime.UtcNow);
        var command = new DeleteCeloCommand { Id = 1, MotivoEliminacion = "Otro motivo" };

        _repository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(celo);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _interactor.Handle(command, CancellationToken.None));

        Assert.Equal("El registro de celo ya se encuentra eliminado.", exception.Message);
        await _repository.DidNotReceive().UpdateAsync(Arg.Any<Celo>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Handle_Should_ThrowArgumentException_When_MotivoEliminacion_Is_NullOrWhitespace(string? motivo)
    {
        var celo = CreateValidCelo(id: 1);
        var command = new DeleteCeloCommand { Id = 1, MotivoEliminacion = motivo };

        _repository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(celo);

        await Assert.ThrowsAsync<ArgumentException>(
            () => _interactor.Handle(command, CancellationToken.None));

        await _repository.DidNotReceive().UpdateAsync(Arg.Any<Celo>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ThrowArgumentException_When_MotivoEliminacion_Exceeds_MaxLength()
    {
        var celo = CreateValidCelo(id: 1);
        var command = new DeleteCeloCommand { Id = 1, MotivoEliminacion = new string('a', 201) };

        _repository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(celo);

        await Assert.ThrowsAsync<ArgumentException>(
            () => _interactor.Handle(command, CancellationToken.None));

        await _repository.DidNotReceive().UpdateAsync(Arg.Any<Celo>(), Arg.Any<CancellationToken>());
    }

    private static Celo CreateValidCelo(long id)
    {
        var celo = Celo.CreateNew(
            codigo: "CEL-001",
            fechaHora: DateTime.UtcNow.AddHours(-1),
            vacunoId: 1,
            encargadoUsuarioId: 2,
            observaciones: "Observación",
            estadoRegistroCode: "ACTIVO",
            caracteristicaCodes: new List<string> { "CALOR" },
            actorUsuarioId: 2,
            utcNow: DateTime.UtcNow.AddHours(-1));

        return Celo.Rehydrate(
            id: id,
            codigo: celo.Codigo,
            fechaHora: celo.FechaHora,
            vacunoId: celo.VacunoId,
            vacunoCodigo: "VAC-001",
            nombreVacuno: "Manchada",
            encargadoUsuarioId: celo.EncargadoUsuarioId,
            observaciones: celo.Observaciones,
            estadoRegistroCode: celo.EstadoRegistroCode,
            caracteristicaCodes: celo.CaracteristicaCodes.ToList(),
            createdAt: celo.CreatedAt,
            updatedAt: celo.UpdatedAt,
            deletedAt: null,
            motivoEliminacion: null,
            createdBy: celo.CreatedBy,
            updatedBy: celo.UpdatedBy,
            deletedBy: null);
    }
}
