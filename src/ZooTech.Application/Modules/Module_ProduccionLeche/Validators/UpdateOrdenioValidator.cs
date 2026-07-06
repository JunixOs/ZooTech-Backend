using FluentValidation;
using ZooTech.Application.Common.Validator;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.UpdateOrdenio;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.Validators;

internal sealed class UpdateOrdenioValidator : ICommandValidator<UpdateOrdenioCommand>
{
    public ModuleName ModuleName => ModuleName.Produccion_Leche;

    public List<string> Validate(UpdateOrdenioCommand request)
    {
        var errors = new List<string>();

        if (request.Id <= 0)
        {
            errors.Add("PRODUCCION_LECHE-ORDENIO-UPDATE-ID-INVALID");
        }

        if (request.EncargadoUsuarioId <= 0)
        {
            errors.Add("PRODUCCION_LECHE-ORDENIO-UPDATE-ENCARGADO_USUARIO_ID-INVALID");
        }

        if (request.Litros <= 0)
        {
            errors.Add("PRODUCCION_LECHE-ORDENIO-UPDATE-LITROS-INVALID");
        }

        if (string.IsNullOrWhiteSpace(request.EstadoOrdenioCode))
        {
            errors.Add("PRODUCCION_LECHE-ORDENIO-UPDATE-ESTADO_ORDENIO_CODE-NULL");
        }

        if (!string.IsNullOrWhiteSpace(request.Observaciones) &&
            request.Observaciones.Length > 150)
        {
            errors.Add("PRODUCCION_LECHE-ORDENIO-UPDATE-OBSERVACIONES-INVALID");
        }

        return errors;
    }
}
