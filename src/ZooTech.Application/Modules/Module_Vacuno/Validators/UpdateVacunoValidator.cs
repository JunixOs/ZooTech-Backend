using ZooTech.Application.Common.Validator;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.UpdateVacuno;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Vacuno.Validators;

internal sealed class UpdateVacunoValidator :
    ICommandQueryValidator<UpdateVacunoCommand>,
    IValidationErrorDetailsProvider
{
    public ModuleName ModuleName => ModuleName.Vacuno;

    public List<string> Validate(UpdateVacunoCommand request)
    {
        var errors = new List<string>();

        if (request.Id <= 0)
        {
            errors.Add("VACUNO-VACUNO-UPDATE-ID-INVALID");
        }

        VacunoCommonValidationRules.ValidateCommonFields(
            errors,
            "UPDATE",
            request.Nombre,
            request.FechaNacimiento,
            request.TipoAdquisicionCode,
            request.RazaCode,
            request.ColorCode,
            request.SexoCode,
            request.GranjaId,
            request.Granja,
            request.CodigoDistrito,
            request.Observaciones,
            request.PrecioCompra);

        return errors;
    }

    public IReadOnlyList<Application.Common.Exceptions.FieldValidationError> GetFieldErrors(
        IReadOnlyCollection<string> errorCodes)
        => VacunoValidationErrorDetails.FromCodes(errorCodes);
}
