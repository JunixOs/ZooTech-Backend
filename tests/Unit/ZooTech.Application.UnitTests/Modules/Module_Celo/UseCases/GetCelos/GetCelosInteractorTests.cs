using ZooTech.Application.Modules.Module_Celo.UseCases.GetCelos;
using ZooTech.Domain.Module_Celo.Entities;
using ZooTech.Domain.Module_Celo.Interfaces;

namespace ZooTech.Application.UnitTests.Modules.Module_Celo.UseCases.GetCelos;

public sealed class FakeCeloRepository : ICeloRepository
{
    // Esta colección es la que usa GetAllAsync.
    public List<CeloListItem> Celos { get; set; } = [];

    public Dictionary<long, int> Counts { get; set; } = [];

    public Task<List<CeloListItem>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Celos);
    }

    public Task<Dictionary<long, int>> GetVecesEnCeloCountsAsync(
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Counts);
    }

    // No se usa en estos tests, pero se implementa porque ICeloRepository lo exige.
    public Task<List<CeloReporteItem>> GetAllForReporteAsync(
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new List<CeloReporteItem>());
    }

    // No se usa en estos tests, pero se implementa porque ICeloRepository lo exige.
    public Task<(IReadOnlyList<CeloListItem> Items, int TotalCount)> GetPagedAsync(
        string? search,
        int page,
        int pageSize,
        DateTime? fechaInicio = null,
        DateTime? fechaFin = null,
        IReadOnlyDictionary<string, string>? columnFilters = null,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<(IReadOnlyList<CeloListItem> Items, int TotalCount)>((Celos, Celos.Count));
    }

    // No se usa en estos tests, pero se implementa porque ICeloRepository lo exige.
    public Task<(IReadOnlyList<CeloReporteItem> Items, int TotalCount)> GetPagedForReporteAsync(
        string? search,
        int page,
        int pageSize,
        DateTime? fechaInicio = null,
        DateTime? fechaFin = null,
        IReadOnlyDictionary<string, string>? columnFilters = null,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<(IReadOnlyList<CeloReporteItem> Items, int TotalCount)>((new List<CeloReporteItem>(), 0));
    }

    // No se usa en estos tests, pero se implementa porque ICeloRepository lo exige.
    public Task<Dictionary<long, int>> GetCriasCountsAsync(
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new Dictionary<long, int>());
    }

    // No se usa en estos tests, pero se implementa porque ICeloRepository lo exige.
    public Task<Celo?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<Celo?>(null);
    }

    // No se usan en estos tests, pero la interfaz obliga a declararlos.
    public Task<Celo> AddAsync(
        Celo celo,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(celo);
    }

    public Task<Celo> UpdateAsync(
        Celo celo,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(celo);
    }

    public Task<bool> ExistsVacunoAsync(
        long vacunoId,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }

    // No se usa en estos tests, pero se implementa porque ICeloRepository lo exige.
    public Task<bool> HasActiveRecordsByVacunoAsync(
        long vacunoId,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(false);
    }

    public Task<bool> ExistsCodigoAsync(
        string codigo,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(false);
    }

    // No se usa en estos tests, pero se implementa porque ICeloRepository lo exige.
    public Task<List<DateTime>> GetByDateRangeAsync(
        DateTime? fechaInicio,
        DateTime? fechaFin,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new List<DateTime>());
    }
}

public sealed class GetCelosInteractorTests
{
    private static CeloListItem BuildCeloListItem(
        long id,
        long vacunoId,
        string codigo,
        string codigoVacuno,
        string nombreVacuno)
    {
        var fechaHora = DateTime.UtcNow.AddMinutes(-1);

        return CeloListItem.Rehydrate(
            id: id,
            codigo: codigo,
            fechaHora: fechaHora,
            vacunoId: vacunoId,
            vacunoCodigo: codigoVacuno,
            nombreVacuno: nombreVacuno);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnEmptyList_WhenNoCelosExist()
    {
        var repo = new FakeCeloRepository();

        var interactor = new GetCelosInteractor(repo);

        var result = await interactor.HandleAsync();

        Assert.NotNull(result);
        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task HandleAsync_ShouldMapVecesEnCeloFromCounts()
    {
        const long vacunoId = 1;

        var celo = BuildCeloListItem(
            id: 1,
            vacunoId: vacunoId,
            codigo: "CELO-001",
            codigoVacuno: "V-001",
            nombreVacuno: "Blanca");

        var repo = new FakeCeloRepository
        {
            Celos = [celo],
            Counts = new Dictionary<long, int>
            {
                [vacunoId] = 3
            }
        };

        var interactor = new GetCelosInteractor(repo);

        var result = await interactor.HandleAsync();

        Assert.Single(result.Items);

        var item = result.Items[0];

        Assert.Equal(3, item.VecesEnCelo);
        Assert.Equal("CELO-001", item.CodigoRegistro);
        Assert.Equal("V-001", item.CodigoVacuno);
        Assert.Equal("Blanca", item.NombreVacuno);
    }

    [Fact]
    public async Task HandleAsync_ShouldDefaultToOne_WhenVacunoNotInCounts()
    {
        var celo = BuildCeloListItem(
            id: 1,
            vacunoId: 99,
            codigo: "CELO-001",
            codigoVacuno: "V-099",
            nombreVacuno: "Rosa");

        var repo = new FakeCeloRepository
        {
            Celos = [celo],
            Counts = []
        };

        var interactor = new GetCelosInteractor(repo);

        var result = await interactor.HandleAsync();

        Assert.Single(result.Items);
        Assert.Equal(1, result.Items[0].VecesEnCelo);
    }
}