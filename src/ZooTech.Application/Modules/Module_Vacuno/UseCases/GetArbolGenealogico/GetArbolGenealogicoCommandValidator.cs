using ZooTech.Application.Common.Validator;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.GetArbolGenealogico;

public sealed class GetArbolGenealogicoCommandValidator : ICommandValidator<GetArbolGenealogicoCommand>
{
    public ModuleName ModuleName => ModuleName.Vacuno;

    public List<string> Validate(GetArbolGenealogicoCommand request)
    {
        var errors = new List<string>();

        if (request.Id <= 0)
        {
            errors.Add("VACUNO-VACUNO-GET_ARBOL_GENEALOGICO-ID-INVALID");
        }

        if (request.Niveles < 1)
        {
            errors.Add("VACUNO-VACUNO-GET_ARBOL_GENEALOGICO-NIVELES-INVALID");
        }

        return errors;
    }
}
