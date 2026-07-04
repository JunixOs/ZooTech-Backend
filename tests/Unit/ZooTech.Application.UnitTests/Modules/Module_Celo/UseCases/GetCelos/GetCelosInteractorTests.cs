using ZooTech.Application.Modules.Module_Celo.UseCases.GetCelos;
using ZooTech.Domain.Module_Celo.Entities;
using ZooTech.Domain.Module_Celo.Interfaces;

namespace ZooTech.Application.UnitTests.Modules.Module_Celo.UseCases.GetCelos;

public sealed class FakeCeloRepository : ICeloRepository
{
    public List<Celo> Celos { get; set; } = [];
    public Dictionary<long, int> Counts { get; set; } = [];

    public Task<List<Celo>> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(Celos);

    public Task<Dictionary<long, int>> GetVecesEnCeloCountsAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(Counts);

    public Task<Celo?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        => Task.FromResult(Celos.FirstOrDefault(c => c.Id == id));

    public Task<Celo> AddAsync(Celo celo, CancellationToken cancellationToken = default)
        => Task.FromResult(celo);

    public Task<Celo> UpdateAsync(Celo celo, CancellationToken cancellationToken = default)
        => Task.FromResult(celo);

    public Task<bool> ExistsVacunoAsync(long vacunoId, CancellationToken cancellationToken = default)
        => Task.FromResult(true);

    public Task<bool> ExistsCodigoAsync(string codigo, CancellationToken cancellationToken = default)
        => Task.FromResult(false);
    public Task<List<Celo>> GetByDateRangeAsync(
    DateTime? fechaInicio,
    DateTime? fechaFin,
    CancellationToken cancellationToken = default)
    {
        var query = Celos.AsEnumerable();

        if (fechaInicio.HasValue)
        {
            query = query.Where(c => c.FechaHora >= fechaInicio.Value);
        }

        if (fechaFin.HasValue)
        {
            query = query.Where(c => c.FechaHora <= fechaFin.Value);
        }

        return Task.FromResult(query.ToList());
    }
}

public sealed class GetCelosInteractorTests
{
    private static Celo BuildCelo(long id, long vacunoId, string codigo, string codigoVacuno, string nombreVacuno)
    {
        var now = DateTime.UtcNow.AddMinutes(-1);
        return Celo.Rehydrate(
            id: id,
            codigo: codigo,
            fechaHora: now,
            vacunoId: vacunoId,
            vacunoCodigo: codigoVacuno,
            nombreVacuno: nombreVacuno,
            encargadoUsuarioId: 1,
            observaciones: null,
            estadoRegistroCode: "ACTIVO",
            caracteristicaCodes: [],
            createdAt: now,
            updatedAt: now,
            deletedAt: null,
            motivoEliminacion: null,
            createdBy: null,
            updatedBy: null,
            deletedBy: null);
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
        var vacunoId = 1L;
        var celo = BuildCelo(1, vacunoId, "CELO-001", "V-001", "Blanca");

        var repo = new FakeCeloRepository
        {
            Celos = [celo],
            Counts = new Dictionary<long, int> { [vacunoId] = 3 }
        };

        var interactor = new GetCelosInteractor(repo);
        var result = await interactor.HandleAsync();

        Assert.Single(result.Items);
        Assert.Equal(3, result.Items[0].VecesEnCelo);
        Assert.Equal("CELO-001", result.Items[0].CodigoRegistro);
        Assert.Equal("V-001", result.Items[0].CodigoVacuno);
        Assert.Equal("Blanca", result.Items[0].NombreVacuno);
    }

    [Fact]
    public async Task HandleAsync_ShouldDefaultToOne_WhenVacunoNotInCounts()
    {
        var celo = BuildCelo(1, 99L, "CELO-001", "V-099", "Rosa");

        var repo = new FakeCeloRepository
        {
            Celos = [celo],
            Counts = []
        };

        var interactor = new GetCelosInteractor(repo);
        var result = await interactor.HandleAsync();

        Assert.Equal(1, result.Items[0].VecesEnCelo);
    }
}
