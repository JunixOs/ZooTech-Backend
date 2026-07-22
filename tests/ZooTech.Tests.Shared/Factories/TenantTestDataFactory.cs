using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateTenant;
using ZooTech.Domain.Admin.Enums;
using ZooTech.Infrastructure.Persistence.Entities.MainTenantsDb;

namespace ZooTech.Tests.Shared.Factories;

public static class TenantTestDataFactory
{
    public static CreateTenantCommand CreateValidCommand(string code = "ganaderia-central")
    {
        return new CreateTenantCommand
        {
            Code = code,
            SubDomain = code,
            DisplayName = "Ganadería Central",
            LegalName = "Ganadería Central SAC",
            Email = "admin@ganaderia.com",
            Phone = "+51999999999",
            TimeZone = "America/Lima",
            Status = TenantStatus.TRIAL,
            Metadata = "{}",
            TenantAddress = new TenantAddress
            {
                Country = "Perú",
                State = "Lima",
                Province = "Lima",
                City = "Lima",
                AddressLine_1 = "Av. Principal 123",
                AddressLine_2 = "Oficina 501",
                Metadata = "{}"
            },
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
    }

    public static tenant CreateTenantEntity(
        int id = 1,
        string code = "TENANT_1",
        string subdomain = "tenant1",
        string status = "ACTIVE",
        bool dbConnectionActive = true)
    {
        return new tenant
        {
            id = id,
            code = code,
            subdomain = subdomain,
            display_name = $"Granja {subdomain}",
            legal_name = $"Granja {subdomain} S.A.C",
            email = $"{subdomain}@test.com",
            phone = "950678900",
            timezone = "America/Lima",
            status = status,
            address = new address
            {
                id = id * 10,
                country = "Perú",
                state = "Huanuco",
                province = "Huanuco",
                city = "Huanuco",
                address_line_1 = "Av. Abtao 1001",
                created_at = DateTime.UtcNow
            },
            tenant_database_connection = new tenant_database_connection
            {
                id = id * 10,
                database_name = $"ZooTech_{subdomain}_Db",
                is_active = dbConnectionActive,
                created_at = DateTime.UtcNow
            },
            created_at = DateTime.UtcNow
        };
    }
}
