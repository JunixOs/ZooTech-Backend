using FluentAssertions;
using Xunit;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GetActivityStats;

namespace ZooTech.Application.UnitTests.Modules.Module_Vacuno.UseCases.GetActivityStats;

public class GetActivityStatsQueryValidatorTests
{
    private readonly GetActivityStatsQueryValidator _sut;

    public GetActivityStatsQueryValidatorTests()
    {
        _sut = new GetActivityStatsQueryValidator();
    }

    [Fact]
    public void Validate_WhenFechaInicioIsBeforeFechaFin_ReturnsNoErrors()
    {
        // Arrange
        var request = new GetActivityStatsQuery(new DateOnly(2023, 1, 1), new DateOnly(2023, 1, 2));

        // Act
        var result = _sut.Validate(request);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void Validate_WhenFechaInicioIsAfterFechaFin_ReturnsError()
    {
        // Arrange
        var request = new GetActivityStatsQuery(new DateOnly(2023, 1, 3), new DateOnly(2023, 1, 2));

        // Act
        var result = _sut.Validate(request);

        // Assert
        result.Should().ContainSingle()
            .Which.Should().Be("La fecha de inicio no puede ser posterior a la fecha de fin.");
    }
}
