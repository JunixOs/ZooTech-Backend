# Test Plan - ZooTech Backend

**Date:** 08-06-2026  
**Scope:** Full backend test coverage (unit + integration) after refactor corrections.  
**Total Tests:** 67 (all passing)  

---

## 1. Strategy & Scope

This test plan covers the ZooTech backend after applying all refactor corrections marked in `docs/refactors/06-06-2026 - refactor_backend_src_tests.md`. The goal is to validate correctness across all architectural layers (Clean Architecture: Domain → Application → Interface Adapters → Infrastructure) and ensure integration points work correctly.

**Test Pyramid:**
- **Unit Tests:** Fast, isolated, mock external dependencies.
- **Integration Tests:** Validate real infrastructure wiring (InMemory EF Core, TestServer for API).

---

## 2. Shared Test Utilities (TST-08)

Project: `tests/ZooTech.Tests.Shared`

To reduce duplication across 7 test projects, a shared project was created containing:

| Component | Description |
|-----------|-------------|
| `InMemoryTenantCatalogDbFixture` | Reusable EF Core InMemory `TenantCatalogDb` fixture with `EnsureCreated`. |
| `TenantTestDataFactory` | Factory methods for creating valid `Tenant` entities and `TenantInfo` DTOs with sensible defaults. |

Referenced by: `Infrastructure.UnitTests`, `Application.UnitTests`, `API.IntegrationTests`, `Infrastructure.IntegrationTests`.

---

## 3. FluentAssertions Adoption (TST-007)

All test projects were migrated from classic xUnit `Assert.*` assertions to **FluentAssertions** for improved readability and richer failure messages.

**Projects updated:**
- `ZooTech.Domain.UnitTests`
- `ZooTech.Application.UnitTests`
- `ZooTech.InterfaceAdapters.UnitTests`
- `ZooTech.Infrastructure.UnitTests`
- `ZooTech.API.IntegrationTests`
- `ZooTech.Infrastructure.IntegrationTests`
- `ZooTech.InterfaceAdapters.IntegrationTests`

**Example migration:**
```csharp
// Before
Assert.True(result);
Assert.Equal("TEST", code.Value);

// After
result.Should().BeTrue();
code.Value.Should().Be("TEST");
```

---

## 4. Unit Test Coverage

### 4.1 Domain Layer (`ZooTech.Domain.UnitTests`)
**Tests:** 26 passing

| Class Under Test | Tests | Focus |
|------------------|-------|-------|
| `TenantDomainEntity` | 4 | Creation, property validation, behavior encapsulation. |
| `TenantCode` | 7 | Value object equality, validation rules, immutability. |
| `Email` | 6 | Format validation, equality, edge cases (empty, null, invalid). |

### 4.2 Application Layer (`ZooTech.Application.UnitTests`)
**Tests:** 20 passing

| Class Under Test | Tests | Focus |
|------------------|-------|-------|
| `CreateTenantCommand.Validator` | 13 | All validator rules: required fields, format constraints, nested object validation. |
| `CreateTenantCommand.Handler` | 2 | Happy path + error path (provisioning failure). |
| `ValidationBehavior<TRequest,TResponse>` | 3 | Pass-through when valid, throws `ValidationException` when invalid, integration with MediatR pipeline. |

**Note:** `CreateTenantCommand` uses `init` properties (not a C# `record`), so validator tests use a helper factory method with optional parameters instead of `with` expressions.

### 4.3 Interface Adapters Layer (`ZooTech.InterfaceAdapters.UnitTests`)
**Tests:** 8 passing

| Class Under Test | Tests | Focus |
|------------------|-------|-------|
| `ExceptionHandlingMiddleware` | 2 | Catches exceptions, returns correct status codes and JSON problem details. |
| `CreateTenantMapper` | 3 | Maps DTOs to commands correctly, handles nulls. |
| `TenancingController` | 1 | Delegates to MediatR, returns `201 Created` on success. |

### 4.4 Infrastructure Layer (`ZooTech.Infrastructure.UnitTests`)
**Tests:** 11 passing

| Class Under Test | Tests | Focus |
|------------------|-------|-------|
| `TenantMapper` | 3 | Maps `Tenant` entity to `TenantInfo` and back correctly. |
| `TenantDatabaseMigrator` | 1 | Verifies `MigrateAsync` is invoked with the correct connection string on relational providers; correctly throws `InvalidOperationException` on InMemory providers. |
| `TenantProvisioningService` | 2 | Orchestration of store + migrator + seeder. |
| `TenantStore` | 4 | Retrieval by subdomain, caching behavior (MemoryCache). |
| `TenantDbContextFactory` | 1 | Creates context with correct connection string template. |

---

## 5. Integration Test Coverage

### 5.1 Infrastructure Integration (`ZooTech.Infrastructure.IntegrationTests`)
**Tests:** 1 passing

| Test | Focus |
|------|-------|
| `TenantCatalogDbIntegrationTests` | Uses InMemory `TenantCatalogDb` to verify that entities can be added, queried, and tracked by EF Core correctly. |

### 5.2 API Integration (`ZooTech.API.IntegrationTests`)
**Tests:** 1 passing

| Test | Focus |
|------|-------|
| `POST_Tenancing_Should_Return_200_When_Provisioning_Succeeds` | End-to-end via `WebApplicationFactory<Program>`: mocks external services (`ITenantStore`, `ITenantProvisioningService`, `IAppAuditService`, `ITenantContext`), configures subdomain resolution (`test.zootech.com`), overrides `AllowedHosts=*`, and asserts HTTP 200. |

**Key integration challenges resolved:**
- `TenantResolutionMiddleware` requires a subdomain-style `Host` header.
- `appsettings.json` restricts `AllowedHosts` to `localhost`; must be overridden in tests.
- `MongoDbAudit` requires `MongoDbContext` which is unavailable in tests; original service descriptors must be removed from DI before mocking.
- `WebApplicationFactory.Server` must **not** be accessed before `WithWebHostBuilder` to avoid starting the unmodified app.

---

## 6. Test Execution

### Run all tests
```bash
dotnet test
```

### Run a specific project
```bash
dotnet test tests/Unit/ZooTech.Domain.UnitTests
dotnet test tests/Integration/ZooTech.API.IntegrationTests
```

### Build only
```bash
dotnet build
```

---

## 7. Test Summary

| Project | Tests | Status |
|---------|-------|--------|
| ZooTech.Domain.UnitTests | 26 | Passing |
| ZooTech.Application.UnitTests | 20 | Passing |
| ZooTech.InterfaceAdapters.UnitTests | 8 | Passing |
| ZooTech.Infrastructure.UnitTests | 11 | Passing |
| ZooTech.Infrastructure.IntegrationTests | 1 | Passing |
| ZooTech.API.IntegrationTests | 1 | Passing |
| **Total** | **67** | **All Passing** |

---

## 8. Next Steps / Future Work

- Add more API integration tests for error paths (400, 404, 500 scenarios).
- Add integration tests for other modules beyond `Module_Tenancing`.
- Evaluate adding property-based tests (e.g., FsCheck) for value objects.
- Add performance/smoke tests for tenant database provisioning with real SQL Server in CI.

