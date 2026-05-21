using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ZooTech.Application.Common.Gateway.Tenant;
using ZooTech.Infrastructure.Persistence.Context;

namespace ZooTech.Infrastructure.Tenant
{
    public class TenantStore : ITenantStore
    {
        private readonly TenantCatalogDb _tenantCatalogDb;
        private readonly IMemoryCache _cache;

        public TenantStore(TenantCatalogDb tenantCatalogDb , IMemoryCache cache)
        {
            _tenantCatalogDb = tenantCatalogDb;
            _cache = cache;
        }
        public async Task<TenantInfo?> GetBySubDomainAsync(string subDomain)
        {
            var cacheKey = $"tenant:{subDomain}";

            if(_cache.TryGetValue(cacheKey, out TenantInfo tenantCached))
            {
                return tenantCached;
            }

            var tenantInDb = await _tenantCatalogDb.TenantEntity
                .AsNoTracking() // Esto usalo para mejorar rendimiento cuando solo vas a consultar datos
                .Include(t => t.Address)
                .Include(t => t.TenantDatabaseConnection)
                .FirstOrDefaultAsync(t => t.SubDomain == subDomain);

            if(tenantInDb == null || tenantInDb.Status.Equals("INACTIVE") || !tenantInDb.TenantDatabaseConnection.IsActive)
            {
                return null;
            }

            var tenantInfo = new TenantInfo
            {
                Id = tenantInDb.Id,
                SubDomain = tenantInDb.SubDomain,
                Code = tenantInDb.Code,
                DatabaseName = tenantInDb.TenantDatabaseConnection.DatabaseName,
                Status = tenantInDb.Status,
                Email = tenantInDb.Email
            };

            _cache.Set(cacheKey, tenantInfo, TimeSpan.FromMinutes(5));

            return tenantInfo;
        }
    }
}