# Estándar Clean Architecture — Zootech Backend

> **Versión:** 1.0  
> **Última actualización:** Junio 2026  
> **Objetivo:** Definir UN estándar arquitectónico para todo el equipo. Nadie codea sin leer esto primero.

---

## Principio fundamental

**Todas las dependencias apuntan al centro.**  
Domain no conoce a nadie. Application conoce a Domain. Infrastructure conoce a Domain y Application. InterfaceAdapters conoce a Application.

---

## El flujo de una request

```
REQUEST HTTP
  │
  ▼
┌─────────────────────────────────────────────────────────────┐
│ INTERFACE ADAPTERS                                          │
│                                                             │
│  Controller recibe Request DTO                              │
│  Mapper: Request → Command                                  │
│  Llama a I*InputPort.HandleAsync(command, ct)               │
│  Mapper: Output → Response DTO                              │
│  Envuelve en GeneralResponseDTO<T>                          │
└─────────────────────────────────────────────────────────────┘
  │
  ▼
┌─────────────────────────────────────────────────────────────┐
│ APPLICATION                                                 │
│                                                             │
│  I*InputPort recibe el Command                              │
│  FluentValidation valida el Command                         │
│  Interactor ejecuta lógica de negocio                       │
│  Interactor llama a IXxxRepository (puerto en Domain)       │
│  Interactor mapea Entity → Output                           │
└─────────────────────────────────────────────────────────────┘
  │
  ▼
┌─────────────────────────────────────────────────────────────┐
│ DOMAIN                                                      │
│                                                             │
│  Entity: factory methods (CreateNew, Rehydrate)             │
│  XxxRule: validaciones de negocio (static class)            │
│  IXxxRepository: contrato (no implementación)               │
└─────────────────────────────────────────────────────────────┘
  │
  ▼
┌─────────────────────────────────────────────────────────────┐
│ INFRASTRUCTURE                                              │
│                                                             │
│  XxxRepository : IXxxRepository                             │
│  ToEntity(domain) → EF entity                               │
│  _dbContext.SaveChangesAsync(ct)                            │
│  ToDomain(efEntity) → Domain entity                         │
└─────────────────────────────────────────────────────────────┘
```

---

## Estructura de carpetas por módulo

```
src/
├── ZooTech.Domain/
│   └── Module_{Modulo}/
│       ├── Entities/
│       │   └── {Entidad}.cs
│       ├── Rules/
│       │   └── {Entidad}Rule.cs
│       ├── Interfaces/
│       │   └── I{Entidad}Repository.cs
│       ├── Enums/
│       └── ValueObjects/
│
├── ZooTech.Application/
│   └── Modules/
│       └── Module_{Modulo}/
│           ├── UseCases/
│           │   └── {Operacion}/
│           │       ├── I{Operacion}InputPort.cs
│           │       ├── {Operacion}Command.cs
│           │       ├── {Operacion}Interactor.cs
│           │       └── {Operacion}Output.cs
│           ├── DTOs/
│           │   ├── Requests/
│           │   └── Responses/
│           ├── Validators/
│           └── Common/
│
├── ZooTech.Infrastructure/
│   └── Persistence/
│       └── Modules/
│           └── Module_{Modulo}/
│               └── Repositories/
│                   └── {Entidad}Repository.cs
│
└── ZooTech.InterfaceAdapters/
    └── Modules/
        └── Module_{Modulo}/
            ├── Controllers/
            │   └── {Modulo}Controller.cs
            ├── DTOs/
            │   ├── Requests/
            │   └── Responses/
            └── Mappers/
                └── {Modulo}Mapper.cs
```

---

## Las 17 reglas

### DOMAIN — 3 reglas

| # | Regla | Qué pasa si la rompés |
|---|---|---|
| **1** | **Entidad con constructor privado + factory methods** (`CreateNew`, `Rehydrate`) | Setters públicos. Validación esparcida. Entidades inválidas. |
| **2** | **`IXxxRepository` en `Domain/Interfaces/`** | Acoplamiento inverso: el repo conoce DTOs de UseCases. Dependencia circular. |
| **3** | **Reglas de negocio en `static class XxxRule`** | Validación duplicada o ausente. Datos inválidos llegan a la DB. |

### APPLICATION — 6 reglas

| # | Regla | Qué pasa si la rompés |
|---|---|---|
| **4** | **Todo UseCase tiene `I*InputPort` + `*Interactor` + `Command` + `Output`** | No podés mockear el UseCase. Controller acoplado a implementación concreta. |
| **5** | **`Command` y `Output` son `sealed record`, no clases mutables** | Mutabilidad inesperada. Bugs sutiles. |
| **6** | **DTOs de negocio en `Application/Modules/Module_X/DTOs/`** | DTOs en InterfaceAdapters contaminan la lógica de negocio con HTTP. |
| **7** | **Validación con FluentValidation en `Validators/`, ANTES del dominio** | `DbUpdateException` como catch-all. Errores crípticos. |
| **8** | **El repositorio NUNCA devuelve DTOs. Solo entidades de dominio.** | El repo conoce la capa de arriba. Dependencia circular. |
| **9** | **`CancellationToken` en todos los métodos async de toda la cadena** | Queries zombie en la DB. Desperdicio de recursos. |

### INFRASTRUCTURE — 3 reglas

| # | Regla | Qué pasa si la rompés |
|---|---|---|
| **10** | **`ToDomain(entity)` y `ToEntity(domain)` en todo repositorio** | EF entities usadas como domain objects. Cambiar la DB rompe el dominio. |
| **11** | **Un repositorio por agregado. Nada de queries cross-aggregate.** | `IOrdenioRepository.ListVacunosAsync()` viola el principio del repositorio. |
| **12** | **DbContext se registra UNA SOLA VEZ en la composición raíz** | Múltiples pools de conexiones. Conflictos de configuración. |

### INTERFACE ADAPTERS — 4 reglas

| # | Regla | Qué pasa si la rompés |
|---|---|---|
| **13** | **Un controller por módulo. UNA ruta base: `api/v1/{modulo}`** | Colisión de rutas en runtime. ASP.NET no sabe cuál elegir. |
| **14** | **El controller NUNCA inyecta un repositorio. Solo InputPorts.** | Bypassea Application. Viola la arquitectura por completo. |
| **15** | **`GeneralResponseDTO<T>` unificado para TODAS las respuestas** | El frontend recibe formatos distintos según el módulo. Caos. |
| **16** | **Excepciones de dominio mapeadas a HTTP en UN middleware** | try/catch repetido en cada endpoint. Mensajes inconsistentes. |

### TRANSVERSAL — 1 regla

| # | Regla | Qué pasa si la rompés |
|---|---|---|
| **17** | **Cada capa tiene SU `DependencyInjection.cs`. Registra SOLO lo suyo.** | Registros triplicados. No sabés quién registra qué. |

---

## Excepciones tipadas y su mapeo HTTP

| Excepción | Capa | HTTP |
|---|---|---|
| `ValidationException` (FluentValidation) | Application | **400** Bad Request |
| `NotFoundException` | Application | **404** Not Found |
| `ConflictException` | Application | **409** Conflict |
| `ArgumentException` | Domain | **400** Bad Request (envuelta por el Interactor) |
| `OperationCanceledException` | Transversal | **499** Client Closed Request |

---

## Convenciones de código

- **Clases:** `sealed` siempre que no se herede de ellas
- **DTOs, Commands, Outputs:** `sealed record`
- **Mappers:** `internal static class` — visibilidad mínima necesaria
- **Validators:** `internal sealed class` que hereda de `AbstractValidator<T>`
- **Controllers:** `[ApiController]`, `[Route("api/v1/{modulo}")]`, constructor injection
- **Rutas REST:**
  - `GET    api/v1/{modulo}` → listar
  - `GET    api/v1/{modulo}/{id}` → obtener por id
  - `POST   api/v1/{modulo}` → crear
  - `PUT    api/v1/{modulo}/{id}` → actualizar completo
  - `PATCH  api/v1/{modulo}/{id}` → actualizar parcial
  - `DELETE api/v1/{modulo}/{id}` → eliminar (soft delete)

---

## Checklist para un UseCase nuevo

Antes de hacer PR, verificá:

- [ ] `IXxxRepository` en `Domain/Interfaces/`
- [ ] `I{Operacion}InputPort` en `Application/UseCases/{Operacion}/`
- [ ] `{Operacion}Command` como `sealed record`
- [ ] `{Operacion}Interactor` como `sealed class`
- [ ] `{Operacion}Output` como `sealed record`
- [ ] Validator con FluentValidation registrado en DI
- [ ] `CancellationToken` en toda la cadena (InputPort → Interactor → Repository)
- [ ] `XxxRepository` con `ToDomain` y `ToEntity`
- [ ] `XxxMapper` en InterfaceAdapters para Request↔Command y Output↔Response
- [ ] Controller registrado con ruta `api/v1/{modulo}`
- [ ] `GeneralResponseDTO<T>` en la respuesta
- [ ] `[ProducesResponseType]` para cada status code
- [ ] DI registrado solo en su capa correspondiente

---

## Módulo de referencia: Producción Leche

El módulo `Producción Leche` (Ordenios) es el que MÁS SE ACERCA a este estándar. Usalo como molde para nuevos módulos y refactorizaciones. Lo que hace bien:

- ✅ Entidad `Ordenio` con constructor privado y factory methods
- ✅ `OrdenioRule` con validaciones de negocio
- ✅ UseCases con InputPort + Interactor + Command + Output
- ✅ `sealed record` para Commands y Outputs
- ✅ `OrdenioMapper` en InterfaceAdapters
- ✅ `ToDomain` / `ToEntity` en el repositorio
- ✅ `CancellationToken` en toda la cadena
- ✅ Excepciones tipadas (`NotFoundException`, `ConflictException`)

**Cosas a corregir en Producción Leche para cumplir el estándar al 100%:**

- ❌ `IOrdenioRepository` está en `Application/Ports/` → debe moverse a `Domain/Interfaces/`
- ❌ `IOrdenioRepository` depende de `ListOrdeniosQuery` → el query debe moverse o desacoplarse
- ❌ `VacunoSimpleOutput` vive dentro de `IOrdenioRepository.cs` → debe moverse a DTOs
- ❌ `ListVacunosAsync` en repo de Ordeños → debe moverse a un `IVacunoRepository`
- ❌ `Rehydrate` no asigna `MotivoEliminacion` → **bug**
- ❌ Datos hardcodeados en `GetVacunos` → eliminar
- ❌ `GetVacunos` bypasea Application → crear InputPort + Interactor
- ❌ `HomeController` dentro del módulo → mover a raíz de Controllers
- ❌ Ruta `v1/produccion-leche` → debe ser `api/v1/produccion-leche`
- ❌ `OrdenioRule` no es `static class` → corregir
