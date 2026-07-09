using ZooTech.Application.Common.Validator;
using ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.Common;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.UpdateFecundacionEstado;

public sealed class UpdateFecundacionEstadoValidator : ICommandValidator<UpdateFecundacionEstadoCommand>
{
    public ModuleName ModuleName => ModuleName.Celo;

    public List<string> Validate(UpdateFecundacionEstadoCommand request)
    {
        List<string> errors = new List<string>();

        if (request.FecundacionId <= 0)
        {
            errors.Add("CELO-UPDATE-FECUNDACION-ID-INVALID");
        }

        if (string.IsNullOrWhiteSpace(request.EstadoFecundacion))
        {
            errors.Add("CELO-UPDATE-FECUNDACION_ESTADO-NULL");
        }

        if (!request.UpdatedBy.HasValue || request.UpdatedBy.Value <= 0)
        {
            errors.Add("CELO-UPDATE-FECUNDACION_UPDATED_BY-INVALID");
        }

        var estado = request.EstadoFecundacion?.Trim() ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(estado) && !FecundacionEstadoConstants.IsKnownEstado(estado))
        {
            errors.Add("CELO-UPDATE-FECUNDACION_ESTADO-INVALID");
        }

        return errors;
    }
}
