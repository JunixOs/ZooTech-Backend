
namespace ZooTech.Domain.Entities.Configuration
{
    public static class ConfigSettings
    {
        /*
        # ============================================================
        #  Clase generada: Caja
        # ============================================================
        #  Descripción:
        #  Esta clase representa el módulo de configuración CAJA.
        #
        #  Uso:
        #  ConfigSettings.Caja.NombreParametro
        # ============================================================
        */
        public static class Caja
        {
            /// <summary>
            /// Permitir saldo negativo en caja
            /// Código: ALLOW_NEGATIVE_CASH_BALANCE
            /// Tipo: BOOL
            /// </summary>
            public static bool AllowNegativeCashBalance => false;

            /// <summary>
            /// Requiere aprobación de cierre de caja
            /// Código: REQUIRE_CASH_CLOSING_APPROVAL
            /// Tipo: BOOL
            /// </summary>
            public static bool RequireCashClosingApproval => true;

        }

        /*
        # ============================================================
        #  Clase generada: Facturacion
        # ============================================================
        #  Descripción:
        #  Esta clase representa el módulo de configuración FACTURACION.
        #
        #  Uso:
        #  ConfigSettings.Facturacion.NombreParametro
        # ============================================================
        */
        public static class Facturacion
        {
            /// <summary>
            /// Moneda por defecto
            /// Código: DEFAULT_CURRENCY
            /// Tipo: STRING
            /// </summary>
            public static string DefaultCurrency => "PEN";

            /// <summary>
            /// Habilitar facturación electrónica
            /// Código: ENABLE_ELECTRONIC_INVOICE
            /// Tipo: BOOL
            /// </summary>
            public static bool EnableElectronicInvoice => true;

            /// <summary>
            /// Porcentaje de IGV
            /// Código: IGV_PERCENTAGE
            /// Tipo: DECIMAL
            /// </summary>
            public static decimal IgvPercentage => 18.00m;

        }

        /*
        # ============================================================
        #  Clase generada: Inventario
        # ============================================================
        #  Descripción:
        #  Esta clase representa el módulo de configuración INVENTARIO.
        #
        #  Uso:
        #  ConfigSettings.Inventario.NombreParametro
        # ============================================================
        */
        public static class Inventario
        {
            /// <summary>
            /// Permitir stock negativo
            /// Código: ALLOW_NEGATIVE_STOCK
            /// Tipo: BOOL
            /// </summary>
            public static bool AllowNegativeStock => false;

            /// <summary>
            /// Habilitar alerta de stock bajo
            /// Código: LOW_STOCK_ALERT_ENABLED
            /// Tipo: BOOL
            /// </summary>
            public static bool LowStockAlertEnabled => true;

        }

        /*
        # ============================================================
        #  Clase generada: Produccionleche
        # ============================================================
        #  Descripción:
        #  Esta clase representa el módulo de configuración PRODUCCIONLECHE.
        #
        #  Uso:
        #  ConfigSettings.Produccionleche.NombreParametro
        # ============================================================
        */
        public static class Produccionleche
        {
            /// <summary>
            /// Página por defecto
            /// Código: DEFAULT_PAGE
            /// Tipo: INT
            /// </summary>
            public static int DefaultPage => 1;

            /// <summary>
            /// Tamaño de página por defecto
            /// Código: DEFAULT_PAGE_SIZE
            /// Tipo: INT
            /// </summary>
            public static int DefaultPageSize => 20;

            /// <summary>
            /// Tamaño máximo de página
            /// Código: MAX_PAGE_SIZE
            /// Tipo: INT
            /// </summary>
            public static int MaxPageSize => 100;

            /// <summary>
            /// Error de vacuno obligatorio
            /// Código: MILKING_CATTLE_REQUIRED_ERROR
            /// Tipo: STRING
            /// </summary>
            public static string MilkingCattleRequiredError => "El ID del vacuno es obligatorio y debe ser un número positivo.";

            /// <summary>
            /// Error de longitud de código
            /// Código: MILKING_CODE_MAX_LENGTH_ERROR
            /// Tipo: STRING
            /// </summary>
            public static string MilkingCodeMaxLengthError => "El código del ordeño no puede exceder 15 caracteres.";

            /// <summary>
            /// Error de código obligatorio
            /// Código: MILKING_CODE_REQUIRED_ERROR
            /// Tipo: STRING
            /// </summary>
            public static string MilkingCodeRequiredError => "El código del ordeño es obligatorio.";

            /// <summary>
            /// Error de fecha futura
            /// Código: MILKING_DATETIME_FUTURE_ERROR
            /// Tipo: STRING
            /// </summary>
            public static string MilkingDatetimeFutureError => "La fecha y hora del ordeño no puede ser en el futuro.";

            /// <summary>
            /// Error de longitud de motivo de eliminación
            /// Código: MILKING_DELETE_REASON_MAX_LENGTH_ERROR
            /// Tipo: STRING
            /// </summary>
            public static string MilkingDeleteReasonMaxLengthError => "El motivo de eliminación no puede exceder 200 caracteres.";

            /// <summary>
            /// Error de motivo de eliminación obligatorio
            /// Código: MILKING_DELETE_REASON_REQUIRED_ERROR
            /// Tipo: STRING
            /// </summary>
            public static string MilkingDeleteReasonRequiredError => "El motivo de eliminación es obligatorio.";

            /// <summary>
            /// Error de litros negativos
            /// Código: MILKING_LITERS_NEGATIVE_ERROR
            /// Tipo: STRING
            /// </summary>
            public static string MilkingLitersNegativeError => "La cantidad de litros no puede ser negativa.";

            /// <summary>
            /// Error de encargado obligatorio
            /// Código: MILKING_MANAGER_REQUIRED_ERROR
            /// Tipo: STRING
            /// </summary>
            public static string MilkingManagerRequiredError => "El ID del encargado es obligatorio y debe ser un número positivo.";

            /// <summary>
            /// Error de longitud de observaciones
            /// Código: MILKING_OBSERVATIONS_MAX_LENGTH_ERROR
            /// Tipo: STRING
            /// </summary>
            public static string MilkingObservationsMaxLengthError => "Las observaciones no pueden exceder 150 caracteres.";

            /// <summary>
            /// Error de longitud de estado
            /// Código: MILKING_STATUS_MAX_LENGTH_ERROR
            /// Tipo: STRING
            /// </summary>
            public static string MilkingStatusMaxLengthError => "El estado del ordeño no puede exceder 30 caracteres.";

            /// <summary>
            /// Error de estado obligatorio
            /// Código: MILKING_STATUS_REQUIRED_ERROR
            /// Tipo: STRING
            /// </summary>
            public static string MilkingStatusRequiredError => "El estado del ordeño es obligatorio.";

        }

        /*
        # ============================================================
        #  Clase generada: Reporteleche
        # ============================================================
        #  Descripción:
        #  Esta clase representa el módulo de configuración REPORTELECHE.
        #
        #  Uso:
        #  ConfigSettings.Reporteleche.NombreParametro
        # ============================================================
        */
        public static class Reporteleche
        {
            /// <summary>
            /// Nombre de la empresa en reportes
            /// Código: REPORT_COMPANY_NAME
            /// Tipo: STRING
            /// </summary>
            public static string ReportCompanyName => "ZooTech Platform";

            /// <summary>
            /// Título del reporte diario de producción
            /// Código: REPORT_DAILY_PRODUCTION_TITLE
            /// Tipo: STRING
            /// </summary>
            public static string ReportDailyProductionTitle => "Reporte de Producción Diaria de Leche";

            /// <summary>
            /// Nombre del departamento en reportes
            /// Código: REPORT_DEPARTMENT_NAME
            /// Tipo: STRING
            /// </summary>
            public static string ReportDepartmentName => "Producción de Leche";

            /// <summary>
            /// Color del encabezado del Excel
            /// Código: REPORT_EXCEL_HEADER_COLOR
            /// Tipo: STRING
            /// </summary>
            public static string ReportExcelHeaderColor => "LightPink";

            /// <summary>
            /// Nombre base del archivo de reporte
            /// Código: REPORT_FILE_BASE_NAME
            /// Tipo: STRING
            /// </summary>
            public static string ReportFileBaseName => "reporte_produccion";

            /// <summary>
            /// Prefijo de vacuno para nombre de archivo
            /// Código: REPORT_FILE_CATTLE_PREFIX
            /// Tipo: STRING
            /// </summary>
            public static string ReportFileCattlePrefix => "_vacuno_";

            /// <summary>
            /// Formato de fecha para nombre de archivo
            /// Código: REPORT_FILE_DATE_FORMAT
            /// Tipo: STRING
            /// </summary>
            public static string ReportFileDateFormat => "yyyyMMdd";

            /// <summary>
            /// Formato de fecha y hora para nombre de archivo
            /// Código: REPORT_FILE_DATETIME_FORMAT
            /// Tipo: STRING
            /// </summary>
            public static string ReportFileDatetimeFormat => "yyyyMMdd_HHmmss";

            /// <summary>
            /// Extensión del archivo de reporte
            /// Código: REPORT_FILE_EXTENSION
            /// Tipo: STRING
            /// </summary>
            public static string ReportFileExtension => "pdf";

        }

        /*
        # ============================================================
        #  Clase generada: Seguridad
        # ============================================================
        #  Descripción:
        #  Esta clase representa el módulo de configuración SEGURIDAD.
        #
        #  Uso:
        #  ConfigSettings.Seguridad.NombreParametro
        # ============================================================
        */
        public static class Seguridad
        {
            /// <summary>
            /// Habilitar doble factor de autenticación
            /// Código: ENABLE_TWO_FACTOR_AUTH
            /// Tipo: BOOL
            /// </summary>
            public static bool EnableTwoFactorAuth => false;

            /// <summary>
            /// Máximo de intentos de inicio de sesión
            /// Código: MAX_LOGIN_ATTEMPTS
            /// Tipo: INT
            /// </summary>
            public static int MaxLoginAttempts => 5;

            /// <summary>
            /// Días de expiración de contraseña
            /// Código: PASSWORD_EXPIRATION_DAYS
            /// Tipo: INT
            /// </summary>
            public static int PasswordExpirationDays => 90;

            /// <summary>
            /// Tiempo de expiración de sesión
            /// Código: SESSION_TIMEOUT_MINUTES
            /// Tipo: INT
            /// </summary>
            public static int SessionTimeoutMinutes => 30;

        }

        /*
        # ============================================================
        #  Clase generada: Usuarios
        # ============================================================
        #  Descripción:
        #  Esta clase representa el módulo de configuración USUARIOS.
        #
        #  Uso:
        #  ConfigSettings.Usuarios.NombreParametro
        # ============================================================
        */
        public static class Usuarios
        {
            /// <summary>
            /// Rol por defecto de usuario
            /// Código: DEFAULT_USER_ROLE
            /// Tipo: STRING
            /// </summary>
            public static string DefaultUserRole => "USER";

            /// <summary>
            /// Permitir autorregistro de usuarios
            /// Código: ENABLE_USER_SELF_REGISTRATION
            /// Tipo: BOOL
            /// </summary>
            public static bool EnableUserSelfRegistration => false;

        }

        /*
        # ============================================================
        #  Clase generada: Ventas
        # ============================================================
        #  Descripción:
        #  Esta clase representa el módulo de configuración VENTAS.
        #
        #  Uso:
        #  ConfigSettings.Ventas.NombreParametro
        # ============================================================
        */
        public static class Ventas
        {
            /// <summary>
            /// Descuento máximo permitido
            /// Código: MAX_DISCOUNT_PERCENTAGE
            /// Tipo: DECIMAL
            /// </summary>
            public static decimal MaxDiscountPercentage => 20.00m;

            /// <summary>
            /// Aprobar descuentos altos
            /// Código: REQUIRE_APPROVAL_FOR_HIGH_DISCOUNT
            /// Tipo: BOOL
            /// </summary>
            public static bool RequireApprovalForHighDiscount => true;

        }

    }
}

