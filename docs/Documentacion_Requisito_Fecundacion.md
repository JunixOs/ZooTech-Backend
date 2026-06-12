# Documentación: Implementación del Requisito "Registrar Fecundación"

Este documento resume la implementación técnica y el ciclo de desarrollo completo del requerimiento "Registrar Fecundación", perteneciente al módulo de Reproducción del sistema ZooTech. La implementación abarca desde la estructuración de la base de datos hasta el desarrollo de la interfaz de usuario.

---

## 1. Arquitectura y Backend (Clean Architecture)

El backend se construyó siguiendo estrictamente los principios de **Arquitectura Limpia (Clean Architecture)**, separando las responsabilidades en distintas capas:

### Capa de Dominio (Domain)
- Se definió la entidad principal `Fecundacion` y la lógica de negocio para la generación del registro.
- Se implementaron métodos factoría (`CreateNew`) para garantizar que la entidad siempre nazca en un estado válido.
- Se definieron las interfaces de los repositorios `IFecundacionRepository` y `IVacunoReproduccionRepository`.

### Capa de Aplicación (Use Cases)
- **Caso de Uso:** `RegistrarFecundacionUseCase` (usando el patrón *Interactor*).
- **Validaciones (FluentValidation):** Se validó la obligatoriedad y el formato de los campos de entrada (`RegistrarFecundacionCommand`).
- **Lógica de Negocio Centralizada:** Se validaron las siguientes reglas antes de persistir los datos:
  - El vacuno receptor debe **existir**.
  - El vacuno receptor debe ser **hembra**.
  - El vacuno receptor debe estar **vivo** (estado ACTIVO).
  - El vacuno receptor **no debe tener una fecundación pendiente**.
  - Si se indica un macho interno (donante), se verifica su existencia en la base de datos.

### Capa de Infraestructura (Persistence)
- Se implementaron los repositorios definidos en el dominio utilizando **Entity Framework Core**.
- **Refactorización Completa al ORM:** Como parte del requerimiento, se eliminó el uso de consultas SQL puras (`FromSqlRaw` y el patrón CQRS inicial con Dapper) a lo largo de toda la infraestructura. Todas las lecturas y escrituras, incluyendo los reportes complejos de vacunos, ahora se realizan estrictamente a través de las Entidades y LINQ, garantizando una comunicación fluida y unificada a la base de datos.
- Generación asíncrona y transaccional del código correlativo para la fecundación (Ej. `FEC-2026-0001`).

### Capa de Presentación (API)
- Exposición de un endpoint limpio `POST /api/v1/fecundaciones` dentro del controlador `FecundacionController`.

---

## 2. Pruebas Unitarias (xUnit y Moq)

Se garantizó la fiabilidad de la lógica de negocio del backend desarrollando pruebas unitarias exhaustivas para el `RegistrarFecundacionInteractor`. 
Se probaron todos los flujos excepcionales simulando la base de datos con `Moq`:
- `HandleAsync_ValidCommand_ReturnsOutputAndCallsAdd`: Flujo exitoso completo.
- Validación de excepciones tipo `ConflictException` para hembras dadas de baja, vacunos machos que intentan ser registrados como receptores o vacunos con fecundación pendiente.
- Validación de excepciones `NotFoundException` para ID's inexistentes.

---

## 3. Frontend (Angular)

El cliente web se desarrolló para reflejar el diseño solicitado utilizando las prácticas del framework.

### Diseño y Vistas
- **Dashboard (Contenedor):** Se crearon *cards* de navegación al estilo Material (replicando la interfaz de reportes de vacunos) para organizar los submódulos de Reproducción.
- **Formulario Reactivo Dinámico:** Desarrollo del componente `RegistroFecundacionComponent` usando `ReactiveFormsModule`.
  - **Campos condicionales:** El formulario revela inputs específicos de manera reactiva (Ej: `Código de Semen` si es Inseminación Artificial).
  - **Autocompletado Inteligente (RxJS):** Se conectó con el API de vacunos para autocompletar machos y hembras en tiempo real.
  - **Check Macho Externo:** Lógica implementada que desactiva el autocompletado y habilita un campo de texto libre.

### Internacionalización (i18n con ngx-translate)
- Cumplimiento estricto del estándar del proyecto documentado en `i18n.md`.
- El módulo entero ("Reproducción") y el formulario de "Registrar Fecundación" extraen todos sus textos estáticos, placeholders, dropdowns y notificaciones de éxito/error de los archivos `.json` de traducción (`es.json`, `en.json`, `pt.json`) empleando `TranslatePipe` y `TranslateService`.

### Optimización del Build
- Para que la aplicación pudiera compilar soportando los nuevos estilos modulares, se incrementaron y ajustaron correctamente las tolerancias y presupuestos (budgets) de CSS dentro de `angular.json` para evitar el error de "bundle exceeded limits".

---
**Estado Final:** Módulo entregado, compilando sin errores, documentado, con soporte multi-idioma (i18n), interfaz responsiva reactiva y backend robusto respaldado por el ORM y Pruebas Unitarias.
