using ZooTech.Application.Common.Validator;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.UpdateFecundacion;
using ZooTech.Domain.Module_Fecundacion.Rules;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Fecundacion.Validators;

public sealed class UpdateFecundacionValidator : ICommandValidator<UpdateFecundacionCommand>
{
    public ModuleName ModuleName => ModuleName.Fecundacion;
    
    public List<string> Validate(UpdateFecundacionCommand request)
    {
        var errors = new List<string>();

        if (request.Id <= 0)
        {
            errors.Add("FECUNDACION-FECUNDACION-UPDATE-ID-INVALID");
        }

        if (string.IsNullOrWhiteSpace(request.TipoFecundacionCode))
        {
            errors.Add("FECUNDACION-FECUNDACION-UPDATE-TIPO_FECUNDACION_CODE-NULL");
        }
        else if (request.TipoFecundacionCode.Length > 40)
        {
            errors.Add("FECUNDACION-FECUNDACION-UPDATE-TIPO_FECUNDACION_CODE-INVALID");
        }

        if (request.VacunoReceptorId <= 0)
        {
            errors.Add("FECUNDACION-FECUNDACION-UPDATE-VACUNO_RECEPTOR_ID-INVALID");
        }

        if (request.FechaProcedimiento > DateOnly.FromDateTime(DateTime.Today))
        {
            errors.Add("FECUNDACION-FECUNDACION-UPDATE-FECHA_PROCEDIMIENTO-INVALID");
        }

        if (string.IsNullOrWhiteSpace(request.ResponsableNombre))
        {
            errors.Add("FECUNDACION-FECUNDACION-UPDATE-RESPONSABLE_NOMBRE-NULL");
        }
        else if (request.ResponsableNombre.Length > 100)
        {
            errors.Add("FECUNDACION-FECUNDACION-UPDATE-RESPONSABLE_NOMBRE-INVALID");
        }

        if (string.IsNullOrWhiteSpace(request.ResultadoCode))
        {
            errors.Add("FECUNDACION-FECUNDACION-UPDATE-RESULTADO_CODE-NULL");
        }
        else if (request.ResultadoCode.Length > 30)
        {
            errors.Add("FECUNDACION-FECUNDACION-UPDATE-RESULTADO_CODE-INVALID");
        }

        if (string.IsNullOrWhiteSpace(request.EstadoFecundacionCode))
        {
            errors.Add("FECUNDACION-FECUNDACION-UPDATE-ESTADO_FECUNDACION_CODE-NULL");
        }
        else if (request.EstadoFecundacionCode.Length > 30)
        {
            errors.Add("FECUNDACION-FECUNDACION-UPDATE-ESTADO_FECUNDACION_CODE-INVALID");
        }

        if (!string.IsNullOrWhiteSpace(request.ObservacionesVeterinarias) &&
            request.ObservacionesVeterinarias.Length > 250)
        {
            errors.Add("FECUNDACION-FECUNDACION-UPDATE-OBSERVACIONES_VETERINARIAS-INVALID");
        }

        if (string.IsNullOrWhiteSpace(request.TipoDonante))
        {
            errors.Add("FECUNDACION-FECUNDACION-UPDATE-TIPO_DONANTE-NULL");
        }
        else if (!string.Equals(request.TipoDonante, FecundacionRules.TipoDonanteInterno, StringComparison.OrdinalIgnoreCase) &&
                 !string.Equals(request.TipoDonante, FecundacionRules.TipoDonanteExterno, StringComparison.OrdinalIgnoreCase))
        {
            errors.Add("FECUNDACION-FECUNDACION-UPDATE-TIPO_DONANTE-INVALID");
        }

        if (string.Equals(request.TipoDonante, FecundacionRules.TipoDonanteInterno, StringComparison.OrdinalIgnoreCase))
        {
            if (!request.VacunoDonanteId.HasValue)
            {
                errors.Add("FECUNDACION-FECUNDACION-UPDATE-VACUNO_DONANTE_ID-NULL");
            }
            else if (request.VacunoDonanteId.Value <= 0)
            {
                errors.Add("FECUNDACION-FECUNDACION-UPDATE-VACUNO_DONANTE_ID-INVALID");
            }
        }

        if (string.Equals(request.TipoDonante, FecundacionRules.TipoDonanteExterno, StringComparison.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(request.ExternoDonanteNombre))
            {
                errors.Add("FECUNDACION-FECUNDACION-UPDATE-EXTERNO_DONANTE_NOMBRE-NULL");
            }
            else if (request.ExternoDonanteNombre.Length > 100)
            {
                errors.Add("FECUNDACION-FECUNDACION-UPDATE-EXTERNO_DONANTE_NOMBRE-INVALID");
            }
        }

        FecundacionCommonValidationRules.ValidateCodigoSemenAndEmbrion(
            errors, "UPDATE", request.TipoFecundacionCode, request.CodigoSemen, request.CodigoEmbrion);

        return errors;
    }
}
