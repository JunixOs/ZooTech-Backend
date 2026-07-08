---
name: application-layer-patterns
description: >
  Implementar la capa de Application en Clean Architecture: Use Cases (Interactors),
  Commands, Output Ports, Validators, Behaviors Pipeline (logging, validación),
  Gateways (interfaces hacia servicios externos), Features flags, Configuration
  dinámica y Rules Engine. Usar cuando se diseñen casos de uso, se implemente
  orquestación de lógica de negocio, o se necesite un pipeline de comportamientos
  reutilizables en .NET C#.
---

# Application Layer Patterns — Use Cases, Behaviors y Gateways en .NET

## Rol de la Application Layer

La Application Layer **orquesta** sin contener lógica de negocio propia:
- Recibe un Command (DTO de entrada).
- Valida con Behaviors.
- Llama al dominio para crear/modificar entidades.
- Persiste a través de interfaces (Repositories).
- Devuelve el resultado a través del Output Port.

---

## Paso 1 — Gateways: interfaces hacia el exterior

Todos los servicios externos que Application necesita se definen como interfaces en `Common/Gateway/`:

```csharp
// Context — datos de la request actual
public interface IRequestContext
{
    string? UserId { get; }
    string? UserEmail { get; }
    string? CorrelationId { get; }
    string? Path { get; }
}

public interface ITenantContext
{
    Guid TenantId { get; }
    string DatabaseName { get; }
}

// Caching
public interface IAppCacheService
{
    Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan expiration);
}

// Configuración dinámica por tenant
public interface IConfigurationService
{
    Task<T?> GetValueAsync<T>(string key);
}

// Feature flags por tenant
public interface IFeatureService
{
    Task<bool> IsEnabledAsync(string feature);
}

// Motor de reglas dinámicas
public interface IRuleEngineService
{
    Task<bool> EvaluateAsync(string rule, object input);
}

// Identidad del usuario actual
public interface ICurrentUserService
{
    string? UserId { get; }
}

// Abstracción de tiempo (permite mockear en tests)
public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}

// Repositorio genérico (uno por agregado)
public interface IAnimalRepository
{
    Task AddAsync(Animal animal);
    Task<Animal?> GetByIdAsync(Guid id);
    Task<List<Animal>> GetAllAsync();
}
```

---

## Paso 2 — Command, Output y Ports de un Use Case

Cada caso de uso tiene exactamente estos archivos:

```csharp
// CreateAnimalCommand.cs — DTO de entrada
namespace Application.Modules.Animals.UseCases.CreateAnimal;

public class CreateAnimalCommand
{
    public string Name { get; set; } = default!;
    public DateTime BirthDate { get; set; }
    public string AdquisicionPor { get; set; } = "monta";
    public decimal? PrecioCompra { get; set; }
}

// CreateAnimalOutput.cs — DTO de salida
public class CreateAnimalOutput
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
}

// ICreateAnimalInputPort.cs — contrato de entrada
public interface ICreateAnimalInputPort
{
    Task Handle(CreateAnimalCommand command);
}

// ICreateAnimalOutputPort.cs — contrato de salida
public interface ICreateAnimalOutputPort
{
    Task Ok(CreateAnimalOutput output);
    Task Error(string code, string message); // opcional, para errores de negocio
}
```

---

## Paso 3 — Interactor: orquestación del caso de uso

```csharp
// CreateAnimalInteractor.cs
public class CreateAnimalInteractor : ICreateAnimalInputPort
{
    private readonly IAnimalRepository _repo;
    private readonly ICreateAnimalOutputPort _output;
    private readonly IFeatureService _features;
    private readonly IConfigurationService _config;
    private readonly IRuleEngineService _rules;
    private readonly ICurrentUserService _user;
    private readonly IDateTimeProvider _time;
    private readonly IAppCacheService _cache;
    private readonly ITenantContext _tenant;

    public CreateAnimalInteractor(/* todos los gateways */) { /* asignar */ }

    public async Task Handle(CreateAnimalCommand cmd)
    {
        // 1. Verificar feature flag
        if (!await _features.IsEnabledAsync("animals.create"))
            throw new BusinessException("La creación de animales no está habilitada.");

        // 2. Obtener configuración dinámica (con caché por tenant)
        var minNameLength = await _cache.GetOrCreateAsync(
            $"tenant:{_tenant.TenantId}:config:minName",
            async () => await _config.GetValueAsync<int>("animal.name.min"),
            TimeSpan.FromMinutes(5)
        );

        if (cmd.Name.Length < minNameLength)
            throw new BusinessException($"El nombre debe tener al menos {minNameLength} caracteres.");

        // 3. Evaluar regla de negocio dinámica
        var isValid = await _rules.EvaluateAsync(
            "animal.creation.allowed",
            new { cmd.Name, cmd.BirthDate }
        );
        if (!isValid)
            throw new BusinessException("La creación no cumple las reglas del tenant.");

        // 4. Crear entidad de dominio (Domain valida invariantes)
        var animal = Animal.Create(
            AnimalId.New(),
            new AnimalName(cmd.Name),
            cmd.BirthDate,
            _time.UtcNow,
            _user.UserId
        );

        // 5. Persistir
        await _repo.AddAsync(animal);

        // 6. Devolver resultado
        await _output.Ok(new CreateAnimalOutput
        {
            Id = animal.Id,
            Name = animal.Name,
            CreatedAt = animal.CreatedAt
        });
    }
}
```

---

## Paso 4 — Validator del Use Case

```csharp
// CreateAnimalValidator.cs
public class CreateAnimalValidator : IValidator<CreateAnimalCommand>
{
    public void Validate(CreateAnimalCommand cmd)
    {
        if (string.IsNullOrWhiteSpace(cmd.Name))
            throw new ValidationException("El nombre es requerido.");

        if (cmd.Name.Length > 15)
            throw new ValidationException("El nombre no puede exceder 15 caracteres.");

        if (cmd.AdquisicionPor == "compra" && cmd.PrecioCompra == null)
            throw new ValidationException("El precio de compra es obligatorio cuando la adquisición es por compra.");
    }
}
```

---

## Paso 5 — Behaviors Pipeline

Los Behaviors actúan como middlewares **alrededor** del Interactor. Se ejecutan en cadena antes (y después) del caso de uso.

```csharp
// IPipelineBehavior.cs
public interface IPipelineBehavior<TRequest>
{
    Task Handle(TRequest request, Func<Task> next);
}

// ValidationBehavior.cs — valida antes de ejecutar
public class ValidationBehavior<TRequest> : IPipelineBehavior<TRequest>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public async Task Handle(TRequest request, Func<Task> next)
    {
        foreach (var validator in _validators)
            validator.Validate(request);   // lanza si hay error

        await next();   // continúa al siguiente behavior o al Interactor
    }
}

// LoggingBehavior.cs — loguea entrada y salida
public class LoggingBehavior<TRequest> : IPipelineBehavior<TRequest>
{
    public async Task Handle(TRequest request, Func<Task> next)
    {
        Console.WriteLine($"[LOG] → {typeof(TRequest).Name} iniciado");
        await next();
        Console.WriteLine($"[LOG] ✓ {typeof(TRequest).Name} completado");
    }
}
```

**Orden del pipeline:**
```
Request → LoggingBehavior → ValidationBehavior → Interactor → OutputPort → Response
```

---

## Paso 6 — BusinessException

```csharp
// Application/Common/Exceptions/BusinessException.cs
namespace Application.Common.Exceptions;

public class BusinessException : Exception
{
    public string Code { get; }

    public BusinessException(string message, string code = "BUSINESS_ERROR")
        : base(message)
    {
        Code = code;
    }
}

// Uso en Interactor:
if (!await _features.IsEnabledAsync("animals.create"))
    throw new BusinessException("Feature deshabilitada", "FEATURE_DISABLED");
```

---

## Paso 7 — PaginationModel compartido

```csharp
// Application/Common/Models/PaginationModel.cs
public class PaginationModel
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public int Skip => (Page - 1) * PageSize;
}

public class PagedResult<T>
{
    public List<T> Data { get; set; } = new();
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)Total / PageSize);
}
```

---

## Estructura del módulo completo

```
Application/Modules/Animals/
├── UseCases/
│   ├── CreateAnimal/
│   │   ├── CreateAnimalCommand.cs
│   │   ├── CreateAnimalOutput.cs
│   │   ├── ICreateAnimalInputPort.cs
│   │   ├── ICreateAnimalOutputPort.cs
│   │   ├── CreateAnimalInteractor.cs
│   │   └── CreateAnimalValidator.cs
│   ├── GetAnimalById/
│   └── ListAnimals/
└── Interfaces/
    └── IAnimalRepository.cs    ← alternativa a Common/Gateway/Repositories/
```

---

## Manejo de Errores

| Excepción | Origen | Traducción HTTP |
|-----------|--------|-----------------|
| `DomainException` | Domain | 400 Bad Request |
| `BusinessException` | Application | 422 Unprocessable o 400 |
| `ValidationException` | Behavior | 400 Bad Request con details |
| `NotFoundException` | Interactor | 404 Not Found |

---

## Buenas Prácticas

- **Un Interactor = un caso de uso.** No combinar lógica de varios casos de uso.
- **Interactor solo conoce interfaces (Gateways)**, nunca clases concretas de Infrastructure.
- **Command es un DTO plano** sin lógica; la validación la hace el Validator.
- **OutputPort en lugar de return**: el Interactor llama al OutputPort; el Presenter captura el resultado.
- **Feature flags antes de todo**: verificar primero si la feature está habilitada.
- **Cachear configuración por tenant**: evitar hits a BD en cada request.
- **IDateTimeProvider en lugar de `DateTime.UtcNow` directo**: permite mockear en tests.

## Patrones Comunes

- **Pipeline = Logging → Validation → Interactor**: orden estándar de behaviors.
- **Output Port + Presenter**: desacopla el resultado del mecanismo de respuesta HTTP.
- **Command → Domain entity**: el Interactor convierte el Command en Value Objects de dominio.
- **Feature → Config → Rules → Domain → Repo → Output**: flujo estándar de un Interactor.
