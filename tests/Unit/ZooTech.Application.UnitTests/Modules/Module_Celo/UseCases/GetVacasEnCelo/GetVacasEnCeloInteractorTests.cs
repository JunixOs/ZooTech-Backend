using NSubstitute;
using ZooTech.Application.Modules.Module_Celo.UseCases.GetVacasEnCelo;
using ZooTech.Domain.Module_Celo.Entities;
using ZooTech.Domain.Module_Celo.Interfaces;

namespace ZooTech.Application.UnitTests.Modules.Module_Celo.UseCases.GetVacasEnCelo;

public sealed class GetVacasEnCeloInteractorTests
{
    private readonly ICeloRepository _repository;
    private readonly GetVacasEnCeloInteractor _interactor;

    public GetVacasEnCeloInteractorTests()
    {
        _repository = Substitute.For<ICeloRepository>();
        _interactor = new GetVacasEnCeloInteractor(_repository);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(2)]
    public async Task HandleAsync_Should_ReturnEnCelo_When_DiasDesdeUltimoCeloIsZeroOrTwo(int dias)
    {
        var hoy = DateTime.UtcNow.Date;
        var celos = new List<CeloListItem> { BuildCeloListItem(1, "VAC-001", "Manchada", hoy.AddDays(-dias)) };
        SetupRepository(celos, new Dictionary<long, int>(), new Dictionary<long, int>());

        var output = await _interactor.HandleAsync(new GetVacasEnCeloCommand(), CancellationToken.None);

        Assert.Equal("En celo", Assert.Single(output.Items).Estado);
    }

    [Theory]
    [InlineData(18)]
    [InlineData(21)]
    public async Task HandleAsync_Should_ReturnProximo_When_DiasRestanteBetweenZeroAndThree(int dias)
    {
        var hoy = DateTime.UtcNow.Date;
        var celos = new List<CeloListItem> { BuildCeloListItem(1, "VAC-001", "Manchada", hoy.AddDays(-dias)) };
        SetupRepository(celos, new Dictionary<long, int>(), new Dictionary<long, int>());

        var output = await _interactor.HandleAsync(new GetVacasEnCeloCommand(), CancellationToken.None);

        Assert.Equal("Próximo", Assert.Single(output.Items).Estado);
    }

    [Theory]
    [InlineData(3)]
    [InlineData(17)]
    public async Task HandleAsync_Should_ReturnPaso_When_OutsideEnCeloAndProximoRanges(int dias)
    {
        var hoy = DateTime.UtcNow.Date;
        var celos = new List<CeloListItem> { BuildCeloListItem(1, "VAC-001", "Manchada", hoy.AddDays(-dias)) };
        SetupRepository(celos, new Dictionary<long, int>(), new Dictionary<long, int>());

        var output = await _interactor.HandleAsync(new GetVacasEnCeloCommand(), CancellationToken.None);

        Assert.Equal("Pasó", Assert.Single(output.Items).Estado);
    }

    [Fact]
    public async Task HandleAsync_Should_DefaultVecesEnCeloToOne_And_CriasToZero_When_MissingFromCounts()
    {
        var hoy = DateTime.UtcNow.Date;
        var celos = new List<CeloListItem> { BuildCeloListItem(1, "VAC-001", "Manchada", hoy) };
        SetupRepository(celos, new Dictionary<long, int>(), new Dictionary<long, int>());

        var output = await _interactor.HandleAsync(new GetVacasEnCeloCommand(), CancellationToken.None);

        var item = Assert.Single(output.Items);
        Assert.Equal(1, item.VecesEnCelo);
        Assert.Equal(0, item.Crias);
    }

    [Fact]
    public async Task HandleAsync_Should_UseCountsFromRepository_When_Present()
    {
        var hoy = DateTime.UtcNow.Date;
        var celos = new List<CeloListItem> { BuildCeloListItem(1, "VAC-001", "Manchada", hoy) };
        SetupRepository(
            celos,
            new Dictionary<long, int> { [1] = 5 },
            new Dictionary<long, int> { [1] = 2 });

        var output = await _interactor.HandleAsync(new GetVacasEnCeloCommand(), CancellationToken.None);

        var item = Assert.Single(output.Items);
        Assert.Equal(5, item.VecesEnCelo);
        Assert.Equal(2, item.Crias);
    }

    [Fact]
    public async Task HandleAsync_Should_ExcludeRecordsOutsideDateRange_BeforeGrouping()
    {
        var hoy = DateTime.UtcNow.Date;
        var celos = new List<CeloListItem>
        {
            BuildCeloListItem(1, "VAC-001", "Manchada", new DateTime(2026, 1, 5)),
            BuildCeloListItem(2, "VAC-002", "Overa", new DateTime(2026, 6, 5)),
        };
        SetupRepository(celos, new Dictionary<long, int>(), new Dictionary<long, int>());

        var output = await _interactor.HandleAsync(
            new GetVacasEnCeloCommand
            {
                FechaInicio = new DateTime(2026, 6, 1),
                FechaFin = new DateTime(2026, 6, 30),
            },
            CancellationToken.None);

        var item = Assert.Single(output.Items);
        Assert.Equal("VAC-002", item.Codigo);
    }

    [Fact]
    public async Task HandleAsync_Should_OrderResultsDescendingByFecha()
    {
        var hoy = DateTime.UtcNow.Date;
        var celos = new List<CeloListItem>
        {
            BuildCeloListItem(1, "VAC-001", "Manchada", hoy.AddDays(-10)),
            BuildCeloListItem(2, "VAC-002", "Overa", hoy.AddDays(-1)),
            BuildCeloListItem(3, "VAC-003", "Blanca", hoy.AddDays(-5)),
        };
        SetupRepository(celos, new Dictionary<long, int>(), new Dictionary<long, int>());

        var output = await _interactor.HandleAsync(new GetVacasEnCeloCommand(), CancellationToken.None);

        Assert.Equal(new[] { "VAC-002", "VAC-003", "VAC-001" }, output.Items.Select(i => i.Codigo));
    }

    private void SetupRepository(
        List<CeloListItem> celos,
        Dictionary<long, int> vecesEnCeloCounts,
        Dictionary<long, int> criasCounts)
    {
        _repository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(celos);
        _repository.GetVecesEnCeloCountsAsync(Arg.Any<CancellationToken>()).Returns(vecesEnCeloCounts);
        _repository.GetCriasCountsAsync(Arg.Any<CancellationToken>()).Returns(criasCounts);
    }

    private static CeloListItem BuildCeloListItem(long vacunoId, string codigo, string nombre, DateTime fechaHora)
    {
        return CeloListItem.Rehydrate(
            id: vacunoId,
            codigo: $"CEL-{vacunoId:000}",
            fechaHora: fechaHora,
            vacunoId: vacunoId,
            vacunoCodigo: codigo,
            nombreVacuno: nombre,
            observaciones: null,
            caracteristicaCodes: []);
    }
}
