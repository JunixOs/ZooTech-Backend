---
name: api-design-rest
description: >
  Diseñar y documentar contratos REST API siguiendo estándar OpenAPI 3.0.
  Usar cuando se creen o revisen endpoints HTTP, contratos frontend-backend,
  estructuras de request/response, validaciones, códigos de estado, manejo
  de errores estandarizado, paginación, filtros, reportes o autenticación JWT.
  Incluye el módulo vacuno (ZooSoft) como referencia real.
---

# API Design REST — OpenAPI 3.0 Contract-First

## Principio General

> El contrato define lo que el frontend puede consumir y lo que el backend
> debe implementar **antes de empezar el código**.

---

## Paso 1 — Definir la cabecera del contrato

```yaml
Módulo:        Vacuno
Ruta base:     /v1/vacunos
Estándar:      OpenAPI 3.0 — contrato funcional por endpoint
Autenticación: Bearer Token JWT en todos los endpoints
Versión:       1.0.0
```

**Cada endpoint debe documentar:**
1. Endpoint (ruta)
2. Método HTTP
3. Request (path params, query params, body)
4. Response (campos, tipos, descripción)
5. Códigos de estado
6. Validaciones
7. Errores

---

## Paso 2 — Métodos HTTP y cuándo usarlos

| Método | Uso | Idempotente |
|--------|-----|-------------|
| `GET` | Leer recursos, listar, reportes | Sí |
| `POST` | Crear nuevo recurso | No |
| `PATCH` | Actualización parcial | No |
| `PUT` | Reemplazo completo | Sí |
| `DELETE` | Eliminar (soft o hard) | Sí |

---

## Paso 3 — Estructura de cada endpoint

### Ejemplo: Listar vacunos

```
Endpoint:    GET /v1/vacunos
Método HTTP: GET

Query Params:
  page          integer  opcional   default: 1
  limit         integer  opcional   default: 20
  fechaDesde    date     opcional   default: hoy - 30 días (YYYY-MM-DD)
  fechaHasta    date     opcional   default: hoy
  q             string   opcional   busca por código o nombre
  raza          string   opcional   filtra por raza
  procedencia   string   opcional   filtra por granja/distrito/provincia/departamento
  estado        enum     opcional   valores: vivo | muerto

Ejemplo:
  GET /v1/vacunos?page=1&limit=20&fechaDesde=2024-03-01&estado=vivo

Response 200:
  data[].id              integer   ID único del vacuno
  data[].codigo          string    Código único
  data[].fechaRegistro   date      Fecha de registro
  data[].nombre          string    Nombre
  data[].raza            string    Raza
  data[].procedencia     string    Granja y ubicación
  data[].estado          enum      vivo | muerto
  pagination             object    { page, limit, total, totalPages }
```

### Ejemplo: Crear vacuno

```
Endpoint:    POST /v1/vacunos
Método HTTP: POST
Content-Type: multipart/form-data

Body (campos):
  codigo           string   requerido   máx 10 chars, único, MAYÚSCULAS
  nombre           string   requerido   máx 15 chars
  fechaNacimiento  date     requerido
  adquisicionPor   enum     requerido   monta | compra  (default: monta)
  precioCompra     number   condicional obligatorio si adquisicionPor = compra
  raza             string   requerido   default: Angus
  color            string   requerido   máx 15 chars
  sexo             enum     requerido   macho | hembra  (default: hembra)
  codigoPadre      string   requerido   máx 10 chars, MAYÚSCULAS
  codigoMadre      string   requerido   máx 10 chars, MAYÚSCULAS
  granja           string   requerido   máx 15 chars
  distrito         string   requerido
  departamento     string   requerido
  provincia        string   requerido
  aptoPara         enum     requerido   produccion_leche | carne | reproduccion
  fechaEspecif.    date     requerido
  foto             file     opcional    PNG | JPG | JPEG
  observaciones    string   opcional    máx 150 chars o 30 palabras

Response 201:
  id               integer
  codigo           string
  nombre           string
  fechaNacimiento  date
  adquisicionPor   enum
  precioCompra     number | null
  fotoUrl          string | null
  creadoEn         date-time
```

---

## Paso 4 — Paginación estándar

Todos los endpoints de listado deben devolver:

```json
{
  "data": [ ... ],
  "pagination": {
    "page": 1,
    "limit": 20,
    "total": 150,
    "totalPages": 8
  }
}
```

Reglas:
- `page` y `limit` siempre numéricos.
- Por defecto, filtrar los **últimos 30 días** si aplica rango de fechas.
- Fechas siempre en formato `YYYY-MM-DD`.

---

## Paso 5 — Códigos de estado HTTP

| Código | Significado | Cuándo usarlo |
|--------|-------------|---------------|
| 200 OK | Éxito en lectura | GET exitoso |
| 201 Created | Recurso creado | POST exitoso |
| 400 Bad Request | Datos o filtros inválidos | Validación fallida |
| 401 Unauthorized | Token ausente o inválido | JWT faltante/expirado |
| 404 Not Found | Recurso no existe | ID no encontrado |
| 409 Conflict | Duplicado | Código ya registrado |
| 500 Internal Server Error | Error no manejado | Excepción no esperada |

---

## Paso 6 — Formato estándar de errores

**Todos los endpoints deben usar este formato:**

```json
{
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "Los datos enviados no son válidos.",
    "details": [
      { "field": "codigo", "message": "El código no puede exceder 10 caracteres." },
      { "field": "precioCompra", "message": "Obligatorio cuando adquisicionPor es compra." }
    ]
  }
}
```

**Reglas:**
- `code` siempre en `UPPER_SNAKE_CASE`.
- `message` en lenguaje natural para el usuario.
- `details` lista de errores por campo (puede estar vacía).

### Catálogo de códigos de error comunes

| Code | HTTP | Descripción |
|------|------|-------------|
| `VALIDATION_ERROR` | 400 | Campo inválido o formato incorrecto |
| `UNAUTHORIZED` | 401 | Token ausente o expirado |
| `VACUNO_NOT_FOUND` | 404 | ID de vacuno no existe |
| `VACUNO_ALREADY_EXISTS` | 409 | Código duplicado |
| `INVALID_REPORT_FORMAT` | 400 | Formato diferente de json/pdf/excel |
| `INVALID_GENERATION_LEVEL` | 400 | Nivel genealógico fuera de rango |
| `INTERNAL_SERVER_ERROR` | 500 | Error interno no manejado |

---

## Paso 7 — Endpoints especiales: reportes y gráficos

```
GET /v1/vacunos/reporte
  Query: fechaDesde, fechaHasta, q, raza, procedencia, estado, formato (json|pdf|excel)
  Response: data[], downloadUrl (null si formato=json)

GET /v1/vacunos/{id}/reporte
  Query: formato
  Response: vacuno{}, historial[], downloadUrl

GET /v1/vacunos/{id}/genealogia
  Query: nivel (máx 4), formato
  Response: vacunoId, nivel, nodos[], relaciones[], downloadUrl

GET /v1/vacunos/actividad
  Query: fechaDesde, fechaHasta, granularidad (dia|semana|mes), formato
  Response: data[].fecha, data[].cantidadActivos, resumen.maximo, resumen.minimo, leyenda, downloadUrl
```

---

## Ejemplos — Implementación en C# (.NET)

### Request DTO

```csharp
namespace InterfaceAdapters.DTOs.Requests;

public class CreateVacunoRequest
{
    [Required]
    [MaxLength(10)]
    public string Codigo { get; set; } = default!;

    [Required]
    [MaxLength(15)]
    public string Nombre { get; set; } = default!;

    [Required]
    public DateTime FechaNacimiento { get; set; }

    [Required]
    public AdquisicionPor AdquisicionPor { get; set; }

    public decimal? PrecioCompra { get; set; }   // condicional

    public IFormFile? Foto { get; set; }          // opcional PNG/JPG

    [MaxLength(150)]
    public string? Observaciones { get; set; }
}
```

### Response DTO

```csharp
namespace InterfaceAdapters.DTOs.Responses;

public class VacunoListResponse
{
    public List<VacunoSummary> Data { get; set; } = new();
    public PaginationDto Pagination { get; set; } = new();
}

public class PaginationDto
{
    public int Page { get; set; }
    public int Limit { get; set; }
    public int Total { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)Total / Limit);
}
```

### Error estándar

```csharp
public class ApiError
{
    public ErrorDetail Error { get; set; } = new();
}

public class ErrorDetail
{
    public string Code { get; set; } = default!;
    public string Message { get; set; } = default!;
    public List<FieldError> Details { get; set; } = new();
}

public class FieldError
{
    public string Field { get; set; } = default!;
    public string Message { get; set; } = default!;
}
```

---

## Validaciones — Checklist por endpoint

- [ ] Todos los endpoints requieren Bearer Token JWT.
- [ ] Listados aplican filtro de últimos 30 días por defecto.
- [ ] Fechas en formato `YYYY-MM-DD`.
- [ ] Enums solo aceptan valores definidos en el contrato.
- [ ] Campos numéricos no aceptan letras.
- [ ] Campos de texto respetan longitud máxima.
- [ ] Archivos solo aceptan extensiones permitidas (PNG/JPG/JPEG).
- [ ] Campos condicionales validados según contexto (ej: `precioCompra` si `adquisicionPor=compra`).
- [ ] Paginación siempre presente en listados.
- [ ] Errores usan el formato estándar con `code`, `message` y `details`.
- [ ] Descargas de reporte solo permiten PDF y Excel (no json para download).

---

## Buenas Prácticas

- **Contract-first**: definir el contrato completo antes de escribir código.
- **Versionar la API**: usar `/v1/`, `/v2/` en la ruta base.
- **No exponer IDs internos** si no es necesario; preferir códigos de negocio en respuestas de listado.
- **Soft delete**: usar campo `estado` en lugar de eliminar físicamente.
- **PATCH en lugar de PUT** para actualizaciones parciales; documentar qué campos son inmutables.
- **Consistencia de nombres**: usar camelCase en JSON, snake_case en query params.
- **Nunca devolver 200 para errores**; usar el código HTTP correcto.

## Patrones Comunes

- **Filtro por rango de fechas con default**: siempre definir valor por defecto en el contrato.
- **Reporte dual (json + descarga)**: mismo endpoint, comportamiento diferente según `formato`.
- **Genealogía / árbol**: respuesta con `nodos[]` y `relaciones[]` para grafos jerárquicos.
- **Actividad temporal con granularidad**: `dia | semana | mes` para series de tiempo.
