using System.Text.Json;
using ZooTech.Application.Common.Gateway.Caching;
using ZooTech.Application.Common.Gateway.Parametrization.Settings;
using ZooTech.Application.Common.Gateway.Repositories.Parametrization;
using ZooTech.Domain.Configuration;

namespace ZooTech.Infrastructure.Parametrization.Settings;

public sealed class SettingsProvider : ISettingsProvider
{
    private readonly IAppCacheService _cache;
    private readonly ISettingsRepository _repository;

    public SettingsProvider(IAppCacheService cache, ISettingsRepository repository)
    {
        _cache = cache;
        _repository = repository;
    }

    public async Task<T> GetAsync<T>(int tenantId, SettingDefinition<T> setting)
    {
        var snapshot = await GetSnapshotAsync(tenantId);

        if (snapshot.Values.TryGetValue(setting.Code, out var raw) && raw is not null)
            return JsonSerializer.Deserialize<T>(raw)!;

        return default!;
    }

    public async Task RefreshAsync(int tenantId)
    {
        await _cache.RemoveByKeyAsync(BuildKey(tenantId));
    }

    private async Task<SettingsSnapshot> GetSnapshotAsync(int tenantId)
    {
        var key = BuildKey(tenantId);

        var (found, cached) = await _cache.TryGetAsync<SettingsSnapshot>(key);
        if (found)
            return cached!;

        var dbValues = await _repository.GetTenantSettingsAsync(tenantId);

        var snapshot = new SettingsSnapshot
        {
            Values = dbValues.ToDictionary(x => x.Code, x => x.Value)
        };

        await _cache.GetOrCreateAsync(key, () => Task.FromResult(snapshot));

        return snapshot;
    }

    private static string BuildKey(int tenantId) => $"settings:tenant:{tenantId}";
}
