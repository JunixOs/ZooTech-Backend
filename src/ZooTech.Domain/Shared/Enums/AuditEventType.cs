namespace ZooTech.Domain.Shared.Enums
{
    public enum AuditEventType
    {
        // Operaciones CRUD
        Create,
        Update,
        Delete,
        Restore,

        // Consultas
        Read,
        Search,
        Export,

        // Autenticación
        Login,
        Logout,
        LoginFailed,
        PasswordChanged,
        PasswordReset,

        // Seguridad
        AccessDenied,
        PermissionGranted,
        PermissionRevoked,

        // Estado
        Activate,
        Deactivate,
        Lock,
        Unlock,

        // Sistema
        ConfigurationChanged,
        DataImport,
        DataExport,

        // Otros
        Custom
    }
}