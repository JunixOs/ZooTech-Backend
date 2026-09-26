SET XACT_ABORT ON;

BEGIN TRY

BEGIN TRANSACTION;

------------------------------------------------------------
-- TENANTS
------------------------------------------------------------

INSERT INTO tenants
(
    code,
    subdomain,
    display_name,
    legal_name,
    email,
    phone,
    timezone,
    status,
    metadata,
    created_at,
    updated_at,
    deleted_at
)
SELECT
    V.Code,
    V.Subdomain,
    V.DisplayName,
    V.LegalName,
    V.Email,
    V.Phone,
    V.Timezone,
    V.Status,
    NULL,
    SYSUTCDATETIME(),
    SYSUTCDATETIME(),
    NULL
FROM
(
VALUES

('GRANJA_ZOOTECNIA_UNAS',N'zootecniaunas',N'Granja Zootecnia UNAS',N'Granja Zootecnia Universidad Nacional Agraría de la Selva',N'zootecnia@unas.edu.pe',N'+51 987 654 321',N'America/Lima',N'ACTIVE'),
('AGROPECUARIA_EL_ROBLE',N'elroble',N'Agropecuaria El Roble',N'Agropecuaria El Roble S.A.',N'contacto@elroble.com',N'+57 300 123 4567',N'America/Bogota',N'ACTIVE'),
('LACTEOS_DEL_VALLE',N'lacteosdelvalle',N'Lácteos del Valle',N'Lácteos del Valle Ltda.',N'contacto@lacteosdelvalle.com',N'+56 9 8765 4321',N'America/Santiago',N'TRIAL'),
('GRANJA_LOS_ANDES',N'losandes',N'Granja Los Andes',N'Granja Los Andes S.R.L.',N'contacto@granjalosandes.com',N'+593 99 876 5432',N'America/Guayaquil',N'SUSPENDED')

) V (Code, Subdomain, DisplayName, LegalName, Email, Phone, Timezone, Status)
WHERE NOT EXISTS
(
    SELECT 1
    FROM tenants t
    WHERE t.code = V.Code
);

------------------------------------------------------------
-- ADDRESSES
------------------------------------------------------------

INSERT INTO addresses
(
    tenant_id,
    country,
    state,
    province,
    city,
    address_line_1,
    address_line_2,
    metadata,
    created_at,
    updated_at,
    deleted_at
)
SELECT
    t.id,
    V.Country,
    V.State,
    V.Province,
    V.City,
    V.AddressLine1,
    V.AddressLine2,
    NULL,
    SYSUTCDATETIME(),
    SYSUTCDATETIME(),
    NULL
FROM tenants t
JOIN
(
VALUES

('GRANJA_ZOOTECNIA_UNAS',N'Perú',N'Húanuco',N'Rupa Rupa',N'Tingo María',N'Carretera Central km. 1.21',NULL),
('AGROPECUARIA_EL_ROBLE',N'Colombia',N'Antioquia',N'Valle de Aburrá',N'Medellín',N'Calle 10 # 45-67',NULL),
('LACTEOS_DEL_VALLE',N'Chile',N'Región Metropolitana',N'Santiago',N'Santiago',N'Av. Providencia 2020',N'Oficina 501'),
('GRANJA_LOS_ANDES',N'Ecuador',N'Pichincha',N'Quito',N'Quito',N'Av. Amazonas 456',NULL)

) V (Code, Country, State, Province, City, AddressLine1, AddressLine2)
    ON t.code = V.Code
WHERE NOT EXISTS
(
    SELECT 1
    FROM addresses a
    WHERE a.tenant_id = t.id
);

------------------------------------------------------------
-- TENANT BRANDING
------------------------------------------------------------

INSERT INTO tenant_branding
(
    tenant_id,
    primary_color,
    secondary_color,
    accent_color,
    logo_url,
    favicon_url,
    login_background_url,
    custom_css,
    metadata,
    created_at,
    updated_at,
    deleted_at
)
SELECT
    t.id,
    V.PrimaryColor,
    V.SecondaryColor,
    V.AccentColor,
    V.LogoUrl,
    V.FaviconUrl,
    V.LoginBackgroundUrl,
    NULL,
    NULL,
    SYSUTCDATETIME(),
    SYSUTCDATETIME(),
    NULL
FROM tenants t
JOIN
(
VALUES

('GRANJA_ZOOTECNIA_UNAS',N'#2e7d32',N'#1b5e20',N'#e8f5e9',N'https://cdn.zootech.com/tenants/zootecniaunas/logo.png',N'https://cdn.zootech.com/tenants/zootecniaunas/favicon.ico',N'https://cdn.zootech.com/tenants/zootecniaunas/login-bg.jpg'),
('AGROPECUARIA_EL_ROBLE',N'#1565c0',N'#0d47a1',N'#e3f2fd',N'https://cdn.zootech.com/tenants/elroble/logo.png',N'https://cdn.zootech.com/tenants/elroble/favicon.ico',N'https://cdn.zootech.com/tenants/elroble/login-bg.jpg'),
('LACTEOS_DEL_VALLE',N'#6a1b9a',N'#4a148c',N'#f3e5f5',N'https://cdn.zootech.com/tenants/lacteosdelvalle/logo.png',N'https://cdn.zootech.com/tenants/lacteosdelvalle/favicon.ico',N'https://cdn.zootech.com/tenants/lacteosdelvalle/login-bg.jpg'),
('GRANJA_LOS_ANDES',N'#b45309',N'#7c2d12',N'#fef3e7',N'https://cdn.zootech.com/tenants/losandes/logo.png',N'https://cdn.zootech.com/tenants/losandes/favicon.ico',N'https://cdn.zootech.com/tenants/losandes/login-bg.jpg')

) V (Code, PrimaryColor, SecondaryColor, AccentColor, LogoUrl, FaviconUrl, LoginBackgroundUrl)
    ON t.code = V.Code
WHERE NOT EXISTS
(
    SELECT 1
    FROM tenant_branding b
    WHERE b.tenant_id = t.id
);

------------------------------------------------------------
-- TENANT DATABASE CONNECTIONS
-- NOTA: connection_string_encrypted es un valor PLACEHOLDER.
-- Reemplazar por la cadena de conexion real cifrada antes de usar en produccion.
------------------------------------------------------------

INSERT INTO tenant_database_connections
(
    tenant_id,
    database_name,
    server_name,
    provider,
    connection_string_encrypted,
    is_active,
    metadata,
    created_at,
    updated_at,
    deleted_at
)
SELECT
    t.id,
    V.DatabaseName,
    V.ServerName,
    V.Provider,
    V.ConnectionStringEncrypted,
    1,
    NULL,
    SYSUTCDATETIME(),
    SYSUTCDATETIME(),
    NULL
FROM tenants t
JOIN
(
VALUES

('GRANJA_ZOOTECNIA_UNAS',N'ZooTech_ZootecniaUnas_Db',N'sql-zootech-prod01.database.windows.net',N'SQLSERVER',N''),
('AGROPECUARIA_EL_ROBLE',N'ZooTech_ElRoble_Db',N'sql-zootech-prod01.database.windows.net',N'SQLSERVER',N''),
('LACTEOS_DEL_VALLE',N'ZooTech_LacteosDelValle_Db',N'sql-zootech-prod01.database.windows.net',N'SQLSERVER',N''),
('GRANJA_LOS_ANDES',N'ZooTech_LosAndes_Db',N'sql-zootech-prod01.database.windows.net',N'SQLSERVER',N'')

) V (Code, DatabaseName, ServerName, Provider, ConnectionStringEncrypted)
    ON t.code = V.Code
WHERE NOT EXISTS
(
    SELECT 1
    FROM tenant_database_connections c
    WHERE c.tenant_id = t.id
);

------------------------------------------------------------
-- TENANT FEATURES
-- Asocia los 4 tenants nuevos con TODAS las features activas actualmente
-- definidas en la plataforma (tabla features).
------------------------------------------------------------

INSERT INTO tenant_features
(
    tenant_id,
    feature_id,
    is_enabled,
    enabled_at,
    expires_at,
    updated_at,
    metadata
)
SELECT
    t.id,
    f.id,
    1,
    SYSUTCDATETIME(),
    NULL,
    SYSUTCDATETIME(),
    NULL
FROM tenants t
CROSS JOIN features f
WHERE t.code IN ('GRANJA_ZOOTECNIA_UNAS','AGROPECUARIA_EL_ROBLE','LACTEOS_DEL_VALLE','GRANJA_LOS_ANDES')
  AND f.is_active = 1
  AND f.deleted_at IS NULL
  AND NOT EXISTS
  (
      SELECT 1
      FROM tenant_features tf
      WHERE tf.tenant_id = t.id
        AND tf.feature_id = f.id
  );

------------------------------------------------------------
-- SETTING VALUES
-- Asocia los 4 tenants nuevos con TODOS los setting_definitions activos
-- actualmente definidos (VACUNOS, SANIDAD, PRODUCCION_LECHE), usando el
-- default_value de cada definicion como valor inicial a nivel TENANT.
------------------------------------------------------------

INSERT INTO setting_values
(
    tenant_id,
    setting_definition_id,
    actor_type,
    actor_id,
    value,
    is_active,
    metadata,
    created_at,
    updated_at,
    deleted_at
)
SELECT
    t.id,
    sd.id,
    'TENANT',
    NULL,
    sd.default_value,
    1,
    NULL,
    SYSUTCDATETIME(),
    SYSUTCDATETIME(),
    NULL
FROM tenants t
CROSS JOIN setting_definitions sd
WHERE t.code IN ('GRANJA_ZOOTECNIA_UNAS','AGROPECUARIA_EL_ROBLE','LACTEOS_DEL_VALLE','GRANJA_LOS_ANDES')
  AND sd.is_active = 1
  AND sd.deleted_at IS NULL
  AND sd.default_value IS NOT NULL
  AND NOT EXISTS
  (
      SELECT 1
      FROM setting_values sv
      WHERE sv.tenant_id = t.id
        AND sv.setting_definition_id = sd.id
        AND sv.actor_type = 'TENANT'
        AND sv.actor_id IS NULL
  );

------------------------------------------------------------
-- TENANT BUSINESS RULES
-- Asocia los 4 tenants nuevos con TODAS las reglas de negocio activas
-- actualmente definidas en rule_definitions.
------------------------------------------------------------

INSERT INTO tenant_business_rules
(
    tenant_id,
    rule_definition_id,
    is_active,
    priority,
    rule_version,
    execution_mode,
    custom_condition,
    custom_action,
    metadata,
    created_at,
    updated_at,
    deleted_at
)
SELECT
    t.id,
    rd.id,
    1,
    0,
    1,
    'SYNC',
    NULL,
    NULL,
    NULL,
    SYSUTCDATETIME(),
    SYSUTCDATETIME(),
    NULL
FROM tenants t
CROSS JOIN rule_definitions rd
WHERE t.code IN ('GRANJA_ZOOTECNIA_UNAS','AGROPECUARIA_EL_ROBLE','LACTEOS_DEL_VALLE','GRANJA_LOS_ANDES')
  AND rd.is_active = 1
  AND rd.deleted_at IS NULL
  AND NOT EXISTS
  (
      SELECT 1
      FROM tenant_business_rules tbr
      WHERE tbr.tenant_id = t.id
        AND tbr.rule_definition_id = rd.id
  );

COMMIT TRANSACTION;

END TRY
BEGIN CATCH

    IF XACT_STATE() <> 0
        ROLLBACK TRANSACTION;

    THROW;

END CATCH;
