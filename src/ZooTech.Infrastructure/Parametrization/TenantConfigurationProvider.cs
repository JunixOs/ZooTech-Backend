using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using ZooTech.Application.Common.Gateway.Caching;
using ZooTech.Application.Common.Gateway.Context;
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

    private readonly int CurrentTenantId;

    public TenantConfigurationProvider(
        IMemoryCache memoryCache,
        IAppCacheService cache,
        ITenantConfigurationRepository repository,

        ITenantContext tenantContext
    )
    {
        _memoryCache = memoryCache;
        _cache = cache;
        _repository = repository;

        CurrentTenantId = tenantContext.TenantId;
    }

    public async Task<T> GetSettingAsync<T>(SettingDefinition<T> setting)
    {
        var config = await GetConfigAsync();

        if (config.Settings.TryGetValue(setting.Code, out var raw) && raw is not null)
        {
            try
            {
                using var doc = JsonDocument.Parse(raw);
                if (doc.RootElement.ValueKind == JsonValueKind.Object && doc.RootElement.TryGetProperty("value", out var valElement))
                {
                    if (typeof(T) == typeof(string))
                    {
                        var strVal = valElement.ValueKind == JsonValueKind.String ? valElement.GetString() : valElement.GetRawText();
                        return (strVal == null ? default! : (T)(object)strVal);
                    }
                    return JsonSerializer.Deserialize<T>(valElement.GetRawText())!;
                }
            }
            catch (JsonException)
            {
                // Si no se puede parsear como JSON, continuar con la deserialización estándar
            }

            if (typeof(T) == typeof(string))
                return (T)(object)raw;
            return JsonSerializer.Deserialize<T>(raw)!;
        }

        return default!;
    }

    public async Task<bool> IsFeatureEnabledAsync(FeatureCode feature)
    {
        var config = await GetConfigAsync();
        return config.EnabledFeatures.Contains(feature.Value);
    }

    public async Task<bool> IsRuleEnabledAsync(RuleCode rule)
    {
        var config = await GetConfigAsync();
        return config.EnabledRules.Contains(rule.Value);
    }

    public async Task InvalidateTenantAsync()
    {
        var key = BuildKey(CurrentTenantId);
        _memoryCache.Remove(key);
        await _cache.RemoveByKeyAsync(key);
    }

    private async Task<TenantConfiguration> GetConfigAsync()
    {
        var key = BuildKey(CurrentTenantId);

        if (_memoryCache.TryGetValue(key, out TenantConfiguration? cached))
            return cached!;

        var (found, distributed) = await _cache.TryGetAsync<TenantConfiguration>(key);
        if (found)
        {
            _memoryCache.Set(key, distributed, CacheTtl);
            return distributed!;
        }

        var config = await _repository.LoadTenantConfigAsync(CurrentTenantId);

        _memoryCache.Set(key, config, CacheTtl);
        await _cache.GetOrCreateAsync(key, () => Task.FromResult(config), CacheTtl);

        return config;
    }

    private static string BuildKey(int tenantId) => $"tenant:config:{tenantId}";
}
