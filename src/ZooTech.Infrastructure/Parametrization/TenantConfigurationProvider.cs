using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using ZooTech.Application.Common.Gateway.Caching;
using ZooTech.Application.Common.Gateway.Parametrization;
using ZooTech.Application.Common.Gateway.Repositories.Parametrization;
using ZooTech.Domain.Configuration;

namespace ZooTech.Infrastructure.Parametrization;

public sealed class TenantConfigurationProvider : ITenantConfigurationProvider
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(5);

    private readonly IMemoryCache _memoryCache;
    private readonly IAppCacheService _cache;
    private readonly ITenantConfigurationRepository _repository;

    public TenantConfigurationProvider(
        IMemoryCache memoryCache,
        IAppCacheService cache,
        ITenantConfigurationRepository repository)
    {
        _memoryCache = memoryCache;
        _cache = cache;
        _repository = repository;
    }

    public async Task<T> GetSettingAsync<T>(
        int tenantId, SettingDefinition<T> setting)
    {
        var config = await GetConfigAsync(tenantId);

        if (config.Settings.TryGetValue(setting.Code, out var raw) && raw is not null)
        {
            if (typeof(T) == typeof(string))
                return (T)(object)raw;
            return JsonSerializer.Deserialize<T>(raw)!;
        }

        return default!;
    }

    public async Task<bool> IsFeatureEnabledAsync(
        int tenantId, FeatureCode feature)
    {
        var config = await GetConfigAsync(tenantId);
        return config.EnabledFeatures.Contains(feature.Value);
    }

    public async Task<bool> IsRuleEnabledAsync(
        int tenantId, RuleCode rule)
    {
        var config = await GetConfigAsync(tenantId);
        return config.EnabledRules.Contains(rule.Value);
    }

    public async Task InvalidateTenantAsync(int tenantId)
    {
        var key = BuildKey(tenantId);
        _memoryCache.Remove(key);
        await _cache.RemoveByKeyAsync(key);
    }

    private async Task<TenantConfiguration> GetConfigAsync(int tenantId)
    {
        var key = BuildKey(tenantId);

        if (_memoryCache.TryGetValue(key, out TenantConfiguration? cached))
            return cached!;

        var (found, distributed) = await _cache.TryGetAsync<TenantConfiguration>(key);
        if (found)
        {
            _memoryCache.Set(key, distributed, CacheTtl);
            return distributed!;
        }

        var config = await _repository.LoadTenantConfigAsync(tenantId);

        _memoryCache.Set(key, config, CacheTtl);
        await _cache.GetOrCreateAsync(key, () => Task.FromResult(config), CacheTtl);

        return config;
    }

    private static string BuildKey(int tenantId) => $"tenant:config:{tenantId}";
}
