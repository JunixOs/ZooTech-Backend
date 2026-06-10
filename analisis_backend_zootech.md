## **Cuadro comparativo general** 

Leyenda: Verde = correcto | Naranja = inconsistente | Rojo = problema grave | Gris = no aplica 

|**Criterio**|**Sanidad**|**Vacuno**|**Celo**|**Prod. Leche**|
|---|---|---|---|---|
|**Arquitectura general**|||||
|**Patrón usado**|Clean parcial|Dual / sin<br>patrón|Mezcla de 2 devs|Clean + Ports|
|**Carpetas por módulo**|Sí|No|No|Sí|
|**DependencyInjection**|Compartido|Compartido|Compartido|Compartido|
|**Capa Domain**|||||
|**Entidades**|Sí|Vacío|Vacío|Rich Domain<br>Model|
|**Reglas de negocio**|No|No|No|OrdenioRule|
|**Interfaces de repo**|En Domain|En API|En<br>Common.Gateway|En<br>Application/Ports|
|**Capa Application**|||||
|**UseCases**|Sin interfaz|Solo reportes|Parcial, sin lógica|Completos +<br>InputPort|
|**DTOs ubicación**|En Application|En API / un<br>archivo|Dentro del<br>UseCase|En<br>InterfaceAdapters|
|**Manejo de errores**|Sin excepciones|Parcial|Sin excepciones|Excepciones<br>tipadas|
|**CancellationToken**|No|Parcial|Parcial|Sí|
|**Capa Infrastructure**|||||
|**ToDomain / ToEntity**|No|No|No|Sí|
|**Repositorios**|1 repo correcto|2 repos +<br>InMemory|2 repos, 1<br>huérfano|1 repo, sealed|
|**Carpeta por módulo**|Sí|No|No|Sí|
|**Capa InterfaceAdapters**|||||
|**Controllers**|1 controller|Sin controller<br>(Minimal API)|2 controllers, ruta<br>duplicada|2 controllers, 1<br>fuera de lugar|
|**Rutas HTTP**|api/v1/...|Mezcla de rutas|Ruta duplicada|Sin prefijo api/|
|**Mappers**|No|Sí|No|Sí|
|**Respuesta estándar**|Parcial|Parcial|Anónima / mixta|Objeto propio sin<br>estándar|



|**Criterio**|**Sanidad**|**Vacuno**|**Celo**|**Prod. Leche**|
|---|---|---|---|---|
|**Calidad de código**|||||
|**sealed / record**|No|No|No|Sí|
|**Datos hardcodeados**|No|No aplica|No|Sí (fechas y<br>promedios)|
|**Nivel de madurez**|Medio|Bajo|Bajo|Alto|



## **Módulo Sanidad** 

## **Estructura de archivos** 

|**Capa**|**Archivos**|
|---|---|
|**Domain**|Entities (Triaje, TipoPeso, VacunoOption, TriajeHistorialItem), Interfaces<br>(ITriajeRepository)|
|**Application**|DTOs (Responses y Requests), UseCases (GetAll, GetById, Create, Update,<br>Delete, GetAllTiposPesos, GetAllVacunos, GetHistorialByVacunoId)|
|**Infrastructure**|Repositories (TriajeRepository implementa ITriajeRepository),<br>Persistence/Entities (EF generado)|
|**InterfaceAdapters**|Controllers (TriajeController)|



## **Diagrama de dependencias** 

|**API**|→ Application, → Infrastructure, → InterfaceAdapters|
|---|---|
|**InterfaceAdapters**|→ Application (incluyendo DTOs)<br>VIOLACIÓN<br>⚠|
|**Infrastructure**|→ Application, → Domain (sin mapeo limpio)<br>VIOLACIÓN<br>⚠|
|**Application**|→ Domain|
|**Domain**|(sin dependencias) — núcleo puro|



## **Deuda técnica** 

• Sin interfaces en UseCases • Controller acoplado a DTOs de Application • Sin CancellationToken • Sin manejo de errores completo • Sin ToDomain/ToEntity en Repository • DependencyInjection compartido entre módulos 

## **Módulo Vacuno Estructura de archivos** 

|**Capa**|**Archivos**|
|---|---|
|**API**|Endpoints (VacunoEndpoints.cs) Minimal APIs CRUD, Models (todos los DTOs<br>en un archivo), Services (IVacunoRepository, InMemoryVacunoRepository,<br>EfVacunoRepository), Configuration (VacunoRequirementOptions.cs), Security<br>(DemoAuthEndpointFilter.cs)|
|**Domain**|Vacío|
|**Application**|Common (ReporteVacunoDateRangeResolver.cs), UseCases<br>(ListarReportesDisponibles, ListarReporteVacunos,<br>ObtenerRegistroVacunoReporte), DependencyInjection|
|**Infrastructure**|ReadModels (RegistroVacunoReporteRow, ReporteVacunoListadoRow),<br>Repositories (RegistroVacunoReadRepository con Raw SQL,<br>ReporteVacunoReadRepository con LINQ con EF), DependencyInjection — las<br>carpetas no respetan estructura de módulos|
|**InterfaceAdapters**|ExceptionHandlingMiddleware, Controllers (RegistroVacunosReporteController,<br>ReportesVacunosController), DTOs (Requests y Responses), Mappers|



## **Diagrama de dependencias** 

|**API**|→ Application, → Infrastructure, → InterfaceAdapters|
|---|---|
|**InterfaceAdapters**|→ Application|
|**Infrastructure**|→ Application, → Domain|
|**Application**|→ Domain|
|**Domain**|(sin dependencias) — núcleo puro|



## **Deuda técnica** 

• Dualidad arquitectónica: Clean Architecture para reportes, sin patrón para el CRUD • Domain vacío — sin entidades ni reglas de negocio • Repositorios ubicados en la capa API en lugar de Infrastructure • Lógica de negocio en Endpoints sin usar controllers ni UseCases • Filtrado en memoria (revisar impacto en rendimiento) • Carpetas de ReadModels y repositorios no respetan el formato de módulo por carpeta • Todos los modelos (DTOs) definidos en un solo archivo • DTOs sin tipos correctos • DependencyInjection compartido entre módulos 

• Dualidad arquitectónica: Clean Architecture para reportes, sin patrón para el CRUD • Domain vacío — sin entidades ni reglas de negocio • Repositorios ubicados en la capa API en lugar de Infrastructure • Lógica de negocio en Endpoints sin usar controllers ni UseCases • Filtrado en memoria (revisar impacto en rendimiento) • Carpetas de ReadModels y repositorios no respetan el formato de módulo por carpeta • Todos los modelos (DTOs) definidos en un solo archivo • DTOs sin tipos correctos • DependencyInjection compartido entre módulos 

• Dualidad arquitectónica: Clean Architecture para reportes, sin patrón para el CRUD • Domain vacío — sin entidades ni reglas de negocio • Repositorios ubicados en la capa API en lugar de Infrastructure • Lógica de negocio en Endpoints sin usar controllers ni UseCases • Filtrado en memoria (revisar impacto en rendimiento) • Carpetas de ReadModels y repositorios no respetan el formato de módulo por carpeta • Todos los modelos (DTOs) definidos en un solo archivo • DTOs sin tipos correctos • DependencyInjection compartido entre módulos 

• Dualidad arquitectónica: Clean Architecture para reportes, sin patrón para el CRUD • Domain vacío — sin entidades ni reglas de negocio • Repositorios ubicados en la capa API en lugar de Infrastructure • Lógica de negocio en Endpoints sin usar controllers ni UseCases • Filtrado en memoria (revisar impacto en rendimiento) • Carpetas de ReadModels y repositorios no respetan el formato de módulo por carpeta • Todos los modelos (DTOs) definidos en un solo archivo • DTOs sin tipos correctos • DependencyInjection compartido entre módulos 

• Dualidad arquitectónica: Clean Architecture para reportes, sin patrón para el CRUD • Domain vacío — sin entidades ni reglas de negocio • Repositorios ubicados en la capa API en lugar de Infrastructure • Lógica de negocio en Endpoints sin usar controllers ni UseCases • Filtrado en memoria (revisar impacto en rendimiento) • Carpetas de ReadModels y repositorios no respetan el formato de módulo por carpeta • Todos los modelos (DTOs) definidos en un solo archivo • DTOs sin tipos correctos • DependencyInjection compartido entre módulos 

## **Módulo Celo Estructura de archivos** 

|**Capa**|**Archivos**|
|---|---|
|**Domain**|Vacío|
|**Application**|Las carpetas no respetan la estructura de módulos. Tiene dos carpetas:<br>DTOs_Reproduccion (EditarCeloDTO, RegistrarCeloDTO) y<br>Module_Celo/UseCases/ListarCelos (IListarCelosUseCase,<br>ListarCelosUseCase, CeloListItemDto). DependencyInjection.cs|
|**Infrastructure**|Las carpetas no respetan la estructura de módulos. Repositories<br>(CeloRepository — 2 repositorios: uno registrado en DI, uno huérfano)|
|**InterfaceAdapters**|Dos carpetas: Module_Celo (GET con estructura limpia) y<br>Module_ReproduccionCelo (PUT/POST sin estructura). DependencyInjection|



## **Diagrama de dependencias** 

|**API**|→ Application, → Infrastructure, → InterfaceAdapters|
|---|---|
|**InterfaceAdapters**|→ Application, → Domain<br>REVISAR, → Infrastructure<br>REVISAR<br>⚠<br>⚠<br>(controller inyecta repo directamente)|
|**Infrastructure**|→ Application, → Domain|
|**Application**|→ Domain|
|**Domain**|(sin dependencias)|



## **Deuda técnica** 

• Domain completamente vacío — sin entidad Celo ni ICeloRepository en Domain 

• Dos controllers con la misma ruta api/v1/celo — conflicto de rutas en runtime 

• Controller del módulo ReproduccionCelo salta la capa Application e inyecta el repositorio directamente 

• EditarCeloUseCase sin interfaz y sin registro en DI — endpoint de edición inaccesible 

• DTOs fuera del módulo correspondiente (DTOs_Reproduccion en Application) 

• UseCases sin lógica de negocio — solo delegan al repositorio 

• Dos CeloRepository coexisten: uno registrado en DI, otro huérfano con lógica diferente 

• ICeloRepository ubicado en Common.Gateway.Repositories — patrón inventado por un solo desarrollador 

• ICeloRepository depende de CeloListItemDto de un UseCase específico — acoplamiento inverso 

• Sin ToDomain / ToEntity en ningún repositorio de Celo 

• Respuestas anónimas sin tipo (new { mensaje = ... }) 

• DependencyInjection compartido entre módulos 

• Clases sin sealed ni record 

• EditarCeloUseCase con demasiados parámetros en lugar de un objeto Command 

## **Módulo Producción de Leche Estructura de archivos** 

|**Capa**|**Archivos**|
|---|---|
|**Domain**|Entities (Ordenio) — constructor privado, CreateNew, Rehydrate, Update,<br>SoftDelete. Rules (OrdenioRule) — validaciones de negocio|
|**Application**|UseCases/Ordenios/Common (OrdenioMapper, OrdenioOutput,<br>OrdenioReferenceValidator). UseCases por operación: CreateOrdenio,<br>DeleteOrdenio, GetOrdenioById, ListOrdenios, UpdateOrdenio — cada uno con<br>Command, Interactor, Output e InputPort. Ports (IOrdenioRepository).<br>DependencyInjection|
|**Infrastructure**|Modules/Module_ProduccionLeche/Repositories (OrdenioRepository<br>implementa IOrdenioRepository) con ToDomain/ToEntity. DependencyInjection|
|**InterfaceAdapters**|Module_ProduccionLeche — Controllers (ProduccionLecheController,<br>HomeController), DTOs (Requests y Responses), Mappers<br>(ProduccionLecheMapper)|



## **Diagrama de dependencias** 

|**API**|→ Application, → Infrastructure, → InterfaceAdapters|
|---|---|
|**InterfaceAdapters**|→ Application, → Infrastructure<br>VIOLACIÓN (GetVacunos bypasea<br>⚠<br>Application)|



|**Infrastructure**|→ Application, → Domain|
|---|---|
|**Application (interna)**|→ Domain. Ports → UseCases<br>VIOLACIÓN (dependencia circular<br>⚠<br>interna por ListOrdeniosQuery)|
|**Domain**|(sin dependencias) — núcleo puro|



## **Deuda técnica** 

- Rehydrate valida reglas de negocio igual que CreateNew — puede romper datos históricos que violarían una regla nueva 

- GetVacunos bypasea Application — controller inyecta IOrdenioRepository directamente con [FromServices] 

- Datos hardcodeados en producción: ultimoRegistro = '2023-10-01' y promedio = '15L' en el controller 

- HomeController dentro de la carpeta del módulo — endpoint genérico que no pertenece a un módulo de negocio 

- Ruta sin prefijo api/ — v1/produccion-leche es inconsistente con api/v1/... de los demás módulos 

- IOrdenioRepository acoplado a ListOrdeniosQuery — la interfaz del repositorio depende de un UseCase concreto 

- VacunoSimpleOutput definido dentro de IOrdenioRepository — un DTO de salida no debería vivir en la interfaz 

- Dos estilos de inyección mezclados — constructor vacío y [FromServices] en parámetros de acción 

- OrdenioRule no es clase estática — tiene constructor público vacío innecesario 

- DependencyInjection compartido entre módulos 

## **Módulo de referencia** 

Producción de Leche debe ser el estándar a seguir para estandarizar los demás módulos. Sus patrones correctos son: 

- Rich Domain Model con constructor privado y factory methods (CreateNew, Rehydrate) 

- Reglas de negocio en clases de Domain (OrdenioRule) 

- UseCases con interfaz InputPort e implementación Interactor 

- IOrdenioRepository en Application/Ports 

- OrdenioRepository con ToDomain/ToEntity en Infrastructure 

- DTOs y Mappers en InterfaceAdapters 

- Excepciones tipadas (NotFoundException, ConflictException) 

- CancellationToken en todos los métodos asíncronos 

## **Problemas transversales a resolver primero** 

- Unificar el DependencyInjection — cada módulo debe tener su propio archivo de registro 

- Estandarizar el prefijo de rutas HTTP — todos los módulos deben usar api/v1/... 

- Definir y adoptar un único GeneralResponseDTO tipado y público para todas las respuestas 

- Eliminar el CeloRepository huérfano y resolver el conflicto de rutas duplicadas en Celo 

- Habilitar MediatR y FluentValidation que están comentados pero nunca implementados 

## **Prioridad de refactorización** 

|**Prioridad**|**Módulo**|**Acción**|
|---|---|---|
|**Alta**|Celo|Eliminar repositorio huérfano, resolver conflicto de rutas,<br>crear entidad en Domain|
|**Alta**|Vacuno|Mover repositorios de API a Infrastructure, crear<br>entidades en Domain|
|**Media**|Sanidad|Agregar interfaces a UseCases, agregar<br>ToDomain/ToEntity, agregar CancellationToken|
|**Media**|Prod. Leche|Eliminar datos hardcodeados, mover HomeController,<br>corregir GetVacunos|
|**Baja**|Todos|Unificar DependencyInjection, estandarizar rutas y<br>respuestas HTTP|



