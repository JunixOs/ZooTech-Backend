# Arquitectura de Parametrización — ZooTech

**Fecha:** 15-06-2026  
**Versión:** 1.0 — Implementación actual  
**Propósito:** Documentar la arquitectura de parametrización (Settings, Features, Rules) para que cualquier desarrollador pueda entenderla y contribuir.

---

## Índice

1. [Visión General](#1-visión-general)
2. [Estructura del Proyecto](#2-estructura-del-proyecto)
3. [Generación de Código (Console Generator)](#3-generación-de-código-console-generator)
4. [Modelos de Dominio](#4-modelos-de-dominio)
5. [Interfaces de Aplicación (Puertos)](#5-interfaces-de-aplicación-puertos)
6. [Implementaciones de Infraestructura (Adaptadores)](#6-implementaciones-de-infraestructura-adaptadores)
7. [Registro en DI](#7-registro-en-di)
8. [Flujo Completo de Lectura](#8-flujo-completo-de-lectura)
9. [Flujo de Invalidación de Caché](#9-flujo-de-invalidación-de-caché)
10. [Esquema de Base de Datos (Control Plane)](#10-esquema-de-base-de-datos-control-plane)
11. [Planes Futuros](#11-planes-futuros)

---

## 1. Visión General

El sistema de parametrización permite definir configuraciones (`settings`), funcionalidades (`features`) y reglas de negocio (`rules`) en la base de datos de control plano y consumirlas desde cualquier caso de uso de la aplicación con **type-safety** total.

### Principios arquitectónicos

| Principio | Aplicación |
|---|---|
| **Code Generation > Runtime** | Las definiciones se generan en build-time mediante un Console Generator, no en runtime |
| **Type Safety** | Cada setting sabe su tipo (`int`, `bool`, `string`, etc.) en tiempo de compilación |
| **Clean Architecture** | Domain no depende de nada. Application solo conoce interfaces. Infrastructure implementa |
| **Cache-First** | Toda lectura pasa por caché (Garnet/Redis); la BD solo se consulta en cache miss |
| **Tools aislados** | El generador de código vive en `tools/` y no tiene dependencias con `src/` |

### Diagrama de capas

```text
┌─────────────────────────────────────────────────────────────┐
│                        ZooTech.API                          │
├─────────────────────────────────────────────────────────────┤
│                    ZooTech.Application                       │
│  ┌──────────────────────────────────────────────────────┐   │
│  │  Puertos (Interfaces):                               │   │
│  │  • ISettingsProvider        │  • IFearureProvider     │   │
│  │  • IRuleProvider            │  • ISettingsRepository  │   │
│  │  • IFeatureRepository       │  • IRuleRepository      │   │
│  └──────────────────────────────────────────────────────┘   │
├─────────────────────────────────────────────────────────────┤
│                    ZooTech.Domain                            │
│  ┌──────────────────────────────────────────────────────┐   │
│  │  Configuration/                                      │   │
│  │  • SettingDefinition<T>  (record)                    │   │
│  │  • FeatureCode            (record struct)             │   │
│  │  • RuleCode               (record struct)             │   │
│  │  • Settings.g.cs          (generado)                   │   │
│  │  • Features.g.cs          (generado)                   │   │
│  │  • Rules.g.cs             (generado)                   │   │
│  └──────────────────────────────────────────────────────┘   │
├─────────────────────────────────────────────────────────────┤
│                    ZooTech.Infrastructure                    │
│  ┌──────────────────────────────────────────────────────┐   │
│  │  Implementaciones:                                   │   │
│  │  • SettingsProvider     │  • FeatureProvider          │   │
│  │  • RuleProvider         │  • SettingsRepository (stub)│   │
│  │  • FeatureRepository (stub) │  • RuleRepository (stub)│   │
│  │  • GarnetCacheService   │  • RedisCacheService        │   │
│  └──────────────────────────────────────────────────────┘   │
├─────────────────────────────────────────────────────────────┤
│              tools/ZooTech.CodeGeneration                    │
│  (Console App independiente — genera .g.cs en Domain)       │
└─────────────────────────────────────────────────────────────┘
```

---

## 2. Estructura del Proyecto

```text
ZooTech-Backend - Solution/
│
├── src/
│   ├── ZooTech.Domain/
│   │   └── Configuration/
│   │       ├── SettingDefinition.cs          ← [MANUAL] Record genérico
│   │       ├── FeatureCode.cs                ← [MANUAL] Record struct
│   │       ├── RuleCode.cs                   ← [MANUAL] Record struct
│   │       ├── Settings.g.cs                 ← [GENERADO] Clases estáticas
│   │       ├── Features.g.cs                 ← [GENERADO]
│   │       └── Rules.g.cs                    ← [GENERADO]
│   │
│   ├── ZooTech.Application/
│   │   └── Common/
│   │       └── Gateway/
│   │           ├── Parametrization/
│   │           │   ├── Settings/ISettingsProvider.cs
│   │           │   ├── Features/IFeatureProvider.cs
│   │           │   └── Rules/IRuleProvider.cs
│   │           └── Repositories/
│   │               └── Parametrization/
│   │                   ├── ISettingsRepository.cs
│   │                   ├── IFeatureRepository.cs
│   │                   └── IRuleRepository.cs
│   │
│   ├── ZooTech.Infrastructure/
│   │   ├── Parametrization/
│   │   │   ├── Settings/
│   │   │   │   ├── SettingsProvider.cs
│   │   │   │   ├── SettingsRepository.cs     (stub)
│   │   │   │   └── SettingsSnapshot.cs
│   │   │   ├── Features/
│   │   │   │   ├── FeatureProvider.cs
│   │   │   │   └── FeatureRepository.cs      (stub)
│   │   │   └── Rules/
│   │   │       ├── RuleProvider.cs
│   │   │       └── RuleRepository.cs          (stub)
│   │   ├── Caching/
│   │   │   ├── Garnet/GarnetCacheService.cs
│   │   │   └── Redis/RedisCacheService.cs
│   │   └── DependencyInjection.cs
│   │
│   └── ZooTech.API, ZooTech.InterfaceAdapters  (no afectados)
│
└── tools/
    └── ZooTech.CodeGeneration/
        ├── Program.cs
        ├── Models/
        ├── Readers/
        ├── Generators/
        ├── Writers/
        └── appsettings.json / appsettings.Development.json
```

---

## 3. Generación de Código (Console Generator)

### 3.1 Propósito

Reemplazar el antiguo template T4 con una Console App en `tools/` que se ejecuta manualmente o en pipeline CI/CD. Genera los archivos `.g.cs` que contienen las definiciones estáticas de settings, features y rules.

### 3.2 Flujo

```text
[Control Plane DB]
    │
    ▼
ZooTech.CodeGeneration (Console App)
    │
    ├── Lee setting_groups, setting_definitions
    ├── Lee features
    └── Lee rule_definitions
    │
    ▼
Genera → Settings.g.cs, Features.g.cs, Rules.g.cs
    │
    ▼
src/ZooTech.Domain/Configuration/
```

### 3.3 Componentes

| Componente | Responsabilidad |
|---|---|
| `MetadataReader` (Dapper) | Ejecuta consultas SQL contra la Control Plane DB |
| `SettingsGenerator` | Genera `Settings.g.cs` con clases agrupadas por `GroupCode` |
| `FeaturesGenerator` | Genera `Features.g.cs` con `FeatureCode` por cada feature |
| `RulesGenerator` | Genera `Rules.g.cs` con `RuleCode` por cada regla |
| `FileWriter` | Escribe archivos UTF-8, creando directorios si es necesario |
| `Program.cs` | Orquestación: lee → genera → escribe |

### 3.4 Ejecución

```bash
# Manual (desde la raíz de la solución)
dotnet run --project tools/ZooTech.CodeGeneration

# La tool es independiente — no hay MSBuild target que la ejecute
# Tools NO debe ser referenciado por src/
```

### 3.5 Manejo de errores

Si la base de datos no está disponible, la tool genera archivos vacíos (stubs compilables) y continúa sin fallar.

### 3.6 Archivos de configuración

- `appsettings.json` — contiene valores por defecto (connection string vacío). Se versiona.
- `appsettings.Development.json` — contiene la connection string real. NO se versiona (`.gitignore`).

---

## 4. Modelos de Dominio

### 4.1 SettingDefinition\<T\>

```csharp
namespace ZooTech.Domain.Configuration;

public sealed record SettingDefinition<T>(string Code);
```

Un `record` sellado que asocia un código de configuración con su tipo C# en tiempo de compilación.

**Ejemplo de inferencia de tipos:**
```csharp
// Settings.g.cs genera:
public static readonly SettingDefinition<int> VacunosDefaultFilterDays = new("VACUNOS_DEFAULT_FILTER_DAYS");

// Uso:
var value = await _provider.GetAsync(tenantId, Settings.Vacunos.VacunosDefaultFilterDays);
// value es automáticamente `int` — no necesita <int> explícito
```

### 4.2 FeatureCode

```csharp
namespace ZooTech.Domain.Configuration;

public readonly record struct FeatureCode(string Value);
```

Un `record struct` sellado por valor. Dos instancias con el mismo `Value` son iguales.

### 4.3 RuleCode

```csharp
namespace ZooTech.Domain.Configuration;

public readonly record struct RuleCode(string Value);
```

Misma semántica que `FeatureCode`.

### 4.4 Archivos Generados

Los archivos `.g.cs` contienen clases estáticas anidadas:

- `Settings.Vacunos.VacunosDefaultFilterDays` → `SettingDefinition<int>`
- `Features.ModuleVacunos` → `FeatureCode`
- `Rules.VacunosEliminacionCondicionada` → `RuleCode`

Son código generado — **no se editan manualmente**. Se regeneran ejecutando la tool.

---

## 5. Interfaces de Aplicación (Puertos)

### 5.1 ISettingsProvider

```csharp
namespace ZooTech.Application.Common.Gateway.Parametrization.Settings;

public interface ISettingsProvider
{
    Task<T> GetAsync<T>(int tenantId, SettingDefinition<T> setting);
    Task RefreshAsync(int tenantId);
}
```

- `GetAsync<T>`: Recibe un `SettingDefinition<T>` e infiere el tipo de retorno automáticamente.
- `RefreshAsync`: Invalida la caché para el tenant, forzando una recarga desde BD en la siguiente lectura.

### 5.2 IFeatureProvider

```csharp
namespace ZooTech.Application.Common.Gateway.Parametrization.Features;

public interface IFeatureProvider
{
    Task<bool> IsEnabledAsync(int tenantId, FeatureCode feature);
}
```

### 5.3 IRuleProvider

```csharp
namespace ZooTech.Application.Common.Gateway.Parametrization.Rules;

public interface IRuleProvider
{
    Task<bool> IsEnabledAsync(int tenantId, RuleCode rule);
}
```

### 5.4 Repositorios

```csharp
public interface ISettingsRepository
{
    Task<IReadOnlyCollection<SettingValueDto>> GetTenantSettingsAsync(int tenantId);
}

public interface IFeatureRepository
{
    Task<HashSet<string>> GetEnabledFeatureCodesAsync(int tenantId);
}

public interface IRuleRepository
{
    Task<HashSet<string>> GetEnabledRuleCodesAsync(int tenantId);
}
```

---

## 6. Implementaciones de Infraestructura (Adaptadores)

### 6.1 SettingsProvider

```csharp
public sealed class SettingsProvider : ISettingsProvider
{
    private readonly IAppCacheService _cache;
    private readonly ISettingsRepository _repository;

    public async Task<T> GetAsync<T>(int tenantId, SettingDefinition<T> setting)
    {
        var snapshot = await GetSnapshotAsync(tenantId);
        if (snapshot.Values.TryGetValue(setting.Code, out var raw))
            return JsonSerializer.Deserialize<T>(raw)!;
        return default!;
    }
}
```

**Flujo interno (`GetSnapshotAsync`):**
1. Construye key `settings:tenant:{tenantId}`
2. Consulta caché (`TryGetAsync`)
3. Si hay cache hit → devuelve `SettingsSnapshot`
4. Si hay cache miss → consulta repositorio (DB) → guarda en caché → devuelve

### 6.2 FeatureProvider y RuleProvider

Siguen el mismo patrón que `SettingsProvider` pero con sus propias claves de caché:
- `features:tenant:{tenantId}`
- `rules:tenant:{tenantId}`

### 6.3 Servicio de Caché

Dos implementaciones de `IAppCacheService`:

| Implementación | Tecnología | Estado |
|---|---|---|
| `GarnetCacheService` | Garnet (Microsoft) | ✅ Completa |
| `RedisCacheService` | Redis (StackExchange) | ✅ Completa |

**Métodos:**
```csharp
Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory);
Task<(bool Found, T? Value)> TryGetAsync<T>(string key);
Task RemoveByKeyAsync(string key);
```

El TTL por defecto es configurable vía `appsettings.json` (`Garnet:ExpirationTime` o `Redis:ExpirationTime`), formato `mm:ss`. Por defecto: 3 minutos.

### 6.4 Repositorios (Stubs)

Actualmente los tres repositorios lanzan `NotImplementedException`. Están registrados en DI como placeholders hasta que se implemente el acceso real a la Control Plane DB.

---

## 7. Registro en DI

Todo se registra en `ZooTech.Infrastructure.DependencyInjection.AddInfrastructure()`:

```csharp
// Cache
services.AddSingleton<IAppCacheService, GarnetCacheService>();

// Providers
services.AddScoped<ISettingsProvider, SettingsProvider>();
services.AddScoped<IFeatureProvider, FeatureProvider>();
services.AddScoped<IRuleProvider, RuleProvider>();

// Repositories (stubs — pending DB implementation)
services.AddScoped<ISettingsRepository, SettingsRepository>();
services.AddScoped<IFeatureRepository, FeatureRepository>();
services.AddScoped<IRuleRepository, RuleRepository>();
```

---

## 8. Flujo Completo de Lectura

### Ejemplo: Obtener `VacunosDefaultFilterDays` para tenant 1

```text
Application (ej. LoginCommandHandler)
    │
    │  _settingsProvider.GetAsync(1, Settings.Vacunos.VacunosDefaultFilterDays)
    │
    ▼
SettingsProvider.GetAsync<int>(1, SettingDefinition<int>)
    │
    ├──► GetSnapshotAsync(1)
    │       │
    │       ├── Cache key: "settings:tenant:1"
    │       ├── TryGetAsync → HIT  → devuelve snapshot cacheado
    │       └── TryGetAsync → MISS → repository.GetTenantSettingsAsync(1)
    │                                    → cachea resultado → devuelve
    │
    └──► snapshot.Values["VACUNOS_DEFAULT_FILTER_DAYS"] = "30"
         │
         └──► JsonSerializer.Deserialize<int>("30") → 30
    │
    ▼
Application recibe int = 30
```

### Ventajas de la separación

- **Application** no sabe de dónde vienen los datos (caché, DB, etc.)
- **Application** no sabe qué tecnología de caché se usa (Garnet, Redis)
- **Application** solo consume interfaces y modelos de Domain
- **Infrastructure** implementa todo el caching y acceso a datos

---

## 9. Flujo de Invalidación de Caché

### 9.1 Invalidación manual

```csharp
// Cuando un admin actualiza una configuración:
await _settingsProvider.RefreshAsync(tenantId);
```

Esto elimina la clave `settings:tenant:{tenantId}` de la caché. La siguiente lectura recarga desde DB.

### 9.2 Estado actual

- `ISettingsProvider` tiene `RefreshAsync` — **implementado**
- `IFeatureProvider` no tiene método de invalidación — **pendiente**
- `IRuleProvider` no tiene método de invalidación — **pendiente**

---

## 10. Esquema de Base de Datos (Control Plane)

### 10.1 Tablas utilizadas por la generación de código

```sql
-- Grupos de configuraciones
setting_groups (id, code, name, description, is_active, metadata, created_at, updated_at, deleted_at)

-- Definiciones de configuraciones
setting_definitions (id, setting_group_id, code, name, description, data_type,
                     default_value, validation_schema, is_required, is_sensitive,
                     is_active, metadata, created_at, updated_at, deleted_at)
-- UNIQUE(setting_group_id, code)

-- Features
features (id, code, name, description, category, is_active, metadata, created_at, updated_at, deleted_at)

-- Reglas de negocio
rule_definitions (id, code, name, module, description, condition_schema, action_schema,
                  is_active, metadata, created_at, updated_at, deleted_at)
```

### 10.2 Tablas de valores por tenant (para futura implementación)

```sql
-- Valores de configuraciones por tenant
setting_values (id, tenant_id, setting_definition_id, actor_type, actor_id,
                value, is_active, metadata, created_at, updated_at, deleted_at)
-- UNIQUE(tenant_id, setting_definition_id, actor_type, actor_id)

-- Features habilitadas por tenant
tenant_features (tenant_id, feature_id, is_enabled, enabled_at, expires_at, updated_at, metadata)
-- PK(tenant_id, feature_id)

-- Reglas activas por tenant
tenant_business_rules (tenant_id, rule_definition_id, is_active, priority, rule_version,
                       execution_mode, custom_condition, custom_action, metadata,
                       created_at, updated_at, deleted_at)
-- PK(tenant_id, rule_definition_id)
```

---

## 11. Planes Futuros

A continuación se detallan los próximos pasos planificados para evolucionar la arquitectura de parametrización.

### 11.1 Corto Plazo

| # | Tarea | Prioridad |
|---|---|---|
| 1 | **Implementar repositorios reales con Dapper** — reemplazar los stubs de `SettingsRepository`, `FeatureRepository` y `RuleRepository` con implementaciones que consulten la Control Plane DB usando Dapper y las queries del `MetadataReader` | Alta |
| 2 | **Agregar `RefreshAsync` a `IFeatureProvider` e `IRuleProvider`** — consistencia con `ISettingsProvider` | Alta |
| 3 | **Extender `IAppCacheService` con TTL por clave** — agregar overload `GetOrCreateAsync<T>(key, factory, TimeSpan ttl)` para permitir TTL de 5 minutos sin cambiar la config global | Media |

### 11.2 Mediano Plazo

| # | Tarea | Prioridad |
|---|---|---|
| 4 | **Unificar en un solo `TenantConfigurationProvider`** — crear un `ITenantConfigurationProvider` que consolide settings, features y rules en un único snapshot cacheado con una sola clave de caché y un solo viaje a DB. Esto reemplazará los 3 providers actuales | Alta |
| 5 | **`TenantConfiguration` record** — modelo unificado con `Dictionary<string, string> Settings`, `HashSet<string> EnabledFeatures`, `HashSet<string> EnabledRules` | Alta |
| 6 | **Repositorio unificado con `QueryMultipleAsync`** — Dapper para cargar settings + features + rules en una sola transacción | Alta |
| 7 | **Cache TTL de 5 minutos** — el snapshot unificado usará 5 minutos de TTL | Alta |
| 8 | **Invalidación atómica** — cualquier update en DB elimina la única clave de caché del tenant | Alta |
| 9 | **Endpoints CRUD** — crear los endpoints de API REST para actualizar settings, features y rules por tenant (via InterfaceAdapters) | Media |

### 11.3 Largo Plazo

| # | Tarea | Prioridad |
|---|---|---|
| 10 | **Multi-instancia con Redis Pub/Sub** — cuando se actualice una config, publicar un mensaje Redis para que todas las instancias invaliden su caché local | Baja |
| 11 | **SignalR para notificaciones en tiempo real** — notificar a clientes frontend cuando cambie una configuración | Baja |
| 12 | **Arquitectura híbrida L1/L2** — caché en memoria (L1) + caché distribuida Redis/Garnet (L2) para reducir latencia al mínimo | Baja |
| 13 | **Auditoría de cambios** — registrar quién cambió qué configuración y cuándo | Baja |

### 11.4 Diagrama de la arquitectura objetivo

```text
┌─────────────────────────────────────────────────────────────────┐
│                        Application                               │
│  ITenantConfigurationProvider (unificado)                        │
│    • GetSettingAsync<T>(tenantId, SettingDefinition<T>)          │
│    • IsFeatureEnabledAsync(tenantId, FeatureCode)                │
│    • IsRuleEnabledAsync(tenantId, RuleCode)                      │
│    • InvalidateTenantAsync(tenantId)                             │
└───────────────────────┬─────────────────────────────────────────┘
                        │
┌───────────────────────▼─────────────────────────────────────────┐
│                   Infrastructure                                 │
│                                                                  │
│  TenantConfigurationProvider                                     │
│    ┌────────────────────────────────────────────────────────┐   │
│    │  1. TryGetAsync("tenant:config:{id}")                  │   │
│    │     └── HIT  → devuelve TenantConfiguration            │   │
│    │     └── MISS →                                          │   │
│    │         2. Repository.LoadTenantConfigAsync(id)         │   │
│    │            └── Dapper QueryMultipleAsync                │   │
│    │         3. Cache con TTL 5 min                          │   │
│    └────────────────────────────────────────────────────────┘   │
│                                                                  │
│  Repository (Dapper → Control Plane DB)                          │
│    └── 1 query: settings + overrides                             │
│    └── 1 query: enabled features                                 │
│    └── 1 query: enabled rules                                    │
│                                                                  │
│  Cache (Garnet / Redis)                                          │
│    └── TTL configurable (default 5 min)                          │
│                                                                  │
│  Invalidación (Pub/Sub)                                          │
│    └── Redis Pub/Sub → todas las instancias                      │
└─────────────────────────────────────────────────────────────────┘
```

---

## Historial de Revisiones

| Fecha | Versión | Cambio |
|---|---|---|
| 15-06-2026 | 1.0 | Documento inicial — arquitectura actual implementada |
