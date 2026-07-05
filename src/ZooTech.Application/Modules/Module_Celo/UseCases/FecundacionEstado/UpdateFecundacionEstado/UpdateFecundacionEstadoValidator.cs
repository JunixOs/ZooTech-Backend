using ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.Common;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.UpdateFecundacionEstado;

public sealed class UpdateFecundacionEstadoValidator
{
    public string ValidateAndNormalize(UpdateFecundacionEstadoCommand command)
    {
        Dictionary<string, string> errors = [];

        if (command.FecundacionId <= 0)
        {
            errors["fecundacionId"] = "El identificador de fecundacion debe ser mayor a cero.";
        }

        if (string.IsNullOrWhiteSpace(command.EstadoFecundacion))
        {
            errors["estadoFecundacion"] = "El estado de fecundacion es obligatorio.";
        }

        if (!command.UpdatedBy.HasValue || command.UpdatedBy.Value <= 0)
        {
            errors["updatedBy"] = "El usuario responsable de la actualizacion es obligatorio y debe ser mayor a cero.";
        }

        var estado = command.EstadoFecundacion?.Trim() ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(estado) && !FecundacionEstadoConstants.IsKnownEstado(estado))
        {
            errors["estadoFecundacion"] = "El estado de fecundacion indicado no esta permitido.";
        }

        if (errors.Count > 0)
        {
            throw new FecundacionEstadoValidationException(errors);
        }

        return estado;
    }
}
