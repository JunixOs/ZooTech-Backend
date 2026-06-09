using FluentAssertions;
using Xunit;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateTenant;

namespace ZooTech.Application.UnitTests.Modules.Module_Tenancing.UseCases.CreateTenant;

public class CreateTenantValidatorTests
{
    private readonly CreateTenantValidator _validator;

    public CreateTenantValidatorTests()
    {
        _validator = new CreateTenantValidator();
    }

    [Fact]
    public void Validate_CuandoDatosSonCorrectos_NoDebeRetornarErrores()
    {
        // Arrange
        var cmd = new CreateTenantCommand
        {
            Code = "TENANT01",
            SubDomain = "mi-empresa",
            DisplayName = "Mi Empresa",
            LegalName = "Empresa SAC",
            Email = "contacto@empresa.com",
            Phone = "123456789",
            Status = "ACTIVE",
            TenantAddress = new TenantAddress { Country = "PE", State = "Lima", City = "Lima", AddressLine_1 = "Av Principal 123" },
            TenantBranding = new TenantBranding { PrimaryColor = "#FF0000", SecondaryColor = "#00FF00", LogoUrl = "http://logo.com/logo.png" },
            TenantDatabaseConnection = new TenantDatabaseConnection { IsActive = true }
        };

        // Act
        var result = _validator.Validate(cmd);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("SUB_DOMAIN_INVALIDO!")]
    [InlineData("SubDominio")] // No debe tener mayusculas
    public void Validate_CuandoSubDomainEsInvalido_DebeRetornarError(string subDomain)
    {
        // Arrange
        var cmd = new CreateTenantCommand
        {
            Code = "TENANT01",
            SubDomain = subDomain,
            DisplayName = "Mi Empresa",
            LegalName = "Empresa SAC",
            Email = "contacto@empresa.com",
            Phone = "123456789",
            Status = "ACTIVE",
            TenantAddress = new TenantAddress { Country = "PE", State = "Lima", City = "Lima", AddressLine_1 = "Av Principal 123" },
            TenantBranding = new TenantBranding { PrimaryColor = "#FF0000", SecondaryColor = "#00FF00", LogoUrl = "http://logo.com/logo.png" },
            TenantDatabaseConnection = new TenantDatabaseConnection { IsActive = true }
        };

        // Act
        var result = _validator.Validate(cmd);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "SubDomain");
    }

    [Fact]
    public void Validate_CuandoEmailEsInvalido_DebeRetornarError()
    {
        // Arrange
        var cmd = new CreateTenantCommand
        {
            Code = "TENANT01",
            SubDomain = "mi-empresa",
            DisplayName = "Mi Empresa",
            LegalName = "Empresa SAC",
            Email = "correo-invalido",
            Phone = "123456789",
            Status = "ACTIVE",
            TenantAddress = new TenantAddress { Country = "PE", State = "Lima", City = "Lima", AddressLine_1 = "Av Principal 123" },
            TenantBranding = new TenantBranding { PrimaryColor = "#FF0000", SecondaryColor = "#00FF00", LogoUrl = "http://logo.com/logo.png" },
            TenantDatabaseConnection = new TenantDatabaseConnection { IsActive = true }
        };

        // Act
        var result = _validator.Validate(cmd);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email" && e.ErrorMessage.Contains("correo"));
    }

    [Fact]
    public void Validate_CuandoEstadoEsInvalido_DebeRetornarError()
    {
        // Arrange
        var cmd = new CreateTenantCommand
        {
            Code = "TENANT01",
            SubDomain = "mi-empresa",
            DisplayName = "Mi Empresa",
            LegalName = "Empresa SAC",
            Email = "contacto@empresa.com",
            Phone = "123456789",
            Status = "INVALIDO", // Estado no permitido
            TenantAddress = new TenantAddress { Country = "PE", State = "Lima", City = "Lima", AddressLine_1 = "Av Principal 123" },
            TenantBranding = new TenantBranding { PrimaryColor = "#FF0000", SecondaryColor = "#00FF00", LogoUrl = "http://logo.com/logo.png" },
            TenantDatabaseConnection = new TenantDatabaseConnection { IsActive = true }
        };

        // Act
        var result = _validator.Validate(cmd);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Status" && e.ErrorMessage.Contains("Estado inválido"));
    }
}
