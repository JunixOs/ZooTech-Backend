using FluentAssertions;
using ZooTech.Domain.ValueObjects;

namespace ZooTech.Domain.UnitTests.ValueObjects;

public class EmailTests
{
    [Theory]
    [InlineData("user@domain.com")]
    [InlineData("name.surname@company.co.uk")]
    [InlineData("test+tag@example.org")]
    public void Constructor_Should_Succeed_For_Valid_Email(string email)
    {
        // Act
        var emailVo = new Email(email);

        // Assert
        emailVo.Value.Should().Be(email);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_Should_Throw_For_Empty_Or_Null(string? email)
    {
        // Act
        var act = () => new Email(email!);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("not-an-email")]
    [InlineData("missing@tld")]
    [InlineData("@domain.com")]
    [InlineData("spaces in@email.com")]
    public void Constructor_Should_Throw_For_Invalid_Format(string email)
    {
        // Act
        var act = () => new Email(email);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Implicit_Conversion_To_String_Should_Work()
    {
        // Arrange
        var email = new Email("hello@zoo-tech.com");

        // Act
        string value = email;

        // Assert
        value.Should().Be("hello@zoo-tech.com");
    }
}
