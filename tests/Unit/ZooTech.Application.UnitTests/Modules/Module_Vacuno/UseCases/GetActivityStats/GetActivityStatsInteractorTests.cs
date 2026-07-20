using FluentAssertions;
using NSubstitute;
using Xunit;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GetActivityStats;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Interfaces;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Entities;

namespace ZooTech.Application.UnitTests.Modules.Module_Vacuno.UseCases.GetActivityStats;

public class GetActivityStatsInteractorTests
{
    private readonly IVacunoResponseReadRepository _vacunoReadRepository;
    private readonly GetActivityStatsInteractor _sut;

    public GetActivityStatsInteractorTests()
    {
        _vacunoReadRepository = Substitute.For<IVacunoResponseReadRepository>();
        _sut = new GetActivityStatsInteractor(_vacunoReadRepository);
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

        _vacunoReadRepository.GetActivityAggregatesAsync(
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

        _vacunoReadRepository.GetActivityAggregatesAsync(
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
}
