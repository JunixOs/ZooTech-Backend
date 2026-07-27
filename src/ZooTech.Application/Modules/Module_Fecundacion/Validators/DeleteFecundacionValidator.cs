using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Validator;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.DeleteFecundacion;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Fecundacion.Validators;

public sealed class DeleteFecundacionValidator :
    ICommandQueryValidator<DeleteFecundacionCommand>,
    IValidationErrorDetailsProvider
{
    private const string InvalidIdCode = "FECUNDACION-DELETE-ID-INVALID";
    private const string RequiredReasonCode = "FECUNDACION-DELETE-RAZON-REQUIRED";

    public ModuleName ModuleName => ModuleName.Fecundacion;

    public List<string> Validate(DeleteFecundacionCommand request)
    {
        var errors = new List<string>();

        if (request.Id <= 0)
        {
            errors.Add(InvalidIdCode);
        }

        if (string.IsNullOrWhiteSpace(request.Razon))
        {
            errors.Add(RequiredReasonCode);
        }

        return errors;
    }

    public IReadOnlyList<FieldValidationError> GetFieldErrors(
        IReadOnlyCollection<string> errorCodes)
        => errorCodes.Select(ToFieldError).ToList();

    private static FieldValidationError ToFieldError(string code)
        => code switch
        {
            InvalidIdCode => new(
                "id",
                code,
                "El identificador de la fecundacion no es valido."),
            RequiredReasonCode => new(
                "razon",
                code,
                "Debe indicar la razon de eliminacion."),
            _ => new("formulario", code, "El dato ingresado no es valido.")
        };
}
