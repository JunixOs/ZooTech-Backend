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

        VacunoCommonValidationRules.ValidateCommonFields(
            errors,
            "CREATE",
            request.Nombre,
            request.FechaNacimiento,
            request.TipoAdquisicionCode,
            request.RazaCode,
            request.ColorCode,
            request.SexoCode,
            request.GranjaId,
            request.Observaciones,
            request.PrecioCompra);

        return errors;
    }
}
