SET XACT_ABORT ON;

BEGIN TRY

BEGIN TRANSACTION;

------------------------------------------------------------
-- SETTING GROUP
------------------------------------------------------------

IF NOT EXISTS
(
    SELECT 1
    FROM setting_groups
    WHERE code = 'SANIDAD'
)
BEGIN

    INSERT INTO setting_groups
    (
        code,
        name,
        description,
        is_active,
        metadata,
        created_at,
        updated_at,
        deleted_at
    )
    VALUES
    (
        'SANIDAD',
        N'Sanidad',
        N'Grupo único para todos los parámetros configurables del módulo de sanidad (triaje).',
        1,
        NULL,
        SYSUTCDATETIME(),
        SYSUTCDATETIME(),
        NULL
    );

END;

DECLARE @SettingGroupId INT =
(
    SELECT id
    FROM setting_groups
    WHERE code = 'SANIDAD'
);

------------------------------------------------------------
-- LISTADO Y PAGINACION
------------------------------------------------------------

INSERT INTO setting_definitions
(
    setting_group_id,
    code,
    name,
    description,
    data_type,
    default_value,
    validation_schema,
    is_required,
    is_sensitive,
    is_active,
    metadata,
    created_at,
    updated_at,
    deleted_at
)
SELECT
    @SettingGroupId,
    V.Code,
    V.Name,
    V.Description,
    V.DataType,
    V.DefaultValue,
    V.ValidationSchema,
    0,
    0,
    1,
    NULL,
    SYSUTCDATETIME(),
    SYSUTCDATETIME(),
    NULL
FROM
(
VALUES

('SANIDAD__LIST__QUERY_PARAM_PAGINA',N'Query Param Página',N'Nombre del parámetro de query para el número de página.','STRING',N'"pagina"',NULL),
('SANIDAD__LIST__QUERY_PARAM_TAMANO',N'Query Param Tamaño',N'Nombre del parámetro de query para el tamaño de página.','STRING',N'"tamano"',NULL),
('SANIDAD__LIST__DEFAULT_PAGE_CONTROLLER',N'Página por Defecto (Controller)',N'Página por defecto usada en el controller cuando no se especifica.','INT','1','{ "type":"integer","minimum":1,"maximum":100000 }'),
('SANIDAD__LIST__DEFAULT_PAGE_SIZE_CONTROLLER',N'Tamaño de Página por Defecto (Controller)',N'Tamaño de página por defecto usado en el controller cuando no se especifica.','INT','10','{ "type":"integer","minimum":1,"maximum":500 }'),
('SANIDAD__LIST__DEFAULT_PAGE_INTERACTOR',N'Página por Defecto (Interactor)',N'Página aplicada cuando Pagina <= 0.','INT','1','{ "type":"integer","minimum":1,"maximum":100000 }'),
('SANIDAD__LIST__DEFAULT_PAGE_SIZE_INTERACTOR',N'Tamaño de Página por Defecto (Interactor)',N'Tamaño de página aplicado cuando Tamano <= 0.','INT','10','{ "type":"integer","minimum":1,"maximum":500 }'),
('SANIDAD__LIST__MAX_PAGE_SIZE',N'Tamaño Máximo de Página',N'Tamaño máximo de página permitido en el listado de triajes.','INT','100','{ "type":"integer","minimum":1,"maximum":1000 }'),
('SANIDAD__LIST__TOTAL_PAGES_FORMULA',N'Fórmula Total de Páginas',N'Fórmula utilizada para calcular el total de páginas.','STRING',N'"Ceiling(total / tamano)"',NULL),
('SANIDAD__LIST__SKIP_FORMULA',N'Fórmula Skip',N'Fórmula utilizada para calcular los registros a omitir (skip).','STRING',N'"(pagina - 1) * tamano"',NULL),
('SANIDAD__LIST__TAKE',N'Cantidad a Tomar (Take)',N'Variable utilizada como cantidad de registros a tomar (take) en la consulta.','STRING',N'"tamano"',NULL),
('SANIDAD__LIST__DEFAULT_ORDER',N'Orden por Defecto',N'Orden por defecto aplicado al listado de triajes.','STRING',N'"fecha_hora DESC"',NULL),
('SANIDAD__LIST__SOFT_DELETE_FILTER',N'Filtro de Eliminación Lógica',N'Condición aplicada para excluir triajes eliminados lógicamente.','STRING',N'"deleted_at == null"',NULL)

) V (Code, Name, Description, DataType, DefaultValue, ValidationSchema)
WHERE NOT EXISTS
(
    SELECT 1
    FROM setting_definitions s
    WHERE s.setting_group_id = @SettingGroupId
      AND s.code = V.Code
);

------------------------------------------------------------
-- FILTROS DE LISTADO
------------------------------------------------------------

INSERT INTO setting_definitions
(
    setting_group_id,
    code,
    name,
    description,
    data_type,
    default_value,
    validation_schema,
    is_required,
    is_sensitive,
    is_active,
    metadata,
    created_at,
    updated_at,
    deleted_at
)
SELECT
    @SettingGroupId,
    V.Code,
    V.Name,
    V.Description,
    V.DataType,
    V.DefaultValue,
    V.ValidationSchema,
    0,
    0,
    1,
    NULL,
    SYSUTCDATETIME(),
    SYSUTCDATETIME(),
    NULL
FROM
(
VALUES

('SANIDAD__FILTER__FECHA_PARAM',N'Query Param Fecha',N'Nombre del parámetro de query para filtrar por fecha.','STRING',N'"fecha"',NULL),
('SANIDAD__FILTER__CODIGO_PARAM',N'Query Param Código',N'Nombre del parámetro de query para filtrar por código.','STRING',N'"codigo"',NULL),
('SANIDAD__FILTER__NOMBRE_PARAM',N'Query Param Nombre',N'Nombre del parámetro de query para filtrar por nombre del vacuno.','STRING',N'"nombre"',NULL),
('SANIDAD__FILTER__TIPO_PESO_PARAM',N'Query Param Tipo de Peso',N'Nombre del parámetro de query para filtrar por tipo de peso.','STRING',N'"tipoPeso"',NULL),
('SANIDAD__FILTER__PESO_KG_PARAM',N'Query Param Peso (Kg)',N'Nombre del parámetro de query para filtrar por peso en kilogramos.','STRING',N'"pesoKg"',NULL),
('SANIDAD__FILTER__CODIGO_OPERATOR',N'Operador de Filtro Código',N'Operador de comparación aplicado al filtrar por código.','STRING',N'"Contains"',NULL),
('SANIDAD__FILTER__NOMBRE_OPERATOR',N'Operador de Filtro Nombre',N'Operador de comparación aplicado al filtrar por nombre del vacuno.','STRING',N'"Contains sobre vacuno.nombre"',NULL),
('SANIDAD__FILTER__FECHA_PARSE',N'Parseo de Fecha',N'Método utilizado para parsear el filtro de fecha recibido.','STRING',N'"DateTime.TryParse"',NULL),
('SANIDAD__FILTER__FECHA_OPERATOR_RANGE',N'Operador de Rango de Fecha',N'Condición de rango aplicada al filtrar por fecha.','STRING',N'">= desde && < hasta"',NULL),
('SANIDAD__FILTER__FECHA_OPERATOR_DATE',N'Operador de Fecha Exacta',N'Condición aplicada al filtrar por una fecha exacta (sin hora).','STRING',N'"fecha_hora.Date == fechaParsed.Date"',NULL),
('SANIDAD__FILTER__TIPO_PESO_OPERATOR',N'Operador de Filtro Tipo de Peso',N'Operador de comparación exacta aplicado al filtrar por tipo de peso.','STRING',N'"=="',NULL),
('SANIDAD__FILTER__PESO_KG_OPERATOR',N'Operador de Filtro Peso (Kg)',N'Operador de comparación exacta aplicado al filtrar por peso en kilogramos.','STRING',N'"=="',NULL)

) V (Code, Name, Description, DataType, DefaultValue, ValidationSchema)
WHERE NOT EXISTS
(
    SELECT 1
    FROM setting_definitions s
    WHERE s.setting_group_id = @SettingGroupId
      AND s.code = V.Code
);

------------------------------------------------------------
-- CATALOGO TIPO DE PESO
------------------------------------------------------------

INSERT INTO setting_definitions
(
    setting_group_id,
    code,
    name,
    description,
    data_type,
    default_value,
    validation_schema,
    is_required,
    is_sensitive,
    is_active,
    metadata,
    created_at,
    updated_at,
    deleted_at
)
SELECT
    @SettingGroupId,
    V.Code,
    V.Name,
    V.Description,
    V.DataType,
    V.DefaultValue,
    V.ValidationSchema,
    0,
    0,
    1,
    NULL,
    SYSUTCDATETIME(),
    SYSUTCDATETIME(),
    NULL
FROM
(
VALUES

('SANIDAD__TIPO_PESO__ACTIVE_FILTER',N'Filtro Activo Tipo de Peso',N'Condición aplicada para obtener únicamente tipos de peso activos.','STRING',N'"activo == true"',NULL),
('SANIDAD__TIPO_PESO__SELECT_FIELDS',N'Campos Seleccionados Tipo de Peso',N'Campos proyectados al consultar el catálogo de tipos de peso.','ARRAY','["code","nombre"]','{ "type":"array","items":{"type":"string"} }'),
('SANIDAD__TIPO_PESO__RESPONSE_FIELDS',N'Campos de Respuesta Tipo de Peso',N'Campos incluidos en la respuesta del catálogo de tipos de peso.','ARRAY','["code","nombre"]','{ "type":"array","items":{"type":"string"} }')

) V (Code, Name, Description, DataType, DefaultValue, ValidationSchema)
WHERE NOT EXISTS
(
    SELECT 1
    FROM setting_definitions s
    WHERE s.setting_group_id = @SettingGroupId
      AND s.code = V.Code
);

------------------------------------------------------------
-- VACUNOS RELACIONADOS
------------------------------------------------------------

INSERT INTO setting_definitions
(
    setting_group_id,
    code,
    name,
    description,
    data_type,
    default_value,
    validation_schema,
    is_required,
    is_sensitive,
    is_active,
    metadata,
    created_at,
    updated_at,
    deleted_at
)
SELECT
    @SettingGroupId,
    V.Code,
    V.Name,
    V.Description,
    V.DataType,
    V.DefaultValue,
    V.ValidationSchema,
    0,
    0,
    1,
    NULL,
    SYSUTCDATETIME(),
    SYSUTCDATETIME(),
    NULL
FROM
(
VALUES

('SANIDAD__VACUNOS__SOURCE_REPOSITORY',N'Repositorio Fuente de Vacunos',N'Método del repositorio utilizado para obtener los vacunos disponibles para sanidad.','STRING',N'"IVacunoRepository.ListAllAsync"',NULL),
('SANIDAD__VACUNOS__RESPONSE_FIELDS',N'Campos de Respuesta Vacunos',N'Campos incluidos en la respuesta del listado de vacunos para sanidad.','ARRAY','["id","codigo","nombre"]','{ "type":"array","items":{"type":"string"} }'),
('SANIDAD__VACUNOS__OUTPUT_FIELDS',N'Campos de Salida Vacunos',N'Campos del DTO de salida generado por el interactor de vacunos.','ARRAY','["Id","Codigo","Nombre"]','{ "type":"array","items":{"type":"string"} }')

) V (Code, Name, Description, DataType, DefaultValue, ValidationSchema)
WHERE NOT EXISTS
(
    SELECT 1
    FROM setting_definitions s
    WHERE s.setting_group_id = @SettingGroupId
      AND s.code = V.Code
);

------------------------------------------------------------
-- HISTORIAL Y DETALLES POR VACUNO
------------------------------------------------------------

INSERT INTO setting_definitions
(
    setting_group_id,
    code,
    name,
    description,
    data_type,
    default_value,
    validation_schema,
    is_required,
    is_sensitive,
    is_active,
    metadata,
    created_at,
    updated_at,
    deleted_at
)
SELECT
    @SettingGroupId,
    V.Code,
    V.Name,
    V.Description,
    V.DataType,
    V.DefaultValue,
    V.ValidationSchema,
    0,
    0,
    1,
    NULL,
    SYSUTCDATETIME(),
    SYSUTCDATETIME(),
    NULL
FROM
(
VALUES

('SANIDAD__HISTORIAL__FILTER_VACUNO',N'Filtro de Historial por Vacuno',N'Condición aplicada para obtener el historial de triajes de un vacuno.','STRING',N'"vacuno_id == vacunoId"',NULL),
('SANIDAD__HISTORIAL__SOFT_DELETE_FILTER',N'Filtro de Eliminación Lógica (Historial)',N'Condición aplicada para excluir triajes eliminados del historial.','STRING',N'"deleted_at == null"',NULL),
('SANIDAD__HISTORIAL__ORDER_BY',N'Orden del Historial',N'Orden aplicado al historial de triajes por vacuno.','STRING',N'"fecha_hora DESC"',NULL),
('SANIDAD__HISTORIAL__RESPONSE_FIELDS',N'Campos de Respuesta del Historial',N'Campos incluidos en la respuesta del historial de triajes.','ARRAY','["id","fechaHora","tipoPesoCode","pesoKg"]','{ "type":"array","items":{"type":"string"} }'),
('SANIDAD__DETALLES__FILTER_VACUNO',N'Filtro de Detalles por Vacuno',N'Condición aplicada para obtener los detalles de triajes de un vacuno.','STRING',N'"vacuno_id == vacunoId"',NULL),
('SANIDAD__DETALLES__SOFT_DELETE_FILTER',N'Filtro de Eliminación Lógica (Detalles)',N'Condición aplicada para excluir triajes eliminados de los detalles.','STRING',N'"deleted_at == null"',NULL),
('SANIDAD__DETALLES__ORDER_BY',N'Orden de Detalles',N'Orden aplicado a los detalles de triajes por vacuno.','STRING',N'"fecha_hora DESC"',NULL),
('SANIDAD__DETALLES__DATE_FORMAT',N'Formato de Fecha (Detalles)',N'Formato de fecha usado en el detalle de triajes.','STRING',N'"yyyy-MM-dd"','{ "type":"string","enum":["yyyy-MM-dd","dd/MM/yyyy"] }'),
('SANIDAD__DETALLES__HOUR_FORMAT',N'Formato de Hora (Detalles)',N'Formato de hora usado en el detalle de triajes.','STRING',N'"HH:mm:ss"',NULL),
('SANIDAD__DETALLES__TIPO_PESO_SOURCE',N'Origen del Tipo de Peso (Detalles)',N'Propiedad de navegación usada para obtener el nombre del tipo de peso.','STRING',N'"tipo_peso_codeNavigation.nombre"',NULL)

) V (Code, Name, Description, DataType, DefaultValue, ValidationSchema)
WHERE NOT EXISTS
(
    SELECT 1
    FROM setting_definitions s
    WHERE s.setting_group_id = @SettingGroupId
      AND s.code = V.Code
);

------------------------------------------------------------
-- REPORTES DESCARGA
------------------------------------------------------------

INSERT INTO setting_definitions
(
    setting_group_id,
    code,
    name,
    description,
    data_type,
    default_value,
    validation_schema,
    is_required,
    is_sensitive,
    is_active,
    metadata,
    created_at,
    updated_at,
    deleted_at
)
SELECT
    @SettingGroupId,
    V.Code,
    V.Name,
    V.Description,
    V.DataType,
    V.DefaultValue,
    V.ValidationSchema,
    0,
    0,
    1,
    NULL,
    SYSUTCDATETIME(),
    SYSUTCDATETIME(),
    NULL
FROM
(
VALUES

('SANIDAD__REPORT__DOWNLOAD_PAGINA',N'Página de Descarga de Reporte',N'Página fija usada al generar la descarga completa del reporte de triajes.','INT','1','{ "type":"integer","minimum":1,"maximum":100000 }'),
('SANIDAD__REPORT__DOWNLOAD_TAMANO',N'Tamaño de Descarga de Reporte',N'Tamaño de página usado (máximo entero) para incluir todos los registros en la descarga del reporte.','INT','2147483647','{ "type":"integer","minimum":1 }'),
('SANIDAD__REPORT__FORMAT_NORMALIZATION',N'Normalización de Formato',N'Transformación aplicada al formato de reporte solicitado antes de validarlo.','STRING',N'"Trim().ToLowerInvariant()"',NULL),
('SANIDAD__REPORT__ALLOWED_FORMAT_XLSX',N'Formato Permitido XLSX',N'Valor aceptado para solicitar el reporte en formato Excel (extensión xlsx).','STRING',N'"xlsx"',NULL),
('SANIDAD__REPORT__ALLOWED_FORMAT_EXCEL',N'Formato Permitido Excel',N'Valor aceptado para solicitar el reporte en formato Excel (alias excel).','STRING',N'"excel"',NULL),
('SANIDAD__REPORT__ALLOWED_FORMAT_PDF',N'Formato Permitido PDF',N'Valor aceptado para solicitar el reporte en formato PDF.','STRING',N'"pdf"',NULL),
('SANIDAD__REPORT__TIMESTAMP_FORMAT',N'Formato de Timestamp',N'Formato de fecha/hora usado para nombrar los archivos de reporte generados.','STRING',N'"yyyyMMddHHmmss"',NULL),
('SANIDAD__REPORT__EXCEL_CONTENT_TYPE',N'Content-Type Excel',N'Tipo de contenido MIME devuelto al descargar el reporte en Excel.','STRING',N'"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"',NULL),
('SANIDAD__REPORT__PDF_CONTENT_TYPE',N'Content-Type PDF',N'Tipo de contenido MIME devuelto al descargar el reporte en PDF.','STRING',N'"application/pdf"',NULL),
('SANIDAD__REPORT__EXCEL_FILENAME_TEMPLATE',N'Plantilla de Nombre de Archivo Excel',N'Plantilla usada para nombrar el archivo Excel generado.','STRING',N'"reporte-triajes-{timestamp}.xlsx"',NULL),
('SANIDAD__REPORT__PDF_FILENAME_TEMPLATE',N'Plantilla de Nombre de Archivo PDF',N'Plantilla usada para nombrar el archivo PDF generado.','STRING',N'"reporte-triajes-{timestamp}.pdf"',NULL),
('SANIDAD__REPORT__EXCEL_MESSAGE',N'Mensaje de Éxito Excel',N'Mensaje devuelto al generar exitosamente el reporte en Excel.','STRING',N'"Descarga de reporte de triajes en Excel generada correctamente."',NULL),
('SANIDAD__REPORT__PDF_MESSAGE',N'Mensaje de Éxito PDF',N'Mensaje devuelto al generar exitosamente el reporte en PDF.','STRING',N'"Descarga de reporte de triajes en PDF generada correctamente."',NULL),
('SANIDAD__REPORT__ERROR_FORMAT',N'Mensaje de Formato No Soportado',N'Mensaje de error devuelto cuando el formato de reporte solicitado no es válido.','STRING',N'"Formato no soportado. Use ''xlsx'' o ''pdf''."','{ "type":"string","minLength":1,"maxLength":300 }')

) V (Code, Name, Description, DataType, DefaultValue, ValidationSchema)
WHERE NOT EXISTS
(
    SELECT 1
    FROM setting_definitions s
    WHERE s.setting_group_id = @SettingGroupId
      AND s.code = V.Code
);

------------------------------------------------------------
-- REPORTE EXCEL
------------------------------------------------------------

INSERT INTO setting_definitions
(
    setting_group_id,
    code,
    name,
    description,
    data_type,
    default_value,
    validation_schema,
    is_required,
    is_sensitive,
    is_active,
    metadata,
    created_at,
    updated_at,
    deleted_at
)
SELECT
    @SettingGroupId,
    V.Code,
    V.Name,
    V.Description,
    V.DataType,
    V.DefaultValue,
    V.ValidationSchema,
    0,
    0,
    1,
    NULL,
    SYSUTCDATETIME(),
    SYSUTCDATETIME(),
    NULL
FROM
(
VALUES

('SANIDAD__EXCEL__CONTENT_TYPES_PATH',N'Ruta Content Types',N'Ruta interna del archivo [Content_Types].xml dentro del paquete Excel (OOXML).','STRING',N'"[Content_Types].xml"',NULL),
('SANIDAD__EXCEL__RELS_PATH',N'Ruta de Relaciones',N'Ruta interna del archivo de relaciones principales del paquete Excel.','STRING',N'"_rels/.rels"',NULL),
('SANIDAD__EXCEL__WORKBOOK_PATH',N'Ruta del Workbook',N'Ruta interna del archivo workbook.xml del paquete Excel.','STRING',N'"xl/workbook.xml"',NULL),
('SANIDAD__EXCEL__WORKBOOK_RELS_PATH',N'Ruta de Relaciones del Workbook',N'Ruta interna del archivo de relaciones del workbook.','STRING',N'"xl/_rels/workbook.xml.rels"',NULL),
('SANIDAD__EXCEL__WORKSHEET_PATH',N'Ruta de la Hoja de Cálculo',N'Ruta interna de la hoja de cálculo principal del paquete Excel.','STRING',N'"xl/worksheets/sheet1.xml"',NULL),
('SANIDAD__EXCEL__SHEET_NAME',N'Nombre de la Hoja',N'Nombre de la hoja de cálculo dentro del archivo Excel generado.','STRING',N'"Triajes"',NULL),
('SANIDAD__EXCEL__HEADER_ROW_NUMBER',N'Número de Fila de Encabezado',N'Número de fila donde se ubican los encabezados en el reporte Excel.','INT','1','{ "type":"integer","minimum":1,"maximum":10 }'),
('SANIDAD__EXCEL__DATA_START_ROW',N'Fila de Inicio de Datos',N'Número de fila donde inician los datos en el reporte Excel.','INT','2','{ "type":"integer","minimum":1,"maximum":10 }'),
('SANIDAD__EXCEL__HEADER__C_REGISTRO',N'Encabezado Código de Registro',N'Texto del encabezado de la columna código de registro en el Excel.','STRING',N'"C.Registro"',NULL),
('SANIDAD__EXCEL__HEADER__FECHA',N'Encabezado Fecha',N'Texto del encabezado de la columna fecha en el Excel.','STRING',N'"Fecha"',NULL),
('SANIDAD__EXCEL__HEADER__HORA',N'Encabezado Hora',N'Texto del encabezado de la columna hora en el Excel.','STRING',N'"Hora"',NULL),
('SANIDAD__EXCEL__HEADER__VACUNO',N'Encabezado Vacuno',N'Texto del encabezado de la columna vacuno en el Excel.','STRING',N'"Vacuno"',NULL),
('SANIDAD__EXCEL__HEADER__TIPO_PESO_MEDIDO',N'Encabezado Tipo de Peso Medido',N'Texto del encabezado de la columna tipo de peso medido en el Excel.','STRING',N'"Tipo de peso medido"',NULL),
('SANIDAD__EXCEL__HEADER__PESO_KG',N'Encabezado Peso (Kg)',N'Texto del encabezado de la columna peso en kilogramos en el Excel.','STRING',N'"Peso (Kg)"',NULL),
('SANIDAD__EXCEL__HEADER__OBSERVACIONES',N'Encabezado Observaciones',N'Texto del encabezado de la columna observaciones en el Excel.','STRING',N'"Observaciones"',NULL),
('SANIDAD__EXCEL__DATE_FORMAT',N'Formato de Fecha (Excel)',N'Formato de fecha usado en las celdas del reporte Excel.','STRING',N'"yyyy-MM-dd"',NULL),
('SANIDAD__EXCEL__HOUR_FORMAT',N'Formato de Hora (Excel)',N'Formato de hora usado en las celdas del reporte Excel.','STRING',N'"HH:mm:ss"',NULL),
('SANIDAD__EXCEL__ENCODING_UTF8_BOM',N'Usar BOM UTF-8',N'Indica si el archivo Excel generado incluye BOM UTF-8.','BOOL','false',NULL)

) V (Code, Name, Description, DataType, DefaultValue, ValidationSchema)
WHERE NOT EXISTS
(
    SELECT 1
    FROM setting_definitions s
    WHERE s.setting_group_id = @SettingGroupId
      AND s.code = V.Code
);

------------------------------------------------------------
-- REPORTE PDF
------------------------------------------------------------

INSERT INTO setting_definitions
(
    setting_group_id,
    code,
    name,
    description,
    data_type,
    default_value,
    validation_schema,
    is_required,
    is_sensitive,
    is_active,
    metadata,
    created_at,
    updated_at,
    deleted_at
)
SELECT
    @SettingGroupId,
    V.Code,
    V.Name,
    V.Description,
    V.DataType,
    V.DefaultValue,
    V.ValidationSchema,
    0,
    0,
    1,
    NULL,
    SYSUTCDATETIME(),
    SYSUTCDATETIME(),
    NULL
FROM
(
VALUES

('SANIDAD__PDF__TITLE',N'Título del PDF',N'Título mostrado en el reporte PDF de triajes.','STRING',N'"Reporte de triajes"',NULL),
('SANIDAD__PDF__GENERATED_AT_LABEL',N'Etiqueta de Generado',N'Etiqueta usada para mostrar la fecha de generación en el PDF.','STRING',N'"Generado"',NULL),
('SANIDAD__PDF__GENERATED_AT_FORMAT',N'Formato de Fecha de Generación',N'Formato de fecha y hora usado para mostrar cuándo se generó el PDF.','STRING',N'"yyyy-MM-dd HH:mm:ss"',NULL),
('SANIDAD__PDF__TABLE_HEADER',N'Encabezado de Tabla PDF',N'Texto del encabezado de la tabla en el reporte PDF.','STRING',N'"C.Registro | Fecha | Hora | Vacuno | Tipo peso | Peso (Kg) | Observaciones"',NULL),
('SANIDAD__PDF__ROW_DATE_FORMAT',N'Formato de Fecha por Fila',N'Formato de fecha usado en cada fila del reporte PDF.','STRING',N'"yyyy-MM-dd"',NULL),
('SANIDAD__PDF__ROW_HOUR_FORMAT',N'Formato de Hora por Fila',N'Formato de hora usado en cada fila del reporte PDF.','STRING',N'"HH:mm:ss"',NULL),
('SANIDAD__PDF__TRIM__VACUNO_NOMBRE',N'Longitud Máxima Nombre Vacuno (PDF)',N'Cantidad máxima de caracteres mostrados del nombre del vacuno en el PDF.','INT','18','{ "type":"integer","minimum":1,"maximum":200 }'),
('SANIDAD__PDF__TRIM__TIPO_PESO_CODE',N'Longitud Máxima Código Tipo de Peso (PDF)',N'Cantidad máxima de caracteres mostrados del código de tipo de peso en el PDF.','INT','18','{ "type":"integer","minimum":1,"maximum":200 }'),
('SANIDAD__PDF__TRIM__OBSERVACIONES',N'Longitud Máxima Observaciones (PDF)',N'Cantidad máxima de caracteres mostrados de las observaciones en el PDF.','INT','30','{ "type":"integer","minimum":1,"maximum":300 }'),
('SANIDAD__PDF__LINES_PER_PAGE',N'Líneas por Página',N'Cantidad de líneas de datos por página en el reporte PDF.','INT','38','{ "type":"integer","minimum":1,"maximum":100 }'),
('SANIDAD__PDF__BEGIN_TEXT',N'Operador Inicio de Texto',N'Operador PDF que marca el inicio de un bloque de texto.','STRING',N'"BT"',NULL),
('SANIDAD__PDF__FONT_RESOURCE',N'Recurso de Fuente',N'Identificador del recurso de fuente utilizado en el PDF.','STRING',N'"/F1"',NULL),
('SANIDAD__PDF__FONT_SIZE',N'Tamaño de Fuente',N'Tamaño de fuente usado en el contenido del reporte PDF.','INT','9','{ "type":"integer","minimum":6,"maximum":24 }'),
('SANIDAD__PDF__START_X',N'Posición Inicial X',N'Coordenada X inicial donde comienza a escribirse el contenido del PDF.','INT','40','{ "type":"integer","minimum":0,"maximum":2000 }'),
('SANIDAD__PDF__START_Y',N'Posición Inicial Y',N'Coordenada Y inicial donde comienza a escribirse el contenido del PDF.','INT','800','{ "type":"integer","minimum":0,"maximum":2000 }'),
('SANIDAD__PDF__LINE_HEIGHT',N'Altura de Línea',N'Separación vertical entre líneas del contenido del PDF.','INT','18','{ "type":"integer","minimum":1,"maximum":100 }'),
('SANIDAD__PDF__END_TEXT',N'Operador Fin de Texto',N'Operador PDF que marca el fin de un bloque de texto.','STRING',N'"ET"',NULL),
('SANIDAD__PDF__EMPTY_PAGE_COUNT',N'Cantidad de Páginas Vacías',N'Cantidad de páginas generadas cuando el reporte no tiene datos.','INT','1','{ "type":"integer","minimum":0,"maximum":10 }'),
('SANIDAD__PDF__PDF_VERSION',N'Versión de PDF',N'Versión de especificación PDF usada al generar el archivo.','STRING',N'"1.4"',NULL),
('SANIDAD__PDF__PAGE_WIDTH',N'Ancho de Página',N'Ancho de página en puntos (formato A4 horizontal) usado en el PDF.','INT','842','{ "type":"integer","minimum":1,"maximum":5000 }'),
('SANIDAD__PDF__PAGE_HEIGHT',N'Alto de Página',N'Alto de página en puntos (formato A4 horizontal) usado en el PDF.','INT','595','{ "type":"integer","minimum":1,"maximum":5000 }'),
('SANIDAD__PDF__FONT',N'Fuente del PDF',N'Familia tipográfica usada en el reporte PDF.','STRING',N'"Courier"',NULL),
('SANIDAD__PDF__ENCODING',N'Codificación del PDF',N'Codificación de caracteres usada al generar el contenido del PDF.','STRING',N'"ASCII"',NULL)

) V (Code, Name, Description, DataType, DefaultValue, ValidationSchema)
WHERE NOT EXISTS
(
    SELECT 1
    FROM setting_definitions s
    WHERE s.setting_group_id = @SettingGroupId
      AND s.code = V.Code
);

------------------------------------------------------------
-- GENERACION DE CODIGO
------------------------------------------------------------

INSERT INTO setting_definitions
(
    setting_group_id,
    code,
    name,
    description,
    data_type,
    default_value,
    validation_schema,
    is_required,
    is_sensitive,
    is_active,
    metadata,
    created_at,
    updated_at,
    deleted_at
)
SELECT
    @SettingGroupId,
    V.Code,
    V.Name,
    V.Description,
    V.DataType,
    V.DefaultValue,
    V.ValidationSchema,
    0,
    0,
    1,
    NULL,
    SYSUTCDATETIME(),
    SYSUTCDATETIME(),
    NULL
FROM
(
VALUES

('SANIDAD__CODIGO__PREFIX',N'Prefijo de Código',N'Prefijo utilizado al generar el código de triaje.','STRING',N'"TRI"',NULL),
('SANIDAD__CODIGO__PREFIX_LENGTH',N'Longitud del Prefijo',N'Cantidad de caracteres del prefijo del código de triaje.','INT','3','{ "type":"integer","minimum":1,"maximum":10 }'),
('SANIDAD__CODIGO__MIN_NUMBER',N'Número Mínimo de Secuencia',N'Valor mínimo considerado al calcular el siguiente número de secuencia del código.','INT','0','{ "type":"integer","minimum":0,"maximum":1000000 }'),
('SANIDAD__CODIGO__NEXT_INCREMENT',N'Incremento de Secuencia',N'Incremento aplicado al número de secuencia para generar el siguiente código.','INT','1','{ "type":"integer","minimum":1,"maximum":100 }'),
('SANIDAD__CODIGO__FORMAT',N'Formato de Código',N'Formato utilizado para construir el código de triaje (prefijo y correlativo con relleno de ceros).','STRING',N'"TRI{D3}"','{ "type":"string","maxLength":50 }')

) V (Code, Name, Description, DataType, DefaultValue, ValidationSchema)
WHERE NOT EXISTS
(
    SELECT 1
    FROM setting_definitions s
    WHERE s.setting_group_id = @SettingGroupId
      AND s.code = V.Code
);

------------------------------------------------------------
-- REGLAS DEL DOMINIO
------------------------------------------------------------

INSERT INTO setting_definitions
(
    setting_group_id,
    code,
    name,
    description,
    data_type,
    default_value,
    validation_schema,
    is_required,
    is_sensitive,
    is_active,
    metadata,
    created_at,
    updated_at,
    deleted_at
)
SELECT
    @SettingGroupId,
    V.Code,
    V.Name,
    V.Description,
    V.DataType,
    V.DefaultValue,
    V.ValidationSchema,
    0,
    0,
    1,
    NULL,
    SYSUTCDATETIME(),
    SYSUTCDATETIME(),
    NULL
FROM
(
VALUES

('SANIDAD__RULES__VACUNOID_MIN',N'Regla Mínimo VacunoId',N'Condición de dominio que exige que el identificador del vacuno sea mayor a cero.','STRING',N'"> 0"',NULL),
('SANIDAD__RULES__TIPO_PESO_CODE_REQUIRED',N'Regla Código Tipo de Peso Requerido',N'Condición de dominio que exige que el código de tipo de peso no esté vacío.','STRING',N'"NotEmpty/NotWhiteSpace"',NULL),
('SANIDAD__RULES__PESO_KG_MIN',N'Regla Mínimo Peso (Kg)',N'Condición de dominio que exige que el peso en kilogramos sea mayor a cero.','STRING',N'"> 0"',NULL),
('SANIDAD__RULES__FECHAHORA_REQUIRED',N'Regla Fecha/Hora Requerida',N'Condición de dominio que exige que la fecha y hora del triaje sea distinta al valor por defecto.','STRING',N'"!= default"',NULL),
('SANIDAD__RULES__FECHAHORA_MAX',N'Regla Fecha/Hora Máxima',N'Condición de dominio que impide registrar triajes con fecha futura.','STRING',N'"<= DateTime.UtcNow"',NULL),
('SANIDAD__RULES__MOTIVO_ELIMINACION_REQUIRED',N'Regla Motivo de Eliminación Requerido',N'Condición de dominio que exige un motivo al eliminar un triaje.','STRING',N'"NotEmpty/NotWhiteSpace"',NULL),
('SANIDAD__RULES__ESTADO_REGISTRO_REQUIRED',N'Regla Estado de Registro Requerido',N'Condición de dominio que exige un estado de registro válido en el triaje.','STRING',N'"ThrowIfNullOrWhiteSpace"',NULL),
('SANIDAD__RULES__NO_UPDATE_DELETED',N'Regla No Actualizar Eliminados',N'Condición de dominio que impide actualizar un triaje previamente eliminado.','STRING',N'"IsDeleted == false"',NULL),
('SANIDAD__RULES__NO_DELETE_ALREADY_DELETED',N'Regla No Eliminar Ya Eliminados',N'Condición de dominio que impide eliminar un triaje ya eliminado.','STRING',N'"IsDeleted == false"',NULL)

) V (Code, Name, Description, DataType, DefaultValue, ValidationSchema)
WHERE NOT EXISTS
(
    SELECT 1
    FROM setting_definitions s
    WHERE s.setting_group_id = @SettingGroupId
      AND s.code = V.Code
);

------------------------------------------------------------
-- VALIDACIONES FLUENTVALIDATION
------------------------------------------------------------

INSERT INTO setting_definitions
(
    setting_group_id,
    code,
    name,
    description,
    data_type,
    default_value,
    validation_schema,
    is_required,
    is_sensitive,
    is_active,
    metadata,
    created_at,
    updated_at,
    deleted_at
)
SELECT
    @SettingGroupId,
    V.Code,
    V.Name,
    V.Description,
    V.DataType,
    V.DefaultValue,
    V.ValidationSchema,
    0,
    0,
    1,
    NULL,
    SYSUTCDATETIME(),
    SYSUTCDATETIME(),
    NULL
FROM
(
VALUES

('SANIDAD__VALIDATOR_CREATE__VACUNOID_MIN',N'Validador Crear: Mínimo VacunoId',N'Validación al crear que exige VacunoId mayor a cero.','STRING',N'"> 0"',NULL),
('SANIDAD__VALIDATOR_CREATE__TIPO_PESO_CODE_REQUIRED',N'Validador Crear: Tipo de Peso Requerido',N'Validación al crear que exige el código de tipo de peso.','STRING',N'"NotEmpty"',NULL),
('SANIDAD__VALIDATOR_CREATE__PESO_KG_MIN',N'Validador Crear: Mínimo Peso (Kg)',N'Validación al crear que exige un peso en kilogramos mayor a cero.','STRING',N'"> 0"',NULL),
('SANIDAD__VALIDATOR_CREATE__OBSERVACIONES_MAX_LENGTH',N'Validador Crear: Longitud Máxima Observaciones',N'Longitud máxima permitida para las observaciones al crear un triaje.','INT','150','{ "type":"integer","minimum":1,"maximum":1000 }'),
('SANIDAD__VALIDATOR_CREATE__FECHAHORA_REQUIRED',N'Validador Crear: Fecha/Hora Requerida',N'Validación al crear que exige la fecha y hora del triaje.','STRING',N'"NotEmpty"',NULL),
('SANIDAD__VALIDATOR_CREATE__FECHAHORA_MAX',N'Validador Crear: Fecha/Hora Máxima',N'Validación al crear que impide fechas futuras.','STRING',N'"<= DateTime.UtcNow"',NULL),
('SANIDAD__VALIDATOR_UPDATE__TIPO_PESO_CODE_REQUIRED',N'Validador Actualizar: Tipo de Peso Requerido',N'Validación al actualizar que exige el código de tipo de peso.','STRING',N'"NotEmpty"',NULL),
('SANIDAD__VALIDATOR_UPDATE__PESO_KG_MIN',N'Validador Actualizar: Mínimo Peso (Kg)',N'Validación al actualizar que exige un peso en kilogramos mayor a cero.','STRING',N'"> 0"',NULL),
('SANIDAD__VALIDATOR_UPDATE__OBSERVACIONES_MAX_LENGTH',N'Validador Actualizar: Longitud Máxima Observaciones',N'Longitud máxima permitida para las observaciones al actualizar un triaje.','INT','150','{ "type":"integer","minimum":1,"maximum":1000 }'),
('SANIDAD__VALIDATOR_DELETE__MOTIVO_ELIMINACION_REQUIRED',N'Validador Eliminar: Motivo Requerido',N'Validación al eliminar que exige un motivo de eliminación.','STRING',N'"NotEmpty"',NULL),
('SANIDAD__VALIDATOR_DELETE__MOTIVO_ELIMINACION_MAX_LENGTH',N'Validador Eliminar: Longitud Máxima Motivo',N'Longitud máxima permitida para el motivo de eliminación.','INT','250','{ "type":"integer","minimum":1,"maximum":1000 }')

) V (Code, Name, Description, DataType, DefaultValue, ValidationSchema)
WHERE NOT EXISTS
(
    SELECT 1
    FROM setting_definitions s
    WHERE s.setting_group_id = @SettingGroupId
      AND s.code = V.Code
);

------------------------------------------------------------
-- REGLAS DE ENTIDAD Y AUDITORIA
------------------------------------------------------------

INSERT INTO setting_definitions
(
    setting_group_id,
    code,
    name,
    description,
    data_type,
    default_value,
    validation_schema,
    is_required,
    is_sensitive,
    is_active,
    metadata,
    created_at,
    updated_at,
    deleted_at
)
SELECT
    @SettingGroupId,
    V.Code,
    V.Name,
    V.Description,
    V.DataType,
    V.DefaultValue,
    V.ValidationSchema,
    0,
    0,
    1,
    NULL,
    SYSUTCDATETIME(),
    SYSUTCDATETIME(),
    NULL
FROM
(
VALUES

('SANIDAD__ENTITY__CODIGO_TRIM',N'Recortar Código',N'Indica si se aplica Trim() al código del triaje.','BOOL','true',NULL),
('SANIDAD__ENTITY__TIPO_PESO_CODE_TRIM_CREATE',N'Recortar Tipo de Peso (Crear)',N'Indica si se aplica Trim() al código de tipo de peso al crear.','BOOL','true',NULL),
('SANIDAD__ENTITY__ESTADO_REGISTRO_CODE_TRIM',N'Recortar Estado de Registro',N'Indica si se aplica Trim() al código de estado de registro.','BOOL','true',NULL),
('SANIDAD__ENTITY__TIPO_PESO_CODE_TRIM_UPDATE',N'Recortar Tipo de Peso (Actualizar)',N'Indica si se aplica Trim() al código de tipo de peso al actualizar.','BOOL','true',NULL),
('SANIDAD__ENTITY__OBSERVACIONES_EMPTY_TO_NULL',N'Observaciones Vacías a Null',N'Indica si las observaciones vacías o en blanco se guardan como null.','BOOL','true',NULL),
('SANIDAD__ENTITY__OBSERVACIONES_TRIM',N'Recortar Observaciones',N'Indica si se aplica Trim() a las observaciones.','BOOL','true',NULL),
('SANIDAD__ENTITY__OBSERVACIONES_TRIM_MAX_LENGTH',N'Longitud Máxima de Observaciones',N'Longitud máxima permitida para las observaciones del triaje.','INT','150','{ "type":"integer","minimum":1,"maximum":1000 }'),
('SANIDAD__ENTITY__CREATED_BY_SOURCE',N'Origen de Creado Por',N'Fuente utilizada para registrar quién creó el triaje.','STRING',N'"encargadoUsuarioId"',NULL),
('SANIDAD__ENTITY__UPDATED_BY_SOURCE_CREATE',N'Origen de Actualizado Por (Crear)',N'Fuente utilizada para registrar quién actualizó el triaje al crearlo.','STRING',N'"encargadoUsuarioId"',NULL),
('SANIDAD__ENTITY__UPDATED_BY_SOURCE_UPDATE',N'Origen de Actualizado Por (Actualizar)',N'Fuente utilizada para registrar quién actualizó el triaje.','STRING',N'"encargadoUsuarioId"',NULL),
('SANIDAD__ENTITY__DELETED_BY_CREATE',N'Eliminado Por al Crear',N'Valor inicial del campo eliminado por al crear un triaje (nulo).','STRING','null',NULL),
('SANIDAD__ENTITY__DELETED_AT_CREATE',N'Eliminado En al Crear',N'Valor inicial del campo fecha de eliminación al crear un triaje (nulo).','STRING','null',NULL),
('SANIDAD__ENTITY__MOTIVO_ELIMINACION_CREATE',N'Motivo de Eliminación al Crear',N'Valor inicial del motivo de eliminación al crear un triaje (nulo).','STRING','null',NULL),
('SANIDAD__ENTITY__SOFT_DELETE_SET_DELETED_AT',N'Fecha de Eliminación Lógica',N'Origen del valor asignado a la fecha de eliminación lógica.','STRING',N'"utcNow"',NULL),
('SANIDAD__ENTITY__SOFT_DELETE_SET_DELETED_BY',N'Responsable de Eliminación Lógica',N'Origen del valor asignado al responsable de la eliminación lógica.','STRING',N'"actorId"',NULL),
('SANIDAD__ENTITY__SOFT_DELETE_SET_UPDATED_AT',N'Fecha de Actualización en Eliminación Lógica',N'Origen del valor asignado a la fecha de actualización al eliminar lógicamente.','STRING',N'"utcNow"',NULL),
('SANIDAD__ENTITY__SOFT_DELETE_SET_UPDATED_BY',N'Responsable de Actualización en Eliminación Lógica',N'Origen del valor asignado al responsable de la actualización al eliminar lógicamente.','STRING',N'"actorId"',NULL)

) V (Code, Name, Description, DataType, DefaultValue, ValidationSchema)
WHERE NOT EXISTS
(
    SELECT 1
    FROM setting_definitions s
    WHERE s.setting_group_id = @SettingGroupId
      AND s.code = V.Code
);

------------------------------------------------------------
-- ESTADO DE REGISTRO
------------------------------------------------------------

INSERT INTO setting_definitions
(
    setting_group_id,
    code,
    name,
    description,
    data_type,
    default_value,
    validation_schema,
    is_required,
    is_sensitive,
    is_active,
    metadata,
    created_at,
    updated_at,
    deleted_at
)
SELECT
    @SettingGroupId,
    V.Code,
    V.Name,
    V.Description,
    V.DataType,
    V.DefaultValue,
    V.ValidationSchema,
    0,
    0,
    1,
    NULL,
    SYSUTCDATETIME(),
    SYSUTCDATETIME(),
    NULL
FROM
(
VALUES

('SANIDAD__ESTADO_REGISTRO__ACTIVE_CODE',N'Código de Estado Activo',N'Código del estado de registro considerado activo para triajes.','STRING',N'"ACTIVO"',NULL),
('SANIDAD__ESTADO_REGISTRO__ACTIVE_SELECT',N'Campo Seleccionado de Estado Activo',N'Campo proyectado al consultar el estado de registro activo.','STRING',N'"code"',NULL),
('SANIDAD__ESTADO_REGISTRO__ACTIVE_REQUIRED',N'Comportamiento Estado Activo Requerido',N'Método utilizado para obligar la existencia del estado de registro activo.','STRING',N'"FirstAsync"',NULL)

) V (Code, Name, Description, DataType, DefaultValue, ValidationSchema)
WHERE NOT EXISTS
(
    SELECT 1
    FROM setting_definitions s
    WHERE s.setting_group_id = @SettingGroupId
      AND s.code = V.Code
);

------------------------------------------------------------
-- REPOSITORIO TRIAJE
------------------------------------------------------------

INSERT INTO setting_definitions
(
    setting_group_id,
    code,
    name,
    description,
    data_type,
    default_value,
    validation_schema,
    is_required,
    is_sensitive,
    is_active,
    metadata,
    created_at,
    updated_at,
    deleted_at
)
SELECT
    @SettingGroupId,
    V.Code,
    V.Name,
    V.Description,
    V.DataType,
    V.DefaultValue,
    V.ValidationSchema,
    0,
    0,
    1,
    NULL,
    SYSUTCDATETIME(),
    SYSUTCDATETIME(),
    NULL
FROM
(
VALUES

('SANIDAD__REPOSITORY__GET_BY_ID_SOFT_DELETE_FILTER',N'Filtro de Eliminación Lógica (GetById)',N'Condición aplicada al obtener un triaje por id para excluir eliminados.','STRING',N'"deleted_at == null"',NULL),
('SANIDAD__REPOSITORY__GET_BY_ID_INCLUDE',N'Include en GetById',N'Relación incluida al consultar un triaje por id.','STRING',N'"vacuno"',NULL),
('SANIDAD__REPOSITORY__GET_ALL_INCLUDE',N'Include en GetAll',N'Relación incluida al consultar el listado de triajes.','STRING',N'"vacuno"',NULL),
('SANIDAD__REPOSITORY__DELETE_SOFT_DELETE_FILTER',N'Filtro de Eliminación (Delete)',N'Condición aplicada para localizar el triaje a eliminar lógicamente.','STRING',N'"id == id && deleted_at == null"',NULL),
('SANIDAD__REPOSITORY__DELETE_NOT_FOUND_BEHAVIOR',N'Comportamiento Cuando No Existe (Delete)',N'Comportamiento del repositorio cuando el triaje a eliminar no existe.','STRING',N'"return"',NULL),
('SANIDAD__REPOSITORY__DELETE_SET_DELETED_AT',N'Origen de Fecha de Eliminación',N'Origen del valor asignado a la fecha de eliminación en el repositorio.','STRING',N'"ServerNow"',NULL),
('SANIDAD__REPOSITORY__DELETE_SET_UPDATED_AT',N'Origen de Fecha de Actualización (Delete)',N'Origen del valor asignado a la fecha de actualización al eliminar en el repositorio.','STRING',N'"ServerNow"',NULL)

) V (Code, Name, Description, DataType, DefaultValue, ValidationSchema)
WHERE NOT EXISTS
(
    SELECT 1
    FROM setting_definitions s
    WHERE s.setting_group_id = @SettingGroupId
      AND s.code = V.Code
);

------------------------------------------------------------
-- FEATURES
------------------------------------------------------------

INSERT INTO features
(
    code,
    name,
    description,
    category,
    is_active,
    metadata,
    created_at,
    updated_at,
    deleted_at
)
SELECT
    V.Code,
    V.Name,
    V.Description,
    V.Category,
    1,
    NULL,
    SYSUTCDATETIME(),
    SYSUTCDATETIME(),
    NULL
FROM
(
VALUES

(
    'MODULE_SANIDAD',
    N'Módulo Sanidad',
    N'Habilita/deshabilita el acceso completo al módulo de sanidad (listado, registro, edición, eliminación y detalle de triajes).',
    'Modules'
),
(
    'MODULE_SANIDAD_REPORT',
    N'Reportes Sanidad',
    N'Habilita/deshabilita el submódulo de reportes de sanidad (descarga de reportes de triajes en Excel y PDF).',
    'Modules'
)

) V (Code, Name, Description, Category)
WHERE NOT EXISTS
(
    SELECT 1
    FROM features f
    WHERE f.code = V.Code
);

COMMIT TRANSACTION;

END TRY
BEGIN CATCH

    IF XACT_STATE() <> 0
        ROLLBACK TRANSACTION;

    THROW;

END CATCH;
