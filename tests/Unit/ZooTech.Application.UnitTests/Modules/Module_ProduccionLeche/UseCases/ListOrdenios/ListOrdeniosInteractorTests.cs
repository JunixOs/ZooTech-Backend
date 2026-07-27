using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.ListOrdenios;
using ZooTech.Domain.Module_ProduccionLeche.Entities;
using ZooTech.Domain.Module_ProduccionLeche.Interfaces;

namespace ZooTech.Application.UnitTests.Modules.Module_ProduccionLeche.UseCases.ListOrdenios;

public class ListOrdeniosInteractorTests
{
    [Theory]
    [InlineData(0, 0, 1, 20)]
    [InlineData(-5, 200, 1, 100)]
    [InlineData(3, 50, 3, 50)]
    public async Task HandleAsync_NormalizesPagingValuesBeforeCallingRepository(int page, int pageSize, int expectedPage, int expectedPageSize)
    {
        var repository = new FakeOrdenioRepository();
        var interactor = new ListOrdeniosInteractor(repository);

        await interactor.HandleAsync(
            new ListOrdeniosQuery
            {
                VacunoId = null,
                EstadoOrdenioCode = null,
                FechaDesde = null,
                FechaHasta = null,
                Page = page,
                PageSize = pageSize
            },
            CancellationToken.None);

        Assert.Equal(expectedPage, repository.CapturedPage);
        Assert.Equal(expectedPageSize, repository.CapturedPageSize);
    }

    private sealed class FakeOrdenioRepository : IOrdenioRepository
    {
        public int CapturedPage { get; private set; }
        public int CapturedPageSize { get; private set; }

        public Task<bool> HasActiveRecordsByVacunoAsync(long vacunoId, CancellationToken cancellationToken) => Task.FromResult(false);

        public Task<bool> ExistsCodigoAsync(string codigo, CancellationToken cancellationToken)
            => Task.FromResult(false);

        public Task<bool> ExistsVacunoFechaAsync(long vacunoId, DateTime fechaHora, long? excludeOrdenioId, CancellationToken cancellationToken)
            => Task.FromResult(false);

        public Task<bool> ExistsVacunoAsync(long vacunoId, CancellationToken cancellationToken)
            => Task.FromResult(true);

        public Task<bool> ExistsUsuarioAsync(long usuarioId, CancellationToken cancellationToken)
            => Task.FromResult(true);

        public Task<bool> ExistsEstadoAsync(string estadoOrdenioCode, CancellationToken cancellationToken)
            => Task.FromResult(true);

        public Task<Ordenio?> GetByIdAsync(long id, CancellationToken cancellationToken)
            => Task.FromResult<Ordenio?>(null);

        public Task<Ordenio?> GetByCodigoAsync(string codigo, CancellationToken cancellationToken)
            => Task.FromResult<Ordenio?>(null);

        public Task<(IReadOnlyList<OrdenioList> Items, int TotalCount)> ListAsync(long? vacunoId, string? estadoOrdenioCode, DateTime? fechaDesde, DateTime? fechaHasta, int page, int pageSize, CancellationToken cancellationToken)
        {
            CapturedPage = page;
            CapturedPageSize = pageSize;
            return Task.FromResult<(IReadOnlyList<OrdenioList> Items, int TotalCount)>((Array.Empty<OrdenioList>(), 0));
        }

        public Task<IReadOnlyList<OrdenioList>> ListReportAsync(long? vacunoId, string? estadoOrdenioCode, DateTime? fechaDesde, DateTime? fechaHasta, CancellationToken cancellationToken)
            => Task.FromResult<IReadOnlyList<OrdenioList>>(Array.Empty<OrdenioList>());

        public Task<Ordenio> AddAsync(Ordenio ordenio, CancellationToken cancellationToken)
            => Task.FromResult(ordenio);

        public Task<Ordenio> UpdateAsync(Ordenio ordenio, CancellationToken cancellationToken)
            => Task.FromResult(ordenio);
    }
}

