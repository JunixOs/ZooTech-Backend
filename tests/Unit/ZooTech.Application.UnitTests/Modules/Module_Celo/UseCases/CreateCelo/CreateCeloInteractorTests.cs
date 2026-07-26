using NSubstitute;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Modules.Module_Celo.UseCases.CreateCelo;
using ZooTech.Domain.Module_Celo.Entities;
using ZooTech.Domain.Module_Celo.Interfaces;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.UnitTests.Modules.Module_Celo.UseCases.CreateCelo;

public sealed class CreateCeloInteractorTests
{
    private readonly ICeloRepository _repository;
    private readonly CreateCeloInteractor _interactor;

    public CreateCeloInteractorTests()
    {
        _repository = Substitute.For<ICeloRepository>();
        _interactor = new CreateCeloInteractor(_repository);
    }

    [Fact]
    public async Task HandleAsync_Should_Create_Celo_When_Vacuno_Exists()
    {
        var command = new CreateCeloCommand
        {
            VacunoId = 1,
            EncargadoUsuarioId = 2,
            FechaHora = DateTime.UtcNow.AddHours(-1),
            Observaciones = "Observación de prueba",
            CaracteristicaCodes = new List<string> { "CALOR", "MOUNT" }
        };

        _repository.ExistsVacunoAsync(command.VacunoId, Arg.Any<CancellationToken>())
            .Returns(true);
        _repository.ExistsCodigoAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(false);
        _repository.AddAsync(Arg.Any<Celo>(), Arg.Any<CancellationToken>())
            .Returns(c => BuildRehydratedCelo(c.Arg<Celo>(), id: 1));

        var output = await _interactor.HandleAsync(command, CancellationToken.None);

        Assert.Equal(1, output.Id);
        Assert.StartsWith("C", output.Codigo);
        Assert.Equal(command.FechaHora, output.FechaHora);
        await _repository.Received(1).AddAsync(Arg.Any<Celo>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_Should_Create_Celo_Without_Observaciones_When_Not_Provided()
    {
        var command = new CreateCeloCommand
        {
            VacunoId = 1,
            EncargadoUsuarioId = 2,
            FechaHora = DateTime.UtcNow.AddHours(-1),
            CaracteristicaCodes = new List<string> { "CALOR" }
        };

        _repository.ExistsVacunoAsync(command.VacunoId, Arg.Any<CancellationToken>())
            .Returns(true);
        _repository.ExistsCodigoAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(false);
        _repository.AddAsync(Arg.Any<Celo>(), Arg.Any<CancellationToken>())
            .Returns(c => BuildRehydratedCelo(c.Arg<Celo>(), id: 1));

        var output = await _interactor.HandleAsync(command, CancellationToken.None);

        Assert.NotNull(output);
        await _repository.Received(1).AddAsync(Arg.Any<Celo>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_Should_Throw_ConflictException_When_Vacuno_Does_Not_Exist()
    {
        var command = new CreateCeloCommand
        {
            VacunoId = 99,
            EncargadoUsuarioId = 2,
            FechaHora = DateTime.UtcNow.AddHours(-1),
            CaracteristicaCodes = new List<string> { "CALOR" }
        };

        _repository.ExistsVacunoAsync(command.VacunoId, Arg.Any<CancellationToken>())
            .Returns(false);

        var exception = await Assert.ThrowsAsync<ConflictException>(
            () => _interactor.HandleAsync(command, CancellationToken.None));

        Assert.Equal(ScopeName.Application, exception.ScopeName);
        Assert.Equal(ModuleName.Celo, exception.ModuleName);
        Assert.Contains("CELO-VACUNO-ID-NOT_EXISTS", exception.Details);
        await _repository.DidNotReceive().AddAsync(Arg.Any<Celo>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_Should_Throw_ArgumentException_When_FechaHora_Is_Future()
    {
        var command = new CreateCeloCommand
        {
            VacunoId = 1,
            EncargadoUsuarioId = 2,
            FechaHora = DateTime.UtcNow.AddHours(1),
            CaracteristicaCodes = new List<string> { "CALOR" }
        };

        _repository.ExistsVacunoAsync(command.VacunoId, Arg.Any<CancellationToken>())
            .Returns(true);

        await Assert.ThrowsAsync<ArgumentException>(
            () => _interactor.HandleAsync(command, CancellationToken.None));

        await _repository.DidNotReceive().AddAsync(Arg.Any<Celo>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_Should_Throw_ArgumentException_When_Observaciones_Exceed_MaxLength()
    {
        var command = new CreateCeloCommand
        {
            VacunoId = 1,
            EncargadoUsuarioId = 2,
            FechaHora = DateTime.UtcNow.AddHours(-1),
            Observaciones = new string('a', 151),
            CaracteristicaCodes = new List<string> { "CALOR" }
        };

        _repository.ExistsVacunoAsync(command.VacunoId, Arg.Any<CancellationToken>())
            .Returns(true);

        await Assert.ThrowsAsync<ArgumentException>(
            () => _interactor.HandleAsync(command, CancellationToken.None));

        await _repository.DidNotReceive().AddAsync(Arg.Any<Celo>(), Arg.Any<CancellationToken>());
    }

    private static Celo BuildRehydratedCelo(Celo celo, long id)
    {
        return Celo.Rehydrate(
            id: id,
            codigo: celo.Codigo,
            fechaHora: celo.FechaHora,
            vacunoId: celo.VacunoId,
            vacunoCodigo: string.Empty,
            nombreVacuno: string.Empty,
            encargadoUsuarioId: celo.EncargadoUsuarioId,
            observaciones: celo.Observaciones,
            estadoRegistroCode: celo.EstadoRegistroCode,
            caracteristicaCodes: celo.CaracteristicaCodes.ToList(),
            createdAt: DateTime.UtcNow,
            updatedAt: DateTime.UtcNow,
            deletedAt: null,
            motivoEliminacion: null,
            createdBy: celo.CreatedBy,
            updatedBy: celo.UpdatedBy,
            deletedBy: null);
    }
}
