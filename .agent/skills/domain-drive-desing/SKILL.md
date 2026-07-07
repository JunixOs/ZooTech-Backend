---
name: domain-driven-design
description: >
  Aplicar patrones de Domain-Driven Design (DDD) en C#/.NET: entidades con
  identidad, value objects, enums de dominio, reglas de negocio, excepciones
  de dominio y factory methods. Usar cuando se modelen entidades de negocio,
  se implementen restricciones de dominio, se diseñen agregados, o se necesite
  encapsular la lógica de negocio lejos de la infraestructura.
---

# Domain-Driven Design (DDD) — Patrones en C#

## Conceptos Fundamentales

| Concepto | Descripción | Ejemplo |
|----------|-------------|---------|
| **Entity** | Objeto con identidad propia que cambia en el tiempo | `Animal`, `MilkRecord` |
| **Value Object** | Objeto sin identidad, definido por su valor | `AnimalId`, `Weight` |
| **Enum** | Valores fijos del dominio | `AnimalStatus`, `ReproductionType` |
| **Domain Rule** | Restricción que siempre debe cumplirse | Fecha de nacimiento no puede ser futura |
| **Domain Exception** | Error que viola una regla de dominio | `DomainException` |
| **Factory Method** | Constructor controlado de entidades | `Animal.Create(...)` |
| **Domain Service** | Lógica de negocio que no pertenece a una sola entidad | Cálculo de productividad |

---

## Paso 1 — Excepciones de Dominio

La base de cualquier regla de negocio es una excepción tipada:

```csharp
// Domain/Exceptions/DomainException.cs
namespace Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}
```

**Nunca lanzar `Exception` genérica desde el dominio.** Usar siempre `DomainException` o subclases.

---

## Paso 2 — Value Objects

Los Value Objects **no tienen ID** y son inmutables. Su igualdad se basa en el valor.

```csharp
// Domain/ValueObjects/AnimalId.cs
namespace Domain.ValueObjects;

public class AnimalId
{
    public Guid Value { get; }

    public AnimalId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Id inválido: no puede ser vacío.");
        Value = value;
    }

    public static AnimalId New() => new(Guid.NewGuid());
}
```

```csharp
// Domain/ValueObjects/AnimalName.cs
using Domain.Exceptions;

namespace Domain.ValueObjects;

public class AnimalName
{
    public string Value { get; }

    public AnimalName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("El nombre es obligatorio.");
        if (value.Length < 3)
            throw new DomainException("El nombre es demasiado corto (mínimo 3 caracteres).");
        if (value.Length > 15)
            throw new DomainException("El nombre no puede exceder 15 caracteres.");

        Value = value;
    }
}
```

```csharp
// Domain/ValueObjects/Weight.cs
public class Weight
{
    public decimal Value { get; }
    public string Unit { get; }  // "kg" | "lb"

    public Weight(decimal value, string unit = "kg")
    {
        if (value <= 0)
            throw new DomainException("El peso debe ser mayor a cero.");
        Value = value;
        Unit = unit;
    }
}
```

---

## Paso 3 — Enums de Dominio

Los enums deben vivir en el dominio, no en capas externas:

```csharp
// Domain/Enums/AnimalStatus.cs
namespace Domain.Enums;

public enum AnimalStatus
{
    Active,
    Inactive,
    Sold
}

// Domain/Enums/ReproductionType.cs
public enum ReproductionType
{
    Monta,
    InseminacionArtificial
}

// Domain/Enums/AptoPara.cs
public enum AptoPara
{
    ProduccionLeche,
    Carne,
    Reproduccion
}
```

---

## Paso 4 — Reglas de Dominio (Domain Rules)

Las reglas son validaciones reutilizables que pueden aplicarse en múltiples entidades:

```csharp
// Domain/Rules/AnimalRules.cs
using Domain.Exceptions;

namespace Domain.Rules;

public static class AnimalRules
{
    public static void ValidateBirthDate(DateTime birthDate)
    {
        if (birthDate > DateTime.UtcNow)
            throw new DomainException("La fecha de nacimiento no puede ser futura.");
    }

    public static void ValidateParentCode(string code, string role)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException($"El código del {role} es obligatorio.");
        if (code.Length > 10)
            throw new DomainException($"El código del {role} no puede exceder 10 caracteres.");
        if (code != code.ToUpper())
            throw new DomainException($"El código del {role} debe estar en mayúsculas.");
    }

    public static void ValidatePrecioCompra(decimal? precio, string adquisicionPor)
    {
        if (adquisicionPor == "compra" && (precio == null || precio <= 0))
            throw new DomainException("El precio de compra es obligatorio cuando la adquisición es por compra.");
    }
}
```

---

## Paso 5 — Entidad con Factory Method

**Nunca usar constructores públicos con setters para entidades de dominio.**
Usar Factory Methods que aplican las reglas antes de crear el objeto.

```csharp
// Domain/Entities/Animal.cs
using Domain.ValueObjects;
using Domain.Enums;
using Domain.Rules;

namespace Domain.Entities;

public class Animal
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public DateTime BirthDate { get; private set; }
    public AnimalStatus Status { get; private set; }
    public string? CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Constructor privado — obliga a usar el Factory Method
    private Animal() { }

    // Factory Method — punto de entrada controlado
    public static Animal Create(
        AnimalId id,
        AnimalName name,
        DateTime birthDate,
        DateTime createdAt,
        string? createdBy)
    {
        // Aplicar reglas de dominio antes de crear
        AnimalRules.ValidateBirthDate(birthDate);

        return new Animal
        {
            Id = id.Value,
            Name = name.Value,
            BirthDate = birthDate,
            CreatedAt = createdAt,
            CreatedBy = createdBy,
            Status = AnimalStatus.Active   // estado inicial siempre Active
        };
    }

    // Comportamientos del dominio — NO setters públicos
    public void Deactivate()
    {
        if (Status == AnimalStatus.Inactive)
            throw new DomainException("El animal ya está inactivo.");
        Status = AnimalStatus.Inactive;
    }

    public void MarkAsSold()
    {
        if (Status == AnimalStatus.Sold)
            throw new DomainException("El animal ya fue vendido.");
        Status = AnimalStatus.Sold;
    }
}
```

---

## Paso 6 — Entidades relacionadas

```csharp
// Domain/Entities/MilkRecord.cs
public class MilkRecord
{
    public Guid Id { get; private set; }
    public Guid AnimalId { get; private set; }
    public decimal Liters { get; private set; }
    public DateTime RecordedAt { get; private set; }

    private MilkRecord() { }

    public static MilkRecord Register(Guid animalId, decimal liters, DateTime recordedAt)
    {
        if (liters <= 0)
            throw new DomainException("La cantidad de leche debe ser mayor a cero.");
        if (recordedAt > DateTime.UtcNow)
            throw new DomainException("La fecha de registro no puede ser futura.");

        return new MilkRecord
        {
            Id = Guid.NewGuid(),
            AnimalId = animalId,
            Liters = liters,
            RecordedAt = recordedAt
        };
    }
}
```

---

## Paso 7 — Domain Services (para lógica multi-entidad)

Solo crear un Domain Service cuando la lógica **no pertenece a una sola entidad**:

```csharp
// Domain/Services/ProductivityCalculator.cs
namespace Domain.Services;

public class ProductivityCalculator
{
    // Calcula el promedio de producción entre varias vacas
    public decimal CalculateAverageProduction(IEnumerable<MilkRecord> records)
    {
        if (!records.Any())
            throw new DomainException("No hay registros de producción.");
        return records.Average(r => r.Liters);
    }
}
```

---

## Estructura completa de Domain Layer

```
Domain/
├── Entities/
│   ├── Animal.cs
│   ├── MilkRecord.cs
│   ├── ReproductionEvent.cs
│   └── AuditEntry.cs
├── ValueObjects/
│   ├── AnimalId.cs
│   ├── AnimalName.cs
│   ├── Weight.cs
│   └── MilkQuantity.cs
├── Enums/
│   ├── AnimalStatus.cs
│   ├── ReproductionType.cs
│   └── AptoPara.cs
├── Rules/
│   └── AnimalRules.cs
├── Exceptions/
│   └── DomainException.cs
└── Services/
    └── ProductivityCalculator.cs
```

---

## Manejo de Errores

- **Domain Layer lanza `DomainException`** — nunca `ArgumentException` ni `Exception` genérica.
- **Application Layer atrapa `DomainException`** y la convierte en `BusinessException` o la deja subir al Middleware.
- **Infrastructure nunca debe lanzar `DomainException`** — sus errores son excepciones técnicas.

```csharp
// ExceptionHandlingMiddleware mapeando por tipo
catch (DomainException ex)   => HTTP 400 con message
catch (BusinessException ex) => HTTP 422 con message
catch (Exception ex)         => HTTP 500 "Error interno"
```

---

## Buenas Prácticas

- **Domain Layer cero dependencias externas**: no referenciar EF Core, ASP.NET ni ningún NuGet externo.
- **Value Objects son inmutables**: una vez creados, no pueden modificarse.
- **Toda regla de negocio vive en Domain**: nunca en Controllers, nunca en Infrastructure.
- **Factory Methods en lugar de constructores públicos**: garantizan invariantes desde la creación.
- **Comportamientos en lugar de setters**: `animal.Deactivate()` en lugar de `animal.Status = Inactive`.
- **Enums de dominio en el proyecto Domain**: no usar strings mágicos en el dominio.

## Patrones Comunes

- **Guard Clauses en Value Objects**: validar en el constructor y lanzar inmediatamente.
- **Estado inicial por Factory**: la entidad siempre nace en un estado válido.
- **Métodos de transición de estado**: `Activate()`, `Deactivate()`, `MarkAsSold()`.
- **Invariant enforcement**: el dominio garantiza que nunca exista un estado inconsistente.
