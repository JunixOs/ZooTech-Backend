---
name: clean-architecture
description: >
  Aplicar Clean Architecture en proyectos .NET con enfoque modular y multi-tenant.
  Usar cuando se diseñen o revisen sistemas backend en C#/.NET que requieran
  separación de capas (Domain, Application, InterfaceAdapters, Infrastructure),
  organización de carpetas, diagramas C4, o decisiones de arquitectura de software.
---

# Clean Architecture — .NET Modular + Multi-Tenant

## Visión General

Clean Architecture organiza el código en **4 capas concéntricas**, donde las dependencias solo apuntan hacia adentro:

```
InterfaceAdapters → Application → Domain
         ↑
   Infrastructure
```

| Capa | Responsabilidad |
|------|----------------|
| **Domain** | Reglas de negocio puras, sin dependencias externas |
| **Application** | Orquestación de casos de uso |
| **InterfaceAdapters** | Entrada/salida HTTP, mapeo de DTOs |
| **Infrastructure** | Implementación técnica: BD, caché, servicios externos |

---

## Paso 1 — Organizar la estructura de carpetas

### Domain Layer
```
Domain/
├── Entities/
│   ├── Animal.cs
│   └── MilkRecord.cs
├── ValueObjects/
│   ├── AnimalId.cs
│   └── Weight.cs
├── Enums/
│   └── AnimalStatus.cs
├── Rules/
│   └── AnimalRules.cs
├── Exceptions/
│   └── DomainException.cs
└── Services/         ← solo lógica compleja multi-entidad
```

### Application Layer
```
Application/
├── Common/
│   ├── Gateway/      ← interfaces hacia el exterior
│   │   ├── Context/
│   │   ├── Caching/
│   │   ├── Repositories/
│   │   ├── Identity/
│   │   └── Features/
│   ├── Behaviors/
│   ├── Exceptions/
│   └── Models/
└── Modules/
    └── Animals/
        └── UseCases/
            └── CreateAnimal/
                ├── CreateAnimalCommand.cs
                ├── CreateAnimalOutput.cs
                ├── ICreateAnimalInputPort.cs
                ├── ICreateAnimalOutputPort.cs
                ├── CreateAnimalInteractor.cs
                └── CreateAnimalValidator.cs
```

### InterfaceAdapters Layer
```
InterfaceAdapters/
├── Controllers/
├── DTOs/
│   ├── Requests/
│   └── Responses/
├── Mappers/
├── Presenters/
├── Middleware/
└── Filters/
```

### Infrastructure Layer
```
Infrastructure/
├── Persistence/
│   ├── GanaderiaDbContext.cs
│   ├── Entities/
│   ├── Mappers/
│   ├── Configurations/
│   └── Repositories/
├── Tenant/
├── Configuration/
├── Features/
├── Rules/
├── Caching/
├── Context/
├── Identity/
├── Auditing/
├── Time/
└── External/
```

---

## Paso 2 — Definir dependencias entre capas

**Regla de oro:** una capa solo puede depender de capas más internas.

```
Domain         → no depende de nadie
Application    → depende de Domain
InterfaceAdapters → depende de Application (via interfaces)
Infrastructure → depende de Application (implementa interfaces)
```

**Application define interfaces (Gateways); Infrastructure las implementa:**

```csharp
// Application/Common/Gateway/Repositories/IAnimalRepository.cs
public interface IAnimalRepository
{
    Task AddAsync(Animal animal);
    Task<List<Animal>> GetAllAsync();
}

// Infrastructure/Persistence/Repositories/AnimalRepository.cs
public class AnimalRepository : IAnimalRepository
{
    // implementación concreta con EF Core
}
```

---

## Paso 3 — Implementar el patrón Ports & Adapters por caso de uso

Cada caso de uso expone dos puertos:

```csharp
// Puerto de entrada (lo que llama el Controller)
public interface ICreateAnimalInputPort
{
    Task Handle(CreateAnimalCommand command);
}

// Puerto de salida (lo que llama el Interactor para responder)
public interface ICreateAnimalOutputPort
{
    Task Ok(CreateAnimalOutput output);
}
```

El **Presenter** (en InterfaceAdapters) implementa el OutputPort:

```csharp
public class CreateAnimalPresenter : ICreateAnimalOutputPort
{
    public AnimalResponse? Response { get; private set; }

    public Task Ok(CreateAnimalOutput output)
    {
        Response = new AnimalResponse { Id = output.Id, Name = output.Name };
        return Task.CompletedTask;
    }
}
```

---

## Paso 4 — Flujo completo de una request

```
HTTP Request
  → Middleware (Tenant, Logging)
  → Filter (Validación de modelo)
  → Controller
      → Mapper (Request → Command)
      → InputPort.Handle(command)
          → Behavior Pipeline (Logging → Validation)
          → Interactor
              → Feature check
              → Cache / Config
              → Rules engine
              → Domain entity creation
              → Repository.AddAsync()
              → OutputPort.Ok(output)
  → Presenter.Response
  → Controller → HTTP 201
```

---

## Ejemplos

### Controller conectando todas las capas

```csharp
[ApiController]
[Route("api/[controller]")]
public class AnimalsController : ControllerBase
{
    private readonly ICreateAnimalInputPort _inputPort;
    private readonly CreateAnimalPresenter _presenter;

    public AnimalsController(
        ICreateAnimalInputPort inputPort,
        CreateAnimalPresenter presenter)
    {
        _inputPort = inputPort;
        _presenter = presenter;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateAnimalRequest request)
    {
        var command = AnimalMapper.ToCommand(request);   // InterfaceAdapters → Application
        await _inputPort.Handle(command);                // Application
        return Ok(_presenter.Response);                  // Presenter → HTTP
    }
}
```

### Diagrama C4 — Niveles

| Nivel | Descripción |
|-------|-------------|
| **Context** | Sistema en relación con usuarios y sistemas externos |
| **Containers** | SPA Angular + API REST .NET + SQL + Redis + ClickHouse |
| **Components** | Domain / Application / InterfaceAdapters / Infrastructure |
| **Code** | Clases individuales (Entidades, Interactors, etc.) |

---

## Manejo de Errores por Capa

| Capa | Tipo de excepción | Cuándo usarla |
|------|-------------------|---------------|
| Domain | `DomainException` | Regla de negocio violada |
| Application | `BusinessException` | Feature deshabilitada, regla dinámica |
| InterfaceAdapters | `ExceptionHandlingMiddleware` | Atrapa todo y retorna 500 |
| Infrastructure | Excepciones nativas de EF Core / HttpClient | No atrapar aquí, dejar subir |

```csharp
// ExceptionHandlingMiddleware — captura global
public async Task InvokeAsync(HttpContext context)
{
    try { await _next(context); }
    catch (DomainException ex)
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsJsonAsync(new { error = ex.Message });
    }
    catch (Exception ex)
    {
        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new { error = "Error interno" });
    }
}
```

---

## Buenas Prácticas

- **Domain no debe referenciar ningún namespace externo** (ni EF Core, ni ASP.NET).
- **Application solo conoce interfaces**, nunca clases concretas de Infrastructure.
- **Un caso de uso = una carpeta** con su Command, Output, Ports, Interactor y Validator.
- **No colocar lógica de negocio en el Controller**; el Controller solo mapea y delega.
- **Usar Factory Methods** en las entidades de dominio en lugar de constructores públicos.
- **Registrar dependencias por capa** usando `DependencyInjection.cs` en cada proyecto.

## Patrones Comunes

- **Clean Architecture + CQRS**: separar Commands (escritura) y Queries (lectura) como módulos distintos.
- **Clean Architecture + Módulos**: cada módulo de negocio (Animals, Reproduction, Health) tiene su propia carpeta dentro de `Application/Modules/`.
- **Clean Architecture + Multi-Tenant**: agregar capa de resolución de tenant en Middleware e Infrastructure (ver skill `multi-tenancy`).
