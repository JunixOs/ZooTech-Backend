# Documentación: Actualización del Requisito "Reportes por Vacuno" (Migración al ORM)

Este documento detalla los cambios estructurales y actualizaciones aplicadas al módulo de Vacunos, específicamente relacionados con la generación de Listados Avanzados y el Reporte Individual (Perfil del Vacuno).

La principal motivación de esta actualización fue **unificar la capa de infraestructura**, abandonando las consultas en SQL puro (Dapper / `FromSqlRaw`) para depender estrictamente del ORM nativo (**Entity Framework Core**) haciendo uso de LINQ y Entidades de dominio.

---

## 1. Refactorización del Listado Avanzado de Vacunos

El archivo modificado fue `VacunoRepository.cs`, el cual es el responsable de proveer los datos para la grilla o tabla principal de vacunos con paginación y filtros complejos.

### Antes de la Actualización
- Se utilizaba un extenso bloque de SQL de más de 80 líneas que definía un `WITH filtered_vacunos AS (...)`.
- Se usaba la función de ventana `COUNT(*) OVER()` para calcular el total de registros en la misma consulta para la paginación.
- Los filtros se inyectaban condicionalmente en un `SqlParameter[]` y se resolvían mediante `FromSqlRaw`.

### Después de la Actualización (EF Core LINQ)
- Se eliminó completamente la cadena SQL estática.
- Se implementó un flujo `IQueryable<vacuno>` utilizando `_context.vacunos.AsNoTracking()`.
- Se estructuraron los filtros dinámicos (Código, Raza, Procedencia, Fecha, Estado y Tipo de Utilización) concatenando bloques `.Where(...)` de C# fuertemente tipados.
- **Relaciones (Joins):** En lugar de realizar `LEFT JOIN` manuales en SQL, ahora se utilizan los métodos de navegación propios de EF Core: `.Include(v => v.raza_codeNavigation)`, `.Include(v => v.vacuno_estado_historials)`, etc.
- **Paginación:** Se separó la obtención del total de registros (`.CountAsync()`) de la obtención de la data paginada utilizando `.Skip(offset).Take(limit)`.

---

## 2. Refactorización del Reporte Detallado de Vacuno (PDF / Perfil)

El archivo modificado fue `RegistroVacunoReadRepository.cs`, el cual es el responsable de extraer todos los datos de un vacuno, incluyendo información de granja, linaje (padres y abuelos), adquisiciones, estado actual y fotos para su impresión.

### Antes de la Actualización
- Se dependía de una gigantesca consulta SQL (más de 100 líneas) para extraer columnas específicas haciendo `LEFT JOIN` a 18 tablas diferentes simultáneamente.
- La consulta se inyectaba en la vista materializada `RegistroVacunoReporteRow` a través de `FromSqlRaw`.

### Después de la Actualización (EF Core LINQ)
- Todo el bloque SQL de 100 líneas fue eliminado.
- Se implementó una consulta encadenada utilizando el ORM, anidando los `.Include()` y `.ThenInclude()` correspondientes para extraer en un solo árbol de objetos todas las relaciones:
  - `sexo_codeNavigation`, `raza_codeNavigation`, `color_codeNavigation`
  - `padre` (y `padre.padre` para abuelos)
  - `madre` (y `madre.madre` para abuelas)
  - `granja` -> `distrito` -> `provincia` -> `departamento`
  - `vacuno_foto` -> `archivo`
  - `vacuno_estado_historials` y `vacuno_utilizacion_historials`
- **Extracción de Últimos Estados:** Las sub-consultas en SQL (`SELECT TOP(1)... ORDER BY id DESC`) fueron reemplazadas en memoria (LINQ-to-Objects sobre la entidad materializada) utilizando `v.vacuno_estado_historials.OrderByDescending(h => h.id).FirstOrDefault()`.

---

## 3. Beneficios de la Actualización

1. **Código Fuertemente Tipado:** Todo el acceso a datos ahora está respaldado por el compilador de C#, previniendo errores de tipeo que usualmente suceden al concatenar *strings* de SQL en crudo.
2. **Abstracción Total de la Base de Datos:** Ahora el backend no depende de sintaxis de un motor SQL específico (Ej: funciones específicas de SQL Server). EF Core se encargará de traducir el código LINQ al motor que esté configurado sin necesidad de modificar el repositorio.
3. **Mantenibilidad:** El código es notablemente más limpio, uniforme con el resto de la aplicación y mucho más sencillo de debugear.
