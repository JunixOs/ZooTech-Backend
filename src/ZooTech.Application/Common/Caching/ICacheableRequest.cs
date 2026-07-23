using ZooTech.Application.Common.Gateway.Parametrization;

namespace ZooTech.Application.Common.Caching;

public interface ICacheableRequest
{
    Task<string> GetCacheKeyAsync(ITenantConfigurationProvider tenantConfigurationProvider);
    TimeSpan? GetTtl() => null;
}
