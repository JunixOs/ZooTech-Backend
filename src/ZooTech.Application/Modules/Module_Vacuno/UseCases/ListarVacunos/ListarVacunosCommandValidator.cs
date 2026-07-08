using ZooTech.Application.Common.Validator;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunos;

public sealed class ListarVacunosCommandValidator : ICommandValidator<ListarVacunosCommand>
{
    public ModuleName ModuleName => ModuleName.Vacuno;

    public List<string> Validate(ListarVacunosCommand request)
    {
        var errors = new List<string>();

        if (request.Page < 1)
        {
            errors.Add("VACUNO-VACUNO-LISTAR-PAGE-INVALID");
        }

        if (request.Limit < 1 || request.Limit > 100)
        {
            errors.Add("VACUNO-VACUNO-LISTAR-LIMIT-INVALID");
        }

        return errors;
    }
}
