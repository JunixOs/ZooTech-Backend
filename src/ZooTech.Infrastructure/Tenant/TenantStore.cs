using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ZooTech.Application.Common.Gateway.Tenant;
using ZooTech.Domain.Admin.Enums;

namespace ZooTech.Infrastructure.Tenant
{
    public class TenantStore : ITenantStore
    {
        private readonly ITenantDbContextFactory _tenantDbContextFactory;
        private readonly IMemoryCache _cache;

        public TenantStore(
            ITenantDbContextFactory tenantDbContextFactory, 
            IMemoryCache cache
        )
        {
            _tenantDbContextFactory = tenantDbContextFactory;
            _cache = cache;
        }

        public async Task<TenantInfo?> GetBySubDomainAsync(string subDomain)
        {
            var cacheKey = $"tenant:{subDomain}";

            if (_cache.TryGetValue(cacheKey, out TenantInfo? tenantCached))
            {
                return tenantCached;
            }

            var tenantDbContext = await _tenantDbContextFactory.CreateDbContextBySettingsValue();

            var tenantInfo = await tenantDbContext.tenants
                .AsNoTracking()
                .Where(t => t.subdomain == subDomain)
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
