using ZooTech.Application.UnitTests.Modules.Module_ProduccionLeche.UseCases;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.UpdateOrdenio;
using ZooTech.Domain.Module_ProduccionLeche.Entities;
using ZooTech.Domain.Module_ProduccionLeche.Interfaces;
using ZooTech.Domain.Module_Sanidad.Interfaces;
using ZooTech.Domain.Shared.Interfaces;

namespace ZooTech.Application.UnitTests.Modules.Module_ProduccionLeche.UseCases.UpdateOrdenio;

public class UpdateOrdenioInteractorTests
{
    [Fact]
    public async Task HandleAsync_WhenValidCommandIsSent_UpdatesOrdenio()
    {
        var fecha = new DateTime(2026, 6, 18, 8, 0, 0, DateTimeKind.Local);
        var existing = Ordenio.Rehydrate(
            id: 10,
            codigo: "ORD-010",
            fechaHora: fecha,
            vacunoId: 1,
            nombreVacuno: "Luna",
            encargadoUsuarioId: 2,
            nombreCompleto: "Juan Perez",
            litros: 12,
            estadoOrdenioCode: "ACTIVO",
            observaciones: "Inicial",
            createdAt: fecha,
            updatedAt: fecha,
            deletedAt: null,
            motivoEliminacion: null,
            createdBy: 2,
            updatedBy: 2,
            deletedBy: null);

        var repository = new FakeOrdenioRepository(existing)
        {
            ExistsVacunoResult = true,
            ExistsUsuarioResult = true,
            ExistsEstadoResult = true
        };
        var interactor = new UpdateOrdenioInteractor(new FakeGanaderiaUnitOfWork(repository));
        var nuevaFecha = fecha.AddHours(2);

        var command = new UpdateOrdenioCommand
        {
            Id = 10,
            FechaHora = nuevaFecha,
            EncargadoUsuarioId = 5,
            Litros = 18,
            EstadoOrdenioCode = "FINALIZADO",
            Observaciones = "Actualizado"
        };

        var result = await interactor.Handle(command, CancellationToken.None);

        Assert.Equal(18, result.Data.Litros);
        Assert.Equal(nuevaFecha, result.Data.FechaHora);
        Assert.Equal(5, result.Data.EncargadoUsuarioId);
        Assert.Equal("FINALIZADO", result.Data.EstadoOrdenioCode);
        Assert.Equal("Actualizado", result.Data.Observaciones);
    }

    private sealed class FakeOrdenioRepository : IOrdenioRepository
    {
        private readonly Ordenio _existing;

        public FakeOrdenioRepository(Ordenio existing)
        {
            _existing = existing;
        }

        public bool ExistsCodigoResult { get; set; }
        public bool ExistsVacunoFechaResult { get; set; }
        public bool ExistsVacunoResult { get; set; }
        public bool ExistsUsuarioResult { get; set; }
        public bool ExistsEstadoResult { get; set; }

        public Task<bool> HasActiveRecordsByVacunoAsync(long vacunoId, CancellationToken cancellationToken) => Task.FromResult(false);

        public Task<bool> ExistsCodigoAsync(string codigo, CancellationToken cancellationToken)
            => Task.FromResult(ExistsCodigoResult);

        public Task<bool> ExistsVacunoFechaAsync(long vacunoId, DateTime fechaHora, long? excludeOrdenioId, CancellationToken cancellationToken)
            => Task.FromResult(ExistsVacunoFechaResult);

        public Task<bool> ExistsVacunoAsync(long vacunoId, CancellationToken cancellationToken)
            => Task.FromResult(ExistsVacunoResult);

        public Task<bool> ExistsUsuarioAsync(long usuarioId, CancellationToken cancellationToken)
            => Task.FromResult(ExistsUsuarioResult);

        public Task<bool> ExistsEstadoAsync(string estadoOrdenioCode, CancellationToken cancellationToken)
            => Task.FromResult(ExistsEstadoResult);

        public Task<Ordenio?> GetByIdAsync(long id, CancellationToken cancellationToken)
            => Task.FromResult<Ordenio?>(_existing);

        public Task<Ordenio?> GetByCodigoAsync(string codigo, CancellationToken cancellationToken)
            => Task.FromResult<Ordenio?>(_existing);

        public Task<(IReadOnlyList<OrdenioList> Items, int TotalCount)> ListAsync(
            long? vacunoId,
            string? estadoOrdenioCode,
            DateTime? fechaDesde,
            DateTime? fechaHasta,
            int page,
            int pageSize,
            CancellationToken cancellationToken)
            => Task.FromResult<(IReadOnlyList<OrdenioList> Items, int TotalCount)>((Array.Empty<OrdenioList>(), 0));

        public Task<IReadOnlyList<OrdenioList>> ListReportAsync(
            long? vacunoId,
            string? estadoOrdenioCode,
            DateTime? fechaDesde,
            DateTime? fechaHasta,
            CancellationToken cancellationToken)
            => Task.FromResult<IReadOnlyList<OrdenioList>>(Array.Empty<OrdenioList>());

        public Task<Ordenio> AddAsync(Ordenio ordenio, CancellationToken cancellationToken)
            => Task.FromResult(ordenio);

        public Task<Ordenio> UpdateAsync(Ordenio ordenio, CancellationToken cancellationToken)
            => Task.FromResult(ordenio);
    }

    private sealed class FakeOrdenioUnitOfWork : IGanaderiaUnitOfWork
    {
        public FakeOrdenioUnitOfWork(IOrdenioRepository repository)
        {
            Ordenios = repository;
        }

        public IOrdenioRepository Ordenios { get; }
        public ZooTech.Domain.Ganaderia.Module_Vacuno.Interfaces.IVacunoRepository Vacunos => throw new NotSupportedException();
        public ZooTech.Domain.Ganaderia.Module_Fecundacion.Interfaces.IFecundacionRepository Fecundaciones => throw new NotSupportedException();
        public ITriajeRepository Triajes => throw new NotSupportedException();

        public Task<T> ExecuteInTransactionAsync<T>(
            Func<CancellationToken, Task<T>> operation,
            CancellationToken cancellationToken = default,
            Func<T, CancellationToken, Task<T>>? afterSave = null)
            => ExecuteAsync(operation, cancellationToken, afterSave);

        private static async Task<T> ExecuteAsync<T>(
            Func<CancellationToken, Task<T>> operation,
            CancellationToken cancellationToken,
            Func<T, CancellationToken, Task<T>>? afterSave)
        {
            var result = await operation(cancellationToken);
            return afterSave is null ? result : await afterSave(result, cancellationToken);
        }
    }
}

