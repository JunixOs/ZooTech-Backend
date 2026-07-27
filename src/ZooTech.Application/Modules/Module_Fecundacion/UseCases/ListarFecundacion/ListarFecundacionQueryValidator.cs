using ZooTech.Application.Common.Validator;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.ListarFecundacion;

public sealed class ListarFecundacionQueryValidator : ICommandQueryValidator<ListarFecundacionQuery>
{

    public ModuleName ModuleName => ModuleName.Fecundacion;

    public List<string> Validate(ListarFecundacionQuery request)
    {
        var errors = new List<string>();

        if (request.Page < 1)
        {
            errors.Add("FECUNDACION-FECUNDACION-LISTAR-PAGE-INVALID");
        }

        if (request.Limit < 1 || request.Limit > 100)
        {
            errors.Add("FECUNDACION-FECUNDACION-LISTAR-LIMIT-INVALID");
        }

        return errors;
        }
}
