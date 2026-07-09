using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.DeleteOrdenio;
using ZooTech.Domain.Module_ProduccionLeche.Entities;
using ZooTech.Domain.Module_ProduccionLeche.Interfaces;

namespace ZooTech.Application.UnitTests.Modules.Module_ProduccionLeche.UseCases.DeleteOrdenio;

public class DeleteOrdenioInteractorTests
{
    [Fact]
    public async Task HandleAsync_WhenOrdenioExists_SoftDeletesOrdenio()
    {
        var fecha = new DateTime(2026, 6, 18, 8, 0, 0, DateTimeKind.Utc);
        var existing = CreateOrdenio(fecha);
        var repository = new FakeOrdenioRepository(existing);
        var interactor = new DeleteOrdenioInteractor(new FakeOrdenioUnitOfWork(repository));

        await interactor.Handle(new DeleteOrdenioCommand { Id = 10, MotivoEliminacion = "Registro duplicado" }, CancellationToken.None);

        Assert.True(repository.UpdateWasCalled);
        Assert.True(existing.IsDeleted);
        Assert.Equal("Registro duplicado", existing.MotivoEliminacion);
    }

    [Fact]
    public async Task HandleAsync_WhenOrdenioDoesNotExist_ThrowsNotFoundException()
    {
        var repository = new FakeOrdenioRepository(null);
        var interactor = new DeleteOrdenioInteractor(new FakeOrdenioUnitOfWork(repository));

        await Assert.ThrowsAsync<NotFoundException>(() => interactor.Handle(new DeleteOrdenioCommand { Id = 99, MotivoEliminacion = "Motivo" }, CancellationToken.None));
    }

    private static Ordenio CreateOrdenio(DateTime fecha)
        => Ordenio.Rehydrate(
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

    private sealed class FakeOrdenioRepository : IOrdenioRepository
    {
        private readonly Ordenio? _ordenio;

        public FakeOrdenioRepository(Ordenio? ordenio)
        {
            _ordenio = ordenio;
        }

        public bool UpdateWasCalled { get; private set; }

        public Task<bool> HasActiveRecordsByVacunoAsync(long vacunoId, CancellationToken cancellationToken) => Task.FromResult(false);
        public Task<bool> ExistsCodigoAsync(string codigo, CancellationToken cancellationToken) => Task.FromResult(false);
        public Task<bool> ExistsVacunoFechaAsync(long vacunoId, DateTime fechaHora, long? excludeOrdenioId, CancellationToken cancellationToken) => Task.FromResult(false);
        public Task<bool> ExistsVacunoAsync(long vacunoId, CancellationToken cancellationToken) => Task.FromResult(true);
        public Task<bool> ExistsUsuarioAsync(long usuarioId, CancellationToken cancellationToken) => Task.FromResult(true);
        public Task<bool> ExistsEstadoAsync(string estadoOrdenioCode, CancellationToken cancellationToken) => Task.FromResult(true);
        public Task<Ordenio?> GetByIdAsync(long id, CancellationToken cancellationToken) => Task.FromResult(_ordenio);
        public Task<Ordenio?> GetByCodigoAsync(string codigo, CancellationToken cancellationToken) => Task.FromResult(_ordenio);
        public Task<(IReadOnlyList<OrdenioList> Items, int TotalCount)> ListAsync(long? vacunoId, string? estadoOrdenioCode, DateTime? fechaDesde, DateTime? fechaHasta, int page, int pageSize, CancellationToken cancellationToken) => Task.FromResult<(IReadOnlyList<OrdenioList> Items, int TotalCount)>((Array.Empty<OrdenioList>(), 0));
        public Task<IReadOnlyList<OrdenioList>> ListReportAsync(long? vacunoId, string? estadoOrdenioCode, DateTime? fechaDesde, DateTime? fechaHasta, CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<OrdenioList>>(Array.Empty<OrdenioList>());
        public Task<Ordenio> AddAsync(Ordenio ordenio, CancellationToken cancellationToken) => Task.FromResult(ordenio);

        public Task<Ordenio> UpdateAsync(Ordenio ordenio, CancellationToken cancellationToken)
        {
            UpdateWasCalled = true;
            return Task.FromResult(ordenio);
        }
    }

    private sealed class FakeOrdenioUnitOfWork : IOrdenioUnitOfWork
    {
        public FakeOrdenioUnitOfWork(IOrdenioRepository repository)
        {
            Repository = repository;
        }

        public IOrdenioRepository Repository { get; }

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

