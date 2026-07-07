## Parametros del modulo Sanidad

Formato: `N | PARAMETRO | VALOR_ACTUAL | FUENTE`

## API y rutas

| N | PARAMETRO | VALOR_ACTUAL | FUENTE |
| --- | --- | --- | --- |
| 1 | `SANIDAD__BASE_ROUTE` | `api/v1/triaje` | `TriajeController.cs:20` |
| 2 | `SANIDAD__GET_ALL_ROUTE` | `` (raíz) | `TriajeController.cs:52` |
| 3 | `SANIDAD__GET_BY_ID_ROUTE` | `{id:long}` | `TriajeController.cs:69` |
| 4 | `SANIDAD__CREATE_ROUTE` | `` (raíz) | `TriajeController.cs:78` |
| 5 | `SANIDAD__UPDATE_ROUTE` | `{id:long}` | `TriajeController.cs:89` |
| 6 | `SANIDAD__DELETE_ROUTE` | `{id:long}` | `TriajeController.cs:100` |
| 7 | `SANIDAD__TIPOS_PESO_ROUTE` | `tipos-peso` | `TriajeController.cs:110` |
| 8 | `SANIDAD__VACUNOS_ROUTE` | `vacunos` | `TriajeController.cs:118` |
| 9 | `SANIDAD__HISTORIAL_ROUTE` | `historial/{vacunoId:long}` | `TriajeController.cs:126` |
| 10 | `SANIDAD__CREATED_ROUTE_TEMPLATE` | `/api/v1/triaje/{id}` | `TriajeController.cs:85` |
| 11 | `SANIDAD__CONTROLLER_NAME` | `TriajeController` | `TriajeController.cs:21` |
| 12 | `SANIDAD__CONTROLLER_NAMESPACE` | `ZooTech.InterfaceAdapters.Module_Sanidad.Controllers` | `TriajeController.cs:17` |
| 13 | `SANIDAD__RESPONSE_WRAPPER_TYPE` | `GeneralResponseDTO<T>` | `TriajeController.cs:53` |

## Listado y paginacion

| N | PARAMETRO | VALOR_ACTUAL | FUENTE |
| --- | --- | --- | --- |
| 14 | `SANIDAD__LIST__DEFAULT_PAGE` | `1` | `TriajeController.cs:55` |
| 15 | `SANIDAD__LIST__DEFAULT_PAGE_SIZE` | `10` | `TriajeController.cs:56` |
| 16 | `SANIDAD__LIST__MAX_PAGE_SIZE` | `100` | `GetAllTriajesInteractor.cs:820` |
| 17 | `SANIDAD__LIST__DEFAULT_ORDER` | `fecha_hora DESC` | `TriajeRepository.cs:1543` |

## Filtros de listado

| N | PARAMETRO | VALOR_ACTUAL | FUENTE |
| --- | --- | --- | --- |
| 18 | `SANIDAD__FILTER__FECHA_PARAM` | `fecha` | `TriajeController.cs:57` |
| 19 | `SANIDAD__FILTER__CODIGO_PARAM` | `codigo` | `TriajeController.cs:58` |
| 20 | `SANIDAD__FILTER__NOMBRE_PARAM` | `nombre` | `TriajeController.cs:59` |
| 21 | `SANIDAD__FILTER__TIPO_PESO_PARAM` | `tipoPeso` | `TriajeController.cs:60` |
| 22 | `SANIDAD__FILTER__PESO_KG_PARAM` | `pesoKg` | `TriajeController.cs:61` |
| 23 | `SANIDAD__FILTER__CODIGO_OPERATOR` | `Contains` | `TriajeRepository.cs:1522` |
| 24 | `SANIDAD__FILTER__NOMBRE_OPERATOR` | `Contains` (sobre vacuno.nombre) | `TriajeRepository.cs:1525` |
| 25 | `SANIDAD__FILTER__FECHA_OPERATOR` | `>= desde && < hasta` (rango diario) | `TriajeRepository.cs:1528-1531` |
| 26 | `SANIDAD__FILTER__TIPO_PESO_OPERATOR` | `==` (exacto) | `TriajeRepository.cs:1535` |
| 27 | `SANIDAD__FILTER__PESO_KG_OPERATOR` | `==` (exacto) | `TriajeRepository.cs:1538` |

## Reportes (descarga)

| N | PARAMETRO | VALOR_ACTUAL | FUENTE |
| --- | --- | --- | --- |
| 28 | `SANIDAD__REPORT__DOWNLOAD_TAMANO` | `int.MaxValue` | `DownloadReporteTriajesUseCase.cs:923` |
| 29 | `SANIDAD__REPORT__ALLOWED_FORMATS` | `xlsx`, `excel`, `pdf` | `TriajeReporteFileService.cs:1149-1156` |
| 30 | `SANIDAD__REPORT__TIMESTAMP_FORMAT` | `yyyyMMddHHmmss` | `TriajeReporteFileService.cs:1145` |
| 31 | `SANIDAD__REPORT__EXCEL_CONTENT_TYPE` | `application/vnd.openxmlformats-officedocument.spreadsheetml.sheet` | `TriajeReporteFileService.cs:1138` |
| 32 | `SANIDAD__REPORT__PDF_CONTENT_TYPE` | `application/pdf` | `TriajeReporteFileService.cs:1139` |
| 33 | `SANIDAD__REPORT__EXCEL_FILENAME_PREFIX` | `reporte-triajes` | `TriajeReporteFileService.cs:1153` |
| 34 | `SANIDAD__REPORT__PDF_FILENAME_PREFIX` | `reporte-triajes` | `TriajeReporteFileService.cs:1160` |
| 35 | `SANIDAD__REPORT__EXCEL_MESSAGE` | `Descarga de reporte de triajes en Excel generada correctamente.` | `TriajeReporteFileService.cs:1154` |
| 36 | `SANIDAD__REPORT__PDF_MESSAGE` | `Descarga de reporte de triajes en PDF generada correctamente.` | `TriajeReporteFileService.cs:1161` |
| 37 | `SANIDAD__REPORT__ERROR_FORMAT` | `Formato no soportado. Use 'xlsx' o 'pdf'.` | `TriajeReporteFileService.cs:1163` |
| 38 | `SANIDAD__REPORT__SHEET_NAME` | `Triajes` | `TriajeReporteFileService.cs:1191` |

## Reporte Excel — Columnas y encabezados

| N | PARAMETRO | VALOR_ACTUAL | FUENTE |
| --- | --- | --- | --- |
| 39 | `SANIDAD__EXCEL__HEADER__C_REGISTRO` | `C.Registro` | `TriajeReporteFileService.cs:1218` |
| 40 | `SANIDAD__EXCEL__HEADER__FECHA` | `Fecha` | `TriajeReporteFileService.cs:1218` |
| 41 | `SANIDAD__EXCEL__HEADER__HORA` | `Hora` | `TriajeReporteFileService.cs:1218` |
| 42 | `SANIDAD__EXCEL__HEADER__VACUNO` | `Vacuno` | `TriajeReporteFileService.cs:1218` |
| 43 | `SANIDAD__EXCEL__HEADER__TIPO_PESO_MEDIDO` | `Tipo de peso medido` | `TriajeReporteFileService.cs:1218` |
| 44 | `SANIDAD__EXCEL__HEADER__PESO_KG` | `Peso (Kg)` | `TriajeReporteFileService.cs:1218` |
| 45 | `SANIDAD__EXCEL__HEADER__OBSERVACIONES` | `Observaciones` | `TriajeReporteFileService.cs:1218` |
| 46 | `SANIDAD__EXCEL__COLUMN_COUNT` | `7` | `TriajeReporteFileService.cs:1216-1219` |
| 47 | `SANIDAD__EXCEL__DATE_FORMAT` | `yyyy-MM-dd` | `TriajeReporteFileService.cs:1227` |
| 48 | `SANIDAD__EXCEL__HOUR_FORMAT` | `HH:mm:ss` | `TriajeReporteFileService.cs:1228` |

## Reporte PDF — Formato y diseño

| N | PARAMETRO | VALOR_ACTUAL | FUENTE |
| --- | --- | --- | --- |
| 49 | `SANIDAD__PDF__TITLE` | `Reporte de triajes` | `TriajeReporteFileService.cs:1282` |
| 50 | `SANIDAD__PDF__GENERATED_AT_LABEL` | `Generado` | `TriajeReporteFileService.cs:1283` |
| 51 | `SANIDAD__PDF__DATE_FORMAT` | `yyyy-MM-dd HH:mm:ss` | `TriajeReporteFileService.cs:1283` |
| 52 | `SANIDAD__PDF__TABLE_HEADER` | `C.Registro \| Fecha \| Hora \| Vacuno \| Tipo peso \| Peso (Kg) \| Observaciones` | `TriajeReporteFileService.cs:1285` |
| 53 | `SANIDAD__PDF__FONT` | `Courier` | `TriajeReporteFileService.cs:1333` |
| 54 | `SANIDAD__PDF__FONT_SIZE` | `9` | `TriajeReporteFileService.cs:1303` |
| 55 | `SANIDAD__PDF__START_X` | `40` | `TriajeReporteFileService.cs:1304` |
| 56 | `SANIDAD__PDF__START_Y` | `800` | `TriajeReporteFileService.cs:1304` |
| 57 | `SANIDAD__PDF__LINE_HEIGHT` | `18` | `TriajeReporteFileService.cs:1309` |
| 58 | `SANIDAD__PDF__LINES_PER_PAGE` | `38` | `TriajeReporteFileService.cs:1292` |
| 59 | `SANIDAD__PDF__PAGE_WIDTH` | `842` | `TriajeReporteFileService.cs:1333` |
| 60 | `SANIDAD__PDF__PAGE_HEIGHT` | `595` | `TriajeReporteFileService.cs:1333` |
| 61 | `SANIDAD__PDF__PDF_VERSION` | `1.4` | `TriajeReporteFileService.cs:1323` |
| 62 | `SANIDAD__PDF__TRIM__VACUNO_NOMBRE` | `18` | `TriajeReporteFileService.cs:1289` |
| 63 | `SANIDAD__PDF__TRIM__TIPO_PESO_CODE` | `18` | `TriajeReporteFileService.cs:1289` |
| 64 | `SANIDAD__PDF__TRIM__OBSERVACIONES` | `30` | `TriajeReporteFileService.cs:1289` |

## Generación de código

| N | PARAMETRO | VALOR_ACTUAL | FUENTE |
| --- | --- | --- | --- |
| 65 | `SANIDAD__CODIGO__PREFIX` | `TRI` | `TriajeRepository.cs:1594` |
| 66 | `SANIDAD__CODIGO__FORMAT` | `TRI{D3}` (ej: `TRI001`, `TRI002`) | `TriajeRepository.cs:1604` |
| 67 | `SANIDAD__CODIGO__MIN_NUMBER` | `1` | `TriajeRepository.cs:1594-1604` |

## Reglas del dominio

| N | PARAMETRO | VALOR_ACTUAL | FUENTE |
| --- | --- | --- | --- |
| 68 | `SANIDAD__RULES__VACUNOID_MIN` | `> 0` | `TriajeRule.cs:7` |
| 69 | `SANIDAD__RULES__TIPO_PESO_CODE_REQUIRED` | `NotEmpty` | `TriajeRule.cs:13` |
| 70 | `SANIDAD__RULES__PESO_KG_MIN` | `> 0` | `TriajeRule.cs:19` |
| 71 | `SANIDAD__RULES__FECHAHORA_REQUIRED` | `!= default` | `TriajeRule.cs:26` |
| 72 | `SANIDAD__RULES__FECHAHORA_MAX` | `<= DateTime.UtcNow` (no futura) | `TriajeRule.cs:32` |
| 73 | `SANIDAD__RULES__MOTIVO_ELIMINACION_REQUIRED` | `NotEmpty` | `TriajeRule.cs:38` |

## Validaciones (FluentValidation)

| N | PARAMETRO | VALOR_ACTUAL | FUENTE |
| --- | --- | --- | --- |
| 74 | `SANIDAD__VALIDATOR__VACUNOID_MIN` | `> 0` | `CreateTriajeValidator.cs:11` |
| 75 | `SANIDAD__VALIDATOR__TIPO_PESO_CODE_REQUIRED` | `NotEmpty` | `CreateTriajeValidator.cs:14` |
| 76 | `SANIDAD__VALIDATOR__PESO_KG_MIN` | `> 0` | `CreateTriajeValidator.cs:17` |
| 77 | `SANIDAD__VALIDATOR__OBSERVACIONES_MAX_LENGTH` | `150` | `CreateTriajeValidator.cs:20`, `UpdateTriajeValidator.cs:15` |
| 78 | `SANIDAD__VALIDATOR__FECHAHORA_REQUIRED` | `NotEmpty` | `CreateTriajeValidator.cs:23` |
| 79 | `SANIDAD__VALIDATOR__FECHAHORA_MAX` | `<= DateTime.UtcNow` | `CreateTriajeValidator.cs:25` |
| 80 | `SANIDAD__VALIDATOR__MOTIVO_ELIMINACION_REQUIRED` | `NotEmpty` | `DeleteTriajeValidator.cs:10` |
| 81 | `SANIDAD__VALIDATOR__MOTIVO_ELIMINACION_MAX_LENGTH` | `250` | `DeleteTriajeValidator.cs:11` |

## Reglas de la entidad (Triaje.cs)

| N | PARAMETRO | VALOR_ACTUAL | FUENTE |
| --- | --- | --- | --- |
| 82 | `SANIDAD__ENTITY__OBSERVACIONES_TRIM_MAX_LENGTH` | `150` | `Triaje.cs:176` |
| 83 | `SANIDAD__ENTITY__ESTADO_ACTIVO_CODE` | `ACTIVO` | `EstadoRegistroRepository.cs:20` |

## Base de datos (EF Core — triaje)

| N | PARAMETRO | VALOR_ACTUAL | FUENTE |
| --- | --- | --- | --- |
| 84 | `SANIDAD__DB__TABLE_NAME` | `triaje` | `triaje.cs:9` |
| 85 | `SANIDAD__DB__CODIGO_MAX_LENGTH` | `15` | `triaje.cs:22` |
| 86 | `SANIDAD__DB__TIPO_PESO_CODE_MAX_LENGTH` | `30` | `triaje.cs:30` |
| 87 | `SANIDAD__DB__PESO_KG_COLUMN_TYPE` | `numeric(8, 2)` | `triaje.cs:34` |
| 88 | `SANIDAD__DB__OBSERVACIONES_MAX_LENGTH` | `150` | `triaje.cs:37` |
| 89 | `SANIDAD__DB__ESTADO_REGISTRO_CODE_MAX_LENGTH` | `30` | `triaje.cs:41` |
| 90 | `SANIDAD__DB__MOTIVO_ELIMINACION_MAX_LENGTH` | `250` | `triaje.cs:59` |
| 91 | `SANIDAD__DB__UNIQUE_INDEX_CODIGO` | `uq_triaje_codigo` | `triaje.cs:15` |
| 92 | `SANIDAD__DB__UNIQUE_INDEX_VACUNO_FECHA_TIPO` | `uq_triaje_vacuno_fecha_tipo` | `triaje.cs:16` |
| 93 | `SANIDAD__DB__INDEX_DELETED_AT` | `idx_triaje_deleted_at` | `triaje.cs:10` |
| 94 | `SANIDAD__DB__INDEX_ESTADO` | `idx_triaje_estado` | `triaje.cs:11` |
| 95 | `SANIDAD__DB__INDEX_FECHA_HORA` | `idx_triaje_fecha_hora` | `triaje.cs:12` |
| 96 | `SANIDAD__DB__INDEX_TIPO_PESO` | `idx_triaje_tipo_peso` | `triaje.cs:13` |
| 97 | `SANIDAD__DB__INDEX_VACUNO` | `idx_triaje_vacuno` | `triaje.cs:14` |

## Historial

| N | PARAMETRO | VALOR_ACTUAL | FUENTE |
| --- | --- | --- | --- |
| 98 | `SANIDAD__HISTORIAL__ORDER_BY` | `fecha_hora DESC` | `TriajeRepository.cs:1611-1612` |
| 99 | `SANIDAD__HISTORIAL__SOFT_DELETE_FILTER` | `deleted_at == null` | `TriajeRepository.cs:1611` |

## Detalles por vacuno

| N | PARAMETRO | VALOR_ACTUAL | FUENTE |
| --- | --- | --- | --- |
| 100 | `SANIDAD__DETALLES__DATE_FORMAT` | `yyyy-MM-dd` | `GetDetallesTriajeByVacunoIdUseCase.cs:679` |
| 101 | `SANIDAD__DETALLES__HOUR_FORMAT` | `HH:mm:ss` | `GetDetallesTriajeByVacunoIdUseCase.cs:680` |
| 102 | `SANIDAD__DETALLES__ORDER_BY` | `fecha_hora DESC` | `TriajeRepository.cs:1629` |

## Capa de Application — UseCases registrados

| N | PARAMETRO | VALOR_ACTUAL | FUENTE |
| --- | --- | --- | --- |
| 103 | `SANIDAD__USECASE__GET_ALL` | `GetAllTriajesInteractor` | `DependencyInjection.cs:36` |
| 104 | `SANIDAD__USECASE__GET_BY_ID` | `GetTriajeByIdInteractor` | `DependencyInjection.cs:37` |
| 105 | `SANIDAD__USECASE__CREATE` | `CreateTriajeInteractor` | `DependencyInjection.cs:38` |
| 106 | `SANIDAD__USECASE__UPDATE` | `UpdateTriajeInteractor` | `DependencyInjection.cs:39` |
| 107 | `SANIDAD__USECASE__DELETE` | `DeleteTriajeInteractor` | `DependencyInjection.cs:40` |
| 108 | `SANIDAD__USECASE__GET_TIPOS_PESO` | `GetAllTipoPesosInteractor` | `DependencyInjection.cs:41` |
| 109 | `SANIDAD__USECASE__GET_VACUNOS` | `GetAllVacunosSanidadInteractor` | `DependencyInjection.cs:42` |
| 110 | `SANIDAD__USECASE__GET_HISTORIAL` | `GetHistorialByVacunoIdInteractor` | `DependencyInjection.cs:43` |

## Mensajes de error

| N | PARAMETRO | VALOR_ACTUAL | FUENTE |
| --- | --- | --- | --- |
| 111 | `SANIDAD__ERROR__NOT_FOUND` | `No se encontró el triaje solicitado.` | `GetTriajeByIdInteractor.cs:585`, `UpdateTriajeInteractor.cs:511`, `DeleteTriajeInteractor.cs:1104` |
| 112 | `SANIDAD__ERROR__DUPLICATE` | `No se pudo registrar el triaje. Verifique que no exista un registro con el mismo vacuno, tipo de peso y fecha.` | `TriajeRepository.cs:1566` |
| 113 | `SANIDAD__ERROR__UPDATE_DELETED` | `No se puede actualizar un triaje eliminado.` | `Triaje.cs:147` |
| 114 | `SANIDAD__ERROR__ALREADY_DELETED` | `El triaje ya se encuentra eliminado.` | `Triaje.cs:161` |
| 115 | `SANIDAD__ERROR__VACUNOID` | `El vacuno debe ser obligatorio.` | `TriajeRule.cs:8` |
| 116 | `SANIDAD__ERROR__TIPO_PESO_CODE` | `El tipo de peso es obligatorio.` | `TriajeRule.cs:14` |
| 117 | `SANIDAD__ERROR__PESO_KG` | `El peso debe ser mayor que cero.` | `TriajeRule.cs:20` |
| 118 | `SANIDAD__ERROR__FECHAHORA_REQUIRED` | `La fecha y hora del triaje es obligatoria.` | `TriajeRule.cs:27` |
| 119 | `SANIDAD__ERROR__FECHAHORA_FUTURA` | `La fecha y hora del triaje no puede ser futura.` | `TriajeRule.cs:33` |
| 120 | `SANIDAD__ERROR__MOTIVO_ELIMINACION` | `El motivo de eliminación es obligatorio.` | `TriajeRule.cs:39` |
