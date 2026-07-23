using System;
using System.Threading.Tasks;
using ZooTech.Application.Common.Caching;
using ZooTech.Application.Common.Gateway.Caching;
using ZooTech.Application.Common.Gateway.Parametrization;

namespace ZooTech.Application.Common.Behaviors;

public sealed class QueryCacheBehavior<TRequest, TResponse> : IBehavior<TRequest, TResponse>
{
    private readonly IAppCacheService _cacheService;
    private readonly ITenantConfigurationProvider _tenantConfigurationProvider;

    public QueryCacheBehavior(
        IAppCacheService cacheService,
        ITenantConfigurationProvider tenantConfigurationProvider)
    {
        _cacheService = cacheService;
        _tenantConfigurationProvider = tenantConfigurationProvider;
    }

    public async Task<TResponse> Handle(TRequest request, Func<Task<TResponse>> next)
    {
        if (request is ICacheableRequest cacheable)
        {
            var key = await cacheable.GetCacheKeyAsync(_tenantConfigurationProvider);
            var ttl = cacheable.GetTtl();

            return ttl.HasValue
                ? await _cacheService.GetOrCreateAsync(key, next, ttl.Value)
                : await _cacheService.GetOrCreateAsync(key, next);
        }

        return await next();
    }
}
