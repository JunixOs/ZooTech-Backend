using FluentAssertions;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateTenant;
using ZooTech.InterfaceAdapters.Modules.Module_Tenancing.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Tenancing.DTOs.Responses;
using ZooTech.InterfaceAdapters.Modules.Module_Tenancing.Mappers;

namespace ZooTech.InterfaceAdapters.UnitTests.Modules.Module_Tenancing.Mappers;

public class CreateTenantMapperTests
{
    [Fact]
    public void ToCommand_Should_Map_All_Fields()
    {
        // Arrange
        var dto = new CreateTenantRequestDto
        {
            Code = "tenant-01",
            SubDomain = "tenant-01",
            DisplayName = "Tenant Display",
            LegalName = "Tenant Legal SAC",
            Email = "admin@tenant.com",
            Phone = "+51999999999",
            TimeZone = "America/Lima",
            TenantAddress = new TenantAddressRequestDto
            {
                Country = "Perú",
                State = "Lima",
                Province = "Lima",
                City = "Lima",
                AddressLine1 = "Av. Principal 123",
                AddressLine2 = "Oficina 501"
            },
            TenantBranding = new TenantBrandingRequestDto
            {
                PrimaryColor = "#2563EB",
                SecondaryColor = "#1E293B",
                LogoUrl = "https://cdn.zoo-tech.com/logo.png"
            },
            TenantDatabaseConnection = new TenantDatabaseConnectionRequestDto
            {
                IsActive = true
            }
        };

        // Act
        var command = CreateTenantMapper.ToCommand(dto);

        // Assert
        command.Code.Should().Be(dto.Code);
        command.SubDomain.Should().Be(dto.SubDomain);
        command.DisplayName.Should().Be(dto.DisplayName);
        command.LegalName.Should().Be(dto.LegalName);
        command.Email.Should().Be(dto.Email);
        command.Phone.Should().Be(dto.Phone);
        command.TimeZone.Should().Be(dto.TimeZone);
        command.TenantAddress.Should().NotBeNull();
        command.TenantAddress.Country.Should().Be(dto.TenantAddress.Country);
        command.TenantAddress.AddressLine_1.Should().Be(dto.TenantAddress.AddressLine1);
        command.TenantAddress.AddressLine_2.Should().Be(dto.TenantAddress.AddressLine2);
        command.TenantBranding.Should().NotBeNull();
        command.TenantBranding.PrimaryColor.Should().Be(dto.TenantBranding.PrimaryColor);
        command.TenantDatabaseConnection.Should().NotBeNull();
        command.TenantDatabaseConnection.IsActive.Should().Be(dto.TenantDatabaseConnection.IsActive);
    }

    [Fact]
    public void ToCommand_Should_Set_Empty_AddressLine2_When_Null()
    {
        // Arrange
        var dto = new CreateTenantRequestDto
        {
            Code = "t",
            SubDomain = "t",
            DisplayName = "D",
            LegalName = "L",
            Email = "a@b.com",
            Phone = "1",
            TimeZone = "UTC",
            TenantAddress = new TenantAddressRequestDto
            {
                Country = "C",
                State = "S",
                Province = "P",
                City = "C",
                AddressLine1 = "A1",
                AddressLine2 = null
            },
            TenantBranding = new TenantBrandingRequestDto
            {
                PrimaryColor = "#000000",
                SecondaryColor = "#FFFFFF",
                LogoUrl = ""
            },
            TenantDatabaseConnection = new TenantDatabaseConnectionRequestDto { IsActive = false }
        };

        // Act
        var command = CreateTenantMapper.ToCommand(dto);

        // Assert
        command.TenantAddress.AddressLine_2.Should().BeEmpty();
    }

    [Fact]
    public void ToResponseDto_Should_Map_All_Fields()
    {
        // Arrange
        var result = new CreateTenantResult
        {
            Code = "tenant-01",
            SubDomain = "tenant-01",
            DisplayName = "Tenant Display",
            LegalName = "Tenant Legal SAC"
        };

        // Act
        var dto = CreateTenantMapper.ToResponseDto(result);

        // Assert
        dto.Code.Should().Be(result.Code);
        dto.SubDomain.Should().Be(result.SubDomain);
        dto.DisplayName.Should().Be(result.DisplayName);
        dto.LegalName.Should().Be(result.LegalName);
    }
}
