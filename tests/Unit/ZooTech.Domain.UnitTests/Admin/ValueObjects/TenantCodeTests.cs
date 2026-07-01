using FluentAssertions;
using ZooTech.Domain.Admin.ValueObjects;

namespace ZooTech.Domain.UnitTests.Admin.ValueObjects;

public class TenantCodeTests
{
    [Theory]
    [InlineData("abc-123")]
    [InlineData("ABC")]
    [InlineData("tenant-code-99")]
    public void Constructor_Should_Succeed_For_Valid_Code(string code)
    {
        // Act
        var tenantCode = new TenantCode(code);

        // Assert
        tenantCode.Value.Should().Be(code);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_Should_Throw_For_Empty_Or_Whitespace(string code)
    {
        // Act
        var act = () => new TenantCode(code);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_Should_Throw_For_Null()
    {
        // Act
        var act = () => new TenantCode(null!);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("tenant_code_with_underscores")]
    [InlineData("tenant code with spaces")]
    [InlineData("code!@#")]
    public void Constructor_Should_Throw_For_Invalid_Format(string code)
    {
        // Act
        var act = () => new TenantCode(code);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_Should_Throw_When_Exceeds_50_Chars()
    {
        // Act
        var act = () => new TenantCode(new string('a', 51));

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Implicit_Conversion_To_String_Should_Work()
    {
        // Arrange
        var tenantCode = new TenantCode("my-code");

        // Act
        string value = tenantCode;

        // Assert
        value.Should().Be("my-code");
    }
}
