# Plan de Implementación: Parametrización con T4 + Caché Híbrida + SignalR

Este documento es la guía paso a paso para implementar completamente el sistema de parametrización en ZooTech. Un agente de código debe seguir estos pasos en orden estricto para garantizar que todas las dependencias se resuelvan correctamente.

---

## 📋 Documentos de Referencia

| Documento | Propósito |
|---|---|
| `parameterization_architecture.md` | Arquitectura, diagramas, especificación de componentes y código de implementación |
| `parameterization_governance.md` | Gobernanza, contratos SQL, SLAs, mapeo de tipos, estrategia de pruebas |
| `project_analysis.md` | Estado actual del proyecto, estructura, dependencias y deudas técnicas |

---

## 🔧 Pre-requisitos del Entorno

Antes de iniciar, verificar:

```powershell
# 1. Verificar que la solución compila en su estado actual
dotnet build "ZooTech Backend - Solution.slnx"

# 2. Verificar que los tests existentes pasan
dotnet test "ZooTech Backend - Solution.slnx"

# 3. Verificar que dotnet-t4 está disponible (o instalarlo)
dotnet tool install dotnet-t4 --create-manifest-if-needed

# 4. Verificar conexión a BD (opcional, para T4 en desarrollo)
# Requiere acceso a TenantCatalogConnection con setting_definitions pobladas
```

Si `dotnet build` falla ANTES de cualquier cambio, detener y reportar al usuario.

---

## 📂 Fase 0: Preparación del Proyecto

### Paso 0.1: Crear tool manifest para dotnet-t4

**Archivo:** `.config/dotnet-tools.json` (en raíz de solución)

```json
{
  "version": 1,
  "isRoot": true,
  "tools": {
    "dotnet-t4": {
      "version": "3.0.0",
      "commands": ["t4"]
    }
  }
}
```

**Verificación:**
```powershell
dotnet tool restore
dotnet t4 --version
```

### Paso 0.2: Crear script de desarrollo (watcher)

**Archivo:** `scripts/watch-db-params.ps1`

```powershell
# Watcher de cambios en tabla setting_definitions para regenerar ZooParameters.cs
# Uso: .\scripts\watch-db-params.ps1
# Requiere: dotnet-t4 instalado

$interval = 5
$projectRoot = Split-Path -Parent $PSScriptRoot
$ttFile = Join-Path $projectRoot "src\ZooTech.Infrastructure\Configuration\ZooParameters.tt"
$outputFile = Join-Path $projectRoot "src\ZooTech.Domain\Generated\ZooParameters.cs"

Write-Host "[Watcher] Monitoreando cambios en setting_definitions cada ${interval}s..."
Write-Host "[Watcher] Presiona Ctrl+C para detener."

$lastHash = ""

while ($true) {
    $result = dotnet t4 $ttFile -o $outputFile 2>&1
    $currentHash = (Get-FileHash $outputFile -Algorithm MD5 -ErrorAction SilentlyContinue).Hash

    if ($currentHash -ne $lastHash) {
        Write-Host "[$(Get-Date -Format 'HH:mm:ss')] ZooParameters.cs regenerado." -ForegroundColor Green
        $lastHash = $currentHash
    }

    Start-Sleep -Seconds $interval
}
```

---

## 📂 Fase 1: Capa de Dominio — Estructuras Tipadas

### Paso 1.1: Crear estructuras base de parámetros

**Archivos a crear:**

| # | Ruta | Contenido |
|---|---|---|
| 1 | `src/ZooTech.Domain/Parameters/SettingDefinition.cs` | Ver `parameterization_architecture.md` sección 4.A |
| 2 | `src/ZooTech.Domain/Parameters/FeatureFlag.cs` | Ver `parameterization_architecture.md` sección 4.A |
| 3 | `src/ZooTech.Domain/Parameters/RuleSchema.cs` | Ver `parameterization_architecture.md` sección 4.A |

### Paso 1.2: Crear carpeta Generated con stub inicial

**Archivo:** `src/ZooTech.Domain/Generated/ZooParameters.cs`

Crear un stub vacío que compile (será reemplazado por T4):

```csharp
namespace ZooTech.Domain.Generated;

using ZooTech.Domain.Parameters;

public static class ZooSettings
{
}

public static class ZooFeatures
{
}

public static class ZooRules
{
}
```

### Paso 1.3: Eliminar archivo stub Class1.cs

**Archivo a eliminar:** `src/ZooTech.Domain/Class1.cs`

**Verificación:**
```powershell
dotnet build src/ZooTech.Domain/ZooTech.Domain.csproj
```
Debe compilar sin errores.

---

## 📂 Fase 2: Capa de Aplicación — Puertos y Casos de Uso

### Paso 2.1: Crear puertos (Gateway Interfaces)

**Archivos a crear:**

| # | Ruta | Contenido |
|---|---|---|
| 1 | `src/ZooTech.Application/Common/Gateway/Configuration/ITenantConfiguration.cs` | Ver `parameterization_architecture.md` sección 4.B |
| 2 | `src/ZooTech.Application/Common/Gateway/Configuration/ITenantConfigurationRepository.cs` | Ver `parameterization_architecture.md` sección 4.B |
| 3 | `src/ZooTech.Application/Common/Gateway/Configuration/ICacheInvalidationPublisher.cs` | Ver `parameterization_architecture.md` sección 4.B |
| 4 | `src/ZooTech.Application/Common/Gateway/Configuration/TenantConfigurationSnapshot.cs` | Ver `parameterization_architecture.md` sección 4.B |
| 5 | `src/ZooTech.Application/Common/Gateway/SignalR/IParameterSyncNotifier.cs` | Ver `parameterization_architecture.md` sección 4.B |

### Paso 2.2: Reemplazar IConfiguration vacío

**Archivo a modificar:** `src/ZooTech.Application/Common/Gateway/Configuration/IConfiguration.cs`

Eliminar este archivo vacío. Ya no se necesita (los puertos reales lo reemplazan).

### Paso 2.3: Crear enum ParameterType

**Archivo:** `src/ZooTech.Application/Modules/Module_Tenancing/UseCases/ParameterType.cs`

Ver `parameterization_architecture.md` sección 4.B.

### Paso 2.4: Crear caso de uso GetTenantConfiguration

**Archivos a crear en `src/ZooTech.Application/Modules/Module_Tenancing/UseCases/GetTenantConfiguration/`:**

| # | Archivo |
|---|---|
| 1 | `GetTenantConfigurationQuery.cs` |
| 2 | `GetTenantConfigurationHandler.cs` |
| 3 | `GetTenantConfigurationValidator.cs` |
| 4 | `GetTenantConfigurationDto.cs` |

Ver código completo en `parameterization_architecture.md` sección 4.B — Caso de Uso: GetTenantConfiguration.

### Paso 2.5: Crear caso de uso UpdateTenantParameter

**Archivos a crear en `src/ZooTech.Application/Modules/Module_Tenancing/UseCases/UpdateTenantParameter/`:**

| # | Archivo |
|---|---|
| 1 | `UpdateTenantParameterCommand.cs` |
| 2 | `UpdateTenantParameterHandler.cs` |
| 3 | `UpdateTenantParameterValidator.cs` |

Ver código completo en `parameterization_architecture.md` sección 4.B — Caso de Uso: UpdateTenantParameter.

### Paso 2.6: Crear caso de uso ResetTenantParameter

**Archivos a crear en `src/ZooTech.Application/Modules/Module_Tenancing/UseCases/ResetTenantParameter/`:**

| # | Archivo |
|---|---|
| 1 | `ResetTenantParameterCommand.cs` |
| 2 | `ResetTenantParameterHandler.cs` |
| 3 | `ResetTenantParameterValidator.cs` |

Ver código completo en `parameterization_architecture.md` sección 4.B — Caso de Uso: ResetTenantParameter.

### Paso 2.7: Cambiar visibilidad de GeneralResponseDTO

**Archivo a modificar:** `src/ZooTech.Application/Common/Models/GeneralResponseDTO.cs`

Cambiar `internal class` a `public class`.

### Paso 2.8: Agregar referencia a Domain.Parameters en Application

Verificar que `ZooTech.Application.csproj` ya referencia `ZooTech.Domain`. Si no lo hace, agregar:

```xml
<ProjectReference Include="..\ZooTech.Domain\ZooTech.Domain.csproj" />
```

(Esta referencia ya existe según el análisis del proyecto.)

**Verificación:**
```powershell
dotnet build src/ZooTech.Application/ZooTech.Application.csproj
```
Debe compilar sin errores. Si falla, revisar que todas las interfaces y clases referencien correctamente los namespaces.

---

## 📂 Fase 3: Capa de Infraestructura — Implementaciones

### Paso 3.1: Limpiar código muerto

**3.1.1 — Eliminar `tenant_setting.cs` (entidad orphaned)**

**Archivo a eliminar:** `src/ZooTech.Infrastructure/Persistence/Entities/MainTenantsDb/tenant_setting.cs`

Esta entidad NO tiene DbSet en `TenantCatalogDb`, NO tiene Fluent API, NO tiene navegación desde `tenant.cs`, y usa tipos `long` inconsistentes con el modelo actual (`int`). Fue reemplazada por `setting_value.cs`.

**3.1.2 — Verificar que TenantCatalogDb ya está actualizado**

El `TenantCatalogDb.cs` ya tiene los DbSets y Fluent API necesarios. Verificar que existan:

```csharp
public virtual DbSet<setting_group> setting_groups { get; set; }       // ✅ Ya existe
public virtual DbSet<setting_definition> setting_definitions { get; set; }  // ✅ Ya existe
public virtual DbSet<setting_value> setting_values { get; set; }        // ✅ Ya existe
```

**3.1.3 — Verificar que `tenant.cs` ya tiene navegación `setting_values`**

```csharp
[InverseProperty("tenant")]
public virtual ICollection<setting_value> setting_values { get; set; }  // ✅ Ya existe
```

Si cualquiera de estas verificaciones falla, reportar al usuario antes de continuar.

### Paso 3.2: Crear implementaciones de parametrización

**Archivos a crear en `src/ZooTech.Infrastructure/Parameterization/`:**

| # | Archivo | Implementa |
|---|---|---|
| 1 | `TenantConfigurationService.cs` | `ITenantConfiguration` |
| 2 | `TenantConfigurationRepository.cs` | `ITenantConfigurationRepository` |
| 3 | `CacheInvalidationPublisher.cs` | `ICacheInvalidationPublisher` |
| 4 | `RedisSyncSubscriber.cs` | `BackgroundService` |
| 5 | `SignalRNotifier.cs` | `IParameterSyncNotifier` |

Ver código completo en `parameterization_architecture.md` sección 4.C.

### Paso 3.3: Crear SignalR Hub

**Archivo:** `src/ZooTech.Infrastructure/SignalR/ParameterSyncHub.cs`

Ver código completo en `parameterization_architecture.md` sección 4.D.

### Paso 3.4: Actualizar DependencyInjection.cs de Infrastructure

**Archivo:** `src/ZooTech.Infrastructure/DependencyInjection.cs`

Agregar al final del método `AddInfrastructure`, antes del `return`:

```csharp
// ============================================
// Parameterization
// ============================================

services.AddScoped<ITenantConfiguration, TenantConfigurationService>();
services.AddScoped<ITenantConfigurationRepository, TenantConfigurationRepository>();
services.AddScoped<ICacheInvalidationPublisher, CacheInvalidationPublisher>();
services.AddScoped<IParameterSyncNotifier, SignalRNotifier>();

// Background Services
services.AddHostedService<RedisSyncSubscriber>();
```

Agregar los using necesarios:

```csharp
using ZooTech.Application.Common.Gateway.Configuration;
using ZooTech.Application.Common.Gateway.SignalR;
using ZooTech.Infrastructure.Parameterization;
```

### Paso 3.5: Agregar MSBuild Target a csproj

**Archivo:** `src/ZooTech.Infrastructure/ZooTech.Infrastructure.csproj`

Agregar antes del cierre `</Project>`:

```xml
<Target Name="GenerateZooParameters" BeforeTargets="BeforeBuild">
  <Message Importance="High" Text="[T4] Regenerando parámetros de dominio desde BD..." />
  <Exec Command="dotnet t4 &quot;$(ProjectDir)Configuration\ZooParameters.tt&quot; -o &quot;$(SolutionDir)src\ZooTech.Domain\Generated\ZooParameters.cs&quot;"
        IgnoreExitCode="true" />
</Target>
```

> **Nota:** `IgnoreExitCode="true"` permite que el build continúe si el T4 falla (ej. sin conexión a BD).

### Paso 3.6: Crear archivo T4

**Archivo:** `src/ZooTech.Infrastructure/Configuration/ZooParameters.tt`

Crear el archivo T4 que:
1. Se conecta a la BD via ADO.NET (`Microsoft.Data.SqlClient`)
2. Lee la connection string desde `appsettings.Development.json` (sección `ConnectionStrings:TenantCatalogConnection`)
3. Consulta las 3 tablas: `setting_definitions`, `features`, `rule_definitions`
4. Genera `ZooParameters.cs` siguiendo el formato de `parameterization_architecture.md` sección 4.A

El T4 debe:
- Agrupar settings por `category` y generar sub-clases dentro de `ZooSettings`
- Mapear `data_type` SQL a tipos C# (INT→int, BOOLEAN→bool, DECIMAL→decimal, STRING→string)
- Convertir `UPPER_SNAKE_CASE` codes a `PascalCase` para nombres de campos
- Agrupar features por `category` y generar sub-clases dentro de `ZooFeatures`
- Agrupar rules por `module` y generar sub-clases dentro de `ZooRules`
- Manejar errores de conexión con try/catch (generar stub vacío si falla)

**Verificación:**
```powershell
dotnet build src/ZooTech.Infrastructure/ZooTech.Infrastructure.csproj
```
Debe compilar sin errores.

---

## 📂 Fase 4: InterfaceAdapters — Controller y DTOs

### Paso 4.1: Crear DTOs

**Archivo:** `src/ZooTech.InterfaceAdapters/Modules/Module_Tenancing/DTOs/Requests/UpdateParameterRequestDto.cs`

```csharp
namespace ZooTech.InterfaceAdapters.Modules.Module_Tenancing.DTOs.Requests;

public record UpdateParameterRequestDto(string Value);
```

### Paso 4.2: Crear Controller

**Archivo:** `src/ZooTech.InterfaceAdapters/Modules/Module_Tenancing/Controllers/TenantParametersController.cs`

Ver código completo en `parameterization_architecture.md` sección 4.E.

**Verificación:**
```powershell
dotnet build src/ZooTech.InterfaceAdapters/ZooTech.InterfaceAdapters.csproj
```

---

## 📂 Fase 5: API — Configuración de SignalR y DI

### Paso 5.1: Agregar paquete NuGet

**Archivo:** `src/ZooTech.API/ZooTech.API.csproj`

Agregar:

```xml
<PackageReference Include="Microsoft.AspNetCore.SignalR.StackExchangeRedis" Version="10.0.7" />
```

### Paso 5.2: Modificar Program.cs

**Archivo:** `src/ZooTech.API/Program.cs`

Agregar las siguientes líneas en la sección de registro de servicios:

```csharp
// SignalR
builder.Services.AddSignalR()
    .AddStackExchangeRedis(
        builder.Configuration["Redis:ConnectionString"] ?? "localhost:6379",
        options =>
        {
            options.Configuration.ChannelPrefix = new RedisChannel("ZooTech", RedisChannel.PatternMode.Literal);
        }
    );
```

Agregar el mapping del hub en el pipeline (después de `app.MapControllers()`):

```csharp
app.MapHub<ZooTech.Infrastructure.SignalR.ParameterSyncHub>("/hubs/parameters");
```

Actualizar la política CORS para permitir SignalR:

```csharp
policy.AllowCredentials();
```

Agregar el using:

```csharp
using ZooTech.Infrastructure.SignalR;
```

### Paso 5.3: Agregar Swagger para el nuevo endpoint

Asegurar que `TenantParametersController` tenga el atributo:

```csharp
[ApiExplorerSettings(GroupName = "public")]
```

**Verificación:**
```powershell
dotnet build "ZooTech Backend - Solution.slnx"
```
La solución completa debe compilar sin errores.

---

## 📂 Fase 6: T4 Template Completo

### Paso 6.1: Escribir el T4 Template

**Archivo:** `src/ZooTech.Infrastructure/Configuration/ZooParameters.tt`

El template debe:

```
<#@ template language="C#" #>
<#@ output extension=".cs" #>
<#@ assembly name="Microsoft.Data.SqlClient" #>
<#@ import namespace="Microsoft.Data.SqlClient" #>
<#@ import namespace="System.Collections.Generic" #>
<#@ import namespace="System.Linq" #>
<#@ import namespace="System.Globalization" #>
<#@ import namespace="System.IO" #>

[Control code: leer connection string de appsettings]
[Control code: conectar a BD]
[Control code: SELECT * FROM setting_definitions]
[Control code: SELECT * FROM features WHERE deleted_at IS NULL]
[Control code: SELECT * FROM rule_definitions]

[Output: namespace ZooTech.Domain.Generated;]
[Output: using ZooTech.Domain.Parameters;]
[Output: public static class ZooSettings { ... }]
[Output: public static class ZooFeatures { ... }]
[Output: public static class ZooRules { ... }]
```

### Paso 6.2: Ejecutar T4 manualmente para generar código

```powershell
dotnet t4 src/ZooTech.Infrastructure/Configuration/ZooParameters.tt -o src/ZooTech.Domain/Generated/ZooParameters.cs
```

### Paso 6.3: Verificar que el código generado compila

```powershell
dotnet build src/ZooTech.Domain/ZooTech.Domain.csproj
```

---

## 📂 Fase 7: Pruebas Unitarias

### Paso 7.1: Crear proyecto de tests de Application (si no tiene tests reales)

Verificar que `tests/Unit/ZooTech.Application.UnitTests/` existe y tiene referencia a `ZooTech.Application`.

### Paso 7.2: Tests de Handlers

**Archivo:** `tests/Unit/ZooTech.Application.UnitTests/Modules/Module_Tenancing/UpdateTenantParameterHandlerUnitTests.cs`

```
Tests a implementar:
1. Handle_Setting_Should_Call_UpsertSettingAsync_With_Correct_Parameters
2. Handle_Feature_Should_Call_UpsertFeatureAsync_With_Parsed_Bool
3. Handle_Should_Call_CacheInvalidation_When_Upsert_Succeeds
4. Handle_Should_Call_SignalRNotifier_When_Upsert_Succeeds
5. Handle_Should_NOT_Call_CacheInvalidation_When_Upsert_Fails
6. Handle_Should_Return_False_For_Invalid_ParameterType
```

**Archivo:** `tests/Unit/ZooTech.Application.UnitTests/Modules/Module_Tenancing/ResetTenantParameterHandlerUnitTests.cs`

```
Tests a implementar:
1. Handle_Setting_Should_Call_DeleteTenantSettingAsync
2. Handle_Feature_Should_Call_DeleteTenantFeatureAsync
3. Handle_Should_Call_CacheInvalidation_When_Delete_Succeeds
4. Handle_Should_Return_False_When_Setting_Not_Found
5. Handle_Should_Return_False_For_Invalid_ParameterType
```

**Archivo:** `tests/Unit/ZooTech.Application.UnitTests/Modules/Module_Tenancing/GetTenantConfigurationHandlerUnitTests.cs`

```
Tests a implementar:
1. Handle_Should_Call_GetSnapshotAsync_With_Correct_TenantId
2. Handle_Should_Return_Merged_Configuration
```

### Paso 7.3: Tests de Validadores

**Archivo:** `tests/Unit/ZooTech.Application.UnitTests/Modules/Module_Tenancing/UpdateTenantParameterValidatorUnitTests.cs`

```
Tests a implementar:
1. Should_Have_Error_When_TenantId_Is_Zero
2. Should_Have_Error_When_Code_Is_Empty
3. Should_Have_Error_When_Code_Exceeds_100_Chars
4. Should_Have_Error_When_Value_Is_Null
5. Should_Not_Have_Error_When_All_Fields_Valid
```

**Archivo:** `tests/Unit/ZooTech.Application.UnitTests/Modules/Module_Tenancing/ResetTenantParameterValidatorUnitTests.cs`

```
Tests similares a UpdateTenantParameterValidator pero sin Value
```

### Paso 7.4: Tests de Infraestructura

**Archivo:** `tests/Unit/ZooTech.Infrastructure.UnitTests/Parameterization/TenantConfigurationServiceUnitTests.cs`

```
Tests a implementar:
1. Get_Should_Return_L1_Cached_Value_When_Available
2. Get_Should_Return_DefaultValue_When_Setting_Not_In_Snapshot
3. IsEnabled_Should_Return_True_When_Feature_Enabled
4. IsEnabled_Should_Return_False_When_Feature_Disabled
5. GetRaw_Should_Return_Null_When_Code_Not_Found
6. EvictL1_Should_Remove_Cached_Snapshot
```

**Archivo:** `tests/Unit/ZooTech.Infrastructure.UnitTests/Parameterization/TenantConfigurationRepositoryUnitTests.cs`

```
Usar EF Core InMemory database. Crear datos de prueba con setting_group + setting_definition + setting_value.

Tests a implementar:
1. GetSnapshotAsync_Should_Merge_Global_And_Tenant_Values
2. GetSnapshotAsync_Should_Return_Defaults_When_No_Tenant_Values
3. GetSnapshotAsync_Should_Filter_Deleted_Settings (deleted_at != null)
4. GetSnapshotAsync_Should_Only_Include_Active_Settings (is_active = true)
5. UpsertSettingAsync_Should_Create_New_SettingValue_With_ActorType_TENANT
6. UpsertSettingAsync_Should_Update_Existing_SettingValue
7. UpsertSettingAsync_Should_Return_False_When_Definition_Not_Found
8. UpsertSettingAsync_Should_Return_False_When_Definition_Inactive
9. DeleteTenantSettingAsync_Should_SoftDelete_SettingValue (set deleted_at + is_active=false)
10. DeleteTenantSettingAsync_Should_Return_False_When_Not_Found
11. UpsertFeatureAsync_Should_Create_New_TenantFeature
12. UpsertFeatureAsync_Should_Update_Existing_TenantFeature
13. DeleteTenantFeatureAsync_Should_Remove_TenantFeature

Nota: Para tests de setting_value, usar la convención actor_type = "TENANT" y actor_id = null.
Nota: EF Core InMemory no soporta índices únicos compuestos ni [Index] attributes.
Ignorar las restricciones de índice único en los tests.
```

**Archivo:** `tests/Unit/ZooTech.Infrastructure.UnitTests/Parameterization/CacheInvalidationPublisherUnitTests.cs`

```
Tests a implementar (mock IConnectionMultiplexer):
1. InvalidateTenantConfigAsync_Should_Delete_Redis_Key
2. InvalidateTenantConfigAsync_Should_Publish_To_Channel
3. InvalidateTenantConfigAsync_Should_Evict_L1_Cache
```

### Paso 7.5: Agregar paquetes de test necesarios

Verificar que los `.csproj` de test tengan:

```xml
<!-- Application.UnitTests -->
<PackageReference Include="Moq" Version="4.20.72" />
<PackageReference Include="FluentValidation" Version="12.1.1" />

<!-- Infrastructure.UnitTests -->
<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="10.0.8" />
<PackageReference Include="Moq" Version="4.20.72" />
```

### Paso 7.6: Ejecutar tests

```powershell
dotnet test "ZooTech Backend - Solution.slnx" --verbosity normal
```

---

## 📂 Fase 8: Pruebas de Integración

### Paso 8.1: Tests de API (Integration)

**Archivo:** `tests/Integration/ZooTech.API.IntegrationTests/Parameterization/TenantParametersIntegrationTests.cs`

```
Usar WebApplicationFactory<Program> con servicios mockeados.

Tests a implementar:
1. GET_/parameters_Should_Return_200_With_Configuration
2. PUT_/parameters/setting/CODE_Should_Return_200_On_Success
3. PUT_/parameters/setting/INVALID_Should_Return_404
4. PUT_/parameters/invalidtype/CODE_Should_Return_400
5. DELETE_/parameters/setting/CODE_Should_Return_200_On_Success
6. DELETE_/parameters/setting/INVALID_Should_Return_404
```

### Paso 8.2: Tests de SignalR (Integration)

**Archivo:** `tests/Integration/ZooTech.InterfaceAdapters.IntegrationTests/SignalR/ParameterSyncHubIntegrationTests.cs`

```
Tests a implementar:
1. Hub_Should_Add_Connection_To_Tenant_Group
2. Hub_Should_Remove_Connection_From_Tenant_Group_On_Disconnect
```

---

## 📂 Fase 9: Verificación Final

### Paso 9.1: Build completo

```powershell
dotnet build "ZooTech Backend - Solution.slnx" --configuration Release
```

### Paso 9.2: Todos los tests

```powershell
dotnet test "ZooTech Backend - Solution.slnx" --verbosity normal
```

### Paso 9.3: Verificar que la app arranca

```powershell
dotnet run --project src/ZooTech.API/ZooTech.API.csproj
```

Confirmar que:
- Swagger carga en `/swagger`
- El endpoint `GET /api/v1/admin/tenants/{id}/parameters` responde
- El hub SignalR está disponible en `/hubs/parameters`

---

## 📂 Checklist de Archivos

### Archivos Nuevos

| # | Ruta | Fase |
|---|---|---|
| 1 | `.config/dotnet-tools.json` | 0 |
| 2 | `scripts/watch-db-params.ps1` | 0 |
| 3 | `src/ZooTech.Domain/Parameters/SettingDefinition.cs` | 1 |
| 4 | `src/ZooTech.Domain/Parameters/FeatureFlag.cs` | 1 |
| 5 | `src/ZooTech.Domain/Parameters/RuleSchema.cs` | 1 |
| 6 | `src/ZooTech.Domain/Generated/ZooParameters.cs` | 1 |
| 7 | `src/ZooTech.Application/Common/Gateway/Configuration/ITenantConfiguration.cs` | 2 |
| 8 | `src/ZooTech.Application/Common/Gateway/Configuration/ITenantConfigurationRepository.cs` | 2 |
| 9 | `src/ZooTech.Application/Common/Gateway/Configuration/ICacheInvalidationPublisher.cs` | 2 |
| 10 | `src/ZooTech.Application/Common/Gateway/Configuration/TenantConfigurationSnapshot.cs` | 2 |
| 11 | `src/ZooTech.Application/Common/Gateway/SignalR/IParameterSyncNotifier.cs` | 2 |
| 12 | `src/ZooTech.Application/Modules/Module_Tenancing/UseCases/ParameterType.cs` | 2 |
| 13 | `src/ZooTech.Application/Modules/.../GetTenantConfiguration/GetTenantConfigurationQuery.cs` | 2 |
| 14 | `src/ZooTech.Application/Modules/.../GetTenantConfiguration/GetTenantConfigurationHandler.cs` | 2 |
| 15 | `src/ZooTech.Application/Modules/.../GetTenantConfiguration/GetTenantConfigurationValidator.cs` | 2 |
| 16 | `src/ZooTech.Application/Modules/.../GetTenantConfiguration/GetTenantConfigurationDto.cs` | 2 |
| 17 | `src/ZooTech.Application/Modules/.../UpdateTenantParameter/UpdateTenantParameterCommand.cs` | 2 |
| 18 | `src/ZooTech.Application/Modules/.../UpdateTenantParameter/UpdateTenantParameterHandler.cs` | 2 |
| 19 | `src/ZooTech.Application/Modules/.../UpdateTenantParameter/UpdateTenantParameterValidator.cs` | 2 |
| 20 | `src/ZooTech.Application/Modules/.../ResetTenantParameter/ResetTenantParameterCommand.cs` | 2 |
| 21 | `src/ZooTech.Application/Modules/.../ResetTenantParameter/ResetTenantParameterHandler.cs` | 2 |
| 22 | `src/ZooTech.Application/Modules/.../ResetTenantParameter/ResetTenantParameterValidator.cs` | 2 |
| 23 | `src/ZooTech.Infrastructure/Parameterization/TenantConfigurationService.cs` | 3 |
| 24 | `src/ZooTech.Infrastructure/Parameterization/TenantConfigurationRepository.cs` | 3 |
| 25 | `src/ZooTech.Infrastructure/Parameterization/CacheInvalidationPublisher.cs` | 3 |
| 26 | `src/ZooTech.Infrastructure/Parameterization/RedisSyncSubscriber.cs` | 3 |
| 27 | `src/ZooTech.Infrastructure/Parameterization/SignalRNotifier.cs` | 3 |
| 28 | `src/ZooTech.Infrastructure/SignalR/ParameterSyncHub.cs` | 3 |
| 29 | `src/ZooTech.Infrastructure/Configuration/ZooParameters.tt` | 6 |
| 30 | `src/ZooTech.InterfaceAdapters/Modules/.../DTOs/Requests/UpdateParameterRequestDto.cs` | 4 |
| 31 | `src/ZooTech.InterfaceAdapters/Modules/.../Controllers/TenantParametersController.cs` | 4 |
| 32 | `tests/Unit/.../UpdateTenantParameterHandlerUnitTests.cs` | 7 |
| 33 | `tests/Unit/.../ResetTenantParameterHandlerUnitTests.cs` | 7 |
| 34 | `tests/Unit/.../GetTenantConfigurationHandlerUnitTests.cs` | 7 |
| 35 | `tests/Unit/.../UpdateTenantParameterValidatorUnitTests.cs` | 7 |
| 36 | `tests/Unit/.../ResetTenantParameterValidatorUnitTests.cs` | 7 |
| 37 | `tests/Unit/.../TenantConfigurationServiceUnitTests.cs` | 7 |
| 38 | `tests/Unit/.../TenantConfigurationRepositoryUnitTests.cs` | 7 |
| 39 | `tests/Unit/.../CacheInvalidationPublisherUnitTests.cs` | 7 |
| 40 | `tests/Integration/.../TenantParametersIntegrationTests.cs` | 8 |
| 41 | `tests/Integration/.../ParameterSyncHubIntegrationTests.cs` | 8 |

### Archivos Modificados

| # | Ruta | Cambio |
|---|---|---|
| 1 | `src/ZooTech.Infrastructure/DependencyInjection.cs` | Registrar servicios de parametrización |
| 2 | `src/ZooTech.Infrastructure/ZooTech.Infrastructure.csproj` | Agregar MSBuild Target |
| 3 | `src/ZooTech.Application/Common/Models/GeneralResponseDTO.cs` | Cambiar `internal` a `public` |
| 4 | `src/ZooTech.API/Program.cs` | SignalR + Hub mapping + CORS |
| 5 | `src/ZooTech.API/ZooTech.API.csproj` | Agregar SignalR.StackExchangeRedis |

> **Nota:** `TenantCatalogDb.cs` y `tenant.cs` ya están actualizados con los DbSets, Fluent API y navegaciones necesarias. No requieren modificación.

### Archivos Eliminados

| # | Ruta | Razón |
|---|---|---|
| 1 | `src/ZooTech.Domain/Class1.cs` | Stub autogenerado |
| 2 | `src/ZooTech.Application/Common/Gateway/Configuration/IConfiguration.cs` | Interfaz vacía reemplazada |
| 3 | `src/ZooTech.Infrastructure/Persistence/Entities/MainTenantsDb/tenant_setting.cs` | Entidad orphaned (sin DbSet, sin Fluent API, sin navegación) |

---

## 🚨 Manejo de Errores Durante Implementación

### Si `dotnet build` falla

1. Leer el error exacto del compilador
2. Verificar que todos los `using` estén correctos
3. Verificar que las dependencias entre proyectos estén declaradas en los `.csproj`
4. Si el error es de tipo no encontrado: verificar que el archivo fue creado en el namespace correcto
5. Si el error es de método no encontrado: verificar que la interfaz y la implementación coinciden

### Si `dotnet test` falla

1. Identificar si es error de compilación o de ejecución
2. Si es error de ejecución: verificar que los mocks estén configurados correctamente
3. Si es error de EF Core InMemory: verificar que la configuración del DbContext sea compatible con InMemory (evitar `.HasIndex()` que no funciona en InMemory — usar `.ToView()` exclusion si es necesario)
4. Si un test de infraestructura falla por conexión Redis: el test debe mockear `IConnectionMultiplexer`, no usar conexión real

### Si T4 falla

1. Verificar que `dotnet-t4` esté instalado: `dotnet tool list`
2. Verificar que la connection string en `appsettings.Development.json` sea válida
3. Verificar que la BD `zootech_main_tenant` tenga las tablas `setting_groups`, `setting_definitions`, `features`, `rule_definitions`
4. Si la BD no está disponible: el T4 debe generar un stub vacío que compile (ver Paso 6.1 nota sobre error handling)
5. Como fallback: usar el stub manual creado en Paso 1.2

### Si la app no arranca

1. Verificar que todas las dependencias de DI estén registradas
2. Verificar que `IConnectionMultiplexer` esté registrado (lo registra `GarnetCacheConnection` como singleton, pero verificar que `CacheInvalidationPublisher` y `RedisSyncSubscriber` puedan resolverlo)
3. Si Redis no está disponible en desarrollo: la app debe arrancar de todas formas (los servicios que usan Redis deben manejar la excepción)

### Acción correctiva: Redis no disponible en desarrollo

Si Redis/Garnet no está disponible en el entorno de desarrollo:

1. Modificar `CacheInvalidationPublisher` para envolver operaciones Redis en try/catch
2. Modificar `RedisSyncSubscriber` para manejar excepción de conexión en `ExecuteAsync`
3. `TenantConfigurationService` debe funcionar sin Redis (solo L1 + DB fallback)
4. Desactivar `AddStackExchangeRedis` para SignalR en Development (usar SignalR in-memory):

```csharp
if (app.Environment.IsDevelopment())
{
    builder.Services.AddSignalR();
}
else
{
    builder.Services.AddSignalR()
        .AddStackExchangeRedis(config["Redis:ConnectionString"] ?? "");
}
```

---

## 📂 Post-Implementación

### Acciones después de implementar todo

1. **Limpiar stubs de test**: Eliminar `UnitTest1.cs` de los proyectos donde se agregaron tests reales
2. **Verificar que no hay warnings**: `dotnet build --warnaserror` (o al menos revisar warnings críticos)
3. **Documentar endpoints**: Verificar que Swagger muestre los nuevos endpoints correctamente
4. **Actualizar `project_analysis.md`**: Marcar las carpetas vacías que ahora tienen contenido
5. **Sembrar datos de prueba**: Ejecutar los scripts SQL de `parameterization_governance.md` sección 4 en la BD de desarrollo

### Orden de verificación final

```powershell
# 1. Build completo
dotnet build "ZooTech Backend - Solution.slnx"

# 2. Tests
dotnet test "ZooTech Backend - Solution.slnx" --verbosity normal

# 3. Arrancar app
dotnet run --project src/ZooTech.API/ZooTech.API.csproj

# 4. Verificar endpoints (en otra terminal)
curl http://localhost:16000/swagger/public/swagger.json

# 5. Verificar T4
dotnet t4 src/ZooTech.Infrastructure/Configuration/ZooParameters.tt -o src/ZooTech.Domain/Generated/ZooParameters.cs
dotnet build src/ZooTech.Domain/ZooTech.Domain.csproj
```
