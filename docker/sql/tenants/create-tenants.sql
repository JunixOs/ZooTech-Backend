/*
===============================================================================
ZooTech - Base de datos Tenants
SQL Server 2025
===============================================================================

Origen:
    Diagrama DBML de ZooTech.

Objetivo:
    Crear una base de datos Tenants LIMPIA.

Importante:
    - El modelo original fue expresado con tipos PostgreSQL.
    - Se adaptaron a SQL Server:
        boolean   -> bit
        timestamptz -> datetime2(7)
        jsonb     -> nvarchar(max) + ISJSON cuando corresponde
        inet      -> varchar(45)
        numeric   -> decimal
    - El diagrama define estructura, no datos iniciales.
    - Las reglas complejas indicadas como "trigger" o reglas de negocio
      deberán implementarse posteriormente en la aplicación o mediante
      triggers específicos.

===============================================================================
*/

IF DB_ID(N'ZooTechTenants') IS NULL
BEGIN
    CREATE DATABASE ZooTechTenants;
END
GO

USE ZooTechTenants;
GO

/*
===============================================================================
01. CATÁLOGOS / SEGURIDAD / REPORTES
===============================================================================
*/

CREATE TABLE dbo.usuario
(
    id              bigint IDENTITY(1,1) NOT NULL,
    codigo          varchar(15) NOT NULL,
    nombre_usuario  varchar(50) NOT NULL,
    nombre_completo varchar(100) NOT NULL,
    correo          varchar(120) NULL,
    activo          bit NOT NULL CONSTRAINT DF_usuario_activo DEFAULT 1,
    created_at      datetime2(7) NOT NULL CONSTRAINT DF_usuario_created_at DEFAULT SYSUTCDATETIME(),
    updated_at      datetime2(7) NOT NULL CONSTRAINT DF_usuario_updated_at DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_usuario PRIMARY KEY (id),
    CONSTRAINT UQ_usuario_codigo UNIQUE (codigo),
    CONSTRAINT UQ_usuario_nombre_usuario UNIQUE (nombre_usuario),
    CONSTRAINT UQ_usuario_correo UNIQUE (correo)
);
GO

CREATE TABLE dbo.cat_modulo
(
    code        varchar(40) NOT NULL,
    nombre      varchar(80) NOT NULL,
    descripcion varchar(250) NULL,
    activo      bit NOT NULL CONSTRAINT DF_cat_modulo_activo DEFAULT 1,

    CONSTRAINT PK_cat_modulo PRIMARY KEY (code),
    CONSTRAINT UQ_cat_modulo_nombre UNIQUE (nombre)
);
GO

CREATE TABLE dbo.cat_estado_registro
(
    code        varchar(30) NOT NULL,
    nombre      varchar(50) NOT NULL,
    descripcion varchar(150) NULL,

    CONSTRAINT PK_cat_estado_registro PRIMARY KEY (code),
    CONSTRAINT UQ_cat_estado_registro_nombre UNIQUE (nombre)
);
GO

CREATE TABLE dbo.cat_formato_reporte
(
    code   varchar(10) NOT NULL,
    nombre varchar(20) NOT NULL,

    CONSTRAINT PK_cat_formato_reporte PRIMARY KEY (code),
    CONSTRAINT UQ_cat_formato_reporte_nombre UNIQUE (nombre)
);
GO

CREATE TABLE dbo.cat_tipo_reporte
(
    code        varchar(60) NOT NULL,
    modulo_code varchar(40) NOT NULL,
    nombre      varchar(120) NOT NULL,
    descripcion varchar(300) NULL,
    activo      bit NOT NULL CONSTRAINT DF_cat_tipo_reporte_activo DEFAULT 1,

    CONSTRAINT PK_cat_tipo_reporte PRIMARY KEY (code),
    CONSTRAINT UQ_cat_tipo_reporte_modulo_nombre
        UNIQUE (modulo_code, nombre)
);
GO

CREATE TABLE dbo.archivo
(
    id              bigint IDENTITY(1,1) NOT NULL,
    modulo_code     varchar(40) NOT NULL,
    nombre_original varchar(150) NOT NULL,
    nombre_almacenado varchar(150) NOT NULL,
    ruta_archivo    varchar(max) NOT NULL,
    extension       varchar(10) NOT NULL,
    mime_type       varchar(80) NOT NULL,
    tamano_bytes    bigint NULL,
    hash_sha256     varchar(64) NULL,
    creado_por      bigint NULL,
    created_at      datetime2(7) NOT NULL CONSTRAINT DF_archivo_created_at DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_archivo PRIMARY KEY (id)
);
GO

CREATE TABLE dbo.reporte_descarga
(
    id               bigint IDENTITY(1,1) NOT NULL,
    tipo_reporte_code varchar(60) NOT NULL,
    formato_code      varchar(10) NOT NULL,
    fecha_inicio      date NOT NULL,
    fecha_fin         date NOT NULL,
    palabra_clave     varchar(50) NULL,
    filtros           nvarchar(max) NOT NULL,
    vacuno_id         bigint NULL,
    archivo_id        bigint NULL,
    estado            varchar(20) NOT NULL CONSTRAINT DF_reporte_estado DEFAULT 'GENERADO',
    mensaje           varchar(150) NULL,
    solicitado_por    bigint NULL,
    created_at        datetime2(7) NOT NULL CONSTRAINT DF_reporte_created_at DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_reporte_descarga PRIMARY KEY (id),
    CONSTRAINT CK_reporte_descarga_filtros_json CHECK (ISJSON(filtros) = 1)
);
GO

CREATE TABLE dbo.parametro_sistema
(
    id          bigint IDENTITY(1,1) NOT NULL,
    modulo_code varchar(40) NOT NULL,
    clave       varchar(80) NOT NULL,
    valor       varchar(150) NOT NULL,
    descripcion varchar(250) NULL,
    vigente     bit NOT NULL CONSTRAINT DF_parametro_vigente DEFAULT 1,
    created_at  datetime2(7) NOT NULL CONSTRAINT DF_parametro_created_at DEFAULT SYSUTCDATETIME(),
    updated_at  datetime2(7) NOT NULL CONSTRAINT DF_parametro_updated_at DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_parametro_sistema PRIMARY KEY (id)
);
GO

CREATE TABLE dbo.bitacora_auditoria
(
    id              bigint IDENTITY(1,1) NOT NULL,
    modulo_code     varchar(40) NOT NULL,
    entidad         varchar(80) NOT NULL,
    entidad_id      bigint NOT NULL,
    accion          varchar(40) NOT NULL,
    datos_anteriores nvarchar(max) NULL,
    datos_nuevos    nvarchar(max) NULL,
    motivo          varchar(250) NULL,
    usuario_id      bigint NULL,
    ip_origen       varchar(45) NULL,
    user_agent      varchar(250) NULL,
    created_at      datetime2(7) NOT NULL CONSTRAINT DF_bitacora_created_at DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_bitacora_auditoria PRIMARY KEY (id),
    CONSTRAINT CK_bitacora_datos_anteriores_json
        CHECK (datos_anteriores IS NULL OR ISJSON(datos_anteriores) = 1),
    CONSTRAINT CK_bitacora_datos_nuevos_json
        CHECK (datos_nuevos IS NULL OR ISJSON(datos_nuevos) = 1)
);
GO

CREATE TABLE dbo.cat_tipo_responsable
(
    code        varchar(30) NOT NULL,
    nombre      varchar(50) NOT NULL,
    descripcion varchar(150) NULL,

    CONSTRAINT PK_cat_tipo_responsable PRIMARY KEY (code),
    CONSTRAINT UQ_cat_tipo_responsable_nombre UNIQUE (nombre)
);
GO

CREATE TABLE dbo.responsable
(
    id                    bigint IDENTITY(1,1) NOT NULL,
    usuario_id            bigint NULL,
    tipo_responsable_code varchar(30) NOT NULL,
    nombre_completo       varchar(100) NOT NULL,
    documento             varchar(30) NULL,
    telefono              varchar(20) NULL,
    activo                bit NOT NULL CONSTRAINT DF_responsable_activo DEFAULT 1,
    created_at            datetime2(7) NOT NULL CONSTRAINT DF_responsable_created_at DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_responsable PRIMARY KEY (id)
);
GO

/*
===============================================================================
02. UBICACIÓN / CATÁLOGOS DE VACUNOS
===============================================================================
*/

CREATE TABLE dbo.cat_sexo
(
    code        varchar(10) NOT NULL,
    nombre      varchar(15) NOT NULL,
    descripcion varchar(150) NULL,

    CONSTRAINT PK_cat_sexo PRIMARY KEY (code),
    CONSTRAINT UQ_cat_sexo_nombre UNIQUE (nombre)
);
GO

CREATE TABLE dbo.cat_raza
(
    code   varchar(30) NOT NULL,
    nombre varchar(50) NOT NULL,
    activo bit NOT NULL CONSTRAINT DF_cat_raza_activo DEFAULT 1,

    CONSTRAINT PK_cat_raza PRIMARY KEY (code),
    CONSTRAINT UQ_cat_raza_nombre UNIQUE (nombre)
);
GO

CREATE TABLE dbo.cat_color
(
    id     bigint IDENTITY(1,1) NOT NULL,
    nombre varchar(15) NOT NULL,
    activo bit NOT NULL CONSTRAINT DF_cat_color_activo DEFAULT 1,

    CONSTRAINT PK_cat_color PRIMARY KEY (id),
    CONSTRAINT UQ_cat_color_nombre UNIQUE (nombre)
);
GO

CREATE TABLE dbo.cat_tipo_adquisicion
(
    code        varchar(30) NOT NULL,
    nombre      varchar(50) NOT NULL,
    descripcion varchar(150) NULL,
    activo      bit NOT NULL CONSTRAINT DF_cat_tipo_adquisicion_activo DEFAULT 1,

    CONSTRAINT PK_cat_tipo_adquisicion PRIMARY KEY (code),
    CONSTRAINT UQ_cat_tipo_adquisicion_nombre UNIQUE (nombre)
);
GO

CREATE TABLE dbo.cat_tipo_utilizacion
(
    code        varchar(30) NOT NULL,
    nombre      varchar(50) NOT NULL,
    descripcion varchar(150) NULL,
    activo      bit NOT NULL CONSTRAINT DF_cat_tipo_utilizacion_activo DEFAULT 1,

    CONSTRAINT PK_cat_tipo_utilizacion PRIMARY KEY (code),
    CONSTRAINT UQ_cat_tipo_utilizacion_nombre UNIQUE (nombre)
);
GO

CREATE TABLE dbo.cat_estado_vacuno
(
    code        varchar(15) NOT NULL,
    nombre      varchar(15) NOT NULL,
    descripcion varchar(150) NULL,

    CONSTRAINT PK_cat_estado_vacuno PRIMARY KEY (code),
    CONSTRAINT UQ_cat_estado_vacuno_nombre UNIQUE (nombre)
);
GO

CREATE TABLE dbo.geo_departamento
(
    codigo varchar(2) NOT NULL,
    nombre varchar(60) NOT NULL,

    CONSTRAINT PK_geo_departamento PRIMARY KEY (codigo),
    CONSTRAINT UQ_geo_departamento_nombre UNIQUE (nombre)
);
GO

CREATE TABLE dbo.geo_provincia
(
    codigo               varchar(4) NOT NULL,
    departamento_codigo  varchar(2) NOT NULL,
    nombre               varchar(60) NOT NULL,

    CONSTRAINT PK_geo_provincia PRIMARY KEY (codigo),
    CONSTRAINT UQ_geo_provincia_departamento_nombre
        UNIQUE (departamento_codigo, nombre)
);
GO

CREATE TABLE dbo.geo_distrito
(
    codigo          varchar(6) NOT NULL,
    provincia_codigo varchar(4) NOT NULL,
    nombre          varchar(60) NOT NULL,

    CONSTRAINT PK_geo_distrito PRIMARY KEY (codigo),
    CONSTRAINT UQ_geo_distrito_provincia_nombre
        UNIQUE (provincia_codigo, nombre)
);
GO

CREATE TABLE dbo.granja
(
    id              bigint IDENTITY(1,1) NOT NULL,
    nombre          varchar(15) NOT NULL,
    distrito_codigo varchar(6) NOT NULL,
    activo          bit NOT NULL CONSTRAINT DF_granja_activo DEFAULT 1,
    created_at      datetime2(7) NOT NULL CONSTRAINT DF_granja_created_at DEFAULT SYSUTCDATETIME(),
    updated_at      datetime2(7) NOT NULL CONSTRAINT DF_granja_updated_at DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_granja PRIMARY KEY (id),
    CONSTRAINT UQ_granja_nombre_distrito UNIQUE (nombre, distrito_codigo)
);
GO

/*
===============================================================================
03. MODULO VACUNOS
===============================================================================
*/

CREATE TABLE dbo.vacuno
(
    id                       bigint IDENTITY(1,1) NOT NULL,
    codigo                   varchar(15) NOT NULL,
    nombre                   varchar(15) NOT NULL,
    fecha_nacimiento         date NOT NULL,
    tipo_adquisicion_code    varchar(30) NOT NULL,
    precio_compra            decimal(12,2) NOT NULL CONSTRAINT DF_vacuno_precio DEFAULT 0,
    raza_code                varchar(30) NOT NULL,
    color_id                 bigint NOT NULL,
    sexo_code                varchar(10) NOT NULL,
    padre_id                 bigint NULL,
    madre_id                 bigint NULL,
    granja_id                bigint NOT NULL,
    tipo_utilizacion_code    varchar(30) NOT NULL,
    fecha_registro_utilizacion date NOT NULL,
    observaciones            varchar(150) NULL,
    fecha_registro           date NOT NULL,
    created_by               bigint NULL,
    updated_by               bigint NULL,
    deleted_by               bigint NULL,
    created_at               datetime2(7) NOT NULL CONSTRAINT DF_vacuno_created_at DEFAULT SYSUTCDATETIME(),
    updated_at               datetime2(7) NOT NULL CONSTRAINT DF_vacuno_updated_at DEFAULT SYSUTCDATETIME(),
    deleted_at               datetime2(7) NULL,
    motivo_eliminacion       varchar(250) NULL,

    CONSTRAINT PK_vacuno PRIMARY KEY (id),
    CONSTRAINT UQ_vacuno_codigo UNIQUE (codigo),
    CONSTRAINT CK_vacuno_precio CHECK (precio_compra >= 0),
    CONSTRAINT CK_vacuno_padre_diferente CHECK (padre_id IS NULL OR padre_id <> id),
    CONSTRAINT CK_vacuno_madre_diferente CHECK (madre_id IS NULL OR madre_id <> id)
);
GO

CREATE TABLE dbo.vacuno_foto
(
    id           bigint IDENTITY(1,1) NOT NULL,
    vacuno_id    bigint NOT NULL,
    archivo_id   bigint NOT NULL,
    es_principal bit NOT NULL CONSTRAINT DF_vacuno_foto_principal DEFAULT 1,
    uploaded_by  bigint NULL,
    created_at   datetime2(7) NOT NULL CONSTRAINT DF_vacuno_foto_created DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_vacuno_foto PRIMARY KEY (id)
);
GO

CREATE TABLE dbo.vacuno_estado_historial
(
    id           bigint IDENTITY(1,1) NOT NULL,
    vacuno_id    bigint NOT NULL,
    estado_code  varchar(15) NOT NULL,
    fecha_estado date NOT NULL,
    motivo       varchar(150) NULL,
    vigente      bit NOT NULL CONSTRAINT DF_vacuno_estado_vigente DEFAULT 1,
    created_by   bigint NULL,
    created_at   datetime2(7) NOT NULL CONSTRAINT DF_vacuno_estado_created DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_vacuno_estado_historial PRIMARY KEY (id)
);
GO

CREATE TABLE dbo.incidente_vacuno
(
    id                  bigint IDENTITY(1,1) NOT NULL,
    vacuno_id           bigint NOT NULL,
    fecha_incidente     date NOT NULL,
    descripcion         varchar(150) NOT NULL,
    estado_registro_code varchar(30) NOT NULL CONSTRAINT DF_incidente_estado DEFAULT 'ACTIVO',
    created_by          bigint NULL,
    created_at           datetime2(7) NOT NULL CONSTRAINT DF_incidente_created DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_incidente_vacuno PRIMARY KEY (id)
);
GO

/*
===============================================================================
04. PRODUCCIÓN DE LECHE
===============================================================================
*/

CREATE TABLE dbo.cat_estado_ordenio
(
    code        varchar(30) NOT NULL,
    nombre      varchar(50) NOT NULL,
    descripcion varchar(150) NULL,

    CONSTRAINT PK_cat_estado_ordenio PRIMARY KEY (code),
    CONSTRAINT UQ_cat_estado_ordenio_nombre UNIQUE (nombre)
);
GO

CREATE TABLE dbo.ordenio
(
    id                    bigint IDENTITY(1,1) NOT NULL,
    codigo                varchar(15) NOT NULL,
    fecha_hora            datetime2(7) NOT NULL,
    vacuno_id             bigint NOT NULL,
    encargado_usuario_id  bigint NOT NULL,
    litros                decimal(8,3) NOT NULL,
    observaciones         varchar(150) NULL,
    estado_ordenio_code   varchar(30) NOT NULL CONSTRAINT DF_ordenio_estado DEFAULT 'ACTIVO',
    created_by            bigint NULL,
    updated_by            bigint NULL,
    deleted_by            bigint NULL,
    created_at            datetime2(7) NOT NULL CONSTRAINT DF_ordenio_created DEFAULT SYSUTCDATETIME(),
    updated_at            datetime2(7) NOT NULL CONSTRAINT DF_ordenio_updated DEFAULT SYSUTCDATETIME(),
    deleted_at            datetime2(7) NULL,
    motivo_eliminacion    varchar(250) NULL,

    CONSTRAINT PK_ordenio PRIMARY KEY (id),
    CONSTRAINT UQ_ordenio_codigo UNIQUE (codigo),
    CONSTRAINT UQ_ordenio_vacuno_fecha UNIQUE (vacuno_id, fecha_hora),
    CONSTRAINT CK_ordenio_litros CHECK (litros > 0)
);
GO

CREATE TABLE dbo.produccion_leche_estandar
(
    id                   bigint IDENTITY(1,1) NOT NULL,
    vacuno_id            bigint NULL,
    fecha_inicio         date NOT NULL,
    fecha_fin            date NULL,
    litros_esperados_dia decimal(8,3) NOT NULL,
    descripcion          varchar(150) NULL,
    activo               bit NOT NULL CONSTRAINT DF_produccion_estandar_activo DEFAULT 1,
    created_by           bigint NULL,
    created_at           datetime2(7) NOT NULL CONSTRAINT DF_produccion_estandar_created DEFAULT SYSUTCDATETIME(),
    updated_at           datetime2(7) NOT NULL CONSTRAINT DF_produccion_estandar_updated DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_produccion_leche_estandar PRIMARY KEY (id),
    CONSTRAINT CK_produccion_estandar_litros CHECK (litros_esperados_dia > 0),
    CONSTRAINT CK_produccion_estandar_fechas CHECK (fecha_fin IS NULL OR fecha_fin >= fecha_inicio)
);
GO

CREATE TABLE dbo.cat_estado_sequia
(
    code        varchar(30) NOT NULL,
    nombre      varchar(50) NOT NULL,
    descripcion varchar(150) NULL,

    CONSTRAINT PK_cat_estado_sequia PRIMARY KEY (code),
    CONSTRAINT UQ_cat_estado_sequia_nombre UNIQUE (nombre)
);
GO

CREATE TABLE dbo.periodo_sequia
(
    id                 bigint IDENTITY(1,1) NOT NULL,
    vacuno_id          bigint NOT NULL,
    fecha_inicio       date NOT NULL,
    fecha_fin_estimada date NULL,
    fecha_fin_real     date NULL,
    estado_sequia_code varchar(30) NOT NULL,
    motivo             varchar(150) NULL,
    observaciones      varchar(150) NULL,
    created_by         bigint NULL,
    updated_by         bigint NULL,
    created_at         datetime2(7) NOT NULL CONSTRAINT DF_periodo_sequia_created DEFAULT SYSUTCDATETIME(),
    updated_at         datetime2(7) NOT NULL CONSTRAINT DF_periodo_sequia_updated DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_periodo_sequia PRIMARY KEY (id)
);
GO

/*
===============================================================================
05. SANIDAD / TRIAJE
===============================================================================
*/

CREATE TABLE dbo.cat_tipo_peso
(
    code        varchar(30) NOT NULL,
    nombre      varchar(50) NOT NULL,
    descripcion varchar(150) NULL,
    activo      bit NOT NULL CONSTRAINT DF_cat_tipo_peso_activo DEFAULT 1,

    CONSTRAINT PK_cat_tipo_peso PRIMARY KEY (code),
    CONSTRAINT UQ_cat_tipo_peso_nombre UNIQUE (nombre)
);
GO

CREATE TABLE dbo.triaje
(
    id                    bigint IDENTITY(1,1) NOT NULL,
    codigo                varchar(15) NOT NULL,
    fecha_hora            datetime2(7) NOT NULL,
    vacuno_id             bigint NOT NULL,
    tipo_peso_code        varchar(30) NOT NULL,
    peso_kg               decimal(8,2) NOT NULL,
    observaciones         varchar(150) NULL,
    estado_registro_code  varchar(30) NOT NULL CONSTRAINT DF_triaje_estado DEFAULT 'ACTIVO',
    encargado_usuario_id  bigint NULL,
    created_by            bigint NULL,
    updated_by            bigint NULL,
    deleted_by            bigint NULL,
    created_at            datetime2(7) NOT NULL CONSTRAINT DF_triaje_created DEFAULT SYSUTCDATETIME(),
    updated_at            datetime2(7) NOT NULL CONSTRAINT DF_triaje_updated DEFAULT SYSUTCDATETIME(),
    deleted_at            datetime2(7) NULL,
    motivo_eliminacion    varchar(250) NULL,

    CONSTRAINT PK_triaje PRIMARY KEY (id),
    CONSTRAINT UQ_triaje_codigo UNIQUE (codigo),
    CONSTRAINT UQ_triaje_vacuno_fecha_tipo UNIQUE (vacuno_id, fecha_hora, tipo_peso_code),
    CONSTRAINT CK_triaje_peso CHECK (peso_kg > 0)
);
GO

/*
===============================================================================
06. REPRODUCCIÓN / CELO
===============================================================================
*/

CREATE TABLE dbo.celo_configuracion
(
    id                       bigint IDENTITY(1,1) NOT NULL,
    nombre                   varchar(80) NOT NULL,
    dias_ciclo_estandar      int NOT NULL,
    dias_alerta_previa       int NOT NULL,
    dias_tolerancia_posterior int NOT NULL,
    activo                   bit NOT NULL CONSTRAINT DF_celo_config_activo DEFAULT 1,
    created_by               bigint NULL,
    created_at               datetime2(7) NOT NULL CONSTRAINT DF_celo_config_created DEFAULT SYSUTCDATETIME(),
    updated_at               datetime2(7) NOT NULL CONSTRAINT DF_celo_config_updated DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_celo_configuracion PRIMARY KEY (id),
    CONSTRAINT CK_celo_config_dias CHECK (
        dias_ciclo_estandar > 0
        AND dias_alerta_previa >= 0
        AND dias_tolerancia_posterior >= 0
    )
);
GO

CREATE TABLE dbo.cat_estado_celo
(
    code        varchar(30) NOT NULL,
    nombre      varchar(50) NOT NULL,
    descripcion varchar(150) NULL,

    CONSTRAINT PK_cat_estado_celo PRIMARY KEY (code),
    CONSTRAINT UQ_cat_estado_celo_nombre UNIQUE (nombre)
);
GO

CREATE TABLE dbo.cat_caracteristica_celo
(
    id          bigint IDENTITY(1,1) NOT NULL,
    nombre      varchar(50) NOT NULL,
    descripcion varchar(150) NULL,
    activo      bit NOT NULL CONSTRAINT DF_cat_caracteristica_celo_activo DEFAULT 1,

    CONSTRAINT PK_cat_caracteristica_celo PRIMARY KEY (id),
    CONSTRAINT UQ_cat_caracteristica_celo_nombre UNIQUE (nombre)
);
GO

CREATE TABLE dbo.celo_registro
(
    id                   bigint IDENTITY(1,1) NOT NULL,
    codigo               varchar(15) NOT NULL,
    fecha_hora           datetime2(7) NOT NULL,
    vacuno_id            bigint NOT NULL,
    encargado_usuario_id bigint NOT NULL,
    observaciones        varchar(150) NULL,
    estado_registro_code varchar(30) NOT NULL CONSTRAINT DF_celo_registro_estado DEFAULT 'ACTIVO',
    created_by           bigint NULL,
    updated_by           bigint NULL,
    deleted_by           bigint NULL,
    created_at           datetime2(7) NOT NULL CONSTRAINT DF_celo_registro_created DEFAULT SYSUTCDATETIME(),
    updated_at           datetime2(7) NOT NULL CONSTRAINT DF_celo_registro_updated DEFAULT SYSUTCDATETIME(),
    deleted_at           datetime2(7) NULL,
    motivo_eliminacion   varchar(250) NULL,

    CONSTRAINT PK_celo_registro PRIMARY KEY (id),
    CONSTRAINT UQ_celo_registro_codigo UNIQUE (codigo),
    CONSTRAINT UQ_celo_registro_vacuno_fecha UNIQUE (vacuno_id, fecha_hora)
);
GO

CREATE TABLE dbo.celo_registro_caracteristica
(
    id               bigint IDENTITY(1,1) NOT NULL,
    celo_registro_id bigint NOT NULL,
    caracteristica_id bigint NOT NULL,

    CONSTRAINT PK_celo_registro_caracteristica PRIMARY KEY (id),
    CONSTRAINT UQ_celo_registro_caracteristica
        UNIQUE (celo_registro_id, caracteristica_id)
);
GO

CREATE TABLE dbo.celo_registro_caracteristica_libre
(
    id               bigint IDENTITY(1,1) NOT NULL,
    celo_registro_id bigint NOT NULL,
    descripcion      varchar(80) NOT NULL,
    created_at       datetime2(7) NOT NULL CONSTRAINT DF_celo_caracteristica_libre_created DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_celo_registro_caracteristica_libre PRIMARY KEY (id)
);
GO

CREATE TABLE dbo.celo_estado_vacuno
(
    vacuno_id                 bigint NOT NULL,
    estado_celo_code          varchar(30) NOT NULL,
    fecha_ultimo_celo         date NULL,
    fecha_proximo_celo_estimada date NULL,
    dias_restantes            int NULL,
    veces_celo                int NOT NULL CONSTRAINT DF_celo_estado_veces DEFAULT 0,
    crias                     int NOT NULL CONSTRAINT DF_celo_estado_crias DEFAULT 0,
    calculated_at             datetime2(7) NOT NULL CONSTRAINT DF_celo_estado_calculated DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_celo_estado_vacuno PRIMARY KEY (vacuno_id),
    CONSTRAINT CK_celo_estado_contadores CHECK (veces_celo >= 0 AND crias >= 0)
);
GO

/*
===============================================================================
07. REPRODUCCIÓN / FECUNDACIÓN
===============================================================================
*/

CREATE TABLE dbo.cat_tipo_fecundacion
(
    code        varchar(40) NOT NULL,
    nombre      varchar(80) NOT NULL,
    descripcion varchar(200) NULL,

    CONSTRAINT PK_cat_tipo_fecundacion PRIMARY KEY (code),
    CONSTRAINT UQ_cat_tipo_fecundacion_nombre UNIQUE (nombre)
);
GO

CREATE TABLE dbo.cat_resultado_fecundacion
(
    code        varchar(30) NOT NULL,
    nombre      varchar(60) NOT NULL,
    descripcion varchar(150) NULL,

    CONSTRAINT PK_cat_resultado_fecundacion PRIMARY KEY (code),
    CONSTRAINT UQ_cat_resultado_fecundacion_nombre UNIQUE (nombre)
);
GO

CREATE TABLE dbo.cat_estado_fecundacion_vacuno
(
    code        varchar(30) NOT NULL,
    nombre      varchar(60) NOT NULL,
    descripcion varchar(150) NULL,

    CONSTRAINT PK_cat_estado_fecundacion_vacuno PRIMARY KEY (code),
    CONSTRAINT UQ_cat_estado_fecundacion_vacuno_nombre UNIQUE (nombre)
);
GO

CREATE TABLE dbo.cat_estado_trazabilidad
(
    code        varchar(30) NOT NULL,
    nombre      varchar(60) NOT NULL,
    descripcion varchar(150) NULL,

    CONSTRAINT PK_cat_estado_trazabilidad PRIMARY KEY (code),
    CONSTRAINT UQ_cat_estado_trazabilidad_nombre UNIQUE (nombre)
);
GO

CREATE TABLE dbo.reproductor_externo
(
    id             bigint IDENTITY(1,1) NOT NULL,
    nombre         varchar(100) NOT NULL,
    codigo_externo varchar(50) NULL,
    sexo_code      varchar(10) NULL,
    descripcion    varchar(150) NULL,
    activo         bit NOT NULL CONSTRAINT DF_reproductor_externo_activo DEFAULT 1,
    created_by     bigint NULL,
    created_at     datetime2(7) NOT NULL CONSTRAINT DF_reproductor_externo_created DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_reproductor_externo PRIMARY KEY (id)
);
GO

CREATE TABLE dbo.fecundacion
(
    id                     bigint IDENTITY(1,1) NOT NULL,
    codigo                 varchar(15) NOT NULL,
    tipo_fecundacion_code  varchar(40) NOT NULL,
    vacuno_receptor_id     bigint NOT NULL,
    donante_vacuno_id      bigint NULL,
    donante_externo_id     bigint NULL,
    celo_registro_id       bigint NULL,
    fecha_procedimiento    date NOT NULL,
    responsable_id         bigint NOT NULL,
    resultado_code         varchar(30) NOT NULL,
    codigo_semen           varchar(30) NULL,
    codigo_embrion         varchar(30) NULL,
    observaciones_veterinarias varchar(250) NULL,
    created_by             bigint NULL,
    updated_by             bigint NULL,
    created_at             datetime2(7) NOT NULL CONSTRAINT DF_fecundacion_created DEFAULT SYSUTCDATETIME(),
    updated_at             datetime2(7) NOT NULL CONSTRAINT DF_fecundacion_updated DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_fecundacion PRIMARY KEY (id),
    CONSTRAINT UQ_fecundacion_codigo UNIQUE (codigo),
    CONSTRAINT CK_fecundacion_un_donante CHECK (
        (donante_vacuno_id IS NOT NULL AND donante_externo_id IS NULL)
        OR
        (donante_vacuno_id IS NULL AND donante_externo_id IS NOT NULL)
    )
);
GO

CREATE TABLE dbo.fecundacion_cria
(
    id                      bigint IDENTITY(1,1) NOT NULL,
    fecundacion_id          bigint NOT NULL,
    vacuno_hijo_id          bigint NOT NULL,
    estado_trazabilidad_code varchar(30) NOT NULL CONSTRAINT DF_fecundacion_cria_estado DEFAULT 'VALIDA',
    fecha_vinculacion       date NOT NULL,
    observaciones           varchar(150) NULL,
    created_by              bigint NULL,
    updated_by              bigint NULL,
    created_at              datetime2(7) NOT NULL CONSTRAINT DF_fecundacion_cria_created DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_fecundacion_cria PRIMARY KEY (id),
    CONSTRAINT UQ_fecundacion_cria UNIQUE (fecundacion_id, vacuno_hijo_id)
);
GO

CREATE TABLE dbo.vacuno_estado_fecundacion_historial
(
    id                     bigint IDENTITY(1,1) NOT NULL,
    vacuno_id              bigint NOT NULL,
    estado_fecundacion_code varchar(30) NOT NULL,
    fecundacion_id         bigint NULL,
    fecha_actualizacion    date NOT NULL,
    observaciones          varchar(150) NULL,
    vigente                bit NOT NULL CONSTRAINT DF_vacuno_estado_fec_vigente DEFAULT 1,
    created_by             bigint NULL,
    deleted_by             bigint NULL,
    created_at             datetime2(7) NOT NULL CONSTRAINT DF_vacuno_estado_fec_created DEFAULT SYSUTCDATETIME(),
    deleted_at             datetime2(7) NULL,
    motivo_eliminacion     varchar(250) NULL,

    CONSTRAINT PK_vacuno_estado_fec_historial PRIMARY KEY (id)
);
GO

/*
===============================================================================
08. ÍNDICES
===============================================================================
*/

CREATE INDEX IX_archivo_hash_sha256 ON dbo.archivo(hash_sha256);
CREATE INDEX IX_archivo_modulo ON dbo.archivo(modulo_code);

CREATE INDEX IX_reporte_tipo ON dbo.reporte_descarga(tipo_reporte_code);
CREATE INDEX IX_reporte_formato ON dbo.reporte_descarga(formato_code);
CREATE INDEX IX_reporte_vacuno ON dbo.reporte_descarga(vacuno_id);
CREATE INDEX IX_reporte_solicitado_por ON dbo.reporte_descarga(solicitado_por);
CREATE INDEX IX_reporte_fechas ON dbo.reporte_descarga(fecha_inicio, fecha_fin);

CREATE INDEX IX_parametro_modulo_clave_vigente
    ON dbo.parametro_sistema(modulo_code, clave, vigente);

CREATE INDEX IX_bitacora_modulo ON dbo.bitacora_auditoria(modulo_code);
CREATE INDEX IX_bitacora_entidad ON dbo.bitacora_auditoria(entidad);
CREATE INDEX IX_bitacora_entidad_id ON dbo.bitacora_auditoria(entidad_id);
CREATE INDEX IX_bitacora_usuario ON dbo.bitacora_auditoria(usuario_id);
CREATE INDEX IX_bitacora_created_at ON dbo.bitacora_auditoria(created_at);

CREATE INDEX IX_responsable_usuario ON dbo.responsable(usuario_id);
CREATE INDEX IX_responsable_tipo ON dbo.responsable(tipo_responsable_code);
CREATE INDEX IX_responsable_nombre ON dbo.responsable(nombre_completo);

CREATE INDEX IX_geo_provincia_departamento ON dbo.geo_provincia(departamento_codigo);
CREATE INDEX IX_geo_distrito_provincia ON dbo.geo_distrito(provincia_codigo);
CREATE INDEX IX_granja_distrito ON dbo.granja(distrito_codigo);

CREATE INDEX IX_vacuno_nombre ON dbo.vacuno(nombre);
CREATE INDEX IX_vacuno_fecha_nacimiento ON dbo.vacuno(fecha_nacimiento);
CREATE INDEX IX_vacuno_raza ON dbo.vacuno(raza_code);
CREATE INDEX IX_vacuno_sexo ON dbo.vacuno(sexo_code);
CREATE INDEX IX_vacuno_granja ON dbo.vacuno(granja_id);
CREATE INDEX IX_vacuno_padre ON dbo.vacuno(padre_id);
CREATE INDEX IX_vacuno_madre ON dbo.vacuno(madre_id);
CREATE INDEX IX_vacuno_fecha_registro ON dbo.vacuno(fecha_registro);
CREATE INDEX IX_vacuno_deleted_at ON dbo.vacuno(deleted_at);

CREATE INDEX IX_vacuno_foto_vacuno ON dbo.vacuno_foto(vacuno_id);
CREATE INDEX IX_vacuno_foto_archivo ON dbo.vacuno_foto(archivo_id);

CREATE INDEX IX_vacuno_estado_vacuno ON dbo.vacuno_estado_historial(vacuno_id);
CREATE INDEX IX_vacuno_estado_code ON dbo.vacuno_estado_historial(estado_code);
CREATE INDEX IX_vacuno_estado_fecha ON dbo.vacuno_estado_historial(fecha_estado);
CREATE INDEX IX_vacuno_estado_vigente ON dbo.vacuno_estado_historial(vigente);

CREATE INDEX IX_incidente_vacuno ON dbo.incidente_vacuno(vacuno_id);
CREATE INDEX IX_incidente_fecha ON dbo.incidente_vacuno(fecha_incidente);
CREATE INDEX IX_incidente_estado ON dbo.incidente_vacuno(estado_registro_code);

CREATE INDEX IX_ordenio_fecha ON dbo.ordenio(fecha_hora);
CREATE INDEX IX_ordenio_vacuno ON dbo.ordenio(vacuno_id);
CREATE INDEX IX_ordenio_encargado ON dbo.ordenio(encargado_usuario_id);
CREATE INDEX IX_ordenio_estado ON dbo.ordenio(estado_ordenio_code);
CREATE INDEX IX_ordenio_deleted ON dbo.ordenio(deleted_at);

CREATE INDEX IX_produccion_estandar_vacuno ON dbo.produccion_leche_estandar(vacuno_id);
CREATE INDEX IX_produccion_estandar_inicio ON dbo.produccion_leche_estandar(fecha_inicio);
CREATE INDEX IX_produccion_estandar_fin ON dbo.produccion_leche_estandar(fecha_fin);
CREATE INDEX IX_produccion_estandar_activo ON dbo.produccion_leche_estandar(activo);

CREATE INDEX IX_periodo_sequia_vacuno ON dbo.periodo_sequia(vacuno_id);
CREATE INDEX IX_periodo_sequia_inicio ON dbo.periodo_sequia(fecha_inicio);
CREATE INDEX IX_periodo_sequia_fin_estimada ON dbo.periodo_sequia(fecha_fin_estimada);
CREATE INDEX IX_periodo_sequia_fin_real ON dbo.periodo_sequia(fecha_fin_real);
CREATE INDEX IX_periodo_sequia_estado ON dbo.periodo_sequia(estado_sequia_code);

CREATE INDEX IX_triaje_fecha ON dbo.triaje(fecha_hora);
CREATE INDEX IX_triaje_vacuno ON dbo.triaje(vacuno_id);
CREATE INDEX IX_triaje_tipo ON dbo.triaje(tipo_peso_code);
CREATE INDEX IX_triaje_peso ON dbo.triaje(peso_kg);
CREATE INDEX IX_triaje_estado ON dbo.triaje(estado_registro_code);
CREATE INDEX IX_triaje_deleted ON dbo.triaje(deleted_at);

CREATE INDEX IX_celo_config_activo ON dbo.celo_configuracion(activo);

CREATE INDEX IX_celo_registro_fecha ON dbo.celo_registro(fecha_hora);
CREATE INDEX IX_celo_registro_vacuno ON dbo.celo_registro(vacuno_id);
CREATE INDEX IX_celo_registro_encargado ON dbo.celo_registro(encargado_usuario_id);
CREATE INDEX IX_celo_registro_estado ON dbo.celo_registro(estado_registro_code);
CREATE INDEX IX_celo_registro_deleted ON dbo.celo_registro(deleted_at);

CREATE INDEX IX_celo_caracteristica_libre_registro
    ON dbo.celo_registro_caracteristica_libre(celo_registro_id);

CREATE INDEX IX_celo_estado_vacuno_estado
    ON dbo.celo_estado_vacuno(estado_celo_code);

CREATE INDEX IX_celo_estado_vacuno_proximo
    ON dbo.celo_estado_vacuno(fecha_proximo_celo_estimada);

CREATE INDEX IX_reproductor_externo_nombre ON dbo.reproductor_externo(nombre);
CREATE INDEX IX_reproductor_externo_codigo ON dbo.reproductor_externo(codigo_externo);

CREATE INDEX IX_fecundacion_tipo ON dbo.fecundacion(tipo_fecundacion_code);
CREATE INDEX IX_fecundacion_receptor ON dbo.fecundacion(vacuno_receptor_id);
CREATE INDEX IX_fecundacion_donante_vacuno ON dbo.fecundacion(donante_vacuno_id);
CREATE INDEX IX_fecundacion_donante_externo ON dbo.fecundacion(donante_externo_id);
CREATE INDEX IX_fecundacion_celo ON dbo.fecundacion(celo_registro_id);
CREATE INDEX IX_fecundacion_fecha ON dbo.fecundacion(fecha_procedimiento);
CREATE INDEX IX_fecundacion_responsable ON dbo.fecundacion(responsable_id);
CREATE INDEX IX_fecundacion_resultado ON dbo.fecundacion(resultado_code);

CREATE INDEX IX_fecundacion_cria_fecundacion
    ON dbo.fecundacion_cria(fecundacion_id);

CREATE INDEX IX_fecundacion_cria_vacuno
    ON dbo.fecundacion_cria(vacuno_hijo_id);

CREATE INDEX IX_fecundacion_cria_estado
    ON dbo.fecundacion_cria(estado_trazabilidad_code);

CREATE INDEX IX_vacuno_estado_fec_vacuno
    ON dbo.vacuno_estado_fecundacion_historial(vacuno_id);

CREATE INDEX IX_vacuno_estado_fec_estado
    ON dbo.vacuno_estado_fecundacion_historial(estado_fecundacion_code);

CREATE INDEX IX_vacuno_estado_fec_fecundacion
    ON dbo.vacuno_estado_fecundacion_historial(fecundacion_id);

CREATE INDEX IX_vacuno_estado_fec_fecha
    ON dbo.vacuno_estado_fecundacion_historial(fecha_actualizacion);

CREATE INDEX IX_vacuno_estado_fec_vigente
    ON dbo.vacuno_estado_fecundacion_historial(vigente);

CREATE INDEX IX_vacuno_estado_fec_deleted
    ON dbo.vacuno_estado_fecundacion_historial(deleted_at);
GO

/*
===============================================================================
09. FOREIGN KEYS
===============================================================================
*/

ALTER TABLE dbo.archivo
ADD CONSTRAINT FK_archivo_modulo
    FOREIGN KEY (modulo_code) REFERENCES dbo.cat_modulo(code);

ALTER TABLE dbo.archivo
ADD CONSTRAINT FK_archivo_creado_por
    FOREIGN KEY (creado_por) REFERENCES dbo.usuario(id);

ALTER TABLE dbo.cat_tipo_reporte
ADD CONSTRAINT FK_tipo_reporte_modulo
    FOREIGN KEY (modulo_code) REFERENCES dbo.cat_modulo(code);

ALTER TABLE dbo.reporte_descarga
ADD CONSTRAINT FK_reporte_tipo
    FOREIGN KEY (tipo_reporte_code) REFERENCES dbo.cat_tipo_reporte(code);

ALTER TABLE dbo.reporte_descarga
ADD CONSTRAINT FK_reporte_formato
    FOREIGN KEY (formato_code) REFERENCES dbo.cat_formato_reporte(code);

ALTER TABLE dbo.reporte_descarga
ADD CONSTRAINT FK_reporte_vacuno
    FOREIGN KEY (vacuno_id) REFERENCES dbo.vacuno(id);

ALTER TABLE dbo.reporte_descarga
ADD CONSTRAINT FK_reporte_archivo
    FOREIGN KEY (archivo_id) REFERENCES dbo.archivo(id);

ALTER TABLE dbo.reporte_descarga
ADD CONSTRAINT FK_reporte_usuario
    FOREIGN KEY (solicitado_por) REFERENCES dbo.usuario(id);

ALTER TABLE dbo.parametro_sistema
ADD CONSTRAINT FK_parametro_modulo
    FOREIGN KEY (modulo_code) REFERENCES dbo.cat_modulo(code);

ALTER TABLE dbo.bitacora_auditoria
ADD CONSTRAINT FK_bitacora_modulo
    FOREIGN KEY (modulo_code) REFERENCES dbo.cat_modulo(code);

ALTER TABLE dbo.bitacora_auditoria
ADD CONSTRAINT FK_bitacora_usuario
    FOREIGN KEY (usuario_id) REFERENCES dbo.usuario(id);

ALTER TABLE dbo.responsable
ADD CONSTRAINT FK_responsable_usuario
    FOREIGN KEY (usuario_id) REFERENCES dbo.usuario(id);

ALTER TABLE dbo.responsable
ADD CONSTRAINT FK_responsable_tipo
    FOREIGN KEY (tipo_responsable_code) REFERENCES dbo.cat_tipo_responsable(code);

ALTER TABLE dbo.geo_provincia
ADD CONSTRAINT FK_geo_provincia_departamento
    FOREIGN KEY (departamento_codigo) REFERENCES dbo.geo_departamento(codigo);

ALTER TABLE dbo.geo_distrito
ADD CONSTRAINT FK_geo_distrito_provincia
    FOREIGN KEY (provincia_codigo) REFERENCES dbo.geo_provincia(codigo);

ALTER TABLE dbo.granja
ADD CONSTRAINT FK_granja_distrito
    FOREIGN KEY (distrito_codigo) REFERENCES dbo.geo_distrito(codigo);

ALTER TABLE dbo.vacuno
ADD CONSTRAINT FK_vacuno_tipo_adquisicion
    FOREIGN KEY (tipo_adquisicion_code) REFERENCES dbo.cat_tipo_adquisicion(code);

ALTER TABLE dbo.vacuno
ADD CONSTRAINT FK_vacuno_raza
    FOREIGN KEY (raza_code) REFERENCES dbo.cat_raza(code);

ALTER TABLE dbo.vacuno
ADD CONSTRAINT FK_vacuno_color
    FOREIGN KEY (color_id) REFERENCES dbo.cat_color(id);

ALTER TABLE dbo.vacuno
ADD CONSTRAINT FK_vacuno_sexo
    FOREIGN KEY (sexo_code) REFERENCES dbo.cat_sexo(code);

ALTER TABLE dbo.vacuno
ADD CONSTRAINT FK_vacuno_padre
    FOREIGN KEY (padre_id) REFERENCES dbo.vacuno(id);

ALTER TABLE dbo.vacuno
ADD CONSTRAINT FK_vacuno_madre
    FOREIGN KEY (madre_id) REFERENCES dbo.vacuno(id);

ALTER TABLE dbo.vacuno
ADD CONSTRAINT FK_vacuno_granja
    FOREIGN KEY (granja_id) REFERENCES dbo.granja(id);

ALTER TABLE dbo.vacuno
ADD CONSTRAINT FK_vacuno_utilizacion
    FOREIGN KEY (tipo_utilizacion_code) REFERENCES dbo.cat_tipo_utilizacion(code);

ALTER TABLE dbo.vacuno
ADD CONSTRAINT FK_vacuno_created_by
    FOREIGN KEY (created_by) REFERENCES dbo.usuario(id);

ALTER TABLE dbo.vacuno
ADD CONSTRAINT FK_vacuno_updated_by
    FOREIGN KEY (updated_by) REFERENCES dbo.usuario(id);

ALTER TABLE dbo.vacuno
ADD CONSTRAINT FK_vacuno_deleted_by
    FOREIGN KEY (deleted_by) REFERENCES dbo.usuario(id);

ALTER TABLE dbo.vacuno_foto
ADD CONSTRAINT FK_vacuno_foto_vacuno
    FOREIGN KEY (vacuno_id) REFERENCES dbo.vacuno(id);

ALTER TABLE dbo.vacuno_foto
ADD CONSTRAINT FK_vacuno_foto_archivo
    FOREIGN KEY (archivo_id) REFERENCES dbo.archivo(id);

ALTER TABLE dbo.vacuno_foto
ADD CONSTRAINT FK_vacuno_foto_usuario
    FOREIGN KEY (uploaded_by) REFERENCES dbo.usuario(id);

ALTER TABLE dbo.vacuno_estado_historial
ADD CONSTRAINT FK_vacuno_estado_vacuno
    FOREIGN KEY (vacuno_id) REFERENCES dbo.vacuno(id);

ALTER TABLE dbo.vacuno_estado_historial
ADD CONSTRAINT FK_vacuno_estado_catalogo
    FOREIGN KEY (estado_code) REFERENCES dbo.cat_estado_vacuno(code);

ALTER TABLE dbo.vacuno_estado_historial
ADD CONSTRAINT FK_vacuno_estado_usuario
    FOREIGN KEY (created_by) REFERENCES dbo.usuario(id);

ALTER TABLE dbo.incidente_vacuno
ADD CONSTRAINT FK_incidente_vacuno
    FOREIGN KEY (vacuno_id) REFERENCES dbo.vacuno(id);

ALTER TABLE dbo.incidente_vacuno
ADD CONSTRAINT FK_incidente_estado
    FOREIGN KEY (estado_registro_code) REFERENCES dbo.cat_estado_registro(code);

ALTER TABLE dbo.incidente_vacuno
ADD CONSTRAINT FK_incidente_usuario
    FOREIGN KEY (created_by) REFERENCES dbo.usuario(id);

ALTER TABLE dbo.ordenio
ADD CONSTRAINT FK_ordenio_vacuno
    FOREIGN KEY (vacuno_id) REFERENCES dbo.vacuno(id);

ALTER TABLE dbo.ordenio
ADD CONSTRAINT FK_ordenio_encargado
    FOREIGN KEY (encargado_usuario_id) REFERENCES dbo.usuario(id);

ALTER TABLE dbo.ordenio
ADD CONSTRAINT FK_ordenio_estado
    FOREIGN KEY (estado_ordenio_code) REFERENCES dbo.cat_estado_ordenio(code);

ALTER TABLE dbo.ordenio
ADD CONSTRAINT FK_ordenio_created
    FOREIGN KEY (created_by) REFERENCES dbo.usuario(id);

ALTER TABLE dbo.ordenio
ADD CONSTRAINT FK_ordenio_updated
    FOREIGN KEY (updated_by) REFERENCES dbo.usuario(id);

ALTER TABLE dbo.ordenio
ADD CONSTRAINT FK_ordenio_deleted
    FOREIGN KEY (deleted_by) REFERENCES dbo.usuario(id);

ALTER TABLE dbo.produccion_leche_estandar
ADD CONSTRAINT FK_produccion_estandar_vacuno
    FOREIGN KEY (vacuno_id) REFERENCES dbo.vacuno(id);

ALTER TABLE dbo.produccion_leche_estandar
ADD CONSTRAINT FK_produccion_estandar_usuario
    FOREIGN KEY (created_by) REFERENCES dbo.usuario(id);

ALTER TABLE dbo.periodo_sequia
ADD CONSTRAINT FK_periodo_sequia_vacuno
    FOREIGN KEY (vacuno_id) REFERENCES dbo.vacuno(id);

ALTER TABLE dbo.periodo_sequia
ADD CONSTRAINT FK_periodo_sequia_estado
    FOREIGN KEY (estado_sequia_code) REFERENCES dbo.cat_estado_sequia(code);

ALTER TABLE dbo.periodo_sequia
ADD CONSTRAINT FK_periodo_sequia_created
    FOREIGN KEY (created_by) REFERENCES dbo.usuario(id);

ALTER TABLE dbo.periodo_sequia
ADD CONSTRAINT FK_periodo_sequia_updated
    FOREIGN KEY (updated_by) REFERENCES dbo.usuario(id);

ALTER TABLE dbo.triaje
ADD CONSTRAINT FK_triaje_vacuno
    FOREIGN KEY (vacuno_id) REFERENCES dbo.vacuno(id);

ALTER TABLE dbo.triaje
ADD CONSTRAINT FK_triaje_tipo_peso
    FOREIGN KEY (tipo_peso_code) REFERENCES dbo.cat_tipo_peso(code);

ALTER TABLE dbo.triaje
ADD CONSTRAINT FK_triaje_estado
    FOREIGN KEY (estado_registro_code) REFERENCES dbo.cat_estado_registro(code);

ALTER TABLE dbo.triaje
ADD CONSTRAINT FK_triaje_encargado
    FOREIGN KEY (encargado_usuario_id) REFERENCES dbo.usuario(id);

ALTER TABLE dbo.triaje
ADD CONSTRAINT FK_triaje_created
    FOREIGN KEY (created_by) REFERENCES dbo.usuario(id);

ALTER TABLE dbo.triaje
ADD CONSTRAINT FK_triaje_updated
    FOREIGN KEY (updated_by) REFERENCES dbo.usuario(id);

ALTER TABLE dbo.triaje
ADD CONSTRAINT FK_triaje_deleted
    FOREIGN KEY (deleted_by) REFERENCES dbo.usuario(id);

ALTER TABLE dbo.celo_configuracion
ADD CONSTRAINT FK_celo_config_usuario
    FOREIGN KEY (created_by) REFERENCES dbo.usuario(id);

ALTER TABLE dbo.celo_registro
ADD CONSTRAINT FK_celo_registro_vacuno
    FOREIGN KEY (vacuno_id) REFERENCES dbo.vacuno(id);

ALTER TABLE dbo.celo_registro
ADD CONSTRAINT FK_celo_registro_encargado
    FOREIGN KEY (encargado_usuario_id) REFERENCES dbo.usuario(id);

ALTER TABLE dbo.celo_registro
ADD CONSTRAINT FK_celo_registro_estado
    FOREIGN KEY (estado_registro_code) REFERENCES dbo.cat_estado_registro(code);

ALTER TABLE dbo.celo_registro
ADD CONSTRAINT FK_celo_registro_created
    FOREIGN KEY (created_by) REFERENCES dbo.usuario(id);

ALTER TABLE dbo.celo_registro
ADD CONSTRAINT FK_celo_registro_updated
    FOREIGN KEY (updated_by) REFERENCES dbo.usuario(id);

ALTER TABLE dbo.celo_registro
ADD CONSTRAINT FK_celo_registro_deleted
    FOREIGN KEY (deleted_by) REFERENCES dbo.usuario(id);

ALTER TABLE dbo.celo_registro_caracteristica
ADD CONSTRAINT FK_celo_caracteristica_registro
    FOREIGN KEY (celo_registro_id) REFERENCES dbo.celo_registro(id);

ALTER TABLE dbo.celo_registro_caracteristica
ADD CONSTRAINT FK_celo_caracteristica_catalogo
    FOREIGN KEY (caracteristica_id) REFERENCES dbo.cat_caracteristica_celo(id);

ALTER TABLE dbo.celo_registro_caracteristica_libre
ADD CONSTRAINT FK_celo_libre_registro
    FOREIGN KEY (celo_registro_id) REFERENCES dbo.celo_registro(id);

ALTER TABLE dbo.celo_estado_vacuno
ADD CONSTRAINT FK_celo_estado_vacuno
    FOREIGN KEY (vacuno_id) REFERENCES dbo.vacuno(id);

ALTER TABLE dbo.celo_estado_vacuno
ADD CONSTRAINT FK_celo_estado_catalogo
    FOREIGN KEY (estado_celo_code) REFERENCES dbo.cat_estado_celo(code);

ALTER TABLE dbo.reproductor_externo
ADD CONSTRAINT FK_reproductor_externo_sexo
    FOREIGN KEY (sexo_code) REFERENCES dbo.cat_sexo(code);

ALTER TABLE dbo.reproductor_externo
ADD CONSTRAINT FK_reproductor_externo_usuario
    FOREIGN KEY (created_by) REFERENCES dbo.usuario(id);

ALTER TABLE dbo.fecundacion
ADD CONSTRAINT FK_fecundacion_tipo
    FOREIGN KEY (tipo_fecundacion_code) REFERENCES dbo.cat_tipo_fecundacion(code);

ALTER TABLE dbo.fecundacion
ADD CONSTRAINT FK_fecundacion_receptor
    FOREIGN KEY (vacuno_receptor_id) REFERENCES dbo.vacuno(id);

ALTER TABLE dbo.fecundacion
ADD CONSTRAINT FK_fecundacion_donante_vacuno
    FOREIGN KEY (donante_vacuno_id) REFERENCES dbo.vacuno(id);

ALTER TABLE dbo.fecundacion
ADD CONSTRAINT FK_fecundacion_donante_externo
    FOREIGN KEY (donante_externo_id) REFERENCES dbo.reproductor_externo(id);

ALTER TABLE dbo.fecundacion
ADD CONSTRAINT FK_fecundacion_celo
    FOREIGN KEY (celo_registro_id) REFERENCES dbo.celo_registro(id);

ALTER TABLE dbo.fecundacion
ADD CONSTRAINT FK_fecundacion_responsable
    FOREIGN KEY (responsable_id) REFERENCES dbo.responsable(id);

ALTER TABLE dbo.fecundacion
ADD CONSTRAINT FK_fecundacion_resultado
    FOREIGN KEY (resultado_code) REFERENCES dbo.cat_resultado_fecundacion(code);

ALTER TABLE dbo.fecundacion
ADD CONSTRAINT FK_fecundacion_created
    FOREIGN KEY (created_by) REFERENCES dbo.usuario(id);

ALTER TABLE dbo.fecundacion
ADD CONSTRAINT FK_fecundacion_updated
    FOREIGN KEY (updated_by) REFERENCES dbo.usuario(id);

ALTER TABLE dbo.fecundacion_cria
ADD CONSTRAINT FK_fecundacion_cria_fecundacion
    FOREIGN KEY (fecundacion_id) REFERENCES dbo.fecundacion(id);

ALTER TABLE dbo.fecundacion_cria
ADD CONSTRAINT FK_fecundacion_cria_vacuno
    FOREIGN KEY (vacuno_hijo_id) REFERENCES dbo.vacuno(id);

ALTER TABLE dbo.fecundacion_cria
ADD CONSTRAINT FK_fecundacion_cria_estado
    FOREIGN KEY (estado_trazabilidad_code) REFERENCES dbo.cat_estado_trazabilidad(code);

ALTER TABLE dbo.fecundacion_cria
ADD CONSTRAINT FK_fecundacion_cria_created
    FOREIGN KEY (created_by) REFERENCES dbo.usuario(id);

ALTER TABLE dbo.fecundacion_cria
ADD CONSTRAINT FK_fecundacion_cria_updated
    FOREIGN KEY (updated_by) REFERENCES dbo.usuario(id);

ALTER TABLE dbo.vacuno_estado_fecundacion_historial
ADD CONSTRAINT FK_vacuno_estado_fec_vacuno
    FOREIGN KEY (vacuno_id) REFERENCES dbo.vacuno(id);

ALTER TABLE dbo.vacuno_estado_fecundacion_historial
ADD CONSTRAINT FK_vacuno_estado_fec_estado
    FOREIGN KEY (estado_fecundacion_code)
    REFERENCES dbo.cat_estado_fecundacion_vacuno(code);

ALTER TABLE dbo.vacuno_estado_fecundacion_historial
ADD CONSTRAINT FK_vacuno_estado_fec_fecundacion
    FOREIGN KEY (fecundacion_id) REFERENCES dbo.fecundacion(id);

ALTER TABLE dbo.vacuno_estado_fecundacion_historial
ADD CONSTRAINT FK_vacuno_estado_fec_created
    FOREIGN KEY (created_by) REFERENCES dbo.usuario(id);

ALTER TABLE dbo.vacuno_estado_fecundacion_historial
ADD CONSTRAINT FK_vacuno_estado_fec_deleted
    FOREIGN KEY (deleted_by) REFERENCES dbo.usuario(id);
GO

/*
===============================================================================
10. DATOS INICIALES DE CATÁLOGOS
===============================================================================

Estos valores están documentados como "valores sugeridos" en el diagrama.
Puedes conservarlos o modificar este bloque según las necesidades reales
de tu aplicación.
===============================================================================
*/

INSERT INTO dbo.cat_modulo(code, nombre, descripcion)
VALUES
('VACUNOS', 'Vacunos', 'Gestión de vacunos'),
('PRODUCCION_LECHE', 'Producción de leche', 'Producción y ordeños'),
('SANIDAD_TRIAJE', 'Sanidad / Triaje', 'Control sanitario y peso'),
('REPRODUCCION_CELO', 'Reproducción / Celo', 'Gestión de celos'),
('REPRODUCCION_FECUNDACION', 'Reproducción / Fecundación', 'Gestión de fecundaciones'),
('REPORTES', 'Reportes', 'Generación de reportes');
GO

INSERT INTO dbo.cat_estado_registro(code, nombre)
VALUES
('ACTIVO', 'Activo'),
('ELIMINADO_LOGICO', 'Eliminado lógico'),
('ANULADO', 'Anulado'),
('DESHABILITADO', 'Deshabilitado');
GO

INSERT INTO dbo.cat_formato_reporte(code, nombre)
VALUES
('PDF', 'PDF'),
('XLSX', 'XLSX');
GO

INSERT INTO dbo.cat_sexo(code, nombre)
VALUES
('HEMBRA', 'Hembra'),
('MACHO', 'Macho');
GO

INSERT INTO dbo.cat_estado_vacuno(code, nombre)
VALUES
('VIVO', 'Vivo'),
('MUERTO', 'Muerto'),
('VENDIDO', 'Vendido'),
('TRASLADADO', 'Trasladado');
GO

INSERT INTO dbo.cat_tipo_adquisicion(code, nombre)
VALUES
('COMPRA', 'Compra'),
('MONTA', 'Monta'),
('INSEMINACION', 'Inseminación'),
('NACIMIENTO_GRANJA', 'Nacimiento en granja'),
('DONACION', 'Donación'),
('TRANSFERENCIA_EMBRION', 'Transferencia de embrión');
GO

INSERT INTO dbo.cat_tipo_utilizacion(code, nombre)
VALUES
('LECHE', 'Leche'),
('CARNE', 'Carne'),
('REPRODUCCION', 'Reproducción'),
('DOBLE_PROPOSITO', 'Doble propósito'),
('OTRO', 'Otro');
GO

INSERT INTO dbo.cat_tipo_responsable(code, nombre)
VALUES
('ENCARGADO', 'Encargado'),
('VETERINARIO', 'Veterinario'),
('TECNICO', 'Técnico'),
('ADMINISTRADOR', 'Administrador'),
('EXTERNO', 'Externo');
GO

INSERT INTO dbo.cat_estado_ordenio(code, nombre)
VALUES
('ACTIVO', 'Activo'),
('ELIMINADO_LOGICO', 'Eliminado lógico'),
('ANULADO', 'Anulado');
GO

INSERT INTO dbo.cat_estado_sequia(code, nombre)
VALUES
('NO_SEQUIA', 'No sequía'),
('POR_ENTRAR', 'Por entrar'),
('EN_SEQUIA', 'En sequía'),
('FINALIZADA', 'Finalizada'),
('CANCELADA', 'Cancelada');
GO

INSERT INTO dbo.cat_tipo_peso(code, nombre)
VALUES
('RUTINARIO', 'Rutinario'),
('NACIMIENTO', 'Nacimiento'),
('CONTROL', 'Control'),
('PRE_PARTO', 'Pre parto'),
('POST_PARTO', 'Post parto'),
('RECUPERACION', 'Recuperación'),
('OTRO', 'Otro');
GO

INSERT INTO dbo.cat_estado_celo(code, nombre)
VALUES
('PROXIMO', 'Próximo'),
('EN_CELO', 'En celo'),
('PASO', 'Pasó'),
('SIN_DATOS', 'Sin datos');
GO

INSERT INTO dbo.cat_tipo_fecundacion(code, nombre)
VALUES
('MONTA_NATURAL', 'Monta natural'),
('INSEMINACION_ARTIFICIAL', 'Inseminación artificial'),
('TRANSFERENCIA_EMBRIONES', 'Transferencia de embriones');
GO

INSERT INTO dbo.cat_resultado_fecundacion(code, nombre)
VALUES
('PENDIENTE', 'Pendiente'),
('EXITOSA', 'Exitosa'),
('FALLIDA', 'Fallida');
GO

INSERT INTO dbo.cat_estado_fecundacion_vacuno(code, nombre)
VALUES
('NO_FECUNDADO', 'No fecundado'),
('EN_PROCESO', 'En proceso'),
('FECUNDADO', 'Fecundado'),
('EN_GESTACION', 'En gestación'),
('SIN_ESTADO', 'Sin estado');
GO

INSERT INTO dbo.cat_estado_trazabilidad(code, nombre)
VALUES
('VALIDA', 'Válida'),
('ANULADA', 'Anulada'),
('PENDIENTE', 'Pendiente');
GO