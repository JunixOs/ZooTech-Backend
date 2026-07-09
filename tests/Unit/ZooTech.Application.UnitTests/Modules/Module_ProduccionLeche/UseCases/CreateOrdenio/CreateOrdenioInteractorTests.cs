using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.CreateOrdenio;
using ZooTech.Domain.Module_ProduccionLeche.Entities;
using ZooTech.Domain.Module_ProduccionLeche.Interfaces;

namespace ZooTech.Application.UnitTests.Modules.Module_ProduccionLeche.UseCases.CreateOrdenio;

public class CreateOrdenioInteractorTests
{
    [Fact]
    public async Task HandleAsync_WhenCodigoAlreadyExists_ThrowsConflictException()
    {
        var repository = new FakeOrdenioRepository
        {
            ExistsCodigoResult = true,
            ExistsVacunoResult = true,
            ExistsUsuarioResult = true,
            ExistsEstadoResult = true
        };
        var interactor = new CreateOrdenioInteractor(new FakeOrdenioUnitOfWork(repository));

        var command = new CreateOrdenioCommand
        {
            Codigo = "ORD-001",
            FechaHora = DateTime.UtcNow,
            VacunoId = 1,
            EncargadoUsuarioId = 2,
            Litros = 10,
            EstadoOrdenioCode = "ACTIVO",
            Observaciones = null
        };

        var action = () => interactor.Handle(command, CancellationToken.None);

        await Assert.ThrowsAsync<ConflictException>(action);
    }

    private sealed class FakeOrdenioRepository : IOrdenioRepository
    {
        public bool ExistsCodigoResult { get; set; }
        public bool ExistsVacunoFechaResult { get; set; }
        public bool ExistsVacunoResult { get; set; }
        public bool ExistsUsuarioResult { get; set; }
        public bool ExistsEstadoResult { get; set; }

        public Task<bool> ExistsCodigoAsync(string codigo, CancellationToken cancellationToken)
            => Task.FromResult(ExistsCodigoResult);

        public Task<bool> ExistsVacunoFechaAsync(long vacunoId, DateTime fechaHora, long? excludeOrdenioId, CancellationToken cancellationToken)
            => Task.FromResult(ExistsVacunoFechaResult);

        public Task<bool> ExistsVacunoAsync(long vacunoId, CancellationToken cancellationToken)
            => Task.FromResult(ExistsVacunoResult);

        public Task<bool> HasActiveRecordsByVacunoAsync(long vacunoId, CancellationToken cancellationToken)
            => Task.FromResult(false);

        public Task<bool> ExistsUsuarioAsync(long usuarioId, CancellationToken cancellationToken)
            => Task.FromResult(ExistsUsuarioResult);

        public Task<bool> ExistsEstadoAsync(string estadoOrdenioCode, CancellationToken cancellationToken)
            => Task.FromResult(ExistsEstadoResult);

        public Task<Ordenio?> GetByIdAsync(long id, CancellationToken cancellationToken)
            => Task.FromResult<Ordenio?>(null);

        public Task<Ordenio?> GetByCodigoAsync(string codigo, CancellationToken cancellationToken)
            => Task.FromResult<Ordenio?>(null);

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
