# Parametrización de valores variables

Este documento lista los valores del backend que pueden variar por ambiente, despliegue, tenant, reglas operativas o decisiones de negocio. La idea NO es parametrizar todo: lo que es invariante del dominio debe quedar en el dominio. Parametrizar sin criterio es deuda técnica con otro nombre.

> Nota de seguridad: se detectaron cadenas de conexión con credenciales en `appsettings*.json`. No se repiten acá por seguridad. Deben rotarse y moverse a Secret Manager, variables de entorno o Azure Key Vault.

## Prioridad alta

| Área | Valor actual / patrón | Ubicación | Por qué puede variar | Recomendación |
|---|---|---|---|---|
| Conexión a BD | `ConnectionStrings:DefaultConnection` con host, usuario y contraseña | `src/ZooTech.API/appsettings.json`, `src/ZooTech.API/appsettings.Development.json` | Cambia por ambiente y contiene secretos | Usar `ConnectionStrings__DefaultConnection` por variable de entorno, Secret Manager local y Key Vault en Azure. Rotar credenciales expuestas. |
| Hosts permitidos | `AllowedHosts: localhost` | `src/ZooTech.API/appsettings.json` | Cambia entre local, staging, producción y dominios internos | Configurar por ambiente. |
| URL de Kestrel | `http://localhost:16000` | `src/ZooTech.API/appsettings*.json` | Puerto/protocolo dependen del hosting | Preferir `ASPNETCORE_URLS` o config por ambiente. |
| CORS frontend | `FrontendProtocol`, `FrontendIP`, `FrontendPort` | `src/ZooTech.API/appsettings*.json`, `src/ZooTech.API/Program.cs` | El origen cambia por frontend local, staging, prod o CDN | Reemplazar por `Cors:AllowedOrigins` como array completo. Ej: `["http://localhost:4200", "https://app.zootech.com"]`. |
| Zona horaria | `SA Pacific Standard Time` | `src/ZooTech.Infrastructure/Common/Time/DateTimeProvider.cs` | Depende de país, tenant y sistema operativo | Mover a `TimeZone:Id`. Preferir UTC internamente y conversión por tenant/presentación. |
| Paginación | Defaults `1`, `10`, `20`; máximo `100` | `ListOrdeniosInteractor`, `GetAllTriajesInteractor`, `TriajeController`, `ProduccionLecheController` | Impacta UX, performance y SLA | Crear `PaginationOptions`: `DefaultPage`, `DefaultPageSize`, `MaxPageSize`. Unificar entre módulos. |
| Generación de códigos | `CELO-`, formato `yyyyMMddHHmmss`, fallback `yyyyMMddHHmmssfff`; `TRI` + `D3` | `CreateCeloInteractor`, `TriajeRepository` | Puede variar por tenant, módulo, secuencia legal o integración externa | Crear servicio/política de generación de códigos. Configurable por módulo o respaldado por secuencia en BD. |
| Estado inicial | `ACTIVO` | `CreateCeloInteractor` y otras entidades/catálogos | Puede variar por flujo de aprobación o tenant | Usar catálogo de estados + política explícita de estado inicial. Evitar string literal disperso. |

## Configuración de API y documentación

| Valor actual / patrón | Ubicación | Por qué puede variar | Recomendación |
|---|---|---|---|
| Swagger docs: `Authentication API`, `Users API`, `Public API`, `v1` | `src/ZooTech.API/Program.cs` | Nombre/versionado cambian por release o producto | Centralizar en constantes de metadata o `ApiDocumentationOptions`. |
| Swagger UI endpoints `/swagger/public/swagger.json`, etc. | `src/ZooTech.API/Program.cs` | Puede cambiar si se versiona por gateway o grupo | Centralizar junto con metadata Swagger. |
| Rutas base `api/v1/...` | Controllers en `src/ZooTech.InterfaceAdapters/Modules/*/Controllers` | Cambia con versionado API | Mantener como contrato de API; si cambia seguido, usar constantes o API Versioning. No hacerlo variable de ambiente salvo base path del gateway. |
| URLs `Created("/api/v1/.../{id}")` | `VacunoController`, `ProduccionLecheController` | Duplica rutas y se rompe si cambia el versionado | Usar `CreatedAtAction` o route names. |

## Reglas de negocio y validaciones

Estas reglas están en Application validators y Domain rules. Algunas son invariantes, otras son límites operativos o de esquema.

| Valor actual / patrón | Ubicación | Criterio | Recomendación |
|---|---|---|---|
| Longitud de códigos: `15`, `30`, `50` | Validators y `Domain/*/Rules` | Si reflejan columnas BD, son restricciones de esquema; si negocio las cambia, son política | Crear constantes de dominio/esquema por agregado para no duplicar valores entre Application y Domain. |
| Longitud de nombres: `100` | Vacuno validators/rules | Restricción de negocio/esquema | Constante compartida del dominio. |
| Observaciones: `150`, `500` | Validators/rules de Vacuno, Celo, Sanidad, Producción Leche | Varía por módulo; puede estar desalineado con BD | Definir límites por módulo en constantes de dominio. Parametrizar solo si el negocio realmente lo administra. |
| Motivo de eliminación: `200`, `500` | Validators/rules | Política de auditoría/UX | Unificar criterio. Constantes si es regla fija; Options si lo administra operación. |
| `GreaterThan(0)` para ids, litros, peso | Validators/rules | Invariante del dominio | NO parametrizar. Debe quedar en Domain. |
| Fecha no futura con `DateTime.UtcNow` | `CeloRule`, `OrdenioRule` | Puede necesitar tolerancia por reloj cliente/zona horaria | Pasar fecha de referencia desde Application o usar `IDateTimeProvider`. Si hay tolerancia, modelarla como política. |

## Tiempo y auditoría

| Valor actual / patrón | Ubicación | Riesgo | Recomendación |
|---|---|---|---|
| `DateTime.UtcNow` directo | Varios interactors de Application y reglas de Domain | Dificulta tests, zona horaria y consistencia | Usar `IDateTimeProvider` en Application. En Domain recibir `utcNow`/fecha de referencia como parámetro. |
| `ServerNow` convertido a zona Perú | `DateTimeProvider`, repositorios | Mezcla persistencia con zona local | Persistir UTC y convertir en bordes; si se necesita hora local, que venga de `TimeZoneOptions`. |

## Datos de catálogo y códigos

| Valor actual / patrón | Ubicación | Por qué puede variar | Recomendación |
|---|---|---|---|
| Códigos de estado como `ACTIVO` | Application/Domain/Infra según módulo | Catálogos pueden variar por tenant o configuración inicial | Mantener valores en tablas catálogo y referenciarlos con constantes si son códigos del sistema. |
| Tablas `cat_*` | `GanaderiaDbContext`, entidades EF | Son contrato de persistencia, no config runtime | No parametrizar por ambiente. Cambian por migración/schema. |
| Prefijos `TRI`, `CELO-` | Repositorios/use cases | Numeración puede variar por módulo/tenant | Mover a política de código o catálogo de numeradores. |

## Respuestas, mensajes y placeholders

| Valor actual / patrón | Ubicación | Problema | Recomendación |
|---|---|---|---|
| Mensajes como `Swagger funcionando correctamente`, `Api funcionando...` | `HomeController`, `ProduccionLecheController` | Texto público variable y poco machine-readable | Usar HealthChecks estándar o constantes de presentación. |
| `diasRestante = 0`, `crias = 0`, `estado = "En celo"` | `CeloController.GetVacasEnCelo` | Son placeholders de negocio, no parametrización | Calcular desde datos reales o devolver explícitamente `null`/campo omitido hasta implementarlo. |
| Mensajes de error en español | Exceptions, validators, controllers | Puede requerir localización | Mantener códigos de error estables y mensajes localizables si habrá multi-idioma. |

## Infraestructura, scripts y pipeline

| Valor actual / patrón | Ubicación | Por qué puede variar | Recomendación |
|---|---|---|---|
| Framework `net10.0` y versiones de paquetes | `*.csproj` | Cambia con estrategia de upgrade | Centralizar en `Directory.Build.props` y `Directory.Packages.props`. |
| Scripts con rutas y ramas/personas hardcodeadas | `scripts/modificate-project.ps1`, `scripts/generate-branchs.ps1` | Cambian por equipo, estructura y convención Git | Parametrizar rutas, nombres y ramas; usar archivo de entrada. |
| Comandos de coverage/build/test | Pipeline/Azure DevOps y docs | Herramientas y thresholds cambian | Documentar thresholds en CI. Si hay mínimo de cobertura, configurarlo explícitamente en pipeline. |

## Qué NO conviene parametrizar

| Valor | Motivo |
|---|---|
| `id > 0`, `litros > 0`, `pesoKg > 0` | Son invariantes del dominio. Si se vuelven configurables, el modelo pierde sentido. |
| HTTP status `200`, `201`, `204`, `400`, `404`, `409` | Son contrato HTTP/API, no configuración. |
| `application/json` | Contrato de transporte. |
| `deleted_at == null` para soft delete | Convención estructural del modelo de persistencia. |
| Nombres de tablas/columnas EF scaffold | Contrato con el schema. Cambian con migraciones, no por appsettings. |
| Datos fixture en tests | Son ejemplos de prueba; parametrizarlos haría los tests más frágiles, no más flexibles. |

## Propuesta de estructura de configuración

```json
{
  "Cors": {
    "AllowedOrigins": ["http://localhost:4200"]
  },
  "Pagination": {
    "DefaultPage": 1,
    "DefaultPageSize": 20,
    "MaxPageSize": 100
  },
  "TimeZone": {
    "Id": "SA Pacific Standard Time"
  },
  "CodeGeneration": {
    "Celo": {
      "Prefix": "CELO-",
      "TimestampFormat": "yyyyMMddHHmmss"
    },
    "Triaje": {
      "Prefix": "TRI",
      "NumericFormat": "D3"
    }
  },
  "ApiDocumentation": {
    "Version": "v1",
    "Groups": {
      "Public": "Public API",
      "Auth": "Authentication API",
      "Users": "Users API"
    }
  }
}
```

## Orden recomendado de implementación

1. Sacar y rotar secretos de `appsettings*.json`.
2. Implementar `CorsOptions` con `AllowedOrigins`.
3. Centralizar paginación en `PaginationOptions`.
4. Usar `IDateTimeProvider` consistentemente en Application y parametrizar zona horaria.
5. Extraer generación de códigos a un servicio/política por módulo.
6. Consolidar límites de validación en constantes de dominio para eliminar duplicación.
7. Reemplazar placeholders de `CeloController` por datos reales o respuesta explícitamente parcial.
