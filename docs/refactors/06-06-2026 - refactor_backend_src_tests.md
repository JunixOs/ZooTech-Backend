# Análisis de Refactorización — ZooTech Backend

**Fecha:** 06-06-2026  
**Objetivo:** Identificar errores, inconsistencias, violaciones arquitectónicas y vacíos de cobertura de pruebas en `src/` y `tests/`.  
**Restricción:** Solo análisis y planificación; el código fuente no ha sido modificado.

---

## Índice

1. [Resumen Ejecutivo](#1-resumen-ejecutivo)
2. [Capa de Dominio (Domain)](#2-capa-de-dominio-domain)
3. [Capa de Aplicación (Application)](#3-capa-de-aplicación-application)
4. [Capa de Infraestructura (Infrastructure)](#4-capa-de-infraestructura-infrastructure)
5. [Capa de Adaptadores (InterfaceAdapters)](#5-capa-de-adaptadores-interfaceadapters)
6. [Composition Root (API / Program.cs)](#6-composition-root-api--programcs)
7. [Pruebas Unitarias y de Integración](#7-pruebas-unitarias-y-de-integración)
8. [Priorización Global](#8-priorización-global)

---

## 1. Resumen Ejecutivo

Se detectaron **más de 40 hallazgos** distribuidos en 6 áreas. Los temas críticos son:

- **Código muerto y stubs** (`Class1.cs`, `tenant_setting.cs`, interfaces vacías, `LoggingBehavior`).
- **Violación de reglas de dependencia y DI fragmentada**: todo el registro de servicios reside en `Program.cs`; los `DependencyInjection.cs` de Application e InterfaceAdapters están comentados o incompletos.
- **Patrón arquitectónico dual**: MediatR + Ports & Adapters conviven generando dependencias muertas (`ICreateTenantInputPort` inyectado pero no usado en el controller).
- **Inconsistencia de tipos y enums**: `TenantStatus` existe en Domain pero Application e Infrastructure operan con *magic strings* (`"INACTIVE"`, `"TRIAL"`). Además, las PKs son `int` en EF pero `long` en Domain/Application.
- **Swallowing de excepciones y manejo de errores deficiente**: `TenantProvisioningService` captura `Exception` y devuelve `false`; `ExceptionHandlingMiddleware` no registra logs.
- **DbContext multitenant mal cableado**: `GanaderiaDbContext` se registra con `DefaultConnection` fija; `TenantDbContextFactory` (el dinámico por tenant) no está registrado en DI.
- **Vacíos masivos de cobertura**: 7 proyectos de prueba contienen solo stubs `UnitTest1.cs`; faltan pruebas para validadores, mappers, controllers, middlewares, migradores y flujos de integración.

---

## 2. Capa de Dominio (Domain)

### DOM-001 — `Class1.cs` stub autogenerado
- **Severidad:** Baja
- **Archivos:** `src/ZooTech.Domain/Class1.cs`
- **Descripción:** Clase vacía generada por la plantilla de proyecto. No aporta valor y genera ruido.
- **Corrección completa:** Eliminar el archivo.
```csharp
// Eliminar: src/ZooTech.Domain/Class1.cs
```
- [x] **¿Aplicar corrección?**
- **Opinión técnica:** Limpieza trivial. Se debe hacer en un commit dedicado `chore(domain): eliminar stub Class1`.

---

### DOM-002 — `TenantDomainEntity` expone setters públicos en propiedades sensibles
- **Severidad:** Media
- **Archivos:** `src/ZooTech.Domain/Entities/TenantDomainEntity.cs`
- **Descripción:** La entidad de dominio permite mutación externa directa en `SubDomain`, `DisplayName`, `Email`, `Phone`, `Status`, `Metadata` y `UpdatedAt`. Sólo `Code` y `CreatedAt` tienen `private set`. La política de entidades del proyecto exige evitar setters públicos innecesarios.
- **Corrección completa:**
```csharp
public class TenantDomainEntity
{
    public long Id { get; private set; }
    public string Code { get; private set; } = default!;
    public string SubDomain { get; private set; } = default!;
    public string DisplayName { get; private set; } = default!;
    public string LegalName { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string Phone { get; private set; } = default!;
    public TenantStatus Status { get; private set; }
    public string Metadata { get; private set; } = default!;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private TenantDomainEntity() { }

    public static TenantDomainEntity Create(/* ... */)
    {
        // Factory method remains the only construction path
    }

    // Métodos de mutación explícita si el dominio lo requiere:
    public void UpdateContactInfo(string email, string phone) { /* ... */ }
}
```
- [x] **¿Aplicar corrección?**
- **Opinión técnica:** Escencial para proteger invariantes. Requiere actualizar `TenantMapper` y los casos de uso que mutan la entidad.

---

### DOM-003 — `TenantDomainEntity.Create` realiza `Enum.Parse` no validado sobre `status`
- **Severidad:** Alta
- **Archivos:** `src/ZooTech.Domain/Entities/TenantDomainEntity.cs`
- **Descripción:** El factory method recibe `status` como `string` y ejecuta `Enum.Parse<TenantStatus>(status)` sin control. Un valor inválido lanza `System.ArgumentException` (excepción del framework), no una excepción de dominio.
- **Corrección completa:** Cambiar la firma para recibir `TenantStatus` directamente (ya que el comando debería usar el enum), o validar explícitamente:
```csharp
public static TenantDomainEntity Create(
    long id,
    string code,
    string subDomain,
    string displayName,
    string legalName,
    string email,
    string phone,
    TenantStatus status, // <-- usar enum
    DateTime? createdAt,
    DateTime updatedAt)
{
    // eliminar Enum.Parse
}
```
- [x] **¿Aplicar corrección?**
- **Opinión técnica:** Alta prioridad. Mejora la type-safety y elimina dependencia de parseo de cadenas en el dominio.

---

### DOM-004 — `TenantDomainEntity.Create` usa `DateTime.Now` en lugar de `DateTime.UtcNow`
- **Severidad:** Media
- **Archivos:** `src/ZooTech.Domain/Entities/TenantDomainEntity.cs`
- **Descripción:** El valor por defecto de `createdAt` usa `DateTime.Now` (hora local del servidor), lo que genera inconsistencias en entornos multizona.
- **Corrección completa:**
```csharp
CreatedAt = createdAt.GetValueOrDefault(DateTime.UtcNow),
```
- [x] **¿Aplicar corrección?**
- **Opinión técnica:** Fácil de corregir. Afecta a todos los campos `created_at` del sistema.

---

### DOM-005 — Desajuste de tipos en claves primarias (`int` en BD vs `long` en Domain/Application)
- **Severidad:** Media
- **Archivos:**
  - `src/ZooTech.Domain/Entities/TenantDomainEntity.cs`
  - `src/ZooTech.Application/Common/Gateway/Context/ITenantContext.cs`
  - `src/ZooTech.Application/Common/Gateway/Tenant/TenantInfo.cs`
  - `src/ZooTech.Infrastructure/Persistence/Entities/MainTenantsDb/tenant.cs`
- **Descripción:** Las entidades EF usan `int id`, pero Domain y Application modelan `long Id`. Esto obliga a conversiones implícitas y es propenso a errores futuros (desbordamiento, incompatibilidad con otros sistemas).
- **Corrección completa:** Unificar a `int` en Domain y Application (dado que la capa de persistencia ya fue migrada a `int` y no se prevé escalar por encima de 2.147M en el catálogo de tenants), **o** unificar a `long` en EF (requiere migración de base de datos). La opción más rápida y coherente con el modelo actual es cambiar Domain/Application a `int`.
```csharp
// Domain
public int Id { get; private set; }

// ITenantContext
int TenantId { get; }
void SetTenant(int id, ...);

// TenantInfo
public int Id { get; set; }
```
- [x] **¿Aplicar corrección?**
- **Opinión técnica:** Requiere cascada de cambios en Mappers, Handlers, Tests y Base de Datos. Recomiendo hacerlo antes de que el modelo crezca.

---

### DOM-006 — Carpetas vacías (`ValueObjects/`, `Exceptions/`, `Rules/`)
- **Severidad:** Baja
- **Archivos:** Carpetas en `src/ZooTech.Domain/`
- **Descripción:** Contienen únicamente `.gitkeep`. La arquitectura preveé Value Objects y excepciones de dominio, pero no están implementadas.
- **Corrección completa:**
  - Crear `TenantCode` como Value Object (valida formato, longitud).
  - Crear `Email` como Value Object (valida estructura).
  - Crear excepciones de dominio (ej. `InvalidTenantCodeException`).
- [x] **¿Aplicar corrección?**
- **Opinión técnica:** Mejora la robustez, pero puede postergarse hasta que se estabilice el modelo de dominio.

---

### DOM-007 — `TenantStatus` enum declarado pero no utilizado en Application e Infrastructure
- **Severidad:** Media
- **Archivos:** Múltiples (validators, commands, store, provisioning)
- **Descripción:** Existe el enum `TenantStatus` en Domain, pero Application e Infrastructure usan cadenas mágicas (`"TRIAL"`, `"ACTIVE"`, `"INACTIVE"`). Esto duplica la definición de estados en validadores y lógica de negocio.
- **Corrección completa:**
  1. Cambiar `CreateTenantCommand.Status` a `TenantStatus`.
  2. Cambiar `TenantInfo.Status` a `TenantStatus`.
  3. Actualizar `TenantStore` para comparar con `TenantStatus.INACTIVE`.
  4. Actualizar `CreateTenantValidator` para usar `.IsInEnum()`.
  5. Actualizar `TenantMapper` para convertir `tenantEntity.status` string <-> enum (mediante `Enum.Parse` controlado en la capa de infraestructura, no en dominio).
- [x] **¿Aplicar corrección?**
- **Opinión técnica:** Reduce errores por tipeo y centraliza los estados válidos.

---

## 3. Capa de Aplicación (Application)

### APP-001 — `Class1.cs` stub autogenerado
- **Severidad:** Baja
- **Archivos:** `src/ZooTech.Application/Class1.cs`
- **Descripción:** Clase vacía sin uso.
- **Corrección completa:** Eliminar archivo.
- [x] **¿Aplicar corrección?**
- **Opinión técnica:** Trivial.

---

### APP-002 — `IPipelineBehavior<TRequest,TResponse>` custom duplica interfaz nativa de MediatR
- **Severidad:** Alta
- **Archivos:**
  - `src/ZooTech.Application/Common/Behaviors/IPipelineBehavior.cs`
  - `src/ZooTech.Application/Common/Behaviors/ValidationBehavior.cs`
  - `src/ZooTech.API/Program.cs`
- **Descripción:** La aplicación define su propia `IPipelineBehavior` con la misma firma que `MediatR.IPipelineBehavior<TRequest,TResponse>`. `ValidationBehavior` implementa la versión custom, y `Program.cs` la registra explícitamente con el FQN de la custom, lo que genera confusión y riesgo de desacoplamiento del pipeline de MediatR.
- **Corrección completa:**
  1. Eliminar `src/ZooTech.Application/Common/Behaviors/IPipelineBehavior.cs`.
  2. Modificar `ValidationBehavior` para implementar `MediatR.IPipelineBehavior<TRequest,TResponse>`.
  3. En `Program.cs`, registrar:
```csharp
builder.Services.AddTransient(
    typeof(MediatR.IPipelineBehavior<,>),
    typeof(ValidationBehavior<,>)
);
```
- [x] **¿Aplicar corrección?**
- **Opinión técnica:** Crítico para mantener compatibilidad con futuras versiones de MediatR y evitar comportamientos extraños en el pipeline.

---

### APP-003 — `IValidator<T>` interfaz custom sin uso
- **Severidad:** Baja
- **Archivos:** `src/ZooTech.Application/Common/Validator/IValidator.cs`
- **Descripción:** Interfaz genérica obsoleta; el proyecto usa FluentValidation (`IValidator<T>` de FluentValidation).
- **Corrección completa:** Eliminar archivo.
- [x] **¿Aplicar corrección?**
- **Opinión técnica:** Trivial. Evita confusión con la interfaz homónima de FluentValidation.

---

### APP-004 — `IConfiguration` vacío en `Gateway/Configuration`
- **Severidad:** Baja
- **Archivos:** `src/ZooTech.Application/Common/Gateway/Configuration/IConfiguration.cs`
- **Descripción:** Interfaz sin miembros. No tiene implementaciones ni consumidores.
- **Corrección completa:** Eliminar archivo.
- [x] **¿Aplicar corrección?**
- **Opinión técnica:** Limpieza de código muerto.

---

### APP-005 — `LoggingBehavior` esqueleto vacío
- **Severidad:** Baja
- **Archivos:** `src/ZooTech.Application/Common/Behaviors/LoggingBehavior.cs`
- **Descripción:** Clase declarada pero sin implementación.
- **Corrección completa:** Implementar comportamiento de logging (medir duración, loggear request/response) o eliminar si no se necesita.
```csharp
public class LoggingBehavior<TRequest, TResponse> : MediatR.IPipelineBehavior<TRequest, TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger) => _logger = logger;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {RequestName}", typeof(TRequest).Name);
        var response = await next();
        _logger.LogInformation("Handled {RequestName}", typeof(TRequest).Name);
        return response;
    }
}
```
- [x] **¿Aplicar corrección?**
- **Opinión técnica:** Útil para trazabilidad en producción. Prioridad media-baja.
- **Sugerencia personal:** Incorporar registro de auditoria en esta clase con interfaces de auditoria en `src/ZooTech.Application/Common/Auditing` la lógica de auditoria ya está implementada y se pueden realizar correcciones al modelo de auditoria si es necesario.

---

### APP-006 — `DependencyInjection.cs` completamente comentado
- **Severidad:** Alta
- **Archivos:** `src/ZooTech.Application/DependencyInjection.cs`, `src/ZooTech.API/Program.cs`
- **Descripción:** El registro de MediatR, FluentValidation y Behaviors está inline en `Program.cs`, violando el principio de que cada capa debe auto-registrar sus dependencias. El `DependencyInjection.cs` de Application es un esqueleto comentado.
- **Corrección completa:**
```csharp
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        // services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        return services;
    }
}
```
Y en `Program.cs` reemplazar las líneas de MediatR/Validators/Behaviors por `builder.Services.AddApplication();`.
- [x] **¿Aplicar corrección?**
- **Opinión técnica:** Fundamental para la mantenibilidad. Program.cs debe ser solo un orquestador de llamadas `Add{Layer}()`.

---

### APP-007 — `GeneralResponseDTO<T>` marcado como `internal`
- **Severidad:** Media
- **Archivos:** `src/ZooTech.Application/Common/Models/GeneralResponseDTO.cs`
- **Descripción:** La clase es `internal`, por lo que `InterfaceAdapters` (ensamblado diferente) no puede usarla. Como resultado, InterfaceAdapters duplicó su propia versión.
- **Corrección completa:**
```csharp
public class GeneralResponseDTO<T>
{
    public T? Data { get; set; }
    public static GeneralResponseDTO<T> Ok(T? data) => new() { Data = data };
}
```
Eliminar la copia duplicada en `src/ZooTech.InterfaceAdapters/DTOs/GeneralResponseDTO.cs`.
- [x] **¿Aplicar corrección?**
- **Opinión técnica:** Elimina duplicación y respeta DRY. Prioridad media.

---

### APP-008 — `CreateTenantCommand` permite mutación externa y expone `CreatedAt`
- **Severidad:** Media
- **Archivos:** `src/ZooTech.Application/Modules/Module_Tenancing/UseCases/CreateTenant/CreateTenantCommand.cs`
- **Descripción:** Las propiedades del comando tienen setters públicos. `CreatedAt` tiene valor por defecto `DateTime.Now`, permitiendo al cliente (o al mapper) manipular la fecha de creación.
- **Corrección completa:** Usar `init` o constructor. Eliminar `CreatedAt` del comando; la fecha debe ser asignada por el dominio o el interactor en el momento de la creación.
```csharp
public class CreateTenantCommand : IRequest<Unit>
{
    public string Code { get; init; } = default!;
    public string SubDomain { get; init; } = default!;
    // ... resto de propiedades init-only
    // Eliminar: public DateTime CreatedAt { get; set; } = DateTime.Now;
}
```
- [x] **¿Aplicar corrección?**
- **Opinión técnica:** Protege la integridad de los datos de entrada y evita fechas manipuladas.

---

### APP-009 — `CreateTenantValidator` valida `Status` con arreglo de cadenas hardcodeado
- **Severidad:** Media
- **Archivos:** `src/ZooTech.Application/Modules/Module_Tenancing/UseCases/CreateTenant/CreateTenantValidator.cs`
- **Descripción:** El validador duplica la definición de estados válidos en un arreglo de strings, en lugar de usar el enum `TenantStatus`.
- **Corrección completa:**
```csharp
RuleFor(x => x.Status)
    .IsInEnum()
    .WithMessage("Estado inválido.");
```
(Esto requiere que `Status` sea `TenantStatus` en el comando, ver DOM-007).
- [x] **¿Aplicar corrección?**
- **Opinión técnica:** Centraliza la fuente de verdad del estado en el enum de dominio.

---

### APP-010 — Patrón dual MediatR + Ports & Adapters genera código muerto
- **Severidad:** Alta
- **Archivos:**
  - `src/ZooTech.Application/Modules/Module_Tenancing/UseCases/CreateTenant/CreateTenantHandler.cs`
  - `src/ZooTech.Application/Modules/Module_Tenancing/UseCases/CreateTenant/CreateTenantInteractor.cs`
  - `src/ZooTech.Application/Modules/Module_Tenancing/UseCases/CreateTenant/Ports/ICreateTenantInputPort.cs`
  - `src/ZooTech.Application/Modules/Module_Tenancing/UseCases/CreateTenant/Ports/ICreateTenantOutputPort.cs`
  - `src/ZooTech.InterfaceAdapters/Modules/Module_Tenancing/Controllers/TenancingController.cs`
- **Descripción:** El handler de MediatR delega a `ICreateTenantInputPort` (Interactor), que a su vez usa `ICreateTenantOutputPort` (Presenter). Sin embargo, el controller usa `IMediator.Send()` directamente e inyecta `ICreateTenantInputPort` que nunca utiliza. Esto es una combinación redundante de dos patrones.
- **Corrección completa:**  
  **Opción A (Recomendada por la documentación del proyecto):** Eliminar `ICreateTenantInputPort`, `ICreateTenantOutputPort`, `CreateTenantInteractor` y `CreateTenantPresenter`. Mover la lógica de `CreateTenantInteractor` directamente a `CreateTenantHandler`. El controller devuelve el resultado del handler.
  **Opción B:** Eliminar MediatR del flujo y que el controller invoque directamente `ICreateTenantInputPort.Handle()`.
```csharp
// Opción A: Handler simplificado
public class CreateTenantHandler : IRequestHandler<CreateTenantCommand, CreateTenantResult>
{
    private readonly ITenantProvisioningService _provisioningService;
    public CreateTenantHandler(ITenantProvisioningService provisioningService) => _provisioningService = provisioningService;

    public async Task<CreateTenantResult> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
    {
        var success = await _provisioningService.ProvisionAsync(request);
        if (!success) throw new TenantProvisioningException();
        return new CreateTenantResult { Code = request.Code, SubDomain = request.SubDomain, ... };
    }
}
```
- [x] **¿Aplicar corrección?**
- **Opinión técnica:** La duplicidad de patrones confunde a nuevos desarrolladores y complica el testing. Opción A es la más idiomática en el ecosistema .NET + MediatR.
- **Sugerencia Personal**: Aplicar Opcion A

---

### APP-011 — `ValidationException` pierde metadatos de campo (solo mensajes)
- **Severidad:** Media
- **Archivos:** `src/ZooTech.Application/Common/Exceptions/ValidationException.cs`, `src/ZooTech.Application/Common/Behaviors/ValidationBehavior.cs`
- **Descripción:** `ValidationBehavior` extrae solo `ErrorMessage` de cada `ValidationFailure`. Se pierden el nombre de la propiedad (`PropertyName`), el código de error (`ErrorCode`) y la severidad. El middleware devuelve una lista plana de strings, dificultando que el frontend muestre errores por campo.
- **Corrección completa:**
```csharp
public class ValidationException : AppException
{
    public override int StatusCode => (int)HttpStatusCode.BadRequest;
    public IReadOnlyList<ValidationError> Errors { get; }

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : base("VALIDATION_ERROR", "Errores de validación")
    {
        Errors = failures.Select(f => new ValidationError(f.PropertyName, f.ErrorMessage)).ToList();
    }
}

public record ValidationError(string PropertyName, string Message);
```
Y actualizar `ExceptionHandlingMiddleware` para serializar la estructura:
```csharp
Details = ex is ValidationException ve ? ve.Errors.Select(e => $"{e.PropertyName}: {e.Message}").ToList() : ex.Details
```
- [x] **¿Aplicar corrección?**
- **Opinión técnica:** Mejora significativamente la UX del consumidor de la API. Prioridad media-alta.

---

### APP-012 — `ITenantRepository.GetByIdAsync()` sin parámetro de identificación
- **Severidad:** Alta
- **Archivos:** `src/ZooTech.Application/Common/Gateway/Repositories/MainTenantsDb/ITenantRepository.cs`
- **Descripción:** La interfaz declara `Task<TenantDomainEntity> GetByIdAsync();` sin recibir el `id`. Es un error de diseño que impediría cualquier uso real.
- **Corrección completa:**
```csharp
public interface ITenantRepository
{
    Task<TenantDomainEntity?> GetByIdAsync(int id); // o long, según unificación DOM-005
    Task<List<TenantDomainEntity>> ListAllAsync();
}
```
- [x] **¿Aplicar corrección?**
- **Opinión técnica:** Bug obvio. Debe corregirse inmediatamente si se pretende usar el repositorio.

---

## 4. Capa de Infraestructura (Infrastructure)

### INF-001 — `Class1.cs` stub autogenerado
- **Severidad:** Baja
- **Archivos:** `src/ZooTech.Infrastructure/Class1.cs`
- **Descripción:** Clase vacía.
- **Corrección completa:** Eliminar.
- [x] **¿Aplicar corrección?**
- **Opinión técnica:** Trivial.

---

### INF-002 — `TenantRepository` vacío y sin implementar `ITenantRepository`
- **Severidad:** Alta
- **Archivos:** `src/ZooTech.Infrastructure/Persistence/Repositories/MainTenantsDb/TenantRepository.cs`
- **Descripción:** La clase está vacía y no implementa la interfaz `ITenantRepository`. Es código muerto que confunde.
- **Corrección completa:**
```csharp
public class TenantRepository : ITenantRepository
{
    private readonly TenantCatalogDb _context;
    public TenantRepository(TenantCatalogDb context) => _context = context;

    public async Task<TenantDomainEntity?> GetByIdAsync(int id)
    {
        var entity = await _context.tenants.FindAsync(id);
        return entity == null ? null : TenantMapper.ToDomain(entity);
    }

    public async Task<List<TenantDomainEntity>> ListAllAsync()
    {
        var entities = await _context.tenants.AsNoTracking().ToListAsync();
        return entities.Select(TenantMapper.ToDomain).ToList();
    }
}
```
- [x] **¿Aplicar corrección?**
- **Opinión técnica:** Si `ITenantRepository` no se usa, conviene eliminar ambos para evitar mantener abstracciones fantasmas. Si sí se usará, implementar y registrar.

---

### INF-003 — `tenant_setting.cs` entidad huérfana (código muerto)
- **Severidad:** Media
- **Archivos:** `src/ZooTech.Infrastructure/Persistence/Entities/MainTenantsDb/tenant_setting.cs`
- **Descripción:** No tiene `DbSet` en `TenantCatalogDb`, ni Fluent API, ni navegaciones desde `tenant`. Usa tipos `long` y `DateTimeOffset` inconsistentes con el modelo actual (`int`, `DateTime?`). Fue reemplazada por `setting_value`.
- **Corrección completa:** Eliminar archivo.
- [x] **¿Aplicar corrección?**
- **Opinión técnica:** Limpieza obligatoria antes de implementar la feature de parametrización.

---

### INF-004 — `TenantProvisioningService` oculta excepciones (`catch Exception`)
- **Severidad:** Crítica
- **Archivos:** `src/ZooTech.Infrastructure/Tenant/TenantProvisioningService.cs`
- **Descripción:** El método `ProvisionAsync` captura cualquier `Exception` y devuelve `false`. Esto hace *swallowing* de errores, dificultando el diagnóstico de fallos en migraciones o conexiones a BD.
- **Corrección completa:**
```csharp
public async Task<bool> ProvisionAsync(CreateTenantCommand cmd)
{
    // ... lógica de creación ...
    try
    {
        _tenantCatalogDb.tenants.Add(tenant);
        await _tenantCatalogDb.SaveChangesAsync();
        // ... migración ...
        return true;
    }
    catch (Exception ex)
    {
        // Opción A: propagar como excepción de aplicación
        throw new TenantProvisioningException("Error al provisionar tenant", ex);

        // Opción B: usar Result pattern (requiere más cambios)
        // return Result.Failure<TenantProvisioningError>(...);
    }
}
```
- [x] **¿Aplicar corrección?**
- **Opinión técnica:** Crítico para operaciones. Un fallo en provisioning debe ser trazable. Recomiendo Opción A inmediatamente.
- **Sugerencia personal**: Aplicar Opcion A

---

### INF-005 — `TenantProvisioningService` no asigna `tenant_branding` y usa `CreatedAt` del comando
- **Severidad:** Media
- **Archivos:** `src/ZooTech.Infrastructure/Tenant/TenantProvisioningService.cs`
- **Descripción:** El comando incluye `TenantBranding`, pero el servicio nunca lo asigna a `tenant.tenant_branding`. Además, usa `cmd.CreatedAt` en lugar de generar la fecha del servidor.
- **Corrección completa:**
```csharp
tenant.tenant_branding = new tenant_branding
{
    primary_color = cmd.TenantBranding.PrimaryColor,
    secondary_color = cmd.TenantBranding.SecondaryColor,
    logo_url = cmd.TenantBranding.LogoUrl,
    metadata = cmd.TenantBranding.Metadata,
    created_at = DateTime.UtcNow
};
tenant.created_at = DateTime.UtcNow;
```
- [x] **¿Aplicar corrección?**
- **Opinión técnica:** El branding es parte del contrato de creación; omitirlo es un bug funcional.

---

### INF-006 — `TenantProvisioningService` sanitización insuficiente del nombre de base de datos
- **Severidad:** Media
- **Archivos:** `src/ZooTech.Infrastructure/Tenant/TenantProvisioningService.cs`
- **Descripción:** Construye el nombre de BD con `$"ZooTech_{cmd.Code.Replace("-", "_")}_Db"`. Solo reemplaza guiones, pero no valida otros caracteres peligrosos para identificadores SQL (espacios, comillas, puntos, etc.).
- **Corrección completa:**
```csharp
private static string SanitizeDbName(string code)
{
    var sanitized = Regex.Replace(code, @"[^a-zA-Z0-9_]", "_");
    return $"ZooTech_{sanitized}_Db";
}
```
- [x] **¿Aplicar corrección?**
- **Opinión técnica:** Previene errores de creación de BD y potenciales vectores de inyección en el nombre del catálogo.

---

### INF-007 — `TenantStore` compara `status` como cadena `"INACTIVE"`
- **Severidad:** Media
- **Archivos:** `src/ZooTech.Infrastructure/Tenant/TenantStore.cs`
- **Descripción:** Usa comparación de string (`tenantInDb.status.Equals("INACTIVE")`) en lugar del enum `TenantStatus`.
- **Corrección completa:**
```csharp
if (tenantInDb == null || tenantInDb.status == TenantStatus.INACTIVE.ToString() || !tenantInDb.tenant_database_connection.is_active)
```
O mejor, si se implementa DOM-007, comparar contra el enum directamente si la entidad EF usa el enum (requeriría mapeo de enum en EF).
- [x] **¿Aplicar corrección?**
- **Opinión técnica:** Mejora la type-safety.

---

### INF-008 — `TenantDbContextFactory` no registrado en DI
- **Severidad:** Alta
- **Archivos:** `src/ZooTech.Infrastructure/Tenant/TenantDbContextFactory.cs`, `src/ZooTech.API/Program.cs`
- **Descripción:** `ITenantDbContextFactory` existe y está implementado, pero `Program.cs` no lo registra. Ningún servicio puede resolver un `GanaderiaDbContext` dinámico por tenant.
- **Corrección completa:**
```csharp
builder.Services.AddScoped<ITenantDbContextFactory, TenantDbContextFactory>();
```
- [x] **¿Aplicar corrección?**
- **Opinión técnica:** Esencial para el funcionamiento multitenant real.

---

### INF-009 — `GanaderiaDbContext` registrado con `DefaultConnection` fija en lugar de resolución dinámica por tenant
- **Severidad:** Crítica
- **Archivos:** `src/ZooTech.Infrastructure/DependencyInjection.cs`, `src/ZooTech.API/Program.cs`
- **Descripción:** `AddDbContext<GanaderiaDbContext>` usa la connection string `DefaultConnection`. En un entorno multitenant, las peticiones reales deben resolverse contra la BD del tenant actual (mediante `ITenantContext.DatabaseName`).
- **Corrección completa:**
  1. Eliminar `AddDbContext<GanaderiaDbContext>` de `DependencyInjection.cs`.
  2. Registrar `GanaderiaDbContext` como `Scoped` con una factory que use `ITenantDbContextFactory`:
```csharp
builder.Services.AddScoped<GanaderiaDbContext>(sp =>
{
    var factory = sp.GetRequiredService<ITenantDbContextFactory>();
    return factory.CreateDbContext();
});
```
  O usar `IDbContextFactory<GanaderiaDbContext>` si se prefiere factory pattern explícito.
- [x] **¿Aplicar corrección?**
- **Opinión técnica:** Sin este cambio, el sistema siempre apunta a la base de datos por defecto, rompiendo el aislamiento por tenant.

---

### INF-010 — `MongoDbAudit` (`IAppAuditService`) implementado pero no registrado en DI
- **Severidad:** Media
- **Archivos:** `src/ZooTech.Infrastructure/Auditing/MongoDb/MongoDbAudit.cs`
- **Descripción:** La implementación de auditoría existe pero no se registra en `DependencyInjection.cs` ni en `Program.cs`, por lo que no está disponible para los casos de uso.
- **Corrección completa:**
```csharp
services.AddScoped<IAppAuditService, MongoDbAudit>();
```
- [x] **¿Aplicar corrección?**
- **Opinión técnica:** Necesario si la auditoría es un requerimiento funcional.

---

### INF-011 — `Infrastructure/DependencyInjection.cs` incompleto
- **Severidad:** Alta
- **Archivos:** `src/ZooTech.Infrastructure/DependencyInjection.cs`
- **Descripción:** Solo registra `GanaderiaDbContext` y `GarnetCacheService`. Faltan repositorios, servicios de tenant, audit, factory de contexto, etc.
- **Corrección completa:** Completar el archivo con todos los registros que hoy viven en `Program.cs`:
```csharp
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // Remover si se adopta INF-009
        services.AddDbContext<GanaderiaDbContext>(opt => opt.UseSqlServer(connectionString));

        services.AddSingleton<GarnetCacheConnection>();
        services.AddSingleton<IAppCacheService, GarnetCacheService>();

        services.AddScoped<ITenantStore, TenantStore>();
        services.AddScoped<ITenantContext, TenantContext>();
        services.AddScoped<ITenantProvisioningService, TenantProvisioningService>();
        services.AddScoped<ITenantDatabaseMigrator, TenantDatabaseMigrator>();
        services.AddScoped<IGanaderiaDbContextFactory, GanaderiaDbContextFactory>();
        services.AddScoped<ITenantDbContextFactory, TenantDbContextFactory>();
        services.AddScoped<IAppAuditService, MongoDbAudit>();

        // Repositorios
        services.AddScoped<ITenantRepository, TenantRepository>();

        return services;
    }
}
```
- [x] **¿Aplicar corrección?**
- **Opinión técnica:** Junto con APP-006, consolida el Composition Root.

---

## 5. Capa de Adaptadores (InterfaceAdapters)

### IA-001 — `DependencyInjection.cs` comentado y nunca invocado
- **Severidad:** Alta
- **Archivos:** `src/ZooTech.InterfaceAdapters/DependencyInjection.cs`, `src/ZooTech.API/Program.cs`
- **Descripción:** El método `AddInterfaceAdapters()` existe pero está vacío (comentado). `Program.cs` no lo llama.
- **Corrección completa:**
```csharp
public static class DependencyInjection
{
    public static IServiceCollection AddInterfaceAdapters(this IServiceCollection services)
    {
        // services.AddAutoMapper(typeof(DependencyInjection).Assembly);
        return services;
    }
}
```
Y en `Program.cs`:
```csharp
builder.Services.AddInterfaceAdapters();
```
- [x] **¿Aplicar corrección?**
- **Opinión técnica:** Mantiene simetría con las demás capas. Incluso si está vacío por ahora, la convención debe respetarse.

---

### IA-002 — `TenancingController` inyecta `ICreateTenantInputPort` sin utilizarlo
- **Severidad:** Media
- **Archivos:** `src/ZooTech.InterfaceAdapters/Modules/Module_Tenancing/Controllers/TenancingController.cs`
- **Descripción:** El campo `_createTenantInputPort` se recibe en el constructor pero nunca se usa; el controller delega a `IMediator.Send()`.
- **Corrección completa:**
```csharp
public class TenancingController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly CreateTenantPresenter _tenancingPresenter;

    public TenancingController(IMediator mediator, CreateTenantPresenter tenancingPresenter)
    {
        _mediator = mediator;
        _tenancingPresenter = tenancingPresenter;
    }
    // ...
}
```
O, si se adopta APP-010 (eliminar Ports), simplificar aún más el controller.
- [x] **¿Aplicar corrección?**
- **Opinión técnica:** Código muerto que confunde la intención del flujo.

---

### IA-003 — `GeneralResponseDTO<T>` duplicado en InterfaceAdapters
- **Severidad:** Media
- **Archivos:** `src/ZooTech.InterfaceAdapters/DTOs/GeneralResponseDTO.cs`
- **Descripción:** Es una copia casi idéntica de la versión en `Application/Common/Models`. Además es `internal`, limitando su uso externo.
- **Corrección completa:** Eliminar el archivo de InterfaceAdapters y usar la versión pública de Application (ver APP-007).
- [x] **¿Aplicar corrección?**
- **Opinión técnica:** DRY. Una sola fuente de verdad para DTOs base.

---

### IA-004 — Typo en carpeta `Presentes` (debe ser `Presenters`)
- **Severidad:** Baja
- **Archivos:** `src/ZooTech.InterfaceAdapters/Modules/Module_ProduccionLeche/Presentes/`
- **Descripción:** Error de tipeo en el nombre de carpeta.
- **Corrección completa:** Renombrar carpeta a `Presenters`.
- [x] **¿Aplicar corrección?**
- **Opinión técnica:** Trivial pero importante para convenciones de proyecto.

---

### IA-005 — `ExceptionHandlingMiddleware` no registra logs de excepciones
- **Severidad:** Alta
- **Archivos:** `src/ZooTech.InterfaceAdapters/Middleware/ExceptionHandlingMiddleware.cs`
- **Descripción:** Captura `AppException` y `Exception` pero no inyecta `ILogger`, por lo que no hay trazabilidad en logs del servidor cuando ocurre un error.
- **Corrección completa:**
```csharp
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try { await _next(context); }
        catch (AppException ex)
        {
            _logger.LogWarning(ex, "AppException: {Code} - {Message}", ex.Code, ex.Message);
            // ... respuesta JSON ...
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            // ... respuesta JSON ...
        }
    }
}
```
- [x] **¿Aplicar corrección?**
- **Opinión técnica:** Esencial para observabilidad y debugging en producción.
- **Sugerencia personal**: Se podria implementar tambien la Auditoria de Logs con MongoDB.

---

### IA-006 — `TenantResolutionMiddleware` lee configuración sin validar nulos
- **Severidad:** Media
- **Archivos:** `src/ZooTech.InterfaceAdapters/Middleware/TenantResolutionMiddleware.cs`
- **Descripción:** `_baseDomain = config["MultiTenant:BaseDomain"]` puede ser `null` si falta la clave. Luego `host.EndsWith("." + _baseDomain)` lanzará `NullReferenceException`.
- **Corrección completa:**
```csharp
_baseDomain = config["MultiTenant:BaseDomain"]
    ?? throw new InvalidOperationException("Missing configuration: MultiTenant:BaseDomain");
```
- [x] **¿Aplicar corrección?**
- **Opinión técnica:** Fail-fast es preferible a un NRE críptico durante el pipeline de request.

---

## 6. Composition Root (API / Program.cs)

### API-001 — Registro duplicado de `AddValidatorsFromAssembly`
- **Severidad:** Baja
- **Archivos:** `src/ZooTech.API/Program.cs`
- **Descripción:** Las líneas 73-76 y 79-81 repiten exactamente la misma llamada.
- **Corrección completa:** Eliminar una de las dos invocaciones.
- [x] **¿Aplicar corrección?**
- **Opinión técnica:** Limpieza menor. Sin impacto funcional pero genera ruido.

---

### API-002 — Política CORS `AllowFrontend` definida pero nunca aplicada
- **Severidad:** Media
- **Archivos:** `src/ZooTech.API/Program.cs`
- **Descripción:** Se define `services.AddCors(...)` con la política `"AllowFrontend"`, pero nunca se invoca `app.UseCors("AllowFrontend")` en el pipeline.
- **Corrección completa:**
```csharp
app.UseCors("AllowFrontend");
```
Colocarlo antes de `UseAuthorization` y después de `UseHttpsRedirection` (o según convenga el orden de middlewares).
- [x] **¿Aplicar corrección?**
- **Opinión técnica:** Sin esta línea, las peticiones del frontend serán bloqueadas por el navegador.

---

### API-003 — Lectura de configuración `Frontend:*` sin validación de nulos
- **Severidad:** Media
- **Archivos:** `src/ZooTech.API/Program.cs`
- **Descripción:** `builder.Configuration["Frontend:FrontendPort"]` puede ser `null`, generando una URL CORS malformada (`https://null/null`).
- **Corrección completa:**
```csharp
var frontendPort = builder.Configuration["Frontend:FrontendPort"] ?? "5000";
var frontendIP = builder.Configuration["Frontend:FrontendIP"] ?? "localhost";
var frontendProtocol = builder.Configuration["Frontend:FrontendProtocol"] ?? "http";
```
- [x] **¿Aplicar corrección?**
- **Opinión técnica:** Previene configuraciones rotas en entornos nuevos.

---

### API-004 — Faltan registros de servicios en DI (`TenantDbContextFactory`, `MongoDbAudit`)
- **Severidad:** Alta
- **Archivos:** `src/ZooTech.API/Program.cs`
- **Descripción:** `ITenantDbContextFactory` e `IAppAuditService` no están registrados (detallado en INF-008 e INF-010). Esto provoca `InvalidOperationException` si algún servicio los solicita.
- **Corrección completa:** Mover todos los registros específicos a `Infrastructure/DependencyInjection.cs` (ver INF-011) y mantener `Program.cs` limpio.
- [x] **¿Aplicar corrección?**
- **Opinión técnica:** Prioridad alta para evitar runtime failures.

---

### API-005 — `TenancingController` sin grupo Swagger explícito
- **Severidad:** Baja
- **Archivos:** `src/ZooTech.InterfaceAdapters/Modules/Module_Tenancing/Controllers/TenancingController.cs`
- **Descripción:** No tiene `[ApiExplorerSettings(GroupName = "...")]`, por lo que puede aparecer en documentos Swagger no deseados o mezclado.
- **Corrección completa:**
```csharp
[ApiExplorerSettings(GroupName = "public")] // o "tenants"/"admin" según corresponda
```
- [x] **¿Aplicar corrección?**
- **Opinión técnica:** Mejora la organización de la documentación de API.
- **Sugerencia peronal**: Usar GroupName = "admin"

---

## 7. Pruebas Unitarias y de Integración

### TST-001 — 7 archivos `UnitTest1.cs` vacíos (stubs)
- **Severidad:** Media
- **Archivos:**
  - `tests/Unit/ZooTech.Domain.UnitTests/UnitTest1.cs`
  - `tests/Unit/ZooTech.Application.UnitTests/UnitTest1.cs`
  - `tests/Unit/ZooTech.Infrastructure.UnitTests/UnitTest1.cs`
  - `tests/Unit/ZooTech.InterfaceAdapters.UnitTests/UnitTest1.cs`
  - `tests/Integration/ZooTech.API.IntegrationTests/UnitTest1.cs`
  - `tests/Integration/ZooTech.Infrastructure.IntegrationTests/UnitTest1.cs`
  - `tests/Integration/ZooTech.InterfaceAdapters.IntegrationTests/UnitTest1.cs`
- **Descripción:** Proyectos de prueba que contienen únicamente stubs sin valor. Generan ruido y dan una falsa sensación de cobertura.
- **Corrección completa:** Eliminar todos los `UnitTest1.cs`. Implementar pruebas reales o dejar los proyectos vacíos (sin archivos `.cs`) hasta que se escriban tests.
- [x] **¿Aplicar corrección?**
- **Opinión técnica:** Deben eliminarse inmediatamente para no contaminar métricas de cobertura.

---

### TST-002 — Vacío de cobertura en `ZooTech.Domain.UnitTests`
- **Severidad:** Alta
- **Archivos:** `tests/Unit/ZooTech.Domain.UnitTests/`
- **Descripción:** No existen pruebas para `TenantDomainEntity` (factory, invariantes, mutación), `TenantStatus`, ni futuros ValueObjects.
- **Corrección completa:**
```csharp
public class TenantDomainEntityTests
{
    [Fact]
    public void Create_Should_Initialize_CreatedAt_And_Code()
    {
        var tenant = TenantDomainEntity.Create(0, "CODE", "code", "Display", "Legal", "a@b.com", "123", TenantStatus.TRIAL, null, DateTime.UtcNow);
        Assert.Equal("CODE", tenant.Code);
        Assert.True(tenant.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Create_With_InvalidStatus_Should_Throw()
    {
        // Si se mantiene string en factory (antes de DOM-003), probar guarda
        Assert.Throws<ArgumentException>(() =>
            TenantDomainEntity.Create(0, "C", "c", "D", "L", "e@f.com", "1", "INVALID", null, DateTime.UtcNow));
    }
}
```
- [x] **¿Aplicar corrección?**
- **Opinión técnica:** Obligatorio según la política de testing del proyecto (mínimo Domain y Application).

---

### TST-003 — Vacío de cobertura en `ZooTech.Application.UnitTests`
- **Severidad:** Alta
- **Archivos:** `tests/Unit/ZooTech.Application.UnitTests/`
- **Descripción:** Faltan pruebas para:
  - `CreateTenantHandler`
  - `CreateTenantInteractor` (si se conserva)
  - `CreateTenantValidator` (todos los escenarios: éxito, email inválido, subdominio inválido, status inválido, dirección incompleta, branding con colores malformados)
  - `ValidationBehavior`
- **Corrección completa:** Implementar suite de pruebas unitarias con mocks para `ITenantProvisioningService`, `IMediator`, etc.
```csharp
public class CreateTenantValidatorTests
{
    private readonly CreateTenantValidator _validator = new();

    [Fact]
    public void Should_Fail_When_Code_Is_Empty()
    {
        var command = new CreateTenantCommand { Code = "" };
        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Code");
    }
}
```
- [x] **¿Aplicar corrección?**
- **Opinión técnica:** Los validadores son lógica crítica de entrada; deben tener cobertura cercana al 100%.

---

### TST-004 — Vacío de cobertura en `ZooTech.InterfaceAdapters.UnitTests`
- **Severidad:** Alta
- **Archivos:** `tests/Unit/ZooTech.InterfaceAdapters.UnitTests/`
- **Descripción:** Solo existe `TenantResolutionMiddlewareUnitTests` (2 tests). Faltan:
  - `ExceptionHandlingMiddleware` (verificar que `AppException` devuelve JSON correcto con StatusCode dinámico, que `Exception` genérico devuelve 500).
  - `TenancingController` (verificar que delega a `IMediator` y devuelve respuesta del presenter).
  - `CreateTenantPresenter` (verificar que almacena `Response` tras `Ok()`).
  - `CreateTenantMapper` (verificar mapeo bidireccional completo).
- **Corrección completa:**
```csharp
[Fact]
public async Task ExceptionHandlingMiddleware_Should_Return_Json_For_AppException()
{
    var middleware = new ExceptionHandlingMiddleware(
        _ => throw new ValidationException(new List<string> { "Invalid" }),
        Mock.Of<ILogger<ExceptionHandlingMiddleware>>()
    );
    var context = new DefaultHttpContext();
    context.Response.Body = new MemoryStream();

    await middleware.InvokeAsync(context);

    Assert.Equal(400, context.Response.StatusCode);
    context.Response.Body.Seek(0, SeekOrigin.Begin);
    var json = await new StreamReader(context.Response.Body).ReadToEndAsync();
    Assert.Contains("VALIDATION_ERROR", json);
}
```
- [x] **¿Aplicar corrección?**
- **Opinión técnica:** El middleware de excepciones es crítico; debe validarse que no exponga stack traces y que respete contratos de API.

---

### TST-005 — Vacío de cobertura en servicios Infrastructure no probados
- **Severidad:** Media
- **Archivos:** `tests/Unit/ZooTech.Infrastructure.UnitTests/`
- **Descripción:** Actualmente solo se prueban `TenantProvisioningService`, `TenantStore` y `TenantDbContextFactory`. Faltan:
  - `TenantDatabaseMigrator`
  - `GanaderiaDbContextFactory`
  - `TenantMapper` (ToDomain y ToEntity)
  - `GarnetCacheService` / `RedisCacheService`
  - `MongoDbAudit`
- **Corrección completa:** Agregar suites para cada componente.
```csharp
[Fact]
public void TenantMapper_ToDomain_Should_Map_All_Fields()
{
    var entity = new tenant { id = 1, code = "C", status = "TRIAL", /* ... */ };
    var domain = TenantMapper.ToDomain(entity);
    Assert.Equal(1, domain.Id);
    Assert.Equal(TenantStatus.TRIAL, domain.Status);
}
```
- [x] **¿Aplicar corrección?**
- **Opinión técnica:** Los mappers son un punto frágil de sincronización entre capas.

---

### TST-006 — Ausencia total de pruebas de integración
- **Severidad:** Crítica
- **Archivos:** `tests/Integration/`
- **Descripción:** Los 3 proyectos de integración (`API`, `Infrastructure`, `InterfaceAdapters`) solo contienen stubs. No se prueba el flujo completo: HTTP Request → Middleware → Controller → MediatR → Provisioning → Migración → Respuesta.
- **Corrección completa:**
  1. **API Integration Tests:** Usar `WebApplicationFactory<Program>` para testear endpoints reales con base de datos en memoria o TestContainers.
  ```csharp
  public class TenantProvisioningIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
  {
      [Fact]
      public async Task POST_Tenancing_Should_Create_Tenant_And_Return_200()
      {
          var client = _factory.CreateClient();
          var payload = new { Code = "test", SubDomain = "test", /* ... */ };
          var response = await client.PostAsJsonAsync("/tenancing", payload);
          response.EnsureSuccessStatusCode();
      }
  }
  ```
  2. **Infrastructure Integration Tests:** Probar `TenantCatalogDb` contra SQL Server LocalDB o contenedor, y verificar que las migraciones de `GanaderiaDbContext` se aplican correctamente sobre una BD de tenant creada dinámicamente.
- [x] **¿Aplicar corrección?**
- **Opinión técnica:** Las pruebas de integración son la única forma de garantizar que el pipeline multitenant funciona de punta a punta. Prioridad crítica.

---

### TST-007 — `FluentAssertions` declarado en paquetes pero no utilizado en tests existentes
- **Severidad:** Baja
- **Archivos:** `.csproj` de Infrastructure.UnitTests, y potencialmente otros.
- **Descripción:** Los tests existentes usan `Assert.True`, `Assert.Null`, etc. (xUnit clásico). `FluentAssertions` está instalado pero no aprovechado.
- **Corrección completa:** Estandarizar a FluentAssertions para legibilidad:
```csharp
result.Should().BeTrue();
tenant.Should().NotBeNull();
tenant!.SubDomain.Should().Be("tenant1");
```
O, si no se desea adoptar FluentAssertions, eliminar la dependencia para reducir tiempo de build.
- [x] **¿Aplicar corrección?**
- **Opinión técnica:** FluentAssertions mejora la legibilidad de mensajes de error. Recomiendo adoptarlo y eliminar `Assert.*` clásico.
- **Sugerencia personal:** Adoptar FluentAssertions y corregir todas las pruebas para que adopten este estandar.

---

### TST-008 — Falta de utilidades compartidas para tests (fixtures, helpers)
- **Severidad:** Media
- **Archivos:** Todos los proyectos de test.
- **Descripción:** Cada test configura su propio `DbContextOptions`, `MemoryCache`, `ConfigurationBuilder` y mocks inline. Esto duplica código y dificulta el mantenimiento.
- **Corrección completa:** Crear un proyecto compartido `tests/ZooTech.Tests.Shared` o clases base:
```csharp
public class InMemoryTenantCatalogDbFixture : IDisposable
{
    public TenantCatalogDb Context { get; }
    public InMemoryTenantCatalogDbFixture()
    {
        var options = new DbContextOptionsBuilder<TenantCatalogDb>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        Context = new TenantCatalogDb(options);
    }
    public void Dispose() => Context.Dispose();
}
```
- [x] **¿Aplicar corrección?**
- **Opinión técnica:** Reduce drásticamente la duplicación y hace que los tests sean más rápidos de escribir.

---

## 8. Priorización Global

| Prioridad | ID | Hallazgo | Impacto |
|-----------|----|----------|---------|
| **Crítica** | INF-004 | Swallowing de excepciones en provisioning | Imposibilidad de diagnóstico en producción |
| **Crítica** | INF-009 | `GanaderiaDbContext` con DefaultConnection fija | Ruptura del aislamiento multitenant |
| **Crítica** | TST-006 | Ausencia de pruebas de integración | Riesgo de regresiones en flujo completo |
| **Alta** | APP-002 | `IPipelineBehavior` custom duplicado | Riesgo de incompatibilidad con MediatR |
| **Alta** | APP-006 | `DependencyInjection.cs` comentado en Application | Acoplamiento de DI a Program.cs |
| **Alta** | APP-010 | Patrón dual MediatR + Ports | Confusión arquitectónica y código muerto |
| **Alta** | INF-008 | `TenantDbContextFactory` no registrado | Imposible resolver BD de tenant |
| **Alta** | INF-011 | DI de Infrastructure incompleto | Servicios faltantes en runtime |
| **Alta** | TST-002, TST-003 | Vacíos masivos en Domain/Application tests | Calidad y confianza del código |
| **Media** | DOM-003 | `Enum.Parse` no validado en Domain | Excepciones de framework en dominio |
| **Media** | DOM-005 | Desajuste int/long en PKs | Inconsistencia de tipo propagada |
| **Media** | APP-007 | `GeneralResponseDTO` internal/duplicado | Duplicación y restricción de capas |
| **Media** | APP-011 | ValidationException pierde metadatos | UX de API deficiente |
| **Media** | IA-005 | Middleware no logea excepciones | Pérdida de trazabilidad |
| **Media** | API-002 | CORS definido pero no aplicado | Bloqueo de peticiones frontend |
| **Baja** | DOM-001, APP-001, INF-001 | Stubs `Class1.cs` | Ruido en codebase |
| **Baja** | IA-004 | Typo `Presentes` | Convención de carpetas |
| **Baja** | API-001 | `AddValidatorsFromAssembly` duplicado | Limpieza |

---

> **Nota final:** Este documento fue generado con base en el análisis estático del código fuente y la documentación vigente en `docs/`. Antes de ejecutar cualquier refactorización, se recomienda crear una rama `feature/refactor-2026-06-06` y aplicar los cambios en commits atómicos siguiendo el `commits-contract.md` del proyecto.
