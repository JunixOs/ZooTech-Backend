using ZooTech.Application.Common.Validator;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.CreateFecundacion;
using ZooTech.Domain.Module_Fecundacion.Rules;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Fecundacion.Validators;

public sealed class CreateFecundacionValidator : ICommandValidator<CreateFecundacionCommand>
{
    public ModuleName ModuleName => ModuleName.Fecundacion;

    public List<string> Validate(CreateFecundacionCommand request)
    {
        var errors = new List<string>();

        if (request.VacunoReceptorId <= 0)
        {
            errors.Add("FECUNDACION-FECUNDACION-CREATE-VACUNO_RECEPTOR_ID-INVALID");
        }

        if (string.IsNullOrWhiteSpace(request.TipoFecundacionCode))
        {
            errors.Add("FECUNDACION-FECUNDACION-CREATE-TIPO_FECUNDACION_CODE-NULL");
        }

        if (string.IsNullOrWhiteSpace(request.ResultadoCode))
        {
            errors.Add("FECUNDACION-FECUNDACION-CREATE-RESULTADO_CODE-NULL");
        }

        if (string.IsNullOrWhiteSpace(request.ResponsableName))
        {
            errors.Add("FECUNDACION-FECUNDACION-CREATE-RESPONSABLE_NAME-NULL");
        }
        else if (request.ResponsableName.Length > 100)
        {
            errors.Add("FECUNDACION-FECUNDACION-CREATE-RESPONSABLE_NAME-INVALID");
        }

        if (!string.IsNullOrWhiteSpace(request.ObservacionesVeterinarias) &&
            request.ObservacionesVeterinarias.Length > 250)
        {
            errors.Add("FECUNDACION-FECUNDACION-CREATE-OBSERVACIONES_VETERINARIAS-INVALID");
        }

        if (request.FechaProcedimiento == default)
        {
            errors.Add("FECUNDACION-FECUNDACION-CREATE-FECHA_PROCEDIMIENTO-NULL");
        }

        if (request.MachoExterno)
        {
            if (string.IsNullOrWhiteSpace(request.MachoExternoNombre))
            {
                errors.Add("FECUNDACION-FECUNDACION-CREATE-MACHO_EXTERNO_NOMBRE-NULL");
            }
        }
        else
        {
            if (!request.VacunoDonanteId.HasValue)
            {
                errors.Add("FECUNDACION-FECUNDACION-CREATE-VACUNO_DONANTE_ID-NULL");
            }
            else if (request.VacunoDonanteId.Value <= 0)
            {
                errors.Add("FECUNDACION-FECUNDACION-CREATE-VACUNO_DONANTE_ID-INVALID");
            }
        }

        if (FecundacionRules.EsInseminacionArtificial(request.TipoFecundacionCode))
        {
            if (string.IsNullOrWhiteSpace(request.CodigoSemen))
            {
                errors.Add("FECUNDACION-FECUNDACION-CREATE-CODIGO_SEMEN-NULL");
            }
            else if (request.CodigoSemen.Length > 30)
            {
                errors.Add("FECUNDACION-FECUNDACION-CREATE-CODIGO_SEMEN-INVALID");
            }
        }

        if (FecundacionRules.EsTransferenciaEmbriones(request.TipoFecundacionCode))
        {
            if (string.IsNullOrWhiteSpace(request.CodigoEmbrion))
            {
                errors.Add("FECUNDACION-FECUNDACION-CREATE-CODIGO_EMBRION-NULL");
            }
            else if (request.CodigoEmbrion.Length > 30)
            {
                errors.Add("FECUNDACION-FECUNDACION-CREATE-CODIGO_EMBRION-INVALID");
            }
        }

        return errors;
    }
}
