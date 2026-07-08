using ZooTech.Application.Common.Validator;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.UpdateVacuno;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Vacuno.Validators;

internal sealed class UpdateVacunoValidator : ICommandValidator<UpdateVacunoCommand>
{
    public ModuleName ModuleName => throw new NotImplementedException();

    public List<string> Validate(UpdateVacunoCommand request)
    {
        var errors = new List<string>();

        if (request.Id <= 0)
        {
            errors.Add("VACUNO-VACUNO-UPDATE-ID-INVALID");
        }

        if (string.IsNullOrWhiteSpace(request.Nombre))
        {
            errors.Add("VACUNO-VACUNO-UPDATE-NOMBRE-NULL");
        }
        else if (request.Nombre.Length > 15)
        {
            errors.Add("VACUNO-VACUNO-UPDATE-NOMBRE-INVALID");
        }

        if (request.FechaNacimiento == default)
        {
            errors.Add("VACUNO-VACUNO-UPDATE-FECHA_NACIMIENTO-NULL");
        }

        if (string.IsNullOrWhiteSpace(request.TipoAdquisicionCode))
        {
            errors.Add("VACUNO-VACUNO-UPDATE-TIPO_ADQUISICION_CODE-NULL");
        }
        else if (request.TipoAdquisicionCode.Length > 30)
        {
            errors.Add("VACUNO-VACUNO-UPDATE-TIPO_ADQUISICION_CODE-INVALID");
        }

        if (string.IsNullOrWhiteSpace(request.RazaCode))
        {
            errors.Add("VACUNO-VACUNO-UPDATE-RAZA_CODE-NULL");
        }
        else if (request.RazaCode.Length > 30)
        {
            errors.Add("VACUNO-VACUNO-UPDATE-RAZA_CODE-INVALID");
        }

        if (string.IsNullOrWhiteSpace(request.ColorCode))
        {
            errors.Add("VACUNO-VACUNO-UPDATE-COLOR_CODE-NULL");
        }
        else if (request.ColorCode.Length > 30)
        {
            errors.Add("VACUNO-VACUNO-UPDATE-COLOR_CODE-INVALID");
        }

        if (string.IsNullOrWhiteSpace(request.SexoCode))
        {
            errors.Add("VACUNO-VACUNO-UPDATE-SEXO_CODE-NULL");
        }
        else if (request.SexoCode.Length > 10)
        {
            errors.Add("VACUNO-VACUNO-UPDATE-SEXO_CODE-INVALID");
        }

        if (request.GranjaId <= 0)
        {
            errors.Add("VACUNO-VACUNO-UPDATE-GRANJA_ID-INVALID");
        }

        if (!string.IsNullOrWhiteSpace(request.Observaciones))
        {
            if (request.Observaciones.Length > 150)
            {
                errors.Add("VACUNO-VACUNO-UPDATE-OBSERVACIONES-INVALID");
            }

            if (request.Observaciones
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Length > 30)
            {
                errors.Add("VACUNO-VACUNO-UPDATE-OBSERVACIONES-WORD_LIMIT");
            }
        }

        if (request.TipoAdquisicionCode == "COMPRA" && !request.PrecioCompra.HasValue)
        {
            errors.Add("VACUNO-VACUNO-UPDATE-PRECIO_COMPRA-NULL");
        }

        return errors;
    }
}
