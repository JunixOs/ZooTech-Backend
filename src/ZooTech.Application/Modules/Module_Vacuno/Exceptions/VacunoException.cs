using System;

namespace ZooTech.Application.Modules.Module_Vacuno.Exceptions;

public class VacunoException : Exception
{
    public string ErrorCode { get; }
    public int StatusCode { get; }

    public VacunoException(string message, string errorCode, int statusCode)
        : base(message)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
    }
}

public sealed class VacunoNotFoundException : VacunoException
{
    public VacunoNotFoundException(string message = "No existe el vacuno.")
        : base(message, "VACUNO_NOT_FOUND", 404)
    {
    }
}

public sealed class VacunoAlreadyExistsException : VacunoException
{
    public VacunoAlreadyExistsException(string message = "Ya existe un vacuno con ese código o datos repetidos.")
        : base(message, "VACUNO_ALREADY_EXISTS", 409)
    {
    }
}

public sealed class ImmutableFieldException : VacunoException
{
    public ImmutableFieldException(string message = "Se intentó editar un campo no permitido.")
        : base(message, "IMMUTABLE_FIELD", 400)
    {
    }
}

public sealed class VacunoHasDependenciesException : VacunoException
{
    public VacunoHasDependenciesException(string message = "El vacuno tiene dependencias activas (triajes, celos o producción de leche).")
        : base(message, "VACUNO_HAS_DEPENDENCIES", 409)
    {
    }
}

public sealed class InvalidReportFormatException : VacunoException
{
    public InvalidReportFormatException(string message = "Formato diferente de json, pdf o excel.")
        : base(message, "INVALID_REPORT_FORMAT", 400)
    {
    }
}

public sealed class InvalidGenerationLevelException : VacunoException
{
    public InvalidGenerationLevelException(string message = "Nivel mayor a 4 o menor a 1.")
        : base(message, "INVALID_GENERATION_LEVEL", 400)
    {
    }
}
