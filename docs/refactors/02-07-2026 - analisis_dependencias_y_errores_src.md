# Análisis de Dependencias y Errores — ZooTech Backend `src/`

**Fecha:** 02-07-2026  
**Objetivo:** Registrar todas las dependencias faltantes para que la aplicación corra, e informar errores de sintaxis, lógica y tiempo de ejecución encontrados en `src/`.  
**Restricción:** Solo análisis; el código fuente no ha sido modificado.

---

## Índice

1. [Resumen Ejecutivo](#1-resumen-ejecutivo)
2. [Dependencias Faltantes en DI](#2-dependencias-faltantes-en-di)
3. [Errores de Compilación (Sintaxis)](#3-errores-de-compilación-sintaxis)
4. [Errores de Lógica](#4-errores-de-lógica)
5. [Errores en Tiempo de Ejecución](#5-errores-en-tiempo-de-ejecución)
6. [Warnings Relevantes](#6-warnings-relevantes)
7. [Priorización Global](#7-priorización-global)

---

## 1. Resumen Ejecutivo

Se detectaron **30 hallazgos** en el código fuente (`src/`). Los temas críticos son:

- **2 errores de compilación** en `ValidationBehavior.cs` que bloquean toda la solución.
- **12+ servicios sin registrar** en el contenedor de DI (pipeline factories, interactors, validators, repositorios, factories de DbContext).
- **1 error de lógica grave** en `AppDomainException.CompleteErrorCode` donde `string.Concat()` no muta la cadena.
- **Errores de tipo** entre interfaz e implementación en `ITenantDbContextFactory` / `IGanaderiaDbContextFactory` (retornan `Task<T>` vs `T`).
- **Import innecesario** de `SharpCompress.Factories` en `GarnetCacheService`.
- **Duplicación** de `app.UseAuthorization()` en `Program.cs`.
- **Typo** en ruta del `AuthController` (`"api/vi/auth"` en vez de `"api/v1/auth"`).

---

## 2. Dependencias Faltantes en DI

### DEP-001 — `IAdminLoginBehaviorPipeline` / `AdminLoginBehaviorPipeline` no registrados
- **Severidad:** Crítica
- **Archivos:** `src/ZooTech.Application/Common/Behaviors/Module_Auth/AdminLogin/AdminLoginBehaviorPipeline.cs`, `src/ZooTech.Application/DependencyInjection.cs`
- **Descripción:** `AuthController` inyecta `IAdminLoginBehaviorPipeline` en su constructor, pero ninguna capa lo registra en DI. Al resolver el controller, se lanzará `InvalidOperationException`.
- **Corrección:** Registrar en `DependencyInjection.cs` de Application:
```csharp
services.AddScoped<IAdminLoginBehaviorPipeline, AdminLoginBehaviorPipeline>();
```

---

### DEP-002 — `IRegularLoginBehaviorPipeline` / `RegularLoginBehaviorPipeline` no registrados
- **Severidad:** Crítica
- **Archivos:** `src/ZooTech.Application/Common/Behaviors/Module_Auth/RegularLogin/RegularLoginBehaviorPipeline.cs`, `src/ZooTech.Application/DependencyInjection.cs`
- **Descripción:** Mismo problema que DEP-001. `AuthController` inyecta `IRegularLoginBehaviorPipeline`.
- **Corrección:**
```csharp
services.AddScoped<IRegularLoginBehaviorPipeline, RegularLoginBehaviorPipeline>();
```

---

### DEP-003 — `ICreateTenantPipelineFactory` / `CreateTenantPipelineFactory` no registrados
- **Severidad:** Crítica
- **Archivos:** `src/ZooTech.Application/Common/Behaviors/Module_Tenancing/CreateTenantBehaviorPipeline.cs`, `src/ZooTech.Application/DependencyInjection.cs`
- **Descripción:** `TenancingController` inyecta `ICreateTenantPipelineFactory`, pero no está registrado en DI.
- **Corrección:**
```csharp
services.AddScoped<ICreateTenantPipelineFactory, CreateTenantPipelineFactory>();
```

---

### DEP-004 — Interactors (Input Ports) no registrados
- **Severidad:** Crítica
- **Archivos:** Múltiples
- **Descripción:** Los interactors que implementan los input ports no están registrados en DI:
  - `IAdminLoginInputPort` → `AdminLoginInteractor`
  - `IRegularLoginInputPort` → `RegularLoginInteractor`
  - `ICreateTenantInputPort` → `CreateTenantInteractor`
- **Corrección:**
```csharp
services.AddScoped<IAdminLoginInputPort, AdminLoginInteractor>();
services.AddScoped<IRegularLoginInputPort, RegularLoginInteractor>();
services.AddScoped<ICreateTenantInputPort, CreateTenantInteractor>();
```

---

### DEP-005 — Validators custom (`ICommandValidator<T>`) no registrados
- **Severidad:** Crítica
- **Archivos:** Múltiples validators en `src/ZooTech.Application/Modules/`
- **Descripción:** Los validadores implementan `ICommandValidator<T>` pero no están registrados. El `ValidationBehavior<TRequest, TResponse>` los requiere por constructor.
  - `ICommandValidator<AdminLoginCommand>` → `AdminLoginValidator`
  - `ICommandValidator<string>` (RegularLogin) → `RegularLoginValidator`
  - `ICommandValidator<CreateTenantCommand>` → `CreateTenantValidation`
- **Corrección:**
```csharp
services.AddScoped<ICommandValidator<AdminLoginCommand>, AdminLoginValidator>();
services.AddScoped<ICommandValidator<string>, RegularLoginValidator>();
services.AddScoped<ICommandValidator<CreateTenantCommand>, CreateTenantValidation>();
```

---

### DEP-006 — `IUsuarioRepository` / `UsuarioRepository` no registrados
- **Severidad:** Crítica
- **Archivos:** `src/ZooTech.Application/Common/Gateway/Repositories/GanaderiaDb/IUsuarioRepository.cs`, `src/ZooTech.Infrastructure/Persistence/Repositories/GanaderiaDb/UsuarioRepository.cs`
- **Descripción:** `RegularLoginInteractor` inyecta `IUsuarioRepository` pero no está registrado.
- **Corrección:**
```csharp
services.AddScoped<IUsuarioRepository, UsuarioRepository>();
```

---

### DEP-007 — `IAdminUserRepository` / `AdminUserRepository` no registrados
- **Severidad:** Crítica
- **Archivos:** `src/ZooTech.Application/Common/Gateway/Repositories/MainTenantsDb/IAdminUserRepository.cs`, `src/ZooTech.Infrastructure/Persistence/Repositories/MainTenantsDb/AdminUserRepository.cs`
- **Descripción:** `AdminLoginInteractor` inyecta `IAdminUserRepository` pero no está registrado en `DependencyInjection.cs` de Infrastructure.
- **Corrección:**
```csharp
services.AddScoped<IAdminUserRepository, AdminUserRepository>();
```

---

### DEP-008 — `IDbContextFactory` / `DbContextFactory` no registrados
- **Severidad:** Alta
- **Archivos:** `src/ZooTech.Infrastructure/Persistence/Context/IDbContextFactory.cs`, `src/ZooTech.Infrastructure/Persistence/Context/DbContextFactory.cs`
- **Descripción:** `DbContextFactory` es un factory unificado que decide entre `GanaderiaDbContext` y `TenantCatalogDb` según el `TenantType`. No está registrado en DI.
- **Corrección:**
```csharp
services.AddScoped<IDbContextFactory, DbContextFactory>();
```

---

### DEP-009 — `ValidationBehavior<,>` y `LoggingBehavior<,>` registrados como `MediatR.IPipelineBehavior<,>` pero implementan `IBehavior<,>`
- **Severidad:** Crítica
- **Archivos:** `src/ZooTech.Application/DependencyInjection.cs:16-17`
- **Descripción:** `DependencyInjection.cs` de Application registra:
  ```csharp
  services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(Common.Behaviors.ValidationBehavior<,>));
  services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(Common.Behaviors.LoggingBehavior<,>));
  ```
  Pero `ValidationBehavior<,>` implementa `IBehavior<,>` (interfaz custom), NO `MediatR.IPipelineBehavior<,>`. Esto genera un error de tipo en runtime al resolver el servicio. Los behaviors custom se usan dentro de los `BehaviorPipeline` factories, no a través de MediatR.
- **Corrección:** Eliminar estos registros de MediatR. Los behaviors se resuelven automáticamente cuando los PipelineFactories los inyectan por constructor.
```csharp
// Eliminar estas líneas:
// services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(Common.Behaviors.ValidationBehavior<,>));
// services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(Common.Behaviors.LoggingBehavior<,>));

// Registrar los behaviors como open generics para que el DI los resuelva:
services.AddTransient(typeof(ValidationBehavior<,>));
services.AddTransient(typeof(LoggingBehavior<,>));
services.AddTransient(typeof(AuditBehavior<,>));
```

---

### DEP-010 — `IAppCacheService` inyectado en `GarnetCacheService` requiere `ITenantContext` como Singleton
- **Severidad:** Alta
- **Archivos:** `src/ZooTech.Infrastructure/DependencyInjection.cs:46`, `src/ZooTech.Infrastructure/Caching/Garnet/GarnetCacheService.cs`
- **Descripción:** `GarnetCacheService` se registra como **Singleton** e inyecta `ITenantContext` que es **Scoped**. Esto viola las reglas de captive dependency de .NET: un singleton no puede depender de un scoped. En runtime, `ITenantContext` será la misma instancia para todos los requests, rompiendo el aislamiento multitenant del cache.
- **Corrección:** Cambiar `GarnetCacheService` a Scoped, o extraer el `ITenantContext` a un accessor.

---

### DEP-011 — `AddValidatorsFromAssembly` registra validators de FluentValidation pero el proyecto usa `ICommandValidator<T>`
- **Severidad:** Media
- **Archivos:** `src/ZooTech.Application/DependencyInjection.cs:14`
- **Descripción:** `services.AddValidatorsFromAssembly(...)` busca clases que implementen `FluentValidation.IValidator<T>`. Los validators del proyecto implementan `ICommandValidator<T>` (interfaz custom), por lo que esta llamada no registra ningún validador. Es una dependencia muerta del paquete FluentValidation.
- **Corrección:** Eliminar la llamada y el paquete FluentValidation si no se usa, o migrar los validators a FluentValidation.

---

### DEP-012 — `AddMediatR` registra handlers de MediatR pero no existen `IRequestHandler<,>` en el proyecto
- **Severidad:** Media
- **Archivos:** `src/ZooTech.Application/DependencyInjection.cs:11-12`
- **Descripción:** `services.AddMediatR(...)` busca clases que implementen `IRequestHandler<TRequest, TResponse>`. El proyecto usa interactors custom (`ICreateTenantInputPort`, `IAdminLoginInputPort`, etc.), no handlers de MediatR. Esta llamada no registra nada útil.
- **Corrección:** Eliminar la llamada y el paquete MediatR si no se usa en ningún flujo.

---

## 3. Errores de Compilación (Sintaxis)

### ERR-001 — `ValidationBehavior.cs:25` — Argumentos invertidos en constructor de `ValidationException`
- **Severidad:** Crítica (bloquea compilación)
- **Archivos:** `src/ZooTech.Application/Common/Behaviors/ValidationBehavior.cs:25`
- **Descripción:** El código actual:
  ```csharp
  throw new ValidationException(_validator.ModuleName.ToString(), errors);
  ```
  Pero `ValidationException` ahora tiene la firma:
  ```csharp
  public ValidationException(List<string> errors, ScopeName scopeName, ModuleName? moduleName = null)
  ```
  Se pasa `string` donde se espera `List<string>` (argumento 1) y `List<string>` donde se espera `ScopeName` (argumento 2). Genera **2 errores CS1503**.
- **Corrección:**
```csharp
throw new ValidationException(errors, ScopeName.Application, _validator.ModuleName);
```

---

### ERR-002 — `GanaderiaDbContextFactory` y `TenantDbContextFactory` retornan `Task<T>` pero la interfaz retorna `T`
- **Severidad:** Alta (compila pero causa confusión)
- **Archivos:**
  - `src/ZooTech.Infrastructure/Persistence/Context/IGanaderiaDbContextFactory.cs` — `Task<GanaderiaDbContext> CreateDbContext()`
  - `src/ZooTech.Infrastructure/Persistence/Context/GanaderiaDbContextFactory.cs` — `async Task<GanaderiaDbContext> CreateDbContext()`
  - `src/ZooTech.Infrastructure/Persistence/Context/ITenantDbContextFactory.cs` — `Task<TenantCatalogDb> CreateDbContext()`
  - `src/ZooTech.Infrastructure/Tenant/TenantDbContextFactory.cs` — `async Task<TenantCatalogDb> CreateDbContext()`
- **Descripción:** Las interfaces declaran `Task<T>` y las implementaciones son `async`, lo cual es consistente. Sin embargo, el `TenantDatabaseMigrator` llama `await _ganaderiaDbContextFactory.CreateDbContext()` lo cual funciona. Pero `DbContextFactory` también usa `await`. Aunque compila, el uso de `async` en factories que solo crean un contexto y verifican conexión es inusual. La verificación `CanConnectAsync()` lanza excepción si no hay conexión, lo que impide usar las factories en tests sin BD real.
- **Corrección:** Separar la creación del contexto de la verificación de conexión. Las factories deberían crear el contexto sin verificar conexión; la verificación debería hacerse en un paso posterior.

---

## 4. Errores de Lógica

### LOG-001 — `AppDomainException.CompleteErrorCode` no se concatena correctamente
- **Severidad:** Alta
- **Archivos:** `src/ZooTech.Domain/Shared/Exceptions/AppDomainException.cs:35,58`
- **Descripción:** El código:
  ```csharp
  CompleteErrorCode = ModuleName is null
      ? ScopeName.ToString().ToUpper()
      : $"{ScopeName.ToString().ToUpper()}_{ModuleName?.ToString().ToUpper()}";
  CompleteErrorCode.Concat($"_{errorCode.ToUpper()}");
  ```
  `string.Concat()` **retorna** una nueva cadena pero no muta `CompleteErrorCode`. El resultado se descarta. El `CompleteErrorCode` nunca incluye el `errorCode`.
- **Corrección:**
```csharp
CompleteErrorCode = $"{CompleteErrorCode}_{errorCode.ToUpper()}";
```

---

### LOG-002 — `GarnetCacheService.SaveAsync` no sobrescribe valores existentes
- **Severidad:** Media
- **Archivos:** `src/ZooTech.Infrastructure/Caching/Garnet/GarnetCacheService.cs:111-133`
- **Descripción:** `SaveAsync` verifica si la clave existe (`TryGetAsync`) y solo guarda si **no** se encuentra. Esto significa que si se llama dos veces con la misma clave, la segunda llamada no actualiza el valor. En `RegularLoginInteractor`, el token de sesión se guarda con `SaveAsync`, lo que significa que si el usuario hace login dos veces, el segundo token no reemplaza al primero.
- **Corrección:** Eliminar la verificación `if(!found)` y siempre sobrescribir, o usar un método `SetAsync` dedicado.

---

### LOG-003 — `AuthController` usa ruta `"api/vi/auth"` — typo probable
- **Severidad:** Media
- **Archivos:** `src/ZooTech.InterfaceAdapters/Modules/Module_Auth/AuthController.cs:10`
- **Descripción:** `[Route("api/vi/auth")]` — probablemente debería ser `"api/v1/auth"` (v-uno, no v-i).
- **Corrección:** Cambiar a `"api/v1/auth"`.

---

### LOG-004 — `Program.cs` — `app.UseAuthorization()` duplicado
- **Severidad:** Baja
- **Archivos:** `src/ZooTech.API/Program.cs:77,104`
- **Descripción:** `app.UseAuthorization()` se invoca dos veces: una en la línea 77 (junto a `UseAuthentication`) y otra en la línea 104. La segunda llamada es innecesaria y puede causar comportamiento inesperado.
- **Corrección:** Eliminar la segunda invocación en la línea 104.

---

### LOG-005 — `AdminLoginValidator` y `RegularLoginValidator` — validación de password incorrecta
- **Severidad:** Media
- **Archivos:** `src/ZooTech.Application/Modules/Module_Auth/UseCases/AdminLogin/AdminLoginValidator.cs:19`
- **Descripción:** `cmd.Password.Length <= 8` rechaza contraseñas de exactamente 8 caracteres. La condición lógica correcta para "mínimo 8 caracteres" es `< 8`. Además, la condición `cmd.Password.Length <= 8 || cmd.Password.Length == 0` es redundante: si `Length == 0`, ya es `<= 8`.
- **Corrección:**
```csharp
else if (cmd.Password.Length < 8)
```

---

### LOG-006 — `CreateTenantValidator` usa `ModuleName.Celo` en vez de `ModuleName.Tenancing`
- **Severidad:** Baja (corregido en la última actualización)
- **Archivos:** `src/ZooTech.Application/Modules/Module_Tenancing/UseCases/CreateTenant/CreateTenantValidator.cs:8`
- **Descripción:** Anteriormente retornaba `ModuleName.Celo`. Ahora retorna `ModuleName.Tenancing` (corregido).
- **Estado:** ✅ Corregido.

---

### LOG-007 — `GarnetCacheService` importa `SharpCompress.Factories` innecesariamente
- **Severidad:** Baja
- **Archivos:** `src/ZooTech.Infrastructure/Caching/Garnet/GarnetCacheService.cs:4`
- **Descripción:** `using SharpCompress.Factories;` no se usa en la clase. Es un import residual que genera acoplamiento innecesario con un paquete externo.
- **Corrección:** Eliminar la línea.

---

### LOG-008 — `RestrictTenantTypeFilter` lanza `UnauthorizedAccessException` del sistema
- **Severidad:** Media
- **Archivos:** `src/ZooTech.InterfaceAdapters/Filters/RestrictTenantTypeFilter.cs:25`
- **Descripción:** `throw new Exceptions.UnauthorizedAccessException()` lanza una excepción del sistema (`System.UnauthorizedAccessException`). El `ExceptionHandlingMiddleware` la capturará en el catch genérico `Exception` y devolverá HTTP 500 en vez de 403 Forbidden.
- **Corrección:** Crear una excepción custom de aplicación (ej. `ForbiddenException`) que el middleware reconozca y mapee a 403.

---

### LOG-009 — `RegularLoginInteractor` pasa `domainEntity.Email` (tipo `Email`) donde se espera `string`
- **Severidad:** Media
- **Archivos:** `src/ZooTech.Application/Modules/Module_Auth/UseCases/RegularLogin/RegularLoginInteractor.cs:44`
- **Descripción:** `domainEntity.Email` es de tipo `Email` (Value Object). `GenerateToken` espera `string email`. Existe un operador implícito `Email → string`, por lo que compila. Sin embargo, si el operador implícito se elimina en un refactor futuro, esto se romperá silenciosamente.
- **Corrección:** Usar `domainEntity.Email.Value` explícitamente.

---

### LOG-010 — `AdminLoginInteractor` pasa `domainEntity.Email` (tipo `Email`) donde se espera `string`
- **Severidad:** Media
- **Archivos:** `src/ZooTech.Application/Modules/Module_Auth/UseCases/AdminLogin/AdminLoginInteractor.cs:51`
- **Descripción:** Mismo problema que LOG-009. `AdminUserDomainEntity.Email` es tipo `Email` (Value Object).
- **Corrección:** Usar `domainEntity.Email.Value` explícitamente.

---

## 5. Errores en Tiempo de Ejecución

### RUN-001 — `TenantDatabaseMigrator.MigrateAsync()` llama a `Database.MigrateAsync()` en un contexto InMemory
- **Severidad:** Alta (en tests)
- **Archivos:** `src/ZooTech.Infrastructure/Tenant/TenantDatabaseMigrator.cs:17`
- **Descripción:** `GanaderiaDbContext.Database.MigrateAsync()` falla con `InvalidOperationException` cuando el contexto usa InMemoryDatabase (en tests). El test `GanaderiaDbContextFactoryUnitTests` espera este error, pero el flujo real de provisioning también fallaría si se prueba con InMemory.
- **Corrección:** Verificar si el provider soporta migraciones antes de llamar `MigrateAsync()`.

---

### RUN-002 — `TenantProvisioningService` usa `TenantCatalogDb` directamente en vez de `ITenantDbContextFactory`
- **Severidad:** Alta
- **Archivos:** `src/ZooTech.Infrastructure/Tenant/TenantProvisioningService.cs`
- **Descripción:** El constructor recibe `ITenantDbContextFactory` y lo usa para obtener el `TenantCatalogDb`. Sin embargo, el contexto se obtiene una sola vez en el constructor (scoped). Si el servicio se reutiliza dentro del mismo scope para múltiples tenants, el contexto apuntará siempre al mismo catálogo.
- **Estado:** Actual — el constructor llama `_tenantCatalogDb = tenantDbContextFactory.CreateDbContext()` una sola vez.

---

### RUN-003 — `GarnetCacheService` como Singleton inyecta `ITenantContext` Scoped — Captive Dependency
- **Severidad:** Crítica
- **Archivos:** `src/ZooTech.Infrastructure/DependencyInjection.cs:46`
- **Descripción:** `GarnetCacheService` se registra como Singleton pero depende de `ITenantContext` (Scoped). .NET DI detectará esto como captive dependency y lanzará error en runtime al resolver el servicio (si `ValidateScopes` está habilitado). Si no está habilitado, el `ITenantContext` capturado será el del primer request, rompiendo el aislamiento multitenant.
- **Corrección:** Cambiar `GarnetCacheService` a Scoped o extraer el tenant ID de otra forma.

---

### RUN-004 — `JwtService` usa `int.Parse(_settings.ExpirationMinutes)` que puede fallar
- **Severidad:** Media
- **Archivos:** `src/ZooTech.Infrastructure/Identity/JwtService.cs:50`
- **Descripción:** `int.Parse(_settings.ExpirationMinutes)` lanza `FormatException` si el valor de configuración no es un entero válido. No hay manejo de error ni valor por defecto.
- **Corrección:** Usar `int.TryParse` con un valor por defecto (ej. 30 minutos).

---

### RUN-005 — `DbContextFactory.AutoGetDbContext` compara `_tenantContext.Type` con `TenantType` usando `==`
- **Severidad:** Media
- **Archivos:** `src/ZooTech.Infrastructure/Persistence/Context/DbContextFactory.cs:30,33`
- **Descripción:** `_tenantContext.Type` retorna `TenantType` (enum). La comparación `_tenantContext.Type == TenantType.Tenant` funciona para enums. Sin embargo, si `TenantContext.Type` no ha sido establecido (default = `TenantType.Tenant` por ser el primer valor del enum), el factory siempre retornará `GanaderiaDbContext`, incluso para requests sin tenant resuelto.
- **Corrección:** Validar que `TenantContext` haya sido poblado antes de usar `AutoGetDbContext`.

---

### RUN-006 — `ConnectionMultiplexer.Connect` en `DependencyInjection.AddInfrastructure` se ejecuta al inicio
- **Severidad:** Alta
- **Archivos:** `src/ZooTech.Infrastructure/DependencyInjection.cs:42`
- **Descripción:** `ConnectionMultiplexer.Connect(garnetConnectionString)` se ejecuta durante el registro de servicios (startup). Si Garnet/Redis no está disponible, la aplicación **no arranca**. Esto es problemático en desarrollo o en entornos donde el cache es opcional.
- **Corrección:** Usar lazy initialization o `IConnectionMultiplexer` como factory con retry.

---

## 6. Warnings Relevantes

### WRN-001 — CS8604: Posible argumento nulo en `GenerateToken`
- **Archivos:**
  - `src/ZooTech.Application/Modules/Module_Auth/UseCases/AdminLogin/AdminLoginInteractor.cs:50`
  - `src/ZooTech.Application/Modules/Module_Auth/UseCases/RegularLogin/RegularLoginInteractor.cs:43`
- **Descripción:** `domainEntity.UserName` es `string?` pero `GenerateToken` espera `string` (non-nullable). Puede causar `NullReferenceException` en runtime.

### WRN-002 — CS8618: Propiedades non-nullable sin inicializar en `SettingValueDto`
- **Archivos:** `src/ZooTech.Application/Common/Models/SettingValueDTO.cs:5-6`
- **Descripción:** `Code` y `Value` son `string` non-nullable pero no tienen inicializador. Deberían usar `= default!` o marcarse como nullable.

### WRN-003 — NU1510: Paquete `Microsoft.Extensions.DependencyInjection.Abstractions` innecesario
- **Archivos:** `src/ZooTech.InterfaceAdapters/ZooTech.InterfaceAdapters.csproj`
- **Descripción:** El paquete es redundante ya que se incluye transitivamente.

---

## 7. Priorización Global

| Prioridad | ID | Hallazgo | Impacto |
|-----------|----|---------|---------| 
| **Crítica** | ERR-001 | `ValidationBehavior` no compila (2 errores CS1503) | Bloquea toda la solución |
| **Crítica** | RUN-003 | `GarnetCacheService` Singleton + `ITenantContext` Scoped | Captive dependency, app no arranca o datos cruzados |
| **Crítica** | DEP-001 a DEP-003 | Pipeline factories no registrados | Controllers no se pueden resolver |
| **Crítica** | DEP-004 | Interactors no registrados | Pipeline factories no se pueden construir |
| **Crítica** | DEP-005 | Validators custom no registrados | `ValidationBehavior` no se puede construir |
| **Crítica** | DEP-009 | Behaviors registrados como MediatR pero implementan IBehavior | Error de tipo en runtime |
| **Crítica** | RUN-006 | `ConnectionMultiplexer.Connect` al inicio | App no arranca si Garnet está caído |
| **Alta** | LOG-001 | `CompleteErrorCode` no concatena | Códigos de error incompletos en respuestas |
| **Alta** | DEP-006, DEP-007 | Repositories no registrados | Interactors no se pueden construir |
| **Alta** | DEP-008 | `IDbContextFactory` no registrado | No hay factory unificado de contextos |
| **Alta** | DEP-010 | Singleton + Scoped captive dependency | Aislamiento multitenant roto |
| **Alta** | RUN-001 | `MigrateAsync` falla con InMemory | Tests de provisioning fallan |
| **Media** | LOG-002 | `SaveAsync` no sobrescribe | Tokens de sesión no se refrescan |
| **Media** | LOG-003 | Typo en ruta `api/vi/auth` | Endpoint no descubierto |
| **Media** | LOG-005 | Password validation `<= 8` | Contraseñas de 8 chars rechazadas |
| **Media** | LOG-008 | `UnauthorizedAccessException` → HTTP 500 | Código HTTP incorrecto |
| **Media** | RUN-004 | `int.Parse` sin manejo de error | Crash si config es inválida |
| **Media** | DEP-011, DEP-012 | FluentValidation y MediatR registrados sin uso | Dependencias muertas |
| **Baja** | LOG-004 | `UseAuthorization` duplicado | Ruido en pipeline |
| **Baja** | LOG-007 | Import `SharpCompress` innecesario | Acoplamiento fantasma |

---

> **Nota final:** Este documento fue generado el 02-07-2026 con base en el análisis estático del código fuente actualizado. Los hallazgos ERR-001 y DEP-001 a DEP-009 deben resolverse antes de que la aplicación pueda compilar y arrancar. Se recomienda crear una rama `fix/dependencies-and-compilation-errors` y aplicar los cambios en commits atómicos.
