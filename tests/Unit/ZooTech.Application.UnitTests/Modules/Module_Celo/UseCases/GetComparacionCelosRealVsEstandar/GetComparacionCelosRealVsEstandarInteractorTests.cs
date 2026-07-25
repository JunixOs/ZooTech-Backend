using NSubstitute;
using ZooTech.Application.Modules.Module_Celo.UseCases.GetComparacionCelosRealVsEstandar;
using ZooTech.Domain.Module_Celo.Interfaces;

namespace ZooTech.Application.UnitTests.Modules.Module_Celo.UseCases.GetComparacionCelosRealVsEstandar;

public sealed class GetComparacionCelosRealVsEstandarInteractorTests
{
    private readonly ICeloRepository _repository;
    private readonly GetComparacionCelosRealVsEstandarInteractor _interactor;

    public GetComparacionCelosRealVsEstandarInteractorTests()
    {
        _repository = Substitute.For<ICeloRepository>();
        _interactor = new GetComparacionCelosRealVsEstandarInteractor(_repository);
    }

    [Fact]
    public async Task HandleAsync_Should_ReturnEmptyItems_When_NoRecords()
    {
        _repository.GetByDateRangeAsync(Arg.Any<DateTime?>(), Arg.Any<DateTime?>(), Arg.Any<CancellationToken>())
            .Returns(new List<DateTime>());

        var output = await _interactor.HandleAsync(
            new GetComparacionCelosRealVsEstandarCommand(), CancellationToken.None);

        Assert.Empty(output.Items);
    }

    [Fact]
    public async Task HandleAsync_Should_MatchEstandarAndReal_When_SingleDay()
    {
        var day = new DateTime(2026, 3, 10);
        var registros = new List<DateTime>
        {
            day.AddHours(8),
            day.AddHours(10),
            day.AddHours(15),
        };
        _repository.GetByDateRangeAsync(Arg.Any<DateTime?>(), Arg.Any<DateTime?>(), Arg.Any<CancellationToken>())
            .Returns(registros);

        var output = await _interactor.HandleAsync(
            new GetComparacionCelosRealVsEstandarCommand(), CancellationToken.None);

        var item = Assert.Single(output.Items);
        Assert.Equal(DateOnly.FromDateTime(day), item.Fecha);
        Assert.Equal(3, item.RegistrosReales);
        Assert.Equal(3, item.RegistrosEstandar);
    }

    [Fact]
    public async Task HandleAsync_Should_UseSamePromedio_Across_MultipleDistinctDays()
    {
        var day1 = new DateTime(2026, 3, 10);
        var day2 = new DateTime(2026, 3, 11);
        var day3 = new DateTime(2026, 3, 12);
        var registros = new List<DateTime>
        {
            day1.AddHours(8), day1.AddHours(9),
            day2.AddHours(8), day2.AddHours(9), day2.AddHours(10),
            day3.AddHours(8),
        };
        // total 6 registros / 3 dias = 2 -> promedio == 2 para todos los dias
        _repository.GetByDateRangeAsync(Arg.Any<DateTime?>(), Arg.Any<DateTime?>(), Arg.Any<CancellationToken>())
            .Returns(registros);

        var output = await _interactor.HandleAsync(
            new GetComparacionCelosRealVsEstandarCommand(), CancellationToken.None);

        Assert.Equal(3, output.Items.Count);
        Assert.All(output.Items, item => Assert.Equal(2, item.RegistrosEstandar));

        Assert.Equal(2, output.Items[0].RegistrosReales);
        Assert.Equal(3, output.Items[1].RegistrosReales);
        Assert.Equal(1, output.Items[2].RegistrosReales);
    }

    [Fact]
    public async Task HandleAsync_Should_OrderByFechaAscending_RegardlessOfInputOrder()
    {
        var day1 = new DateTime(2026, 3, 10);
        var day2 = new DateTime(2026, 3, 11);
        var registros = new List<DateTime>
        {
            day2.AddHours(9),
            day1.AddHours(8),
        };
        _repository.GetByDateRangeAsync(Arg.Any<DateTime?>(), Arg.Any<DateTime?>(), Arg.Any<CancellationToken>())
            .Returns(registros);

        var output = await _interactor.HandleAsync(
            new GetComparacionCelosRealVsEstandarCommand(), CancellationToken.None);

        Assert.Equal(DateOnly.FromDateTime(day1), output.Items[0].Fecha);
        Assert.Equal(DateOnly.FromDateTime(day2), output.Items[1].Fecha);
    }

    [Fact]
    public async Task HandleAsync_Should_RoundMidpointToEven_When_PromedioIsExactHalf()
    {
        var day1 = new DateTime(2026, 3, 10);
        var day2 = new DateTime(2026, 3, 11);
        var registros = new List<DateTime>
        {
            day1.AddHours(8), day1.AddHours(9), day2.AddHours(8),
        };
        // 3 registros / 2 dias = 1.5 -> Math.Round default (ToEven) -> 2
        _repository.GetByDateRangeAsync(Arg.Any<DateTime?>(), Arg.Any<DateTime?>(), Arg.Any<CancellationToken>())
            .Returns(registros);

        var output = await _interactor.HandleAsync(
            new GetComparacionCelosRealVsEstandarCommand(), CancellationToken.None);

        Assert.All(output.Items, item => Assert.Equal(2, item.RegistrosEstandar));
    }
}
