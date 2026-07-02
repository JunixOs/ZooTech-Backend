using FluentAssertions;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateTenant;
using ZooTech.Domain.Admin.Enums;

namespace ZooTech.Application.UnitTests.Modules.Module_Tenancing.UseCases.CreateTenant;

public class CreateTenantValidatorTests
{
    private readonly CreateTenantValidation _validator = new();

    private static CreateTenantCommand CreateCommand(
        string? code = "tenant-01",
        string? subDomain = "tenant-01",
        string? displayName = "Tenant Display",
        string? legalName = "Tenant Legal SAC",
        string? email = "admin@tenant.com",
        string? phone = "+51999999999",
        string? timeZone = "America/Lima",
        TenantStatus? status = TenantStatus.TRIAL,
        string? metadata = "{}",
        TenantAddress? tenantAddress = null,
        TenantBranding? tenantBranding = null,
        TenantDatabaseConnection? tenantDatabaseConnection = null)
    {
        return new CreateTenantCommand
        {
            Code = code ?? string.Empty,
            SubDomain = subDomain ?? string.Empty,
            DisplayName = displayName ?? string.Empty,
            LegalName = legalName ?? string.Empty,
            Email = email ?? string.Empty,
            Phone = phone ?? string.Empty,
            TimeZone = timeZone ?? string.Empty,
            Status = status ?? TenantStatus.TRIAL,
            Metadata = metadata ?? string.Empty,
            TenantAddress = tenantAddress ?? new TenantAddress
            {
                Country = "Perú",
                State = "Lima",
                Province = "Lima",
                City = "Lima",
                AddressLine_1 = "Av. Principal 123",
                AddressLine_2 = "Oficina 501",
                Metadata = "{}"
            },
            TenantBranding = tenantBranding ?? new TenantBranding
            {
                PrimaryColor = "#2563EB",
                SecondaryColor = "#1E293B",
                LogoUrl = "https://cdn.zoo-tech.com/logo.png",
                Metadata = "{}"
            },
            TenantDatabaseConnection = tenantDatabaseConnection ?? new TenantDatabaseConnection
            {
                IsActive = true
            }
        };
    }

    [Fact]
    public void Should_Pass_For_Valid_Command()
    {
        // Arrange
        var command = CreateCommand();

        // Act
        var errors = _validator.Validate(command);

        // Assert
        errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Should_Fail_When_Code_Is_Empty(string? code)
    {
        // Arrange
        var command = CreateCommand(code: code);

        // Act
        var errors = _validator.Validate(command);

        // Assert
        errors.Should().NotBeEmpty();
        errors.Should().Contain(e => e.Contains("TENANT-CODE"));
    }

    [Fact]
    public void Should_Fail_When_Code_Exceeds_50_Chars()
    {
        // Arrange
        var command = CreateCommand(code: new string('a', 51));

        // Act
        var errors = _validator.Validate(command);

        // Assert
        errors.Should().NotBeEmpty();
        errors.Should().Contain(e => e.Contains("TENANT-CODE"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("Invalid_SubDomain")]
    [InlineData("sub domain")]
    public void Should_Fail_When_SubDomain_Is_Invalid(string? subDomain)
    {
        // Arrange
        var command = CreateCommand(subDomain: subDomain);

        // Act
        var errors = _validator.Validate(command);

        // Assert
        errors.Should().NotBeEmpty();
        errors.Should().Contain(e => e.Contains("SUBDOMAIN"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    public void Should_Fail_When_Email_Is_Invalid(string? email)
    {
        // Arrange
        var command = CreateCommand(email: email);

        // Act
        var errors = _validator.Validate(command);

        // Assert
        errors.Should().NotBeEmpty();
        errors.Should().Contain(e => e.Contains("EMAIL"));
    }

    [Fact]
    public void Should_Fail_When_Status_Is_Invalid()
    {
        // Arrange
        var command = CreateCommand(status: (TenantStatus)999);

        // Act
        var errors = _validator.Validate(command);

        // Assert
        errors.Should().NotBeEmpty();
        errors.Should().Contain(e => e.Contains("STATUS"));
    }

    [Fact]
    public void Should_Fail_When_Address_Is_Null()
    {
        // Arrange
        var command = new CreateTenantCommand
        {
            Code = "tenant-01",
            SubDomain = "tenant-01",
            DisplayName = "Tenant Display",
            LegalName = "Tenant Legal SAC",
            Email = "admin@tenant.com",
            Phone = "+51999999999",
            TimeZone = "America/Lima",
            Status = TenantStatus.TRIAL,
            Metadata = "{}",
            TenantAddress = null!,
            TenantBranding = new TenantBranding
            {
                PrimaryColor = "#2563EB",
                SecondaryColor = "#1E293B",
                LogoUrl = "https://cdn.zoo-tech.com/logo.png",
                Metadata = "{}"
            },
            TenantDatabaseConnection = new TenantDatabaseConnection
            {
                IsActive = true
            }
        };

        // Act
        var errors = _validator.Validate(command);

        // Assert
        errors.Should().NotBeEmpty();
        errors.Should().Contain(e => e.Contains("TENANT_ADDRESS"));
    }

    [Fact]
    public void Should_Fail_When_Address_Country_Is_Empty()
    {
        // Arrange
        var command = CreateCommand(tenantAddress: new TenantAddress
        {
            Country = "",
            State = "Lima",
            Province = "Lima",
            City = "Lima",
            AddressLine_1 = "Av. Principal 123"
        });

        // Act
        var errors = _validator.Validate(command);

        // Assert
        errors.Should().NotBeEmpty();
        errors.Should().Contain(e => e.Contains("ADDRESS_COUNTRY"));
    }

    [Fact]
    public void Should_Fail_When_Branding_PrimaryColor_Is_Invalid()
    {
        // Arrange
        var command = CreateCommand(tenantBranding: new TenantBranding
        {
            PrimaryColor = "red",
            SecondaryColor = "#1E293B",
            LogoUrl = "https://cdn.zoo-tech.com/logo.png"
        });

        // Act
        var errors = _validator.Validate(command);

        // Assert
        errors.Should().NotBeEmpty();
        errors.Should().Contain(e => e.Contains("PRIMARY_COLOR"));
    }

    [Fact]
    public void Should_Fail_When_Branding_LogoUrl_Is_Invalid()
    {
        // Arrange
        var command = CreateCommand(tenantBranding: new TenantBranding
        {
            PrimaryColor = "#2563EB",
            SecondaryColor = "#1E293B",
            LogoUrl = "not-a-url"
        });

        // Act
        var errors = _validator.Validate(command);

        // Assert
        errors.Should().NotBeEmpty();
        errors.Should().Contain(e => e.Contains("LOGO_URL"));
    }

    [Fact]
    public void Should_Pass_When_Branding_LogoUrl_Is_Empty()
    {
        // Arrange
        var command = CreateCommand(tenantBranding: new TenantBranding
        {
            PrimaryColor = "#2563EB",
            SecondaryColor = "#1E293B",
            LogoUrl = ""
        });

        // Act
        var errors = _validator.Validate(command);

        // Assert
        errors.Should().BeEmpty();
    }
}
