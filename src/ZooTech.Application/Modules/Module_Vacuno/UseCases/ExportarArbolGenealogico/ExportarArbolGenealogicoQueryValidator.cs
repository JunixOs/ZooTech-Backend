using ZooTech.Application.Common.Validator;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ExportarArbolGenealogico;

public sealed class ExportarArbolGenealogicoQueryValidator : ICommandQueryValidator<ExportarArbolGenealogicoQuery>
{
    public ModuleName ModuleName => ModuleName.Vacuno;

    public List<string> Validate(ExportarArbolGenealogicoQuery request)
    {
        var errors = new List<string>();

        if (request.VacunoId <= 0)
        {
            errors.Add("VACUNO-VACUNO-EXPORTAR_ARBOL_GENEALOGICO-VACUNO_ID-INVALID");
        }

        if (request.Niveles < 1)
        {
            errors.Add("VACUNO-VACUNO-EXPORTAR_ARBOL_GENEALOGICO-NIVELES-INVALID");
        }

        if (request.Formato?.ToLowerInvariant() is not "excel" and not "pdf")
        {
            errors.Add("VACUNO-VACUNO-EXPORTAR_ARBOL_GENEALOGICO-FORMATO-INVALID");
        }

        return errors;
    }
}
