# Plan: Tenant Configuration Caching — Control Plane DB → Garnet/Redis

**Date:** 15-06-2026  
**Status:** Analysis & Proposal  
**Scope:** Unify Settings, Features, Rules into a single cached tenant configuration snapshot

---

## 1. Current State Analysis

### 1.1 Fragmented Providers

Today there are **three independent providers**, each with its own cache key and cache miss → DB trip:

| Provider | Cache Key Pattern | Cache TTL | DB Repository |
|---|---|---|---|
| `SettingsProvider` | `settings:tenant:{id}` | 3 min (Garnet default) | `ISettingsRepository` (stub — throws `NotImplementedException`) |
| `FeatureProvider` | `features:tenant:{id}` | 3 min (Garnet default) | `IFeatureRepository` (stub) |
| `RuleProvider` | `rules:tenant:{id}` | 3 min (Garnet default) | `IRuleRepository` (stub) |

**Problems:**
- Up to **3 cache round-trips** per request (one per provider)
- Up to **3 DB trips** on cold start (one per provider cache miss)
- No atomicity — tenant config can be partially cached (settings in cache, features missing)
- `RefreshAsync` only invalidates settings — features and rules have no refresh method
- `IRuleRepository` has no corresponding `RefreshAsync` or invalidation method in `IRuleProvider`

### 1.2 Current Cache API

```csharp
public interface IAppCacheService
{
    Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory);
    Task<(bool Found, T? Value)> TryGetAsync<T>(string key);
    Task RemoveByKeyAsync(string key);
}
```

The `GetOrCreateAsync` uses a **default 3-minute TTL** set at construction time (`GarnetCacheService`). No per-key TTL is possible with the current interface.

### 1.3 Missing Pieces

| Feature | Status |
|---|---|
| Unified tenant config model | ❌ 3 separate models |
| Single cache entry per tenant | ❌ 3 cache entries |
| 5-minute cache TTL | ❌ 3 min hardcoded (configurable) |
| DB-backed repository for settings | ❌ Stub |
| DB-backed repository for features | ❌ Stub |
| DB-backed repository for rules | ❌ Stub |
| Cache invalidation on config update | Partial (settings only) |
| Multi-instance cache sync (Pub/Sub) | ❌ Not implemented |

---

## 2. Proposed Architecture: Unified Tenant Configuration Service

### 2.1 Single Snapshot Model

Replace three providers with one `ITenantConfigurationProvider`:

```csharp
public sealed record TenantConfiguration
{
    public int TenantId { get; init; }
    public Dictionary<string, string> Settings { get; init; } = new();
    public HashSet<string> EnabledFeatures { get; init; } = new();
    public HashSet<string> EnabledRules { get; init; } = new();
    public DateTime LoadedAt { get; init; }
}
```

### 2.2 Unified Interface

```csharp
public interface ITenantConfigurationProvider
{
    Task<T> GetSettingAsync<T>(int tenantId, SettingDefinition<T> setting);
    Task<bool> IsFeatureEnabledAsync(int tenantId, FeatureCode feature);
    Task<bool> IsRuleEnabledAsync(int tenantId, RuleCode rule);
    Task InvalidateTenantAsync(int tenantId);
}
```

### 2.3 Single Cache Key

```
tenant:config:{tenantId}
```

One key. One cache entry. One JSON blob. One DB query.

### 2.4 Implementation Flow

```text
Application Layer
    │
    ▼
ITenantConfigurationProvider
    │
    ▼
TenantConfigurationProvider
    │
    ├── Check Garnet Cache ──► HIT ──► return TenantConfiguration
    │                              (5 min TTL)
    │
    └── MISS ──► TenantConfigurationRepository
                     │
                     ├── Query setting_definitions
                     │     + setting_values (by tenant)
                     ├── Query features
                     │     + tenant_features (by tenant)
                     ├── Query rule_definitions
                     │     + tenant_business_rules (by tenant)
                     │
                     └──► Merge → cache → return
```

---

## 3. Cache Strategy

### 3.1 TTL: 5 Minutes

```csharp
private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(5);
```

The `IAppCacheService` interface currently does not support per-key TTL. Two options:

**Option A (recommended):** Extend `IAppCacheService` with a TTL overload:

```csharp
public interface IAppCacheService
{
    Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory);
    Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan ttl);
    Task<(bool Found, T? Value)> TryGetAsync<T>(string key);
    Task RemoveByKeyAsync(string key);
}
```

The Garnet/Redis implementations use `_expirationTimeSpan` when calling `StringSetAsync`. Adding a TTL parameter is straightforward.

**Option B:** Accept the default TTL from `GarnetCacheService` configuration (currently 3 min). Configure it to 5 min via `appsettings.json`:

```json
{
  "Garnet": {
    "ExpirationTime": "05:00"
  }
}
```

**Recommendation:** Option A for flexibility, but Option B is viable short-term.

### 3.2 Single DB Query per Tenant

The repository executes a single transaction:

```sql
-- 1. Settings with tenant overrides
SELECT
    sd.code,
    COALESCE(sv.value, sd.default_value) AS value
FROM setting_definitions sd
LEFT JOIN setting_groups sg ON sd.setting_group_id = sg.id
LEFT JOIN setting_values sv
    ON sv.setting_definition_id = sd.id
    AND sv.tenant_id = @tenantId
    AND sv.actor_type = 'TENANT'
    AND sv.actor_id IS NULL
    AND sv.deleted_at IS NULL
WHERE sd.is_active = 1
  AND sd.deleted_at IS NULL
  AND sg.is_active = 1
  AND sg.deleted_at IS NULL;

-- 2. Enabled features for tenant
SELECT f.code
FROM features f
INNER JOIN tenant_features tf
    ON tf.feature_id = f.id
    AND tf.tenant_id = @tenantId
WHERE f.is_active = 1
  AND f.deleted_at IS NULL
  AND tf.is_enabled = 1;

-- 3. Enabled rules for tenant
SELECT rd.code
FROM rule_definitions rd
INNER JOIN tenant_business_rules tbr
    ON tbr.rule_definition_id = rd.id
    AND tbr.tenant_id = @tenantId
WHERE rd.is_active = 1
  AND rd.deleted_at IS NULL
  AND tbr.is_active = 1;
```

This can be done in a single Dapper `QueryMultipleAsync` call, or three sequential calls inside a transaction.

### 3.3 Cache-First, DB-Second

```csharp
public async Task<TenantConfiguration> GetConfigAsync(int tenantId)
{
    var key = BuildKey(tenantId);

    // 1. Try cache
    var (found, cached) = await _cache.TryGetAsync<TenantConfiguration>(key);
    if (found)
        return cached!;

    // 2. Cache miss → load from DB
    var config = await _repository.LoadTenantConfigAsync(tenantId);

    // 3. Store in cache with 5-min TTL
    await _cache.GetOrCreateAsync(key, () => Task.FromResult(config), CacheTtl);

    return config;
}
```

---

## 4. Cache Invalidation

### 4.1 Single-Instance Invalidation

When a setting/feature/rule is updated for a tenant:

```csharp
public async Task InvalidateTenantAsync(int tenantId)
{
    await _cache.RemoveByKeyAsync(BuildKey(tenantId));
}
```

Next read triggers a cache miss, loads fresh data from DB, repopulates cache.

### 4.2 Multi-Instance Invalidation (Pub/Sub)

For horizontal scaling, use Redis Pub/Sub:

```csharp
// Publisher (called after DB write)
public async Task InvalidateTenantAsync(int tenantId)
{
    await _cache.RemoveByKeyAsync(BuildKey(tenantId));
    var subscriber = _redis.GetSubscriber();
    await subscriber.PublishAsync("tenant:config:invalidate", tenantId.ToString());
}

// Subscriber (BackgroundService on each instance)
public class ConfigInvalidationSubscriber : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        var subscriber = _redis.GetSubscriber();
        await subscriber.SubscribeAsync("tenant:config:invalidate", (_, message) =>
        {
            if (int.TryParse(message, out var tenantId))
            {
                _cache.RemoveByKeyAsync(BuildKey(tenantId)).GetAwaiter().GetResult();
            }
        });
    }
}
```

### 4.3 Update Flow

```text
Admin → PUT /api/tenants/{id}/settings/{code}
    │
    ├── 1. Save to DB (setting_values)
    ├── 2. _cache.RemoveByKeyAsync("tenant:config:{id}")
    └── 3. Redis.Publish("tenant:config:invalidate", "{id}")
              │
              ├── Instance A: L1 evicted via step 2
              └── Instance B: RedisSyncSubscriber evicts L2
```

---

## 5. Migration Path

### Phase 1 — Unified Model & Repository (no breaking changes)
1. Create `TenantConfiguration` record
2. Create `ITenantConfigurationRepository` that loads settings + features + rules in one DB call
3. Create Dapper-based `TenantConfigurationRepository` implementation querying Control Plane DB
4. Register in DI alongside existing providers

### Phase 2 — Unified Provider
5. Create `TenantConfigurationProvider` implementing `ITenantConfigurationProvider`
6. Internal cache with 5-min TTL + fallback to repository
7. Keep existing `SettingsProvider`/`FeatureProvider`/`RuleProvider` for backward compatibility
8. Gradually migrate consumers to `ITenantConfigurationProvider`

### Phase 3 — Deprecate Fragmented Providers
9. Once all consumers use `ITenantConfigurationProvider`, mark old providers as `[Obsolete]`
10. Remove old providers and their repository stubs
11. Clean up: remove `ISettingsRepository`, `IFeatureRepository`, `IRuleRepository` (replaced by unified repo)

### Phase 4 — Multi-Instance Sync
12. Add `IConnectionMultiplexer` dependency to provider
13. Implement Pub/Sub invalidation
14. Add `ConfigInvalidationSubscriber` as `IHostedService`

---

## 6. Implementation Details

### 6.1 New/Modified Files

| File | Action |
|---|---|
| `src/ZooTech.Domain/Configuration/TenantConfiguration.cs` | **Create** — unified snapshot record |
| `src/ZooTech.Application/Common/Gateway/Parametrization/ITenantConfigurationProvider.cs` | **Create** — unified interface |
| `src/ZooTech.Application/Common/Gateway/Repositories/Parametrization/ITenantConfigurationRepository.cs` | **Create** — unified repository interface |
| `src/ZooTech.Infrastructure/Parametrization/TenantConfigurationProvider.cs` | **Create** — cache-first implementation |
| `src/ZooTech.Infrastructure/Parametrization/TenantConfigurationRepository.cs` | **Create** — Dapper-based DB access |
| `src/ZooTech.Infrastructure/Parametrization/ConfigInvalidationSubscriber.cs` | **Create** — Redis Pub/Sub listener (IHostedService) |
| `src/ZooTech.Application/Common/Gateway/Caching/IAppCacheService.cs` | **Modify** — add `GetOrCreateAsync` with TTL overload |
| `src/ZooTech.Infrastructure/Caching/Garnet/GarnetCacheService.cs` | **Modify** — implement TTL overload |
| `src/ZooTech.Infrastructure/DependencyInjection.cs` | **Modify** — register new unified services |
| `src/ZooTech.Application/Common/Gateway/Repositories/Parametrization/ISettingsRepository.cs` | **Keep** (mark `[Obsolete]` in Phase 3) |
| `src/ZooTech.Application/Common/Gateway/Repositories/Parametrization/IFeatureRepository.cs` | **Keep** (mark `[Obsolete]` in Phase 3) |
| `src/ZooTech.Application/Common/Gateway/Repositories/Parametrization/IRuleRepository.cs` | **Keep** (mark `[Obsolete]` in Phase 3) |
| `src/ZooTech.Infrastructure/Parametrization/Settings/SettingsProvider.cs` | **Keep** (mark `[Obsolete]` in Phase 3) |
| `src/ZooTech.Infrastructure/Parametrization/Features/FeatureProvider.cs` | **Keep** (mark `[Obsolete]` in Phase 3) |
| `src/ZooTech.Infrastructure/Parametrization/Rules/RuleProvider.cs` | **Keep** (mark `[Obsolete]` in Phase 3) |

### 6.2 IAppCacheService — TTL Overload

```csharp
public interface IAppCacheService
{
    Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory);
    Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan ttl);
    Task<(bool Found, T? Value)> TryGetAsync<T>(string key);
    Task RemoveByKeyAsync(string key);
}
```

GarnetCacheService implementation:

```csharp
public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan ttl)
{
    var (found, value) = await TryGetAsync<T>(key);
    if (found)
        return value!;

    var result = await factory();
    var db = _garnetCacheConnection.GetDatabase();
    var serialized = JsonSerializer.Serialize(result);
    await db.StringSetAsync(key, serialized, ttl);
    return result;
}
```

### 6.3 TenantConfigurationProvider — Core Logic

```csharp
public sealed class TenantConfigurationProvider : ITenantConfigurationProvider
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(5);

    private readonly IAppCacheService _cache;
    private readonly ITenantConfigurationRepository _repository;

    public TenantConfigurationProvider(
        IAppCacheService cache,
        ITenantConfigurationRepository repository)
    {
        _cache = cache;
        _repository = repository;
    }

    public async Task<T> GetSettingAsync<T>(
        int tenantId, SettingDefinition<T> setting)
    {
        var config = await GetConfigAsync(tenantId);

        if (config.Settings.TryGetValue(setting.Code, out var raw) && raw is not null)
            return JsonSerializer.Deserialize<T>(raw)!;

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
        await _cache.RemoveByKeyAsync(BuildKey(tenantId));
    }

    private async Task<TenantConfiguration> GetConfigAsync(int tenantId)
    {
        var key = BuildKey(tenantId);

        var (found, cached) = await _cache.TryGetAsync<TenantConfiguration>(key);
        if (found)
            return cached!;

        var config = await _repository.LoadTenantConfigAsync(tenantId);
        await _cache.GetOrCreateAsync(key, () => Task.FromResult(config), CacheTtl);

        return config;
    }

    private static string BuildKey(int tenantId) => $"tenant:config:{tenantId}";
}
```

---

## 7. Risks and Mitigations

| Risk | Impact | Mitigation |
|---|---|---|
| **Stale cache after DB write** | Tenant reads old config for up to 5 min | Call `InvalidateTenantAsync` after every DB write; Pub/Sub for multi-instance |
| **Cache stampede on cold start** | Multiple requests hit DB simultaneously for same tenant | Use `SemaphoreSlim` per key or rely on Garnet/Redis single-flight |
| **Large tenant config (>1MB)** | Increased memory/bandwidth | Compress JSON; paginate if >1000 settings per tenant |
| **`IAppCacheService` interface change** | Breaks existing consumers | Add TTL overload as `default` parameter; existing callers use default 3-min TTL |
| **Repository stub → real DB** | Connection string, schema mismatches | Use Dapper with same queries as T4/CodeGeneration; test against dev DB |

---

## 8. Test Plan

### Unit Tests (new `Infrastructure.UnitTests`)

| Test | Description |
|---|---|
| `GetConfigAsync_CacheHit_ReturnsCachedConfig` | Cache has valid entry → no DB call |
| `GetConfigAsync_CacheMiss_LoadsFromRepo` | Cache miss → repository called → result cached |
| `GetSettingAsync_SettingFound_ReturnsTypedValue` | Correct deserialization for `int`, `bool`, `string` |
| `GetSettingAsync_SettingNotFound_ReturnsDefault` | Missing code returns `default!` |
| `IsFeatureEnabledAsync_FeatureInSet_ReturnsTrue` | Code present in `EnabledFeatures` |
| `IsFeatureEnabledAsync_FeatureNotInSet_ReturnsFalse` | Code absent |
| `IsRuleEnabledAsync_RuleInSet_ReturnsTrue` | Same pattern |
| `InvalidateTenantAsync_RemovesCacheEntry` | `RemoveByKeyAsync` called with correct key |

### Integration Tests (new `Infrastructure.IntegrationTests`)

| Test | Description |
|---|---|
| `Repository_LoadsFullConfig_FromControlPlaneDB` | Requires real DB; verifies all 3 sections |
| `Provider_EndToEnd_CacheThenDB` | Cold → hot → invalidate → cold cycle |
| `MultiInstance_InvalidationViaPubSub` | Requires 2 simulated instances; verifies cross-instance eviction |

---

## 9. Decision Record

| Decision | Option Chosen | Rationale |
|---|---|---|
| **Unified vs fragmented** | Unified (`TenantConfiguration`) | 1 cache key, 1 DB trip, atomic snapshots |
| **Cache TTL** | 5 minutes | Balance between freshness and DB load |
| **Per-key TTL vs global config** | Per-key TTL (extend `IAppCacheService`) | Future-proof; allows different TTLs for different data types |
| **Repository tech** | Dapper (not EF Core) | Read-only; Dapper matches existing CodeGeneration pattern; avoids EF Core overhead |
| **Serialization** | System.Text.Json | Already used by GarnetCacheService |
| **Multi-instance sync** | Redis Pub/Sub (Phase 4) | Required only when horizontally scaling; not needed for single-instance deployments |
