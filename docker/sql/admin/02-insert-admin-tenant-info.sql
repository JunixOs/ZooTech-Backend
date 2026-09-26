SET NOCOUNT ON;

BEGIN TRANSACTION;

BEGIN TRY

    ------------------------------------------------------------
    -- TENANT
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
        created_at
    )
    VALUES
    (
        'ADMIN',
        'admin',
        'ZooTech Admin',
        'ZooTech Admin S.A.C.',
        'zootech@admin.com',
        '+51 999 888 777',
        'America/Lima',
        'ACTIVE',
        NULL,
        SYSDATETIMEOFFSET()
    );

    DECLARE @TenantId INT = SCOPE_IDENTITY();

    ------------------------------------------------------------
    -- ADDRESS
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
        created_at
    )
    VALUES
    (
        @TenantId,
        'Perú',
        'Huánuco',
        'Leoncio Prado',
        'Tingo María',
        'Av. Alameda Perú 123',
        NULL,
        NULL,
        SYSDATETIMEOFFSET()
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
        created_at
    )
    VALUES
    (
        @TenantId,
        '#2E7D32',
        '#1565C0',
        '#F9A825',
        '',
        '',
        '',
        NULL,
        NULL,
        SYSDATETIMEOFFSET()
    );
------------------------------------------------------------
-- TENANT DATABASE CONNECTION
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
    created_at
)
VALUES
(
    @TenantId,
    'ZooTech_Admin_Db',
    'localhost',
    'SQLSERVER',
    NULL,
    1,
    NULL,
    SYSDATETIMEOFFSET()
);
    COMMIT TRANSACTION;

END TRY
BEGIN CATCH

    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    THROW;

END CATCH;