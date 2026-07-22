using ZooTech.Application.Common.Exceptions;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Vacuno.Exceptions;

public class VacunoException : AppApplicationException
{
    public VacunoException(
        ErrorType errorType,
        string? extraCode = null,
        List<string>? details = null,
        string? message = null
    ) : base(
        $"VACUNO_{extraCode}",
        errorType,
        ScopeName.Application,
        message ?? "A conflict occurred while tying to execute a Vacuno use case",
        Domain.Shared.Enums.ModuleName.Vacuno,
        details
    )
    {
    }
}

public sealed class VacunoNotFoundException : VacunoException
{
    public VacunoNotFoundException(
        string message = "No existe el vacuno."
    ) : base(
        ErrorType.NotFound,
        "NOT_FOUND", 
        message: message
    )
    {
    }
}

public sealed class VacunoAlreadyExistsException : VacunoException
{
    public VacunoAlreadyExistsException(
        string message = "Ya existe un vacuno con ese código o datos repetidos."
    ) : base(
        ErrorType.Conflict,
        "ALREADY_EXISTS", 
        message: message
    )
    {
    }
}

public sealed class ImmutableFieldException : VacunoException
{
    public ImmutableFieldException(
        string message = "Se intentó editar un campo no permitido."
    ) : base(
        ErrorType.Conflict,
        "IMMUTABLE_FIELD", 
        message: message
    )
    {
    }
}

public sealed class VacunoHasDependenciesException : VacunoException
{
    public VacunoHasDependenciesException(
        string message = "El vacuno tiene dependencias activas (triajes, celos o producción de leche)."
    ) : base(
        ErrorType.Conflict,
        "HAS_DEPENDENCIES", 
        message: message
    )
    {
    }
}

public sealed class InvalidReportFormatException : VacunoException
{
    public InvalidReportFormatException(
        string message = "Formato diferente de json, pdf o excel."
    ) : base(
        ErrorType.Validation,
        "INVALID_REPORT_FORMAT", 
        message: message 
        )
    {
    }
}

public sealed class InvalidGenerationLevelException : VacunoException
{
    public InvalidGenerationLevelException(
        string message = "Nivel mayor a 4 o menor a 1."
    ) : base(
        ErrorType.Validation,
        "INVALID_GENERATION_LEVEL", 
        message: message
    )
    {
    }
}
