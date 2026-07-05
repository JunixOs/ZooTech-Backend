using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.Common;
using ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.GetFecundacionEstado;
using ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.UpdateFecundacionEstado;

namespace ZooTech.Application.UnitTests.Modules.Module_Celo.UseCases.FecundacionEstado;

public class FecundacionEstadoInteractorTests
{
    [Fact]
    public async Task GetEstado_WhenVacunoHasNoFecundacion_ShouldReturnSinEstado()
    {
        var repository = new FakeFecundacionEstadoRepository
        {
            Snapshot = CreateSnapshot(
                estadoActual: FecundacionEstadoConstants.SinEstado,
                disponibleNuevaFecundacion: true,
                codigoFecundacion: null)
        };
        var interactor = new GetFecundacionEstadoInteractor(repository);

        var output = await interactor.HandleAsync(new GetFecundacionEstadoCommand(1));

        Assert.Equal(FecundacionEstadoConstants.SinEstado, output.EstadoActual);
        Assert.True(output.DisponibleNuevaFecundacion);
        Assert.Null(output.CodigoFecundacion);
    }

    [Fact]
    public async Task GetEstado_WhenVacunoDoesNotExist_ShouldThrowNotFound()
    {
        var interactor = new GetFecundacionEstadoInteractor(new FakeFecundacionEstadoRepository());

        await Assert.ThrowsAsync<NotFoundException>(() =>
            interactor.HandleAsync(new GetFecundacionEstadoCommand(99)));
    }

    [Fact]
    public async Task GetEstado_WhenVacunoIsNotFemale_ShouldThrowConflict()
    {
        var repository = new FakeFecundacionEstadoRepository
        {
            Snapshot = CreateSnapshot(esHembra: false)
        };
        var interactor = new GetFecundacionEstadoInteractor(repository);

        await Assert.ThrowsAsync<ConflictException>(() =>
            interactor.HandleAsync(new GetFecundacionEstadoCommand(1)));
    }

    [Fact]
    public async Task UpdateEstado_WhenTransitionIsAllowed_ShouldUpdateAndReturnNewState()
    {
        var repository = new FakeFecundacionEstadoRepository
        {
            Snapshot = CreateSnapshot(estadoActual: FecundacionEstadoConstants.Pendiente),
            EstadoCode = "PROC"
        };
        var interactor = CreateUpdateInteractor(repository);

        var output = await interactor.HandleAsync(new UpdateFecundacionEstadoCommand(
            10,
            " En proceso ",
            7));

        Assert.True(repository.Updated);
        Assert.Equal(10, repository.UpdatedFecundacionId);
        Assert.Equal("PROC", repository.UpdatedEstadoCode);
        Assert.Equal(7, repository.UpdatedBy);
        Assert.Equal(FecundacionEstadoConstants.EnProceso, output.EstadoActual);
    }

    [Fact]
    public async Task UpdateEstado_WhenTransitionIsNotAllowed_ShouldThrowValidation()
    {
        var repository = new FakeFecundacionEstadoRepository
        {
            Snapshot = CreateSnapshot(estadoActual: FecundacionEstadoConstants.Fallida),
            EstadoCode = "CONF"
        };
        var interactor = CreateUpdateInteractor(repository);

        var exception = await Assert.ThrowsAsync<FecundacionEstadoValidationException>(() =>
            interactor.HandleAsync(new UpdateFecundacionEstadoCommand(
                10,
                FecundacionEstadoConstants.Confirmada,
                7)));

        Assert.Contains("estadoFecundacion", exception.Errors.Keys);
        Assert.False(repository.Updated);
    }

    [Fact]
    public async Task UpdateEstado_WhenUpdatedByIsMissing_ShouldThrowValidation()
    {
        var interactor = CreateUpdateInteractor(new FakeFecundacionEstadoRepository());

        var exception = await Assert.ThrowsAsync<FecundacionEstadoValidationException>(() =>
            interactor.HandleAsync(new UpdateFecundacionEstadoCommand(
                10,
                FecundacionEstadoConstants.EnProceso,
                null)));

        Assert.Contains("updatedBy", exception.Errors.Keys);
    }

    [Fact]
    public async Task UpdateEstado_WhenOtherActiveFecundacionExists_ShouldThrowConflict()
    {
        var repository = new FakeFecundacionEstadoRepository
        {
            Snapshot = CreateSnapshot(estadoActual: FecundacionEstadoConstants.Pendiente),
            HasOtherActiveFecundacion = true
        };
        var interactor = CreateUpdateInteractor(repository);

        await Assert.ThrowsAsync<ConflictException>(() =>
            interactor.HandleAsync(new UpdateFecundacionEstadoCommand(
                10,
                FecundacionEstadoConstants.EnProceso,
                7)));

        Assert.False(repository.Updated);
    }

    private static UpdateFecundacionEstadoInteractor CreateUpdateInteractor(
        FakeFecundacionEstadoRepository repository)
    {
        return new UpdateFecundacionEstadoInteractor(
            repository,
            new UpdateFecundacionEstadoValidator(),
            new FecundacionEstadoTransitionValidator());
    }

    private static FecundacionEstadoSnapshot CreateSnapshot(
        string estadoActual = FecundacionEstadoConstants.Pendiente,
        bool disponibleNuevaFecundacion = false,
        string? codigoFecundacion = "F001",
        bool esHembra = true)
    {
        return new FecundacionEstadoSnapshot(
            1,
            codigoFecundacion is null ? null : 10,
            "V001",
            "Lola",
            estadoActual,
            disponibleNuevaFecundacion,
            new DateTime(2026, 1, 1, 10, 0, 0),
            codigoFecundacion,
            codigoFecundacion is null ? null : "Inseminacion artificial",
            codigoFecundacion is null ? null : "Toro A",
            codigoFecundacion is null ? null : "Dr. Rivera",
            codigoFecundacion is null ? null : new DateOnly(2026, 1, 1),
            codigoFecundacion is null ? null : "Pendiente",
            codigoFecundacion is null ? null : "Sin observaciones",
            esHembra);
    }

    private sealed class FakeFecundacionEstadoRepository : IFecundacionEstadoRepository
    {
        public FecundacionEstadoSnapshot? Snapshot { get; set; }
        public string? EstadoCode { get; set; }
        public bool HasOtherActiveFecundacion { get; set; }
        public bool Updated { get; private set; }
        public long UpdatedFecundacionId { get; private set; }
        public string? UpdatedEstadoCode { get; private set; }
        public long UpdatedBy { get; private set; }

        public Task<FecundacionEstadoSnapshot?> GetByVacunoIdAsync(
            long vacunoId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Snapshot);
        }

        public Task<FecundacionEstadoSnapshot?> GetByFecundacionIdAsync(
            long fecundacionId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Snapshot);
        }

        public Task<string?> GetEstadoCodeByNameAsync(
            string estado,
            CancellationToken cancellationToken = default)
        {
            if (estado == FecundacionEstadoConstants.EnProceso && Snapshot is not null)
            {
                Snapshot = Snapshot with
                {
                    EstadoActual = FecundacionEstadoConstants.EnProceso,
                    DisponibleNuevaFecundacion = false
                };
            }

            return Task.FromResult(EstadoCode);
        }

        public Task<bool> HasOtherActiveFecundacionAsync(
            long vacunoId,
            long fecundacionId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(HasOtherActiveFecundacion);
        }

        public Task UpdateEstadoAsync(
            long fecundacionId,
            string estadoCode,
            long updatedBy,
            CancellationToken cancellationToken = default)
        {
            Updated = true;
            UpdatedFecundacionId = fecundacionId;
            UpdatedEstadoCode = estadoCode;
            UpdatedBy = updatedBy;
            return Task.CompletedTask;
        }
    }
}
