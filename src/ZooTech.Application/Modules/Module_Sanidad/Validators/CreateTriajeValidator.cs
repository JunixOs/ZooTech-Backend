using FluentValidation;
using ZooTech.Application.Common.Validator;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.CreateTriaje;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Sanidad.Validators;

public class CreateTriajeValidator : ICommandQueryValidator<CreateTriajeCommand>
{
    public ModuleName ModuleName => ModuleName.Triaje;

    public List<string> Validate(CreateTriajeCommand request)
    {
        var errors = new List<string>();

        if (request.VacunoId <= 0)
        {
            errors.Add("TRIAJE-TRIAJE-CREATE-VACUNO_ID-INVALID");
        }

        if (string.IsNullOrWhiteSpace(request.TipoPesoCode))
        {
            errors.Add("TRIAJE-TRIAJE-CREATE-TIPO_PESO_CODE-NULL");
        }

        if (request.PesoKg <= 0)
        {
            errors.Add("TRIAJE-TRIAJE-CREATE-PESO_KG-INVALID");
        }

        if (request.FechaHora is null)
        {
            errors.Add("TRIAJE-TRIAJE-CREATE-ESTADO_REGISTRO_CODE-NULL");
        }

        if (!string.IsNullOrWhiteSpace(request.Observaciones) &&
            request.Observaciones.Length > 500)
        {
            errors.Add("TRIAJE-TRIAJE-CREATE-OBSERVACIONES-INVALID");
        }

        if (request.EncargadoUsuarioId.HasValue &&
            request.EncargadoUsuarioId.Value <= 0)
        {
            errors.Add("TRIAJE-TRIAJE-CREATE-ENCARGADO_USUARIO_ID-INVALID");
        }

        return errors;
    }
}
