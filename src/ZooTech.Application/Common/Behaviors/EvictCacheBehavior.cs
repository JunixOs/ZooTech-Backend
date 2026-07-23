using System;
using System.Threading.Tasks;
using ZooTech.Application.Common.Caching;
using ZooTech.Application.Common.Gateway.Caching;

namespace ZooTech.Application.Common.Behaviors;

public sealed class EvictCacheBehavior<TRequest, TResponse> : IBehavior<TRequest, TResponse>
{
    private readonly IAppCacheService _cacheService;

    public EvictCacheBehavior(IAppCacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task<TResponse> Handle(TRequest request, Func<Task<TResponse>> next)
    {
        var response = await next();

        if (request is IEvictCacheRequest evictable)
        {
            foreach (var prefix in evictable.GetCachePrefixesToEvict())
            {
                await _cacheService.RemoveByPrefixAsync(prefix);
            }
        }

        return response;
    }
}
