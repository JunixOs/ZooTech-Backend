using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ZooTech.Application.Common.Gateway.Tenant;
using ZooTech.Domain.Enums;
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

            var tenantInDb = await _tenantCatalogDb.tenants
                .AsNoTracking()
                .Include(t => t.address)
                .Include(t => t.tenant_database_connection)
                .FirstOrDefaultAsync(t => t.subdomain == subDomain);

            if (tenantInDb == null || tenantInDb.status == TenantStatus.INACTIVE.ToString() || !tenantInDb.tenant_database_connection.is_active)
            {
                return null;
            }

            var tenantInfo = new TenantInfo
            {
                Id = tenantInDb.id,
                SubDomain = tenantInDb.subdomain,
                Code = tenantInDb.code,
                DatabaseName = tenantInDb.tenant_database_connection.database_name,
                Status = Enum.Parse<TenantStatus>(tenantInDb.status),
                Email = tenantInDb.email
            };

            _cache.Set(cacheKey, tenantInfo, TimeSpan.FromMinutes(5));

            return tenantInfo;
        }
    }
}
