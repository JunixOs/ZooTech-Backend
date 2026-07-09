# Test Plan — Parametrización (Settings, Features, Rules)

**Date:** 15-06-2026  
**Author:** Architecture Team  
**Scope:** Console Generator (`tools/`) + Domain/Application/Infrastructure providers (`src/`)

---

## 1. Overview

This test plan covers the parametrization subsystem:

- **Tools:** `ZooTech.CodeGeneration` — console app that reads Control Plane DB metadata and generates `Settings.g.cs`, `Features.g.cs`, `Rules.g.cs`
- **Domain:** `SettingDefinition<T>`, `FeatureCode`, `RuleCode`, plus generated static classes
- **Application:** `ISettingsProvider`, `IFeatureProvider`, `IRuleProvider` (port interfaces)
- **Infrastructure:** `SettingsProvider`, `FeatureProvider`, `RuleProvider` (implementations with Garnet/Redis cache)

---

## 2. Test Projects

| Project | Location | Type |
|---|---|---|
| `ZooTech.CodeGeneration.UnitTests` | `tests/Unit/` | Unit tests (xUnit) |
| `ZooTech.CodeGeneration.IntegrationTests` | `tests/Integration/` | Integration tests (xUnit) |
| `ZooTech.Domain.UnitTests` | `tests/Unit/` (existing) | Domain model tests |
| `ZooTech.Application.UnitTests` | `tests/Unit/` (existing) | Interface contract tests |
| `ZooTech.Infrastructure.UnitTests` | `tests/Unit/` (existing) | Provider implementation tests |

---

## 3. Tools — ZooTech.CodeGeneration

### 3.1 Unit Tests

| # | Test | Expected |
|---|---|---|
| UT-T1 | `SettingsGenerator` generates empty file when collection is empty | Returns valid namespace with empty `Settings` class |
| UT-T2 | `SettingsGenerator` generates one group with one setting of type `int` | Correct PascalCase class name and `SettingDefinition<int>` |
| UT-T3 | `SettingsGenerator` generates multiple groups with mixed types (`bool`, `string`, `int`) | Correct nesting per group, correct types |
| UT-T4 | `FeaturesGenerator` generates empty file when collection is empty | Returns valid namespace with empty `Features` class |
| UT-T5 | `FeaturesGenerator` generates one feature | `FeatureCode` with PascalCase name |
| UT-T6 | `FeaturesGenerator` generates multiple features | One `FeatureCode` per item |
| UT-T7 | `RulesGenerator` generates empty file when collection is empty | Returns valid namespace with empty `Rules` class |
| UT-T8 | `RulesGenerator` generates multiple rules | One `RuleCode` per item |
| UT-T9 | `FileWriter.Write` creates directory if it does not exist | Directory created, file written |
| UT-T10 | `FileWriter.Write` overwrites existing file | Content replaced |
| UT-T11 | `FileWriter.Write` uses UTF-8 encoding | File is valid UTF-8 |
| UT-T12 | `MetadataReader` queries Settings from DB via Dapper | Returns `IReadOnlyCollection<SettingMetadata>` (requires mocked `SqlConnection`) |

### 3.2 Integration Tests

| # | Test | Expected |
|---|---|---|
| IT-T1 | `dotnet run --project` with valid connection string generates all 3 `.g.cs` files | Files exist with correct content |
| IT-T2 | `dotnet run --project` with invalid connection string generates empty stubs | Warning logged, empty files generated |
| IT-T3 | Generated `.g.cs` files compile as part of `ZooTech.Domain` | `dotnet build` succeeds with 0 errors |

---

## 4. Domain — ZooTech.Domain.Configuration

### 4.1 Unit Tests (existing project)

| # | Test | Expected |
|---|---|---|
| UT-D1 | `SettingDefinition<T>` record stores `Code` property | Code is accessible |
| UT-D2 | `FeatureCode` record struct stores `Value` property | Value is accessible |
| UT-D3 | `RuleCode` record struct stores `Value` property | Value is accessible |
| UT-D4 | `SettingDefinition<int>` and `SettingDefinition<string>` are different types | Compiler treats them distinctly |
| UT-D5 | `FeatureCode` equality by value | Two instances with same value are equal |
| UT-D6 | `RuleCode` equality by value | Two instances with same value are equal |
| UT-D7 | `Settings.Vacunos.VacunosDefaultFilterDays` is a `SettingDefinition<int>` | Compile-time type check |
| UT-D8 | `Features.ModuleVacunos` is a `FeatureCode` | Compile-time type check |
| UT-D9 | `Rules.VacunosEliminacionCondicionada` is a `RuleCode` | Compile-time type check |

---

## 5. Application — Interface Contracts

### 5.1 Unit Tests (existing project)

| # | Test | Expected |
|---|---|---|
| UT-A1 | `ISettingsProvider.GetAsync<T>` with `SettingDefinition<int>` returns `int` | Type-safe generic inference |
| UT-A2 | `ISettingsProvider.RefreshAsync` completes without error | Task completes |
| UT-A3 | `IFeatureProvider.IsEnabledAsync` with `FeatureCode` returns `bool` | Task<bool> |
| UT-A4 | `IRuleProvider.IsEnabledAsync` with `RuleCode` returns `bool` | Task<bool> |

---

## 6. Infrastructure — Provider Implementations

### 6.1 Unit Tests

| # | Test | Expected |
|---|---|---|
| UT-I1 | `SettingsProvider.GetAsync<T>` returns cached value when cache hit | Cache `TryGetAsync` returns found=true, value deserialized correctly |
| UT-I2 | `SettingsProvider.GetAsync<T>` loads from repository on cache miss | Repository called, result cached, value returned |
| UT-I3 | `SettingsProvider.RefreshAsync` removes cache entry | `RemoveByKeyAsync` called |
| UT-I4 | `FeatureProvider.IsEnabledAsync` returns true when feature is in enabled set | Cache or repository returns matching code |
| UT-I5 | `FeatureProvider.IsEnabledAsync` returns false when feature is not in enabled set | Cache or repository returns non-matching |
| UT-I6 | `RuleProvider.IsEnabledAsync` returns true when rule is in enabled set | Same pattern |
| UT-I7 | `RuleProvider.IsEnabledAsync` returns false when rule is not in enabled set | Same pattern |

### 6.2 Integration Tests

| # | Test | Expected |
|---|---|---|
| IT-I1 | DI registration resolves `ISettingsProvider` | `SettingsProvider` instance returned |
| IT-I2 | DI registration resolves `IFeatureProvider` | `FeatureProvider` instance returned |
| IT-I3 | DI registration resolves `IRuleProvider` | `RuleProvider` instance returned |
| IT-I4 | DI registration resolves `ISettingsRepository` | `SettingsRepository` instance returned |

---

## 7. Code Generation Pipeline (Build Integration)

| # | Test | Expected |
|---|---|---|
| IT-P1 | Generated `.g.cs` files are up-to-date after manual `dotnet run --project tools/...` | No compilation errors |
| IT-P2 | Empty `.g.cs` stubs allow Domain to compile when DB is unavailable | Build succeeds with 0 errors |
| IT-P3 | Full solution build (with DB available) generates correct files | All 3 `.g.cs` match expected schema |

---

## 8. Test Data

For integration tests requiring DB access, the Control Plane DB must have:

- At least 1 `setting_groups` row with `is_active = 1, deleted_at IS NULL`
- At least 1 `setting_definitions` row linked to the group with `is_active = 1, deleted_at IS NULL`
- At least 1 `features` row with `is_active = 1, deleted_at IS NULL`
- (Optional) `rule_definitions` rows with `is_active = 1, deleted_at IS NULL`

If DB is not available, integration tests generate empty stubs.

---

## 9. Test Execution

```bash
# All unit tests
dotnet test tests/Unit/ZooTech.CodeGeneration.UnitTests
dotnet test tests/Unit/ZooTech.Domain.UnitTests
dotnet test tests/Unit/ZooTech.Application.UnitTests
dotnet test tests/Unit/ZooTech.Infrastructure.UnitTests

# Integration tests
dotnet test tests/Integration/ZooTech.CodeGeneration.IntegrationTests

# All tests
dotnet test "ZooTech Backend - Solution.slnx"
```
