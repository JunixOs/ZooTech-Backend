using ZooTech.Infrastructure.Persistence.Context;

namespace ZooTech.Infrastructure.Tenant
{
    public interface ITenantDbContextFactory
    {
        Task<TenantCatalogDb> CreateDbContextByTenantContext();
        Task<TenantCatalogDb> CreateDbContextBySettingsValue();
    }
}