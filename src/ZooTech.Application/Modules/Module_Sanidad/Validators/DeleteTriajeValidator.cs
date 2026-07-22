using ZooTech.Application.Common.Validator;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.DeleteTriaje;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Sanidad.Validators;

public class DeleteTriajeValidator : ICommandValidator<DeleteTriajeCommand>
{
    public ModuleName ModuleName => ModuleName.Triaje;

    public List<string> Validate(DeleteTriajeCommand request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.MotivoEliminacion))
        {
            errors.Add("TRIAJE-TRIAJE-DELETE-MOTIVO_ELIMINACION-NULL");
        }
        else if (request.MotivoEliminacion.Length > 500)
        {
            errors.Add("TRIAJE-TRIAJE-DELETE-MOTIVO_ELIMINACION-INVALID");
        }

        return errors;
    }
}
