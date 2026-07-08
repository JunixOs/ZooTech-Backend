using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Entities.MainTenantsDb;

namespace ZooTech.Infrastructure.IntegrationTests;

public class TenantCatalogDbIntegrationTests
{
    private static TenantCatalogDb CreateContext()
    {
        var options = new DbContextOptionsBuilder<TenantCatalogDb>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TenantCatalogDb(options);
    }

    [Fact]
    public async Task Should_Save_And_Retrieve_Tenant()
    {
        // Arrange
        await using var db = CreateContext();

        var tenant = new tenant
        {
            code = "INT_TENANT",
            subdomain = "int-tenant",
            display_name = "Integration Tenant",
            legal_name = "Integration Tenant SAC",
            email = "int@test.com",
            phone = "+51111111111",
            timezone = "UTC",
            status = "ACTIVE",
            created_at = DateTime.UtcNow,
            address = new address
            {
                country = "Perú",
                state = "Lima",
                province = "Lima",
                city = "Lima",
                address_line_1 = "Av. Int 123",
                created_at = DateTime.UtcNow
            },
            tenant_database_connection = new tenant_database_connection
            {
                database_name = "ZooTech_int_tenant_Db",
                is_active = true,
                created_at = DateTime.UtcNow
            }
        };

        // Act
        db.tenants.Add(tenant);
        await db.SaveChangesAsync();

        var retrieved = await db.tenants.FindAsync(tenant.id);

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.code.Should().Be("INT_TENANT");
        retrieved.subdomain.Should().Be("int-tenant");
        retrieved.address.Should().NotBeNull();
        retrieved.tenant_database_connection.Should().NotBeNull();
    }
}
