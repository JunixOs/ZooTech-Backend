using NSubstitute;
using ZooTech.Application.Modules.Module_Celo.UseCases.GetComparacionCelosRealVsEstandarPorVacuno;
using ZooTech.Domain.Module_Celo.Entities;
using ZooTech.Domain.Module_Celo.Interfaces;

namespace ZooTech.Application.UnitTests.Modules.Module_Celo.UseCases.GetComparacionCelosRealVsEstandarPorVacuno;

public sealed class GetComparacionCelosRealVsEstandarPorVacunoInteractorTests
{
    private readonly ICeloRepository _repository;
    private readonly GetComparacionCelosRealVsEstandarPorVacunoInteractor _interactor;

    public GetComparacionCelosRealVsEstandarPorVacunoInteractorTests()
    {
        _repository = Substitute.For<ICeloRepository>();
        _interactor = new GetComparacionCelosRealVsEstandarPorVacunoInteractor(_repository);
    }

    [Fact]
    public async Task HandleAsync_Should_ReturnEmptyItems_When_RepositoryIsEmpty()
    {
        _repository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(new List<CeloListItem>());

        var output = await _interactor.HandleAsync(
            new GetComparacionCelosRealVsEstandarPorVacunoCommand { CodigoVacuno = "VAC-001" },
            CancellationToken.None);

        Assert.Empty(output.Items);
    }

    [Fact]
    public async Task HandleAsync_Should_ReturnEmptyItems_When_CodigoVacunoDoesNotMatch()
    {
        var celos = new List<CeloListItem>
        {
            BuildCeloListItem("VAC-999", new DateTime(2026, 3, 10)),
        };
        _repository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(celos);

        var output = await _interactor.HandleAsync(
            new GetComparacionCelosRealVsEstandarPorVacunoCommand { CodigoVacuno = "VAC-001" },
            CancellationToken.None);

        Assert.Empty(output.Items);
    }

    [Fact]
    public async Task HandleAsync_Should_ReturnEmptyItems_When_CodigoVacunoIsNull()
    {
        var celos = new List<CeloListItem>
        {
            BuildCeloListItem("VAC-001", new DateTime(2026, 3, 10)),
        };
        _repository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(celos);

        var output = await _interactor.HandleAsync(
            new GetComparacionCelosRealVsEstandarPorVacunoCommand { CodigoVacuno = null },
            CancellationToken.None);

        Assert.Empty(output.Items);
    }

    [Fact]
    public async Task HandleAsync_Should_ReturnSingleMonth_When_RecordsInOneMonth()
    {
        var celos = new List<CeloListItem>
        {
            BuildCeloListItem("VAC-001", new DateTime(2026, 3, 5)),
            BuildCeloListItem("VAC-001", new DateTime(2026, 3, 20)),
        };
        _repository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(celos);

        var output = await _interactor.HandleAsync(
            new GetComparacionCelosRealVsEstandarPorVacunoCommand { CodigoVacuno = "VAC-001" },
            CancellationToken.None);

        var item = Assert.Single(output.Items);
        Assert.Equal(new DateOnly(2026, 3, 1), item.Periodo);
        Assert.Equal(2, item.RegistrosReales);
        var diasEnMarzo = DateTime.DaysInMonth(2026, 3);
        var expectedEstandar = (int)Math.Round(diasEnMarzo / 21.0, MidpointRounding.AwayFromZero);
        Assert.Equal(expectedEstandar, item.RegistrosEstandar);
    }

    [Fact]
    public async Task HandleAsync_Should_FillGapMonths_When_RecordsAreNonConsecutive()
    {
        var celos = new List<CeloListItem>
        {
            BuildCeloListItem("VAC-001", new DateTime(2026, 1, 10)),
            BuildCeloListItem("VAC-001", new DateTime(2026, 3, 10)),
        };
        _repository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(celos);

        var output = await _interactor.HandleAsync(
            new GetComparacionCelosRealVsEstandarPorVacunoCommand { CodigoVacuno = "VAC-001" },
            CancellationToken.None);

        Assert.Equal(3, output.Items.Count);
        Assert.Equal(new DateOnly(2026, 1, 1), output.Items[0].Periodo);
        Assert.Equal(1, output.Items[0].RegistrosReales);
        Assert.Equal(new DateOnly(2026, 2, 1), output.Items[1].Periodo);
        Assert.Equal(0, output.Items[1].RegistrosReales);
        Assert.Equal(new DateOnly(2026, 3, 1), output.Items[2].Periodo);
        Assert.Equal(1, output.Items[2].RegistrosReales);
    }

    [Fact]
    public async Task HandleAsync_Should_BoundRangeByExplicitFechas_And_IncludeFechaFinEndOfDay()
    {
        var celos = new List<CeloListItem>
        {
            BuildCeloListItem("VAC-001", new DateTime(2026, 3, 5)),
            BuildCeloListItem("VAC-001", new DateTime(2026, 3, 31, 23, 59, 0)),
            BuildCeloListItem("VAC-001", new DateTime(2026, 4, 1)),
        };
        _repository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(celos);

        var output = await _interactor.HandleAsync(
            new GetComparacionCelosRealVsEstandarPorVacunoCommand
            {
                CodigoVacuno = "VAC-001",
                FechaInicio = new DateTime(2026, 3, 1),
                FechaFin = new DateTime(2026, 3, 31),
            },
            CancellationToken.None);

        var item = Assert.Single(output.Items);
        Assert.Equal(new DateOnly(2026, 3, 1), item.Periodo);
        Assert.Equal(2, item.RegistrosReales);
    }

    private static CeloListItem BuildCeloListItem(string codigoVacuno, DateTime fechaHora)
    {
        return CeloListItem.Rehydrate(
            id: 1,
            codigo: "CEL-001",
            fechaHora: fechaHora,
            vacunoId: 1,
            vacunoCodigo: codigoVacuno,
            nombreVacuno: "Manchada",
            observaciones: null,
            caracteristicaCodes: []);
    }
}
