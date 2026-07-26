using ZooTech.Application.Modules.Module_Celo.UseCases.ListCelos;
using ZooTech.Domain.Module_Celo.Entities;
using ZooTech.Domain.Module_Celo.Interfaces;

namespace ZooTech.Application.UnitTests.Modules.Module_Celo.UseCases.ListCelos;

public sealed class ListCelosInteractorTests
{
    [Theory]
    [InlineData(0, 1)]
    [InlineData(-5, 1)]
    public async Task HandleAsync_WhenPageIsNotPositive_DefaultsToFirstPage(int page, int expectedPage)
    {
        var repository = new FakeCeloRepository();
        var interactor = new ListCelosInteractor(repository);

        var output = await interactor.HandleAsync(new ListCelosQuery { Search = null, Page = page, PageSize = 20 });

        Assert.Equal(expectedPage, output.Result.Page);
    }

    [Theory]
    [InlineData(0, 20)]
    [InlineData(10, 10)]
    [InlineData(25, 25)]
    [InlineData(50, 50)]
    [InlineData(100, 100)]
    [InlineData(150, 100)]
    public async Task HandleAsync_ClampsPageSizeBetweenDefaultAndMax(int pageSize, int expectedPageSize)
    {
        var repository = new FakeCeloRepository();
        var interactor = new ListCelosInteractor(repository);

        var output = await interactor.HandleAsync(new ListCelosQuery { Search = null, Page = 1, PageSize = pageSize });

        Assert.Equal(expectedPageSize, output.Result.PageSize);
    }

    private sealed class FakeCeloRepository : ICeloRepository
    {
        public Task<(IReadOnlyList<CeloListItem> Items, int TotalCount)> GetPagedAsync(
            string? search, int page, int pageSize, DateTime? fechaInicio = null, DateTime? fechaFin = null,
            IReadOnlyDictionary<string, string>? columnFilters = null, CancellationToken cancellationToken = default)
            => Task.FromResult<(IReadOnlyList<CeloListItem>, int)>((new List<CeloListItem>(), 0));

        public Task<Dictionary<long, int>> GetVecesEnCeloCountsAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(new Dictionary<long, int>());

        public Task<Dictionary<long, int>> GetCriasCountsAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(new Dictionary<long, int>());

        public Task<List<CeloReporteItem>> GetAllForReporteAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(new List<CeloReporteItem>());

        public Task<List<CeloListItem>> GetAllAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(new List<CeloListItem>());

        public Task<(IReadOnlyList<CeloReporteItem> Items, int TotalCount)> GetPagedForReporteAsync(
            string? search, int page, int pageSize, DateTime? fechaInicio = null, DateTime? fechaFin = null,
            IReadOnlyDictionary<string, string>? columnFilters = null, CancellationToken cancellationToken = default)
            => Task.FromResult<(IReadOnlyList<CeloReporteItem>, int)>((new List<CeloReporteItem>(), 0));

        public Task<Celo?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
            => Task.FromResult<Celo?>(null);

        public Task<Celo> AddAsync(Celo celo, CancellationToken cancellationToken = default)
            => Task.FromResult(celo);

        public Task<Celo> UpdateAsync(Celo celo, CancellationToken cancellationToken = default)
            => Task.FromResult(celo);

        public Task<bool> ExistsVacunoAsync(long vacunoId, CancellationToken cancellationToken = default)
            => Task.FromResult(true);

        public Task<bool> HasActiveRecordsByVacunoAsync(long vacunoId, CancellationToken cancellationToken = default)
            => Task.FromResult(false);

        public Task<bool> ExistsCodigoAsync(string codigo, CancellationToken cancellationToken = default)
            => Task.FromResult(false);

        public Task<List<DateTime>> GetByDateRangeAsync(
            DateTime? fechaInicio, DateTime? fechaFin, CancellationToken cancellationToken = default)
            => Task.FromResult(new List<DateTime>());
    }
}
