using System;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Fecundacion.Exceptions;

public class FecundacionException : AppApplicationException
{
    public FecundacionException(
        ErrorType? errorType = null,
        string? message = null,
        string? extraCode = null
    ) : base(
        $"FECUNDACION_{extraCode}",
        errorType ?? ErrorType.Conflict,
        ScopeName.Application,
        message ?? "A conflict occurred while tying to execute a Fecundacion use case",
        Domain.Shared.Enums.ModuleName.Fecundacion
    )
    {
    }
}

public sealed class FecundacionNotFoundException : FecundacionException
{
    public FecundacionNotFoundException(
        string message = "El registro de fecundacion no existe."
    ) : base(
        ErrorType.NotFound,
        message, 
        "NOT_FOUND_ERROR"
    )
    {
    }
}

public sealed class FecundacionPendingActiveException : FecundacionException
{
    public FecundacionPendingActiveException(
        string message = "La hembra seleccionada ya tiene una fecundacion activa. Cambie su estado a Vacia o registre el cierre antes de crear otra fecundacion."
    ) : base(
        ErrorType.Conflict,
        message, 
        "PENDIENTE_ACTIVA_ERROR" 
    )
    {
    }
}

public sealed class FecundacionHasDependenciesException : FecundacionException
{
    public FecundacionHasDependenciesException(
        string message = "Tiene trazabilidades u otros registros activos dependientes."
    ) : base(
        ErrorType.Conflict,
        message, 
        "HAS_DEPENDENCIES_ERROR"
    )
    {
    }
}

public sealed class FecundacionVacunoNotFoundException : FecundacionException
{
    public FecundacionVacunoNotFoundException(
        string message = "El vacuno receptor no existe."
    ) : base(
        ErrorType.NotFound,
        message, 
        "VACUNO_NOT_FOUND_ERROR" 
    )
    {
    }
}

public sealed class FecundacionInvalidEstadoException : FecundacionException
{
    public FecundacionInvalidEstadoException(
        string message = "El estado de fecundacion indicado no existe."
    ) : base(
        ErrorType.Validation,
        message,
        "INVALID_ESTADO_ERROR"
    )
    {
    }
}
