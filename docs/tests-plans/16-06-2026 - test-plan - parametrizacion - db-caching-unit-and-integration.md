# Test Plan — Parametrización DB Caching (Unit & Integration)

**Date:** 16-06-2026  
**Scope:** `TenantConfigurationProvider`, `TenantConfigurationRepository`, `ConfigInvalidationSubscriber`, `IAppCacheService` TTL overload  
**Plan Reference:** `docs/architecture/parametrizacion/15-06-2026 - parametrizacion-plan - DB-Caching.md` §8

---

## 1. Overview

This plan covers the **new unified tenant configuration caching** implementation:

| Component | Layer | Type |
|---|---|---|
| `TenantConfigurationProvider` | Infrastructure | L1 (IMemoryCache) + L2 (Garnet) + DB fallback |
| `TenantConfigurationRepository` | Infrastructure | ADO.NET/SqlClient — 3 queries (settings, features, rules) |
| `ConfigInvalidationSubscriber` | Infrastructure | Redis Pub/Sub listener (IHostedService) |
| `IAppCacheService` (TTL overload) | Application | New `GetOrCreateAsync<T>(key, factory, TimeSpan)` method |
| `GarnetCacheService` / `RedisCacheService` | Infrastructure | TTL overload implementation |

---

## 2. Unit Tests — `ZooTech.Infrastructure.UnitTests`

### 2.1 `TenantConfigurationProviderTests.cs`

| # | Test | Arrange | Expected |
|---|---|---|---|
| UT-P1 | `GetConfigAsync_L1Hit_SkipsL2AndDb` | L1 has valid `TenantConfiguration` entry | Returns cached value; L2 and Repository **never** called |
| UT-P2 | `GetConfigAsync_L1Miss_L2Hit_PopulatesL1` | L1 empty, L2 returns config | Returns L2 value; L1 is populated; Repository never called |
| UT-P3 | `GetConfigAsync_FullMiss_LoadsFromRepo` | L1 + L2 empty, repo returns config | Repository called; result stored in both L1 and L2 |
| UT-P4 | `InvalidateTenantAsync_EvictsBothLayers` | Any state | `_memoryCache.Remove` + `_cache.RemoveByKeyAsync` called with `"tenant:config:{id}"` |
| UT-P5 | `GetSettingAsync_SettingFound_ReturnsTypedValue` | Settings include `"MAX_LOGIN_ATTEMPTS":"5"` | Returns `5` (int) |
| UT-P6 | `GetSettingAsync_SettingNotFound_ReturnsDefault` | Settings don't contain the requested code | Returns `default` (`0` for int, `false` for bool) |
| UT-P7 | `IsFeatureEnabledAsync_FeatureInSet_ReturnsTrue` | `EnabledFeatures` contains `"MODULE_VACUNOS"` | Returns `true` |
| UT-P8 | `IsFeatureEnabledAsync_FeatureNotInSet_ReturnsFalse` | `EnabledFeatures` does NOT contain code | Returns `false` |
| UT-P9 | `IsRuleEnabledAsync_RuleInSet_ReturnsTrue` | `EnabledRules` contains `"VACUNOS_ELIMINACION_CONDICIONADA"` | Returns `true` |
| UT-P10 | `IsRuleEnabledAsync_RuleNotInSet_ReturnsFalse` | `EnabledRules` does NOT contain code | Returns `false` |

**Mocking strategy:**
- `IMemoryCache`: use real `MemoryCache` instance (verify actual cache entries)
- `IAppCacheService`: `Mock<IAppCacheService>` (verify L2 interactions with Moq)
- `ITenantConfigurationRepository`: `Mock<ITenantConfigurationRepository>` (verify DB is skipped on cache hit)

### 2.2 `GarnetCacheServiceTests.cs`

| # | Test | Arrange | Expected |
|---|---|---|---|
| UT-G1 | `GetOrCreateAsync_WithTtl_UsesProvidedTtl` | Factory returns value, TTL = 5 min | `StringSetAsync` called with the provided TTL, not the default |
| UT-G2 | `GetOrCreateAsync_WithoutTtl_UsesDefaultTtl` | Factory returns value, no TTL | `StringSetAsync` called with the configured default TTL (3 min) |
| UT-G3 | `GetOrCreateAsync_WithTtl_CacheHit_ReturnsCached` | Key exists in cache | Factory not executed, cached value returned |

**Note:** These tests require a running Garnet/Redis instance or a mock of `ConnectionMultiplexer`. Option A: use `Mock<IConnectionMultiplexer>` + `Mock<IDatabase>`. Option B: skip if no Redis available.

---

## 3. Integration Tests — `ZooTech.Infrastructure.IntegrationTests`

### 3.1 `TenantConfigurationProviderIntegrationTests.cs`

| # | Test | Approach | Expected |
|---|---|---|---|
| IT-P1 | `Provider_EndToEnd_CacheThenDB` | Mock `ITenantConfigurationRepository`, real `MemoryCache`, mock `IAppCacheService` | Cold → loads from repo → cached in L1/L2 → invalidate → cold again |
| IT-P2 | `Invalidate_ThenRead_ReloadsFromRepo` | Seed L1 + L2 with stale data, invalidate, read | Repository called, fresh data returned |
| IT-P3 | `L1_PopulatedFromL2_OnCacheHit` | Seed L2 only, read twice | First read: L2 hit → L1 populated. Second read: L1 hit, L2/skip |

### 3.2 `TenantConfigurationRepositoryIntegrationTests.cs`

| # | Test | Approach | Expected |
|---|---|---|---|
| IT-R1 | `LoadTenantConfigAsync_WithRealDb` | **Requires real Control Plane DB** (skipped if unavailable) | Returns `TenantConfiguration` with all 3 sections populated |
| IT-R2 | `LoadTenantConfigAsync_EmptyTenant` | Mock DB or InMemory | Returns empty Settings/Features/Rules for tenant with no data |

### 3.3 `ConfigInvalidationSubscriberIntegrationTests.cs`

| # | Test | Approach | Expected |
|---|---|---|---|
| IT-S1 | `Subscriber_EvictsL1_OnPubSubMessage` | Mock `IConnectionMultiplexer` + real `MemoryCache` | After publish, L1 entry for the tenant is removed |
| IT-S2 | `Subscriber_InvalidMessage_DoesNothing` | Publish non-numeric message | No cache eviction occurs |

---

## 4. Test Data

### 4.1 Seed `TenantConfiguration` for unit tests

```csharp
private static TenantConfiguration CreateSampleConfig(int tenantId = 1) => new()
{
    TenantId = tenantId,
    Settings = new() { { "MAX_LOGIN_ATTEMPTS", "5" }, { "SESSION_TIMEOUT_MINUTES", "30" } },
    EnabledFeatures = new() { "MODULE_VACUNOS", "MODULE_REPORTES" },
    EnabledRules = new() { "VACUNOS_ELIMINACION_CONDICIONADA" },
    LoadedAt = DateTime.UtcNow
};
```

### 4.2 IMemoryCache setup for L1 hit tests

```csharp
var cache = new MemoryCache(new MemoryCacheOptions());
cache.Set("tenant:config:1", sampleConfig, TimeSpan.FromMinutes(5));
```

---

## 5. Execution

```bash
# Unit tests
dotnet test tests/Unit/ZooTech.Infrastructure.UnitTests --filter "FullyName~TenantConfigurationProvider"

# All infrastructure unit tests
dotnet test tests/Unit/ZooTech.Infrastructure.UnitTests

# Integration tests
dotnet test tests/Integration/ZooTech.Infrastructure.IntegrationTests

# Full solution test
dotnet test "ZooTech Backend - Solution.slnx"
```
