using ZooTech.Application.Common.Validator;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.DeleteVacuno;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Vacuno.Validators;

internal sealed class DeleteVacunoValidator : ICommandQueryValidator<DeleteVacunoCommand>
{
    public ModuleName ModuleName => ModuleName.Vacuno;

    public List<string> Validate(DeleteVacunoCommand request)
    {
        var errors = new List<string>();

        if (request.Id <= 0)
        {
            errors.Add("VACUNO-VACUNO-DELETE-ID-INVALID");
        }

        if (string.IsNullOrWhiteSpace(request.MotivoEliminacion))
        {
            errors.Add("VACUNO-VACUNO-DELETE-MOTIVO_ELIMINACION-NULL");
        }
        else if (request.MotivoEliminacion.Length > 200)
        {
            errors.Add("VACUNO-VACUNO-DELETE-MOTIVO_ELIMINACION-INVALID");
        }

        return errors;
    }
}
