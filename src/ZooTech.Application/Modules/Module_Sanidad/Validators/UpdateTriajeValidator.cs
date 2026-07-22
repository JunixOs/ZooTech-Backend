using FluentValidation;
using ZooTech.Application.Common.Validator;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.UpdateTriaje;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Sanidad.Validators;

internal sealed class UpdateTriajeValidator : ICommandValidator<UpdateTriajeCommand>
{
    public ModuleName ModuleName => ModuleName.Triaje;

    public List<string> Validate(UpdateTriajeCommand request)
    {
        var errors = new List<string>();

        if (request.Id <= 0)
        {
            errors.Add("SANIDAD-TRIAJE-UPDATE-ID-INVALID");
        }

        if (string.IsNullOrWhiteSpace(request.TipoPesoCode))
        {
            errors.Add("SANIDAD-TRIAJE-UPDATE-TIPO_PESO_CODE-NULL");
        }

        if (request.PesoKg <= 0)
        {
            errors.Add("SANIDAD-TRIAJE-UPDATE-PESO_KG-INVALID");
        }

        if (!string.IsNullOrWhiteSpace(request.Observaciones) &&
            request.Observaciones.Length > 500)
        {
            errors.Add("SANIDAD-TRIAJE-UPDATE-OBSERVACIONES-INVALID");
        }

        if (request.EncargadoUsuarioId.HasValue &&
            request.EncargadoUsuarioId.Value <= 0)
        {
            errors.Add("SANIDAD-TRIAJE-UPDATE-ENCARGADO_USUARIO_ID-INVALID");
        }

        return errors;
    }
}
