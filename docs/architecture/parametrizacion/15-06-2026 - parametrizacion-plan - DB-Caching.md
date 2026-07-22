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
    Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? ttl);
    Task<(bool Found, T? Value)> TryGetAsync<T>(string key);
    Task RemoveByKeyAsync(string key);
}
```

The Garnet/Redis implementations use `_expirationTimeSpan` when calling `StringSetAsync`. Adding a TTL parameter is straightforward.

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

### 3.4 Two-Layer Cache (L1/L2)

The unified provider uses a two-layer cache to balance performance and consistency:

| Layer | Technology | Scope | Latency |
|---|---|---|---|
| **L1** | `IMemoryCache` (in-process) | Per application instance | ~0.01 ms |
| **L2** | Garnet/Redis (distributed) | Shared across all instances | ~0.5–2 ms (network) |

**Access pattern:** L1 → L2 → DB

```csharp
private async Task<TenantConfiguration> GetConfigAsync(int tenantId)
{
    var key = BuildKey(tenantId);

    // 1. L1: Check in-memory cache (near-instant, no network)
    if (_memoryCache.TryGetValue(key, out TenantConfiguration? cached))
        return cached!;

    // 2. L2: Check distributed cache (Garnet)
    var (found, distributed) = await _cache.TryGetAsync<TenantConfiguration>(key);
    if (found)
    {
        _memoryCache.Set(key, distributed, CacheTtl);
        return distributed!;
    }

    // 3. Cache miss → load from DB
    var config = await _repository.LoadTenantConfigAsync(tenantId);

    // 4. Store in both layers
    _memoryCache.Set(key, config, CacheTtl);
    await _cache.GetOrCreateAsync(key, () => Task.FromResult(config), CacheTtl);

    return config;
}
```

**Why two layers?** Garnet is a network call (~0.5–2 ms). L1 in-memory cache is ~0.01 ms. For high-throughput tenant configuration reads (every request needs settings/features/rules), L1 eliminates the network latency on cache hits. L2 provides cross-instance sharing and the Pub/Sub invalidation bus.

---

## 4. Cache Invalidation

### 4.1 Single-Instance Invalidation

When a setting/feature/rule is updated for a tenant:

```csharp
public async Task InvalidateTenantAsync(int tenantId)
{
    var key = BuildKey(tenantId);
    // Evict L1 (in-memory cache)
    _memoryCache.Remove(key);
    // Evict L2 (distributed Garnet cache)
    await _cache.RemoveByKeyAsync(key);
}
```

Next read triggers a cascade miss (L1 → L2 → DB), loads fresh data from DB, repopulates both layers.

### 4.2 Multi-Instance Invalidation (Pub/Sub)

For horizontal scaling, use Redis Pub/Sub:

```csharp
// Publisher (called after DB write)
public async Task InvalidateTenantAsync(int tenantId)
{
    var key = BuildKey(tenantId);
    _memoryCache.Remove(key);                                          // Evict L1
    await _cache.RemoveByKeyAsync(key);                                // Evict L2
    var subscriber = _redis.GetSubscriber();
    await subscriber.PublishAsync("tenant:config:invalidate", tenantId.ToString()); // Notify peers
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
                // Evict L1 only — L2 (Garnet) was already evicted by the writer instance
                _memoryCache.Remove(BuildKey(tenantId));
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
    ├── 2. Invalidate L1: _memoryCache.Remove("tenant:config:{id}")
    ├── 3. Invalidate L2: _cache.RemoveByKeyAsync("tenant:config:{id}")
    └── 4. Redis.Publish("tenant:config:invalidate", "{id}")
              │
              ├── Instance A: L1 + L2 evicted locally via steps 2 & 3
              └── Instance B: ConfigInvalidationSubscriber evicts L1 only
                                (L2 already evicted by Instance A — shared Garnet)
```

---

## 5. Migration Path

### Phase 1 — Unified Model & Repository (no breaking changes)
1. Create `TenantConfiguration` record
2. Create `ITenantConfigurationRepository` that loads settings + features + rules in one DB call
3. Create Dapper-based `TenantConfigurationRepository` implementation querying Control Plane DB
4. Register in DI alongside existing providers

### Phase 2 — Unified Provider with L1/L2 Cache
5. Create `TenantConfigurationProvider` implementing `ITenantConfigurationProvider`
6. L1 in-memory cache (`IMemoryCache`) + L2 distributed cache (Garnet, 5-min TTL) + fallback to repository
7. Register `TenantConfigurationProvider` in DI (requires `IMemoryCache` already registered via `AddMemoryCache()` in `Program.cs`)
8. Keep existing `SettingsProvider`/`FeatureProvider`/`RuleProvider` for backward compatibility
9. Gradually migrate consumers to `ITenantConfigurationProvider`

### Phase 3 — Deprecate Fragmented Providers
10. Once all consumers use `ITenantConfigurationProvider`, mark old providers as `[Obsolete]`
11. Remove old providers and their repository stubs
12. Clean up: remove `ISettingsRepository`, `IFeatureRepository`, `IRuleRepository` (replaced by unified repo)

### Phase 4 — Multi-Instance Sync
13. Add `IConnectionMultiplexer` dependency to provider for Pub/Sub
14. Implement Pub/Sub invalidation (publisher removes L1 + L2, subscriber evicts L1 only)
15. Add `ConfigInvalidationSubscriber` as `IHostedService`

---

## 6. Implementation Details

### 6.1 New/Modified Files

| File | Action |
|---|---|
| `src/ZooTech.Domain/Configuration/TenantConfiguration.cs` | **Create** — unified snapshot record |
| `src/ZooTech.Application/Common/Gateway/Parametrization/ITenantConfigurationProvider.cs` | **Create** — unified interface |
| `src/ZooTech.Application/Common/Gateway/Repositories/Parametrization/ITenantConfigurationRepository.cs` | **Create** — unified repository interface |
| `src/ZooTech.Infrastructure/Parametrization/TenantConfigurationProvider.cs` | **Create** — L1 (IMemoryCache) + L2 (Garnet) cache-first implementation |
| `src/ZooTech.Infrastructure/Parametrization/TenantConfigurationRepository.cs` | **Create** — Dapper-based DB access |
| `src/ZooTech.Infrastructure/Parametrization/ConfigInvalidationSubscriber.cs` | **Create** — Redis Pub/Sub listener, evicts L1 only (IHostedService) |
| `src/ZooTech.Application/Common/Gateway/Caching/IAppCacheService.cs` | **Modify** — add `GetOrCreateAsync` with TTL overload |
| `src/ZooTech.Infrastructure/Caching/Garnet/GarnetCacheService.cs` | **Modify** — implement TTL overload |
| `src/ZooTech.Infrastructure/DependencyInjection.cs` | **Modify** — register new unified services (`IMemoryCache` already registered via `AddMemoryCache()` in `Program.cs`) |
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

### 6.3 TenantConfigurationProvider — Core Logic (with L1/L2)

```csharp
public sealed class TenantConfigurationProvider : ITenantConfigurationProvider
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(5);

    private readonly IMemoryCache _memoryCache;    // L1: in-process
    private readonly IAppCacheService _cache;       // L2: distributed Garnet
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
        var key = BuildKey(tenantId);
        // Evict L1 (in-memory)
        _memoryCache.Remove(key);
        // Evict L2 (distributed Garnet — shared across instances)
        await _cache.RemoveByKeyAsync(key);
    }

    private async Task<TenantConfiguration> GetConfigAsync(int tenantId)
    {
        var key = BuildKey(tenantId);

        // 1. L1: Check in-memory cache (near-instant, no network)
        if (_memoryCache.TryGetValue(key, out TenantConfiguration? cached))
            return cached!;

        // 2. L2: Check distributed cache (Garnet)
        var (found, distributed) = await _cache.TryGetAsync<TenantConfiguration>(key);
        if (found)
        {
            _memoryCache.Set(key, distributed, CacheTtl);
            return distributed!;
        }

        // 3. Cache miss → load from DB
        var config = await _repository.LoadTenantConfigAsync(tenantId);

        // 4. Store in both layers
        _memoryCache.Set(key, config, CacheTtl);
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
| **Stale L1 after another instance writes** | Instance B serves stale config from its L1 even though L2 was evicted by Instance A | Pub/Sub subscriber evicts L1 on all peer instances; L2 eviction alone is insufficient for L1 consistency |
| **Stale L2 after DB write** | Tenant reads old config from Garnet for up to 5 min | Call `InvalidateTenantAsync` after every DB write — evicts both L1 and L2 atomically; Pub/Sub for multi-instance |
| **Cache stampede on cold start** | Multiple requests hit DB simultaneously for same tenant | Use `SemaphoreSlim` per key or rely on Garnet/Redis single-flight |
| **Large tenant config (>1MB)** | Increased memory/bandwidth | Compress JSON; paginate if >1000 settings per tenant |
| **`IAppCacheService` interface change** | Breaks existing consumers | Add TTL overload as `default` parameter; existing callers use default 3-min TTL |
| **Repository stub → real DB** | Connection string, schema mismatches | Use Dapper with same queries as T4/CodeGeneration; test against dev DB |

---

## 8. Test Plan

### Unit Tests (new `Infrastructure.UnitTests`)

| Test | Description |
|---|---|
| `GetConfigAsync_L1Hit_SkipsL2AndDb` | L1 has valid entry → no L2 or DB call |
| `GetConfigAsync_L1Miss_L2Hit_PopulatesL1` | L1 miss, L2 hit → loads from Garnet, stores in L1 |
| `GetConfigAsync_FullMiss_LoadsFromRepo` | L1 + L2 miss → repository called → stored in both layers |
| `InvalidateTenantAsync_EvictsBothLayers` | `_memoryCache.Remove` + `_cache.RemoveByKeyAsync` called with correct key |
| `GetSettingAsync_SettingFound_ReturnsTypedValue` | Correct deserialization for `int`, `bool`, `string` |
| `GetSettingAsync_SettingNotFound_ReturnsDefault` | Missing code returns `default!` |
| `IsFeatureEnabledAsync_FeatureInSet_ReturnsTrue` | Code present in `EnabledFeatures` |
| `IsFeatureEnabledAsync_FeatureNotInSet_ReturnsFalse` | Code absent |
| `IsRuleEnabledAsync_RuleInSet_ReturnsTrue` | Same pattern |

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
| **L1/L2 cache layers** | L1: `IMemoryCache` (in-process), L2: Garnet (distributed) | L1 eliminates network latency on hot reads; L2 provides shared state and Pub/Sub invalidation bus |
