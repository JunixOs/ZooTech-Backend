using ZooTech.Application.Modules.Module_Celo.UseCases.GetReporteCelos;
using ZooTech.Domain.Module_Celo.Entities;
using ZooTech.Domain.Module_Celo.Interfaces;

namespace ZooTech.Application.UnitTests.Modules.Module_Celo.UseCases.GetReporteCelos;

public sealed class GetReporteCelosInteractorTests
{
    [Fact]
    public async Task HandleAsync_MapsRepositoryDataToDto()
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
            caracteristicaCodes: ["INQUIETA", "MUGIDOS"]);

        var repository = new FakeCeloReporteRepository
        {
            Celos = [celo],
            VecesEnCeloCounts = new Dictionary<long, int> { [42] = 3 },
            CriasCounts = new Dictionary<long, int> { [42] = 2 }
        };
        var interactor = new GetReporteCelosInteractor(repository);

        var output = await interactor.HandleAsync(CancellationToken.None);

        var item = Assert.Single(output.Items);
        Assert.Equal("CEL-001", item.CodigoRegistro);
        Assert.Equal(DateOnly.FromDateTime(fechaHora), item.Fecha);
        Assert.Equal(TimeOnly.FromDateTime(fechaHora), item.Hora);
        Assert.Equal("V-042", item.CodigoVacuno);
        Assert.Equal("Lola", item.NombreVacuno);
        Assert.Equal(3, item.VecesEnCelo);
        Assert.Equal(2, item.Caracteristicas);
        Assert.Equal(["INQUIETA", "MUGIDOS"], item.ListaCaracteristicas);
        Assert.Equal("Sin novedades", item.Observaciones);
        Assert.Equal(2, item.Crias);
    }

    [Fact]
    public async Task HandleAsync_WhenVacunoHasNoCounts_DefaultsVecesEnCeloToOneAndCriasToZero()
    {
        var celo = CeloReporteItem.Rehydrate(
            id: 1,
            codigo: "CEL-002",
            fechaHora: DateTime.UtcNow,
            vacunoId: 99,
            vacunoCodigo: "V-099",
            nombreVacuno: "Bella",
            observaciones: string.Empty,
            caracteristicaCodes: []);

        var repository = new FakeCeloReporteRepository { Celos = [celo] };
        var interactor = new GetReporteCelosInteractor(repository);

        var output = await interactor.HandleAsync(CancellationToken.None);

        var item = Assert.Single(output.Items);
        Assert.Equal(1, item.VecesEnCelo);
        Assert.Equal(0, item.Crias);
    }

    private sealed class FakeCeloReporteRepository : ICeloRepository
    {
        public List<CeloReporteItem> Celos { get; set; } = [];
        public Dictionary<long, int> VecesEnCeloCounts { get; set; } = [];
        public Dictionary<long, int> CriasCounts { get; set; } = [];

        public Task<List<CeloReporteItem>> GetAllForReporteAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(Celos);

        public Task<Dictionary<long, int>> GetVecesEnCeloCountsAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(VecesEnCeloCounts);

        public Task<Dictionary<long, int>> GetCriasCountsAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(CriasCounts);

        public Task<List<CeloListItem>> GetAllAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(new List<CeloListItem>());

        public Task<(IReadOnlyList<CeloListItem> Items, int TotalCount)> GetPagedAsync(
            string? search, int page, int pageSize, DateTime? fechaInicio = null, DateTime? fechaFin = null,
            IReadOnlyDictionary<string, string>? columnFilters = null, CancellationToken cancellationToken = default)
            => Task.FromResult<(IReadOnlyList<CeloListItem>, int)>((new List<CeloListItem>(), 0));

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

