using ZooTech.Application.Common.Validator;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.DeleteOrdenio;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.Validators;

public class DeleteOrdenioValidator : ICommandValidator<DeleteOrdenioCommand>
{
    public ModuleName ModuleName => ModuleName.Produccion_Leche;

    public List<string> Validate(DeleteOrdenioCommand request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.MotivoEliminacion))
        {
            errors.Add("PRODUCCION_LECHE-ORDENIO-DELETE-MOTIVO_ELIMINACION-NULL");
        }
        else if (request.MotivoEliminacion.Length > 500)
        {
            errors.Add("PRODUCCION_LECHE-ORDENIO-DELETE-MOTIVO_ELIMINACION-INVALID");
        }

        return errors;
    }
}
