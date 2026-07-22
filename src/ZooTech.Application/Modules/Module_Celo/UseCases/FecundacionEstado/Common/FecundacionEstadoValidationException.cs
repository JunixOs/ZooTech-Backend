using ZooTech.Application.Common.Exceptions;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.Common;

public sealed class FecundacionEstadoValidationException : AppApplicationException
{
    public FecundacionEstadoValidationException(
        List<string>? errors = null,
        string? message = null
    ) : base(
        "",
        ErrorType.Validation,
        ScopeName.Application,
        message ?? "La solicitud de estado de fecundacion contiene errores de validacion.",
        Domain.Shared.Enums.ModuleName.Fecundacion,
        errors
    )
    {
    }
}
