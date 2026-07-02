namespace ZooTech.Domain.Shared.Enums
{
    public enum ErrorType
    {
        Validation,       // Datos inválidos
        NotFound,         // Recurso inexistente
        Conflict,         // Estado o unicidad
        Unauthorized,     // No autenticado
        Forbidden,        // Sin permisos
        External,         // Dependencia externa
        Infrastructure,   // BD, disco, cache, etc.
        Unexpected,        // Error no controlado
        RateLimited,     // Demasiadas peticiones
        Timeout,         // Tiempo de espera agotado
        Cancelled,       // Operación cancelada
        Unavailable      // Servicio temporalmente no disponible
    }
}