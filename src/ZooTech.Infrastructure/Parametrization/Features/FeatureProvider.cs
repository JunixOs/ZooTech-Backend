using ZooTech.Application.Common.Gateway.Caching;
using ZooTech.Application.Common.Gateway.Parametrization.Features;
using ZooTech.Application.Common.Gateway.Repositories.Parametrization;
using ZooTech.Domain.Configuration;

namespace ZooTech.Infrastructure.Parametrization.Features;

public sealed class FeatureProvider : IFeatureProvider
{
    private readonly IAppCacheService _cache;
    private readonly IFeatureRepository _repository;

    public FeatureProvider(IAppCacheService cache, IFeatureRepository repository)
    {
        _cache = cache;
        _repository = repository;
    }

    public async Task<bool> IsEnabledAsync(int tenantId, FeatureCode feature)
    {
        var enabled = await GetEnabledFeatureCodesAsync(tenantId);
        return enabled.Contains(feature.Value);
    }

    private async Task<HashSet<string>> GetEnabledFeatureCodesAsync(int tenantId)
    {
        var key = BuildKey(tenantId);

        var (found, cached) = await _cache.TryGetAsync<HashSet<string>>(key);
        if (found)
            return cached!;

        var codes = await _repository.GetEnabledFeatureCodesAsync(tenantId);
        await _cache.GetOrCreateAsync(key, () => Task.FromResult(codes));

        return codes;
    }

    private static string BuildKey(int tenantId) => $"features:tenant:{tenantId}";
}
