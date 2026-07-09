using ZooTech.Infrastructure.Persistence.Context;

namespace ZooTech.Infrastructure.Tenant
{
    public interface ITenantDbContextFactory
    {
        TenantCatalogDb CreateDbContextByTenantContext();
        TenantCatalogDb CreateDbContextBySettingsValue();
    }
}