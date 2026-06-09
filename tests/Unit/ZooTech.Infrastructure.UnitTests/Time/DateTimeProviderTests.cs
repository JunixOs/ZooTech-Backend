using System;
using FluentAssertions;
using Xunit;
using ZooTech.Infrastructure.Time;

namespace ZooTech.Infrastructure.UnitTests.Time;

public class DateTimeProviderTests
{
    [Fact]
    public void Today_DebeRetornarFechaActualLocal()
    {
        // Arrange
        var provider = new DateTimeProvider();
        
        // Act
        var result = provider.Today;

        // Assert
        result.Should().Be(DateOnly.FromDateTime(DateTime.Now));
    }
}
