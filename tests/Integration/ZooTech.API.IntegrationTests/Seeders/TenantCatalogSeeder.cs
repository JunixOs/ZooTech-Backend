using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Entities.MainTenantsDb;

namespace ZooTech.API.IntegrationTests.Seeders;

public static class TenantCatalogSeeder
{
    public static readonly string[] TenantSubdomains =
    [
        "zootecniaunas",
        "elroble",
        "lacteosdelvalle",
        "losandes",
        "tenant-int"
    ];

    public static void SeedIntegrationTenants(this TenantCatalogDb context)
    {
        foreach (var subdomain in TenantSubdomains)
        {
            if (context.tenants.Any(x => x.subdomain == subdomain))
            {
                continue;
            }

            var tenant = new tenant
            {
                subdomain = subdomain,
                code = subdomain[..Math.Min(4, subdomain.Length)].ToUpperInvariant(),
                status = "ACTIVE",
                email = $"admin@{subdomain}.zentrycorp.local",
                display_name = subdomain,
                legal_name = $"{subdomain} SAC",
                phone = "123456",
                timezone = "America/Lima"
            };

            context.tenants.Add(tenant);
            context.tenant_database_connections.Add(new tenant_database_connection
            {
                tenant = tenant,
                database_name = $"ZooTech_{subdomain}_Db",
                is_active = true
            });
        }
    }
}
