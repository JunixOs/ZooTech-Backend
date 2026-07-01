using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ZooTech.Application.Common.Gateway.Tenant;
using ZooTech.Domain.Admin.Enums;
using ZooTech.Infrastructure.Persistence.Context;

namespace ZooTech.Infrastructure.Tenant
{
    public class TenantStore : ITenantStore
    {
        private readonly TenantCatalogDb _tenantCatalogDb;
        private readonly IMemoryCache _cache;

        public TenantStore(TenantCatalogDb tenantCatalogDb, IMemoryCache cache)
        {
            _tenantCatalogDb = tenantCatalogDb;
            _cache = cache;
        }

        public async Task<TenantInfo?> GetBySubDomainAsync(string subDomain)
        {
            var cacheKey = $"tenant:{subDomain}";

            if (_cache.TryGetValue(cacheKey, out TenantInfo? tenantCached))
            {
                return tenantCached;
            }

            var tenantInfo = await _tenantCatalogDb.tenants
                .AsNoTracking()
                .Include(t => t.address)
                .Include(t => t.tenant_database_connection)
                .Select((t) =>
                    new TenantInfo
                    {
                        Id = t.id,
                        SubDomain = t.subdomain,
                        LegalName = t.legal_name,
                        DisplayName = t.display_name,
                        Code = t.code,
                        DatabaseName = t.tenant_database_connection.database_name,
                        IsDatabaseActive = t.tenant_database_connection.is_active,
                        Status = Enum.Parse<TenantStatus>(t.status),
                        Email = t.email
                    }
                )
                .FirstOrDefaultAsync();

            if (tenantInfo == null || tenantInfo.Status.Equals(TenantStatus.INACTIVE) || !tenantInfo.IsDatabaseActive)
            {
                return null;
            }

            _cache.Set(cacheKey, tenantInfo, TimeSpan.FromMinutes(5));

            return tenantInfo;
        }
    }
}
