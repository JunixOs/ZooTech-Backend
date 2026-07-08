using ZooTech.Application.Common.Validator;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.CreateVacuno;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Vacuno.Validators;

internal sealed class CreateVacunoValidator : ICommandValidator<CreateVacunoCommand>
{

    public ModuleName ModuleName => ModuleName.Vacuno;

    public List<string> Validate(CreateVacunoCommand request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Codigo))
        {
            errors.Add("VACUNO-VACUNO-CREATE-CODIGO-NULL");
        }
        else if (!System.Text.RegularExpressions.Regex.IsMatch(request.Codigo, @"^VAC[0-9]+$"))
        {
            errors.Add("VACUNO-VACUNO-CREATE-CODIGO-INVALID");
        }

        if (string.IsNullOrWhiteSpace(request.Nombre))
        {
            errors.Add("VACUNO-VACUNO-CREATE-NOMBRE-NULL");
        }
        else if (request.Nombre.Length > 15)
        {
            errors.Add("VACUNO-VACUNO-CREATE-NOMBRE-INVALID");
        }

        if (request.FechaNacimiento == default)
        {
            errors.Add("VACUNO-VACUNO-CREATE-FECHA_NACIMIENTO-NULL");
        }

        if (string.IsNullOrWhiteSpace(request.TipoAdquisicionCode))
        {
            errors.Add("VACUNO-VACUNO-CREATE-TIPO_ADQUISICION_CODE-NULL");
        }
        else if (request.TipoAdquisicionCode.Length > 30)
        {
            errors.Add("VACUNO-VACUNO-CREATE-TIPO_ADQUISICION_CODE-INVALID");
        }

        if (string.IsNullOrWhiteSpace(request.RazaCode))
        {
            errors.Add("VACUNO-VACUNO-CREATE-RAZA_CODE-NULL");
        }
        else if (request.RazaCode.Length > 30)
        {
            errors.Add("VACUNO-VACUNO-CREATE-RAZA_CODE-INVALID");
        }

        if (string.IsNullOrWhiteSpace(request.ColorCode))
        {
            errors.Add("VACUNO-VACUNO-CREATE-COLOR_CODE-NULL");
        }
        else if (request.ColorCode.Length > 30)
        {
            errors.Add("VACUNO-VACUNO-CREATE-COLOR_CODE-INVALID");
        }

        if (string.IsNullOrWhiteSpace(request.SexoCode))
        {
            errors.Add("VACUNO-VACUNO-CREATE-SEXO_CODE-NULL");
        }
        else if (request.SexoCode.Length > 10)
        {
            errors.Add("VACUNO-VACUNO-CREATE-SEXO_CODE-INVALID");
        }

        if (request.GranjaId <= 0)
        {
            errors.Add("VACUNO-VACUNO-CREATE-GRANJA_ID-INVALID");
        }

        if (!string.IsNullOrWhiteSpace(request.Observaciones))
        {
            if (request.Observaciones.Length > 150)
            {
                errors.Add("VACUNO-VACUNO-CREATE-OBSERVACIONES-INVALID");
            }

            if (request.Observaciones
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Length > 30)
            {
                errors.Add("VACUNO-VACUNO-CREATE-OBSERVACIONES-WORD_LIMIT");
            }
        }

        if (request.TipoAdquisicionCode == "COMPRA" && !request.PrecioCompra.HasValue)
        {
            errors.Add("VACUNO-VACUNO-CREATE-PRECIO_COMPRA-NULL");
        }

        return errors;
    }
}
