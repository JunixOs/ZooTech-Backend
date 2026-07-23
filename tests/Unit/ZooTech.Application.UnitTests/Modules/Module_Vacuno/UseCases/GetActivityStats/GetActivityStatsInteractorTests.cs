using FluentAssertions;
using NSubstitute;
using Xunit;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GetActivityStats;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Interfaces;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Entities;

namespace ZooTech.Application.UnitTests.Modules.Module_Vacuno.UseCases.GetActivityStats;

public class GetActivityStatsInteractorTests
{
    private readonly IVacunoActivityStatsReadRepository _repository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly GetActivityStatsInteractor _sut;

    public GetActivityStatsInteractorTests()
    {
        _repository = Substitute.For<IVacunoActivityStatsReadRepository>();
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();
        _dateTimeProvider.ServerNow.Returns(new DateTime(2023, 1, 30, 12, 0, 0, DateTimeKind.Utc));
        _sut = new GetActivityStatsInteractor(
            new VacunoActivityStatsService(_repository, _dateTimeProvider));
    }

    [Fact]
    public async Task HandleAsync_WithValidDates_CalculatesStatsCorrectly()
    {
        // Arrange
        var command = new GetActivityStatsQuery(new DateOnly(2023, 1, 1), new DateOnly(2023, 1, 3));
        
        var altasPorDia = new Dictionary<DateOnly, int>
        {
            { new DateOnly(2023, 1, 1), 2 },
            { new DateOnly(2023, 1, 2), 0 },
            { new DateOnly(2023, 1, 3), 5 }
        };
        var bajasPorDia = new Dictionary<DateOnly, int>
        {
            { new DateOnly(2023, 1, 1), 1 },
            { new DateOnly(2023, 1, 2), 3 },
            { new DateOnly(2023, 1, 3), 0 }
        };
        
        var mockAggregates = new ActivityAggregatesOutput(10, altasPorDia, bajasPorDia);

        _repository.GetActivityAggregatesAsync(
            Arg.Any<DateOnly>(), 
            Arg.Any<DateOnly>(), 
            Arg.Any<CancellationToken>())
            .Returns(mockAggregates);

        // Act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Points.Should().HaveCount(3);
        
        // Day 1
        result.Points[0].Fecha.Should().Be("2023-01-01");
        result.Points[0].Cantidad.Should().Be(11);
        
        // Day 2
        result.Points[1].Fecha.Should().Be("2023-01-02");
        result.Points[1].Cantidad.Should().Be(8);
        
        // Day 3
        result.Points[2].Fecha.Should().Be("2023-01-03");
        result.Points[2].Cantidad.Should().Be(13);

        result.Mayor.Should().Be(13);
        result.Menor.Should().Be(8);
    }

    [Fact]
    public async Task HandleAsync_WhenNoData_ReturnsConstantInitialInventory()
    {
        // Arrange
        var command = new GetActivityStatsQuery(new DateOnly(2023, 1, 1), new DateOnly(2023, 1, 2));
        
        var mockAggregates = new ActivityAggregatesOutput(5, new Dictionary<DateOnly, int>(), new Dictionary<DateOnly, int>());

        _repository.GetActivityAggregatesAsync(
            Arg.Any<DateOnly>(), 
            Arg.Any<DateOnly>(), 
            Arg.Any<CancellationToken>())
            .Returns(mockAggregates);

        // Act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Points.Should().HaveCount(2);
        
        result.Points[0].Cantidad.Should().Be(5);
        result.Points[1].Cantidad.Should().Be(5);

        result.Mayor.Should().Be(5);
        result.Menor.Should().Be(5);
    }

    [Fact]
    public async Task HandleAsync_WithoutDates_UsesThirtyInclusiveDays()
    {
        _repository.GetActivityAggregatesAsync(
                new DateOnly(2023, 1, 1),
                new DateOnly(2023, 1, 30),
                Arg.Any<CancellationToken>())
            .Returns(new ActivityAggregatesOutput(3, [], []));

        var result = await _sut.HandleAsync(new GetActivityStatsQuery(null, null));

        result.FechaInicio.Should().Be("2023-01-01");
        result.FechaFin.Should().Be("2023-01-30");
        result.Points.Should().HaveCount(30);
    }

    [Fact]
    public async Task HandleAsync_WithOnlyEndDate_UsesThirtyInclusiveDaysEndingThere()
    {
        _repository.GetActivityAggregatesAsync(
                new DateOnly(2023, 2, 1),
                new DateOnly(2023, 3, 2),
                Arg.Any<CancellationToken>())
            .Returns(new ActivityAggregatesOutput(0, [], []));

        var result = await _sut.HandleAsync(
            new GetActivityStatsQuery(null, new DateOnly(2023, 3, 2)));

        result.Points.Should().HaveCount(30);
        result.FechaInicio.Should().Be("2023-02-01");
        result.FechaFin.Should().Be("2023-03-02");
    }

    [Fact]
    public async Task HandleAsync_WithFutureStartAndNoEnd_ThrowsStructuredValidationException()
    {
        var action = () => _sut.HandleAsync(
            new GetActivityStatsQuery(new DateOnly(2023, 1, 31), null));

        await action.Should().ThrowAsync<ValidationException>();
        await _repository.DidNotReceiveWithAnyArgs()
            .GetActivityAggregatesAsync(default, default, default);
    }
}
