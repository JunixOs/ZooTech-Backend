using System;

namespace ZooTech.Application.Modules.Module_Fecundacion.Exceptions;

public class FecundacionException : Exception
{
    public string ErrorCode { get; }
    public int StatusCode { get; }

    public FecundacionException(string message, string errorCode, int statusCode)
        : base(message)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
    }
}

public sealed class FecundacionNotFoundException : FecundacionException
{
    public FecundacionNotFoundException(string message = "El registro de fecundacion no existe.")
        : base(message, "FECUNDACION_NOT_FOUND", 404)
    {
    }
}

public sealed class FecundacionPendingActiveException : FecundacionException
{
    public FecundacionPendingActiveException(
        string message = "La hembra seleccionada ya tiene una fecundacion activa. Cambie su estado a Vacia o registre el cierre antes de crear otra fecundacion.")
        : base(message, "FECUNDACION_PENDIENTE_ACTIVA", 409)
    {
    }
}

public sealed class FecundacionHasDependenciesException : FecundacionException
{
    public FecundacionHasDependenciesException(string message = "Tiene trazabilidades u otros registros activos dependientes.")
        : base(message, "FECUNDACION_HAS_DEPENDENCIES", 409)
    {
    }
}

public sealed class FecundacionVacunoNotFoundException : FecundacionException
{
    public FecundacionVacunoNotFoundException(string message = "El vacuno receptor no existe.")
        : base(message, "VACUNO_NOT_FOUND", 404)
    {
    }
}
