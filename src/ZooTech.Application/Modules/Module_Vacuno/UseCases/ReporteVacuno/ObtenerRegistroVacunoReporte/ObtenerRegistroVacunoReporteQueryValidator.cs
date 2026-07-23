using ZooTech.Application.Common.Validator;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ObtenerRegistroVacunoReporte;

public sealed class ObtenerRegistroVacunoReporteQueryValidator : ICommandValidator<ObtenerRegistroVacunoReporteQuery>
{
    public ModuleName ModuleName => ModuleName.Vacuno;

    public List<string> Validate(ObtenerRegistroVacunoReporteQuery request)
    {
        var errors = new List<string>();
        if (request.VacunoId <= 0)
        {
            errors.Add("El ID del vacuno debe ser mayor que cero.");
        }

        var formato = string.IsNullOrWhiteSpace(request.Formato)
            ? "json"
            : request.Formato.Trim().ToLowerInvariant();

        if (formato is not ("json" or "excel" or "pdf"))
        {
            errors.Add("El formato debe ser json, excel o pdf.");
        }

        return errors;
    }
}
