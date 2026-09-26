-- =========================================================
-- ZOO TECH - ENTERPRISE MULTI TENANT SAAS PLATFORM
-- CONTROL PLANE DATABASE
-- UNIFIED SETTINGS VERSION - SQL SERVER
-- =========================================================

-- =========================================================
-- TENANTS
-- =========================================================

CREATE TABLE tenants (
    id INT IDENTITY(1,1) PRIMARY KEY,

    code NVARCHAR(50) NOT NULL UNIQUE,
    subdomain NVARCHAR(50) NOT NULL UNIQUE,

    display_name NVARCHAR(150) NOT NULL,
    legal_name NVARCHAR(200) NOT NULL,

    email NVARCHAR(150) NOT NULL,
    phone NVARCHAR(50) NOT NULL,

    timezone NVARCHAR(100) NOT NULL,

    status NVARCHAR(30) NOT NULL 
        CONSTRAINT DF_tenants_status DEFAULT 'TRIAL',

    metadata NVARCHAR(MAX) NULL,

    created_at DATETIME NULL,
    updated_at DATETIME NULL,
    deleted_at DATETIME NULL,

    CONSTRAINT CK_tenants_status 
        CHECK (status IN ('TRIAL', 'ACTIVE', 'SUSPENDED', 'CANCELLED')),

    CONSTRAINT CK_tenants_metadata_json 
        CHECK (metadata IS NULL OR ISJSON(metadata) = 1)
);
GO


-- =========================================================
-- ADDRESSES
-- =========================================================

CREATE TABLE addresses (
    id INT IDENTITY(1,1) PRIMARY KEY,

    tenant_id INT NOT NULL,

    country NVARCHAR(100) NOT NULL,
    state NVARCHAR(100) NOT NULL,
    province NVARCHAR(100) NOT NULL,
    city NVARCHAR(100) NOT NULL,

    address_line_1 NVARCHAR(200) NOT NULL,
    address_line_2 NVARCHAR(200) NULL,

    metadata NVARCHAR(MAX) NULL,

    created_at DATETIME NULL,
    updated_at DATETIME NULL,
    deleted_at DATETIME NULL,

    CONSTRAINT CK_addresses_metadata_json 
        CHECK (metadata IS NULL OR ISJSON(metadata) = 1)
);
GO


-- =========================================================
-- FEATURES
-- =========================================================

CREATE TABLE features (
    id INT IDENTITY(1,1) PRIMARY KEY,

    code NVARCHAR(100) NOT NULL UNIQUE,
    name NVARCHAR(150) NOT NULL,

    description NVARCHAR(MAX) NULL,
    category NVARCHAR(100) NULL,

    is_active BIT NULL 
        CONSTRAINT DF_features_is_active DEFAULT 1,

    metadata NVARCHAR(MAX) NULL,

    created_at DATETIME NULL,
    updated_at DATETIME NULL,
    deleted_at DATETIME NULL,

    CONSTRAINT CK_features_metadata_json 
        CHECK (metadata IS NULL OR ISJSON(metadata) = 1)
);
GO


-- =========================================================
-- TENANT FEATURES
-- =========================================================

CREATE TABLE tenant_features (
    tenant_id INT NOT NULL,
    feature_id INT NOT NULL,

    is_enabled BIT NULL 
        CONSTRAINT DF_tenant_features_is_enabled DEFAULT 1,

    enabled_at DATETIME NULL,
    expires_at DATETIME NULL,
    updated_at DATETIME NULL,

    metadata NVARCHAR(MAX) NULL,

    CONSTRAINT PK_tenant_features 
        PRIMARY KEY (tenant_id, feature_id),

    CONSTRAINT CK_tenant_features_metadata_json 
        CHECK (metadata IS NULL OR ISJSON(metadata) = 1)
);
GO


-- =========================================================
-- SETTING GROUPS
-- =========================================================

CREATE TABLE setting_groups (
    id INT IDENTITY(1,1) PRIMARY KEY,

    code NVARCHAR(100) NOT NULL UNIQUE,
    name NVARCHAR(150) NOT NULL,

    description NVARCHAR(300) NULL,

    is_active BIT NULL 
        CONSTRAINT DF_setting_groups_is_active DEFAULT 1,

    metadata NVARCHAR(MAX) NULL,

    created_at DATETIME NULL,
    updated_at DATETIME NULL,
    deleted_at DATETIME NULL,

    CONSTRAINT CK_setting_groups_metadata_json 
        CHECK (metadata IS NULL OR ISJSON(metadata) = 1)
);
GO


-- =========================================================
-- SETTING DEFINITIONS
-- =========================================================

CREATE TABLE setting_definitions (
    id INT IDENTITY(1,1) PRIMARY KEY,

    setting_group_id INT NOT NULL,

    code NVARCHAR(100) NOT NULL,
    name NVARCHAR(150) NOT NULL,

    description NVARCHAR(300) NULL,

    data_type NVARCHAR(50) NOT NULL,

    default_value NVARCHAR(MAX) NULL,
    validation_schema NVARCHAR(MAX) NULL,

    is_required BIT NULL 
        CONSTRAINT DF_setting_definitions_is_required DEFAULT 0,

    is_sensitive BIT NULL 
        CONSTRAINT DF_setting_definitions_is_sensitive DEFAULT 0,

    is_active BIT NULL 
        CONSTRAINT DF_setting_definitions_is_active DEFAULT 1,

    metadata NVARCHAR(MAX) NULL,

    created_at DATETIME NULL,
    updated_at DATETIME NULL,
    deleted_at DATETIME NULL,

    CONSTRAINT UQ_setting_definitions_group_code 
        UNIQUE (setting_group_id, code),

    CONSTRAINT CK_setting_definitions_data_type 
        CHECK (data_type IN ('STRING', 'INT', 'DECIMAL', 'BOOL', 'DATE', 'DATETIME', 'JSON', 'ARRAY')),

    CONSTRAINT CK_setting_definitions_default_value_json 
        CHECK (default_value IS NULL OR ISJSON(default_value) = 1),

    CONSTRAINT CK_setting_definitions_validation_schema_json 
        CHECK (validation_schema IS NULL OR ISJSON(validation_schema) = 1),

    CONSTRAINT CK_setting_definitions_metadata_json 
        CHECK (metadata IS NULL OR ISJSON(metadata) = 1)
);
GO


-- =========================================================
-- SETTING VALUES
-- =========================================================

CREATE TABLE setting_values (
    id INT IDENTITY(1,1) PRIMARY KEY,

    tenant_id INT NOT NULL,
    setting_definition_id INT NOT NULL,

    actor_type NVARCHAR(50) NOT NULL,

    actor_id INT NULL,

    value NVARCHAR(MAX) NOT NULL,

    is_active BIT NULL 
        CONSTRAINT DF_setting_values_is_active DEFAULT 1,

    metadata NVARCHAR(MAX) NULL,

    created_at DATETIME NULL,
    updated_at DATETIME NULL,
    deleted_at DATETIME NULL,

    CONSTRAINT CK_setting_values_actor_type 
        CHECK (actor_type IN ('TENANT', 'USER', 'ROLE', 'COMPANY', 'BRANCH', 'MODULE')),

    CONSTRAINT CK_setting_values_value_json 
        CHECK (ISJSON(value) = 1),

    CONSTRAINT CK_setting_values_metadata_json 
        CHECK (metadata IS NULL OR ISJSON(metadata) = 1)
);
GO

-- Índice único filtrado para actor_id NULL
-- SQL Server permite múltiples NULL en UNIQUE, por eso se separa el caso TENANT.
CREATE UNIQUE INDEX UQ_setting_values_actor_null
ON setting_values (tenant_id, setting_definition_id, actor_type)
WHERE actor_id IS NULL;
GO

CREATE UNIQUE INDEX UQ_setting_values_actor_not_null
ON setting_values (tenant_id, setting_definition_id, actor_type, actor_id)
WHERE actor_id IS NOT NULL;
GO


-- =========================================================
-- RULE DEFINITIONS
-- =========================================================

CREATE TABLE rule_definitions (
    id INT IDENTITY(1,1) PRIMARY KEY,

    code NVARCHAR(100) NOT NULL UNIQUE,
    name NVARCHAR(150) NOT NULL,

    module NVARCHAR(100) NULL,
    description NVARCHAR(MAX) NULL,

    condition_schema NVARCHAR(MAX) NULL,
    action_schema NVARCHAR(MAX) NULL,

    is_active BIT NULL 
        CONSTRAINT DF_rule_definitions_is_active DEFAULT 1,

    metadata NVARCHAR(MAX) NULL,

    created_at DATETIME NULL,
    updated_at DATETIME NULL,
    deleted_at DATETIME NULL,

    CONSTRAINT CK_rule_definitions_condition_schema_json 
        CHECK (condition_schema IS NULL OR ISJSON(condition_schema) = 1),

    CONSTRAINT CK_rule_definitions_action_schema_json 
        CHECK (action_schema IS NULL OR ISJSON(action_schema) = 1),

    CONSTRAINT CK_rule_definitions_metadata_json 
        CHECK (metadata IS NULL OR ISJSON(metadata) = 1)
);
GO


-- =========================================================
-- TENANT BUSINESS RULES
-- =========================================================

CREATE TABLE tenant_business_rules (
    tenant_id INT NOT NULL,
    rule_definition_id INT NOT NULL,

    is_active BIT NULL 
        CONSTRAINT DF_tenant_business_rules_is_active DEFAULT 1,

    priority INT NULL 
        CONSTRAINT DF_tenant_business_rules_priority DEFAULT 0,

    rule_version INT NULL 
        CONSTRAINT DF_tenant_business_rules_rule_version DEFAULT 1,

    execution_mode NVARCHAR(50) NULL,

    custom_condition NVARCHAR(MAX) NULL,
    custom_action NVARCHAR(MAX) NULL,

    metadata NVARCHAR(MAX) NULL,

    created_at DATETIME NULL,
    updated_at DATETIME NULL,
    deleted_at DATETIME NULL,

    CONSTRAINT PK_tenant_business_rules 
        PRIMARY KEY (tenant_id, rule_definition_id),

    CONSTRAINT CK_tenant_business_rules_execution_mode 
        CHECK (
            execution_mode IS NULL 
            OR execution_mode IN ('SYNC', 'ASYNC', 'EVENT_DRIVEN', 'SCHEDULED')
        ),

    CONSTRAINT CK_tenant_business_rules_custom_condition_json 
        CHECK (custom_condition IS NULL OR ISJSON(custom_condition) = 1),

    CONSTRAINT CK_tenant_business_rules_custom_action_json 
        CHECK (custom_action IS NULL OR ISJSON(custom_action) = 1),

    CONSTRAINT CK_tenant_business_rules_metadata_json 
        CHECK (metadata IS NULL OR ISJSON(metadata) = 1)
);
GO


-- =========================================================
-- TENANT BRANDING
-- =========================================================

CREATE TABLE tenant_branding (
    id INT IDENTITY(1,1) PRIMARY KEY,

    tenant_id INT NOT NULL UNIQUE,

    primary_color NVARCHAR(20) NULL,
    secondary_color NVARCHAR(20) NULL,
    accent_color NVARCHAR(20) NULL,

    logo_url NVARCHAR(MAX) NULL,
    favicon_url NVARCHAR(MAX) NULL,
    login_background_url NVARCHAR(MAX) NULL,

    custom_css NVARCHAR(MAX) NULL,

    metadata NVARCHAR(MAX) NULL,

    created_at DATETIME NULL,
    updated_at DATETIME NULL,
    deleted_at DATETIME NULL,

    CONSTRAINT CK_tenant_branding_metadata_json 
        CHECK (metadata IS NULL OR ISJSON(metadata) = 1)
);
GO


-- =========================================================
-- TENANT DATABASE CONNECTIONS
-- =========================================================

CREATE TABLE tenant_database_connections (
    id INT IDENTITY(1,1) PRIMARY KEY,

    tenant_id INT NOT NULL UNIQUE,

    database_name NVARCHAR(150) NOT NULL,

    server_name NVARCHAR(150) NULL,

    provider NVARCHAR(50) NULL,

    connection_string_encrypted NVARCHAR(MAX) NULL,

    is_active BIT NULL 
        CONSTRAINT DF_tenant_database_connections_is_active DEFAULT 1,

    metadata NVARCHAR(MAX) NULL,

    created_at DATETIME NULL,
    updated_at DATETIME NULL,
    deleted_at DATETIME NULL,

    CONSTRAINT CK_tenant_database_connections_provider 
        CHECK (
            provider IS NULL 
            OR provider IN ('SQLSERVER', 'POSTGRESQL', 'MYSQL')
        ),

    CONSTRAINT CK_tenant_database_connections_metadata_json 
        CHECK (metadata IS NULL OR ISJSON(metadata) = 1)
);
GO


-- =========================================================
-- ADMIN USERS
-- =========================================================

CREATE TABLE admin_users (
    id INT IDENTITY(1,1) PRIMARY KEY,

    email NVARCHAR(150) NOT NULL UNIQUE,
    username NVARCHAR(100) NULL UNIQUE,

    password_hash NVARCHAR(MAX) NOT NULL,

    first_name NVARCHAR(100) NULL,
    last_name NVARCHAR(100) NULL,

    is_active BIT NULL 
        CONSTRAINT DF_admin_users_is_active DEFAULT 1,

    last_login_at DATETIME NULL,

    metadata NVARCHAR(MAX) NULL,

    created_at DATETIME NULL,
    updated_at DATETIME NULL,
    deleted_at DATETIME NULL,

    CONSTRAINT CK_admin_users_metadata_json 
        CHECK (metadata IS NULL OR ISJSON(metadata) = 1)
);
GO


-- =========================================================
-- REFRESH TOKENS
-- =========================================================

CREATE TABLE refresh_tokens (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,

    admin_user_id INT NOT NULL,

    token NVARCHAR(500) NOT NULL UNIQUE,

    jwt_id NVARCHAR(150) NULL,

    is_revoked BIT NULL 
        CONSTRAINT DF_refresh_tokens_is_revoked DEFAULT 0,

    expires_at DATETIME NOT NULL,

    created_at DATETIME NULL,
    revoked_at DATETIME NULL
);
GO


-- =========================================================
-- FOREIGN KEYS
-- =========================================================

ALTER TABLE addresses
ADD CONSTRAINT FK_addresses_tenants
FOREIGN KEY (tenant_id) REFERENCES tenants(id);
GO

ALTER TABLE tenant_features
ADD CONSTRAINT FK_tenant_features_tenants
FOREIGN KEY (tenant_id) REFERENCES tenants(id);
GO

ALTER TABLE tenant_features
ADD CONSTRAINT FK_tenant_features_features
FOREIGN KEY (feature_id) REFERENCES features(id);
GO

ALTER TABLE setting_definitions
ADD CONSTRAINT FK_setting_definitions_setting_groups
FOREIGN KEY (setting_group_id) REFERENCES setting_groups(id);
GO

ALTER TABLE setting_values
ADD CONSTRAINT FK_setting_values_tenants
FOREIGN KEY (tenant_id) REFERENCES tenants(id);
GO

ALTER TABLE setting_values
ADD CONSTRAINT FK_setting_values_setting_definitions
FOREIGN KEY (setting_definition_id) REFERENCES setting_definitions(id);
GO

ALTER TABLE tenant_business_rules
ADD CONSTRAINT FK_tenant_business_rules_tenants
FOREIGN KEY (tenant_id) REFERENCES tenants(id);
GO

ALTER TABLE tenant_business_rules
ADD CONSTRAINT FK_tenant_business_rules_rule_definitions
FOREIGN KEY (rule_definition_id) REFERENCES rule_definitions(id);
GO

ALTER TABLE tenant_branding
ADD CONSTRAINT FK_tenant_branding_tenants
FOREIGN KEY (tenant_id) REFERENCES tenants(id);
GO

ALTER TABLE tenant_database_connections
ADD CONSTRAINT FK_tenant_database_connections_tenants
FOREIGN KEY (tenant_id) REFERENCES tenants(id);
GO

ALTER TABLE refresh_tokens
ADD CONSTRAINT FK_refresh_tokens_admin_users
FOREIGN KEY (admin_user_id) REFERENCES admin_users(id);
GO


-- =========================================================
-- RECOMMENDED INDEXES
-- =========================================================

CREATE INDEX IX_addresses_tenant_id
ON addresses (tenant_id);
GO

CREATE INDEX IX_tenant_features_feature_id
ON tenant_features (feature_id);
GO

CREATE INDEX IX_setting_definitions_group_id
ON setting_definitions (setting_group_id);
GO

CREATE INDEX IX_setting_values_tenant_id
ON setting_values (tenant_id);
GO

CREATE INDEX IX_setting_values_definition_id
ON setting_values (setting_definition_id);
GO

CREATE INDEX IX_tenant_business_rules_rule_definition_id
ON tenant_business_rules (rule_definition_id);
GO

CREATE INDEX IX_refresh_tokens_admin_user_id
ON refresh_tokens (admin_user_id);
GO

CREATE INDEX IX_refresh_tokens_expires_at
ON refresh_tokens (expires_at);
GO