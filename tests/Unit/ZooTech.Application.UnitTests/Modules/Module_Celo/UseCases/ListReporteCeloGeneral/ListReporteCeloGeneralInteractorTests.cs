using ZooTech.Application.Modules.Module_Celo.UseCases.ListReporteCeloGeneral;
using ZooTech.Domain.Module_Celo.Entities;
using ZooTech.Domain.Module_Celo.Interfaces;

namespace ZooTech.Application.UnitTests.Modules.Module_Celo.UseCases.ListReporteCeloGeneral;

public sealed class ListReporteCeloGeneralInteractorTests
{
    [Fact]
    public async Task HandleAsync_MapsRepositoryDataToDtoAndPagesResult()
    {
        var fechaHora = new DateTime(2026, 5, 10, 14, 30, 0);
        var celo = CeloReporteItem.Rehydrate(
            id: 1,
            codigo: "CEL-001",
            fechaHora: fechaHora,
            vacunoId: 42,
            vacunoCodigo: "V-042",
            nombreVacuno: "Lola",
            observaciones: "Sin novedades",
            caracteristicaCodes: ["INQUIETA"]);

        var repository = new FakeCeloReporteRepository
        {
            PagedCelos = [celo],
            PagedTotalCount = 1,
            VecesEnCeloCounts = new Dictionary<long, int> { [42] = 5 },
            CriasCounts = new Dictionary<long, int> { [42] = 1 }
        };
        var interactor = new ListReporteCeloGeneralInteractor(repository);

        var output = await interactor.HandleAsync(
            new ListReporteCeloGeneralQuery { Search = "Lola", Page = 1, PageSize = 20 },
            cancellationToken: CancellationToken.None);

        var item = Assert.Single(output.Result.Data);
        Assert.Equal("CEL-001", item.CodigoRegistro);
        Assert.Equal(5, item.VecesEnCelo);
        Assert.Equal(1, item.Crias);
        Assert.Equal(1, output.Result.TotalCount);
        Assert.Equal(1, output.Result.Page);
        Assert.Equal(20, output.Result.PageSize);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(-5, 1)]
    public async Task HandleAsync_WhenPageIsNotPositive_DefaultsToFirstPage(int page, int expectedPage)
    {
        var repository = new FakeCeloReporteRepository();
        var interactor = new ListReporteCeloGeneralInteractor(repository);

        var output = await interactor.HandleAsync(new ListReporteCeloGeneralQuery { Search = null, Page = page, PageSize = 20 });

        Assert.Equal(expectedPage, output.Result.Page);
    }

    [Theory]
    [InlineData(0, 20)]
    [InlineData(150, 100)]
    public async Task HandleAsync_ClampsPageSizeBetweenDefaultAndMax(int pageSize, int expectedPageSize)
    {
        var repository = new FakeCeloReporteRepository();
        var interactor = new ListReporteCeloGeneralInteractor(repository);

        var output = await interactor.HandleAsync(new ListReporteCeloGeneralQuery { Search = null, Page = 1, PageSize = pageSize });

        Assert.Equal(expectedPageSize, output.Result.PageSize);
    }

    private sealed class FakeCeloReporteRepository : ICeloRepository
    {
        public List<CeloReporteItem> PagedCelos { get; set; } = [];
        public int PagedTotalCount { get; set; }
        public Dictionary<long, int> VecesEnCeloCounts { get; set; } = [];
        public Dictionary<long, int> CriasCounts { get; set; } = [];

        public Task<(IReadOnlyList<CeloReporteItem> Items, int TotalCount)> GetPagedForReporteAsync(
            string? search, int page, int pageSize, DateTime? fechaInicio = null, DateTime? fechaFin = null,
            IReadOnlyDictionary<string, string>? columnFilters = null, CancellationToken cancellationToken = default)
            => Task.FromResult<(IReadOnlyList<CeloReporteItem>, int)>((PagedCelos, PagedTotalCount));

        public Task<Dictionary<long, int>> GetVecesEnCeloCountsAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(VecesEnCeloCounts);

        public Task<Dictionary<long, int>> GetCriasCountsAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(CriasCounts);

        public Task<List<CeloReporteItem>> GetAllForReporteAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(new List<CeloReporteItem>());

        public Task<IReadOnlyList<CeloHistorialItem>> GetHistorialPorVacunoAsync(string codigoVacuno, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<CeloHistorialItem>>([]);

        public Task<List<CeloListItem>> GetAllAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(new List<CeloListItem>());

        public Task<(IReadOnlyList<CeloListItem> Items, int TotalCount)> GetPagedAsync(
            string? search, int page, int pageSize, DateTime? fechaInicio = null, DateTime? fechaFin = null,
            IReadOnlyDictionary<string, string>? columnFilters = null, CancellationToken cancellationToken = default)
            => Task.FromResult<(IReadOnlyList<CeloListItem>, int)>((new List<CeloListItem>(), 0));

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

