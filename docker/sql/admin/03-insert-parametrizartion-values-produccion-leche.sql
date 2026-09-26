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
    WHERE code = 'PRODUCCION_LECHE'
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
        'PRODUCCION_LECHE',
        N'Producción de leche',
        N'Grupo único para todos los parámetros configurables del módulo.',
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
    WHERE code = 'PRODUCCION_LECHE'
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

('PRODUCCION_LECHE__LIST__DEFAULT_PAGE',N'Página por Defecto',N'Página por defecto usada en el listado de ordeños.','INT','1','{ "type":"integer","minimum":1,"maximum":100000 }'),
('PRODUCCION_LECHE__LIST__DEFAULT_PAGE_SIZE',N'Tamaño de Página por Defecto',N'Tamaño de página por defecto usado en el listado de ordeños.','INT','20','{ "type":"integer","minimum":1,"maximum":500 }'),
('PRODUCCION_LECHE__LIST__MAX_PAGE_SIZE',N'Tamaño Máximo de Página',N'Tamaño máximo de página permitido en el listado de ordeños.','INT','100','{ "type":"integer","minimum":1,"maximum":1000 }')

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

('PRODUCCION_LECHE__REPORT__COMPARATIVO_DEFAULT',N'Comparativo por Defecto',N'Indica si el reporte se genera en modo comparativo por defecto.','BOOL','false',NULL),
('PRODUCCION_LECHE__REPORT__CONTENT_TYPE',N'Content-Type del Reporte',N'Tipo de contenido MIME devuelto al descargar el reporte PDF.','STRING',N'"application/pdf"',NULL),
('PRODUCCION_LECHE__REPORT__STANDARD_FILENAME_PREFIX',N'Prefijo de Archivo Estándar',N'Prefijo usado para nombrar el archivo del reporte estándar de ordeños.','STRING',N'"reporte-ordenios"',NULL),
('PRODUCCION_LECHE__REPORT__COMPARATIVE_FILENAME_PREFIX',N'Prefijo de Archivo Comparativo',N'Prefijo usado para nombrar el archivo del reporte comparativo de ordeños.','STRING',N'"reporte-comparativo-ordenios"',NULL),
('PRODUCCION_LECHE__REPORT__TIMESTAMP_FORMAT',N'Formato de Timestamp',N'Formato de fecha/hora usado para nombrar los archivos de reporte generados.','STRING',N'"yyyyMMddHHmmss"',NULL),
('PRODUCCION_LECHE__REPORT__FILE_EXTENSION',N'Extensión de Archivo',N'Extensión de archivo usada para el reporte generado.','STRING',N'".pdf"',NULL)

) V (Code, Name, Description, DataType, DefaultValue, ValidationSchema)
WHERE NOT EXISTS
(
    SELECT 1
    FROM setting_definitions s
    WHERE s.setting_group_id = @SettingGroupId
      AND s.code = V.Code
);

------------------------------------------------------------
-- PDF TABULAR
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

('PRODUCCION_LECHE__PDF__BRAND__NAME',N'Nombre de Marca',N'Nombre de marca mostrado en el reporte PDF.','STRING',N'"ZooTech"',NULL),
('PRODUCCION_LECHE__PDF__TITLE',N'Título del PDF',N'Título mostrado en el reporte PDF de ordeños.','STRING',N'"Reporte de ordenios"',NULL),
('PRODUCCION_LECHE__PDF__EMPTY_MESSAGE',N'Mensaje Vacío',N'Mensaje mostrado cuando no existen registros para los filtros solicitados.','STRING',N'"No se encontraron registros para los filtros solicitados."',NULL),
('PRODUCCION_LECHE__PDF__FILTERS_EMPTY_LABEL',N'Etiqueta Filtros Vacíos',N'Texto mostrado cuando no se aplicó ningún filtro.','STRING',N'"Filtros: sin filtros"',NULL),
('PRODUCCION_LECHE__PDF__FILTER__VACUNOID_LABEL',N'Etiqueta Filtro VacunoId',N'Etiqueta usada para mostrar el filtro por VacunoId.','STRING',N'"VacunoId"',NULL),
('PRODUCCION_LECHE__PDF__FILTER__ESTADO_LABEL',N'Etiqueta Filtro Estado',N'Etiqueta usada para mostrar el filtro por estado.','STRING',N'"Estado"',NULL),
('PRODUCCION_LECHE__PDF__FILTER__DESDE_LABEL',N'Etiqueta Filtro Desde',N'Etiqueta usada para mostrar el filtro de fecha desde.','STRING',N'"Desde"',NULL),
('PRODUCCION_LECHE__PDF__FILTER__HASTA_LABEL',N'Etiqueta Filtro Hasta',N'Etiqueta usada para mostrar el filtro de fecha hasta.','STRING',N'"Hasta"',NULL),
('PRODUCCION_LECHE__PDF__GENERATED_AT_LABEL',N'Etiqueta de Generado',N'Etiqueta usada para mostrar la fecha de generación del PDF.','STRING',N'"Generado"',NULL),
('PRODUCCION_LECHE__PDF__PAGE_LABEL',N'Etiqueta de Página',N'Etiqueta usada para mostrar el número de página en el PDF.','STRING',N'"Pagina"',NULL),
('PRODUCCION_LECHE__PDF__PAGE_SIZE',N'Tamaño de Página',N'Tamaño de página usado para el reporte PDF estándar.','STRING',N'"A4"','{ "type":"string","enum":["A4","LETTER","LEGAL"] }'),
('PRODUCCION_LECHE__PDF__PAGE_MARGIN',N'Margen de Página',N'Margen de página usado en el reporte PDF estándar.','INT','24','{ "type":"integer","minimum":0,"maximum":200 }'),
('PRODUCCION_LECHE__PDF__DEFAULT_FONT_SIZE',N'Tamaño de Fuente por Defecto',N'Tamaño de fuente usado en el contenido del reporte PDF.','INT','10','{ "type":"integer","minimum":6,"maximum":24 }'),
('PRODUCCION_LECHE__PDF__HEADER_FONT_SIZE',N'Tamaño de Fuente del Encabezado',N'Tamaño de fuente usado en el encabezado del reporte PDF.','INT','18','{ "type":"integer","minimum":6,"maximum":48 }'),
('PRODUCCION_LECHE__PDF__META_FONT_SIZE',N'Tamaño de Fuente de Metadatos',N'Tamaño de fuente usado para los metadatos (filtros, fecha de generación) del PDF.','INT','9','{ "type":"integer","minimum":6,"maximum":24 }'),
('PRODUCCION_LECHE__PDF__CONTENT_PADDING_TOP',N'Padding Superior del Contenido',N'Espaciado superior aplicado antes del contenido del PDF.','INT','16','{ "type":"integer","minimum":0,"maximum":100 }'),
('PRODUCCION_LECHE__PDF__SECTION_PADDING',N'Padding de Sección',N'Espaciado aplicado entre secciones del PDF.','INT','16','{ "type":"integer","minimum":0,"maximum":100 }'),
('PRODUCCION_LECHE__PDF__HEADER_CELL_PADDING',N'Padding de Celda de Encabezado',N'Espaciado interno de las celdas de encabezado de la tabla del PDF.','INT','8','{ "type":"integer","minimum":0,"maximum":50 }'),
('PRODUCCION_LECHE__PDF__ROW_CELL_PADDING',N'Padding de Celda de Fila',N'Espaciado interno de las celdas de las filas de la tabla del PDF.','INT','6','{ "type":"integer","minimum":0,"maximum":50 }'),
('PRODUCCION_LECHE__PDF__THEME__PRIMARY',N'Color Primario',N'Color primario del tema del reporte PDF.','STRING',N'"#802b4d"','{ "type":"string","pattern":"^#[0-9a-fA-F]{6}$" }'),
('PRODUCCION_LECHE__PDF__THEME__PRIMARY_DARK',N'Color Primario Oscuro',N'Color primario oscuro del tema del reporte PDF.','STRING',N'"#66223e"','{ "type":"string","pattern":"^#[0-9a-fA-F]{6}$" }'),
('PRODUCCION_LECHE__PDF__THEME__PRIMARY_LIGHT',N'Color Primario Claro',N'Color primario claro del tema del reporte PDF.','STRING',N'"#ecdfe4"','{ "type":"string","pattern":"^#[0-9a-fA-F]{6}$" }'),
('PRODUCCION_LECHE__PDF__THEME__PRIMARY_LIGHTER',N'Color Primario Muy Claro',N'Color primario muy claro del tema del reporte PDF.','STRING',N'"#f9f4f6"','{ "type":"string","pattern":"^#[0-9a-fA-F]{6}$" }'),
('PRODUCCION_LECHE__PDF__THEME__BORDER_SOFT',N'Color de Borde',N'Color de borde suave del tema del reporte PDF.','STRING',N'"#d9bfca"','{ "type":"string","pattern":"^#[0-9a-fA-F]{6}$" }'),
('PRODUCCION_LECHE__PDF__THEME__TEXT_DARK',N'Color de Texto Principal',N'Color de texto principal del tema del reporte PDF.','STRING',N'"#2d2d2d"','{ "type":"string","pattern":"^#[0-9a-fA-F]{6}$" }'),
('PRODUCCION_LECHE__PDF__THEME__TEXT_MUTED',N'Color de Texto Secundario',N'Color de texto secundario (atenuado) del tema del reporte PDF.','STRING',N'"#767676"','{ "type":"string","pattern":"^#[0-9a-fA-F]{6}$" }'),
('PRODUCCION_LECHE__PDF__COLUMN__CODIGO',N'Ancho de Columna Código',N'Ancho relativo de la columna código en la tabla del PDF.','DECIMAL','1.2','{ "type":"number","minimum":0.1,"maximum":10 }'),
('PRODUCCION_LECHE__PDF__COLUMN__FECHA',N'Ancho de Columna Fecha',N'Ancho relativo de la columna fecha en la tabla del PDF.','DECIMAL','1.8','{ "type":"number","minimum":0.1,"maximum":10 }'),
('PRODUCCION_LECHE__PDF__COLUMN__VACUNO',N'Ancho de Columna Vacuno',N'Ancho relativo de la columna vacuno en la tabla del PDF.','DECIMAL','1.8','{ "type":"number","minimum":0.1,"maximum":10 }'),
('PRODUCCION_LECHE__PDF__COLUMN__LITROS',N'Ancho de Columna Litros',N'Ancho relativo de la columna litros en la tabla del PDF.','DECIMAL','1.2','{ "type":"number","minimum":0.1,"maximum":10 }'),
('PRODUCCION_LECHE__PDF__COLUMN__ESTADO',N'Ancho de Columna Estado',N'Ancho relativo de la columna estado en la tabla del PDF.','DECIMAL','1.2','{ "type":"number","minimum":0.1,"maximum":10 }'),
('PRODUCCION_LECHE__PDF__HEADER__CODIGO',N'Encabezado Código',N'Texto del encabezado de la columna código en el PDF.','STRING',N'"Codigo"',NULL),
('PRODUCCION_LECHE__PDF__HEADER__FECHA',N'Encabezado Fecha',N'Texto del encabezado de la columna fecha en el PDF.','STRING',N'"Fecha"',NULL),
('PRODUCCION_LECHE__PDF__HEADER__VACUNO',N'Encabezado Vacuno',N'Texto del encabezado de la columna vacuno en el PDF.','STRING',N'"Vacuno"',NULL),
('PRODUCCION_LECHE__PDF__HEADER__LITROS',N'Encabezado Litros',N'Texto del encabezado de la columna litros en el PDF.','STRING',N'"Litros"',NULL),
('PRODUCCION_LECHE__PDF__HEADER__ESTADO',N'Encabezado Estado',N'Texto del encabezado de la columna estado en el PDF.','STRING',N'"Estado"',NULL),
('PRODUCCION_LECHE__PDF__DATE_FORMAT',N'Formato de Fecha',N'Formato de fecha y hora usado en el contenido del PDF.','STRING',N'"yyyy-MM-dd HH:mm"',NULL),
('PRODUCCION_LECHE__PDF__DECIMAL_FORMAT',N'Formato Decimal',N'Formato numérico usado para mostrar los litros en el PDF.','STRING',N'"0.##"',NULL)

) V (Code, Name, Description, DataType, DefaultValue, ValidationSchema)
WHERE NOT EXISTS
(
    SELECT 1
    FROM setting_definitions s
    WHERE s.setting_group_id = @SettingGroupId
      AND s.code = V.Code
);

------------------------------------------------------------
-- PDF COMPARATIVO
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

('PRODUCCION_LECHE__PDF_COMPARATIVO__TITLE',N'Título del PDF Comparativo',N'Título mostrado en el reporte PDF comparativo.','STRING',N'"Reporte de Produccion de Leche"',NULL),
('PRODUCCION_LECHE__PDF_COMPARATIVO__EMPTY_MESSAGE',N'Mensaje Vacío del Comparativo',N'Mensaje mostrado cuando no hay datos de ordeños para graficar.','STRING',N'"No hay datos de ordenios para mostrar."',NULL),
('PRODUCCION_LECHE__PDF_COMPARATIVO__Y_AXIS_LABEL',N'Etiqueta Eje Y',N'Etiqueta del eje Y del gráfico comparativo.','STRING',N'"Litros (L)"',NULL),
('PRODUCCION_LECHE__PDF_COMPARATIVO__X_AXIS_LABEL',N'Etiqueta Eje X',N'Etiqueta del eje X del gráfico comparativo.','STRING',N'"Fecha"',NULL),
('PRODUCCION_LECHE__PDF_COMPARATIVO__PAGE_SIZE',N'Tamaño de Página Comparativo',N'Tamaño y orientación de página usados en el reporte PDF comparativo.','STRING',N'"A4_LANDSCAPE"',NULL),
('PRODUCCION_LECHE__PDF_COMPARATIVO__PAGE_MARGIN',N'Margen de Página Comparativo',N'Margen de página usado en el reporte PDF comparativo.','INT','30','{ "type":"integer","minimum":0,"maximum":200 }'),
('PRODUCCION_LECHE__PDF_COMPARATIVO__DEFAULT_FONT_SIZE',N'Tamaño de Fuente por Defecto (Comparativo)',N'Tamaño de fuente usado en el contenido del reporte comparativo.','INT','10','{ "type":"integer","minimum":6,"maximum":24 }'),
('PRODUCCION_LECHE__PDF_COMPARATIVO__TITLE_FONT_SIZE',N'Tamaño de Fuente del Título (Comparativo)',N'Tamaño de fuente usado en el título del reporte comparativo.','INT','18','{ "type":"integer","minimum":6,"maximum":48 }'),
('PRODUCCION_LECHE__PDF_COMPARATIVO__SPACER_HEIGHT',N'Altura del Espaciador',N'Altura del espaciador entre secciones del reporte comparativo.','INT','20','{ "type":"integer","minimum":0,"maximum":200 }'),
('PRODUCCION_LECHE__PDF_COMPARATIVO__CHART_HEIGHT',N'Altura del Gráfico',N'Altura del contenedor del gráfico comparativo.','INT','360','{ "type":"integer","minimum":1,"maximum":2000 }'),
('PRODUCCION_LECHE__PDF_COMPARATIVO__CHART_PADDING',N'Padding del Gráfico',N'Espaciado interno del contenedor del gráfico comparativo.','INT','15','{ "type":"integer","minimum":0,"maximum":200 }'),
('PRODUCCION_LECHE__PDF_COMPARATIVO__CHART_BORDER_WIDTH',N'Grosor de Borde del Gráfico',N'Grosor del borde del contenedor del gráfico comparativo.','INT','1','{ "type":"integer","minimum":0,"maximum":20 }'),
('PRODUCCION_LECHE__PDF_COMPARATIVO__CHART_MARGIN_LEFT',N'Margen Izquierdo del Gráfico',N'Margen izquierdo del área de dibujo del gráfico comparativo.','INT','55','{ "type":"integer","minimum":0,"maximum":300 }'),
('PRODUCCION_LECHE__PDF_COMPARATIVO__CHART_MARGIN_RIGHT',N'Margen Derecho del Gráfico',N'Margen derecho del área de dibujo del gráfico comparativo.','INT','25','{ "type":"integer","minimum":0,"maximum":300 }'),
('PRODUCCION_LECHE__PDF_COMPARATIVO__CHART_MARGIN_TOP',N'Margen Superior del Gráfico',N'Margen superior del área de dibujo del gráfico comparativo.','INT','35','{ "type":"integer","minimum":0,"maximum":300 }'),
('PRODUCCION_LECHE__PDF_COMPARATIVO__CHART_MARGIN_BOTTOM',N'Margen Inferior del Gráfico',N'Margen inferior del área de dibujo del gráfico comparativo.','INT','70','{ "type":"integer","minimum":0,"maximum":300 }'),
('PRODUCCION_LECHE__PDF_COMPARATIVO__MIN_Y',N'Valor Mínimo del Eje Y',N'Valor mínimo mostrado en el eje Y del gráfico comparativo.','DECIMAL','0','{ "type":"number","minimum":0,"maximum":100000 }'),
('PRODUCCION_LECHE__PDF_COMPARATIVO__MIN_Y_AXIS_MAX',N'Valor Máximo Mínimo del Eje Y',N'Valor máximo mínimo considerado al escalar automáticamente el eje Y.','DECIMAL','30','{ "type":"number","minimum":0,"maximum":100000 }'),
('PRODUCCION_LECHE__PDF_COMPARATIVO__Y_AXIS_ROUND_STEP',N'Escalón de Redondeo del Eje Y',N'Escalón usado para redondear los valores del eje Y del gráfico comparativo.','DECIMAL','5','{ "type":"number","minimum":0.1,"maximum":1000 }'),
('PRODUCCION_LECHE__PDF_COMPARATIVO__GRID_STEPS',N'Pasos de Grilla',N'Cantidad de divisiones de la grilla del gráfico comparativo.','INT','6','{ "type":"integer","minimum":1,"maximum":50 }'),
('PRODUCCION_LECHE__PDF_COMPARATIVO__LABEL_DIVISOR',N'Divisor de Densidad de Etiquetas',N'Divisor usado para reducir la densidad de etiquetas del eje X.','INT','6','{ "type":"integer","minimum":1,"maximum":50 }'),
('PRODUCCION_LECHE__PDF_COMPARATIVO__TREND_WINDOW',N'Ventana de Tendencia',N'Cantidad de puntos usados para calcular la media móvil de tendencia.','INT','4','{ "type":"integer","minimum":1,"maximum":50 }'),
('PRODUCCION_LECHE__PDF_COMPARATIVO__POINT_OUTER_RADIUS',N'Radio Exterior de Punto',N'Radio exterior de los puntos de datos del gráfico comparativo.','INT','5','{ "type":"integer","minimum":1,"maximum":50 }'),
('PRODUCCION_LECHE__PDF_COMPARATIVO__POINT_INNER_RADIUS',N'Radio Interior de Punto',N'Radio interior de los puntos de datos del gráfico comparativo.','INT','4','{ "type":"integer","minimum":1,"maximum":50 }'),
('PRODUCCION_LECHE__PDF_COMPARATIVO__LINE_STROKE',N'Grosor de Línea',N'Grosor de la línea principal del gráfico comparativo.','INT','3','{ "type":"integer","minimum":1,"maximum":20 }'),
('PRODUCCION_LECHE__PDF_COMPARATIVO__TREND_STROKE',N'Grosor de Línea de Tendencia',N'Grosor de la línea de tendencia del gráfico comparativo.','INT','3','{ "type":"integer","minimum":1,"maximum":20 }'),
('PRODUCCION_LECHE__PDF_COMPARATIVO__GRID_STROKE',N'Grosor de Grilla',N'Grosor de las líneas de la grilla del gráfico comparativo.','INT','1','{ "type":"integer","minimum":1,"maximum":20 }'),
('PRODUCCION_LECHE__PDF_COMPARATIVO__AXIS_STROKE',N'Grosor de Eje',N'Grosor de las líneas de los ejes del gráfico comparativo.','INT','1','{ "type":"integer","minimum":1,"maximum":20 }'),
('PRODUCCION_LECHE__PDF_COMPARATIVO__THEME__PRIMARY',N'Color Primario (Comparativo)',N'Color primario del tema del reporte comparativo.','STRING',N'"#802b4d"','{ "type":"string","pattern":"^#[0-9a-fA-F]{6}$" }'),
('PRODUCCION_LECHE__PDF_COMPARATIVO__THEME__PRIMARY_DARK',N'Color Primario Oscuro (Comparativo)',N'Color primario oscuro del tema del reporte comparativo.','STRING',N'"#66223e"','{ "type":"string","pattern":"^#[0-9a-fA-F]{6}$" }'),
('PRODUCCION_LECHE__PDF_COMPARATIVO__THEME__PRIMARY_LIGHT',N'Color Primario Claro (Comparativo)',N'Color primario claro del tema del reporte comparativo.','STRING',N'"#ecdfe4"','{ "type":"string","pattern":"^#[0-9a-fA-F]{6}$" }'),
('PRODUCCION_LECHE__PDF_COMPARATIVO__THEME__PRIMARY_LIGHTER',N'Color Primario Muy Claro (Comparativo)',N'Color primario muy claro del tema del reporte comparativo.','STRING',N'"#f9f4f6"','{ "type":"string","pattern":"^#[0-9a-fA-F]{6}$" }'),
('PRODUCCION_LECHE__PDF_COMPARATIVO__THEME__GRID_COLOR',N'Color de Grilla (RGB)',N'Color RGB de las líneas de la grilla del gráfico comparativo.','ARRAY','[210,220,235]','{ "type":"array","items":{"type":"integer","minimum":0,"maximum":255},"minItems":3,"maxItems":3 }'),
('PRODUCCION_LECHE__PDF_COMPARATIVO__THEME__AXIS_COLOR',N'Color de Eje (RGB)',N'Color RGB de las líneas de los ejes del gráfico comparativo.','ARRAY','[190,205,220]','{ "type":"array","items":{"type":"integer","minimum":0,"maximum":255},"minItems":3,"maxItems":3 }'),
('PRODUCCION_LECHE__PDF_COMPARATIVO__THEME__TEXT_COLOR',N'Color de Texto (RGB)',N'Color RGB de los textos del gráfico comparativo.','ARRAY','[40,60,90]','{ "type":"array","items":{"type":"integer","minimum":0,"maximum":255},"minItems":3,"maxItems":3 }'),
('PRODUCCION_LECHE__PDF_COMPARATIVO__THEME__TITLE_COLOR',N'Color de Título (RGB)',N'Color RGB del título del gráfico comparativo.','ARRAY','[20,45,75]','{ "type":"array","items":{"type":"integer","minimum":0,"maximum":255},"minItems":3,"maxItems":3 }'),
('PRODUCCION_LECHE__PDF_COMPARATIVO__THEME__AREA_COLOR',N'Color de Área (RGBA)',N'Color RGBA del área bajo la línea del gráfico comparativo.','ARRAY','[130,40,80,45]','{ "type":"array","items":{"type":"integer","minimum":0,"maximum":255},"minItems":4,"maxItems":4 }'),
('PRODUCCION_LECHE__PDF_COMPARATIVO__THEME__LINE_COLOR',N'Color de Línea (RGB)',N'Color RGB de la línea principal del gráfico comparativo.','ARRAY','[130,40,80]','{ "type":"array","items":{"type":"integer","minimum":0,"maximum":255},"minItems":3,"maxItems":3 }'),
('PRODUCCION_LECHE__PDF_COMPARATIVO__THEME__TREND_COLOR',N'Color de Tendencia (RGB)',N'Color RGB de la línea de tendencia del gráfico comparativo.','ARRAY','[105,35,185]','{ "type":"array","items":{"type":"integer","minimum":0,"maximum":255},"minItems":3,"maxItems":3 }'),
('PRODUCCION_LECHE__PDF_COMPARATIVO__GRID_DASH',N'Patrón de Línea de Grilla',N'Patrón de guiones usado para dibujar la grilla del gráfico comparativo.','ARRAY','[6,5]','{ "type":"array","items":{"type":"number"},"minItems":2 }'),
('PRODUCCION_LECHE__PDF_COMPARATIVO__TREND_DASH',N'Patrón de Línea de Tendencia',N'Patrón de guiones usado para dibujar la línea de tendencia del gráfico comparativo.','ARRAY','[10,5]','{ "type":"array","items":{"type":"number"},"minItems":2 }'),
('PRODUCCION_LECHE__PDF_COMPARATIVO__DATE_LABEL_FORMAT',N'Formato de Etiqueta de Fecha',N'Formato de fecha usado en las etiquetas del eje X del gráfico comparativo.','STRING',N'"dd/MM/yyyy"',NULL)

) V (Code, Name, Description, DataType, DefaultValue, ValidationSchema)
WHERE NOT EXISTS
(
    SELECT 1
    FROM setting_definitions s
    WHERE s.setting_group_id = @SettingGroupId
      AND s.code = V.Code
);

------------------------------------------------------------
-- REGLAS DEL MODULO
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

('PRODUCCION_LECHE__RULES__CODIGO_MAX_LENGTH_DOMAIN',N'Longitud Máxima de Código (Dominio)',N'Longitud máxima del código de ordeño validada por la regla de dominio.','INT','15','{ "type":"integer","minimum":1,"maximum":200 }'),
('PRODUCCION_LECHE__RULES__CODIGO_MAX_LENGTH_VALIDATOR',N'Longitud Máxima de Código (Validador)',N'Longitud máxima del código de ordeño validada por el validador de creación.','INT','50','{ "type":"integer","minimum":1,"maximum":200 }'),
('PRODUCCION_LECHE__RULES__ESTADO_MAX_LENGTH',N'Longitud Máxima de Estado',N'Longitud máxima permitida para el estado del ordeño.','INT','30','{ "type":"integer","minimum":1,"maximum":200 }'),
('PRODUCCION_LECHE__RULES__OBSERVACIONES_MAX_LENGTH',N'Longitud Máxima de Observaciones',N'Longitud máxima permitida para las observaciones del ordeño.','INT','150','{ "type":"integer","minimum":1,"maximum":1000 }'),
('PRODUCCION_LECHE__RULES__MOTIVO_ELIMINACION_MAX_LENGTH',N'Longitud Máxima de Motivo de Eliminación',N'Longitud máxima permitida para el motivo de eliminación del ordeño.','INT','200','{ "type":"integer","minimum":1,"maximum":1000 }'),
('PRODUCCION_LECHE__RULES__LITROS_MIN',N'Mínimo de Litros',N'Condición de dominio que exige que los litros registrados sean mayores a cero.','STRING',N'"> 0"',NULL),
('PRODUCCION_LECHE__RULES__VACUNOID_MIN',N'Mínimo de VacunoId',N'Condición de dominio que exige que el identificador del vacuno sea mayor a cero.','STRING',N'"> 0"',NULL),
('PRODUCCION_LECHE__RULES__ENCARGADOUSUARIOID_MIN',N'Mínimo de EncargadoUsuarioId',N'Condición de dominio que exige que el identificador del usuario encargado sea mayor a cero.','STRING',N'"> 0"',NULL)

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
    'MODULE_PRODUCCION_LECHE',
    N'Módulo Producción de Leche',
    N'Habilita/deshabilita el acceso completo al módulo de producción de leche (listado, registro, edición, eliminación, ver detalle).',
    'Modules'
),
(
    'MODULE_PRODUCCION_LECHE_REPORTS',
    N'Reportes Producción de Leche',
    N'Habilita/deshabilita el submódulo de reportes y gráficos (listado de producción de leche, registro por producción de leche, etc.).',
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
