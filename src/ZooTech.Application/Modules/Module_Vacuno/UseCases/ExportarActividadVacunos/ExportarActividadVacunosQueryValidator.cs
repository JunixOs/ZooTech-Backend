using ZooTech.Application.Common.Validator;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ExportarActividadVacunos;

public sealed class ExportarActividadVacunosQueryValidator
    : ICommandQueryValidator<ExportarActividadVacunosQuery>
{
    public ModuleName ModuleName => ModuleName.Vacuno;

    public List<string> Validate(ExportarActividadVacunosQuery request)
    {
        var errors = new List<string>();

        if (request.Formato?.Trim().ToLowerInvariant() is not ("excel" or "pdf"))
        {
            errors.Add("El formato debe ser excel o pdf.");
        }

        if (request.FechaInicio.HasValue &&
            request.FechaFin.HasValue &&
            request.FechaInicio.Value > request.FechaFin.Value)
        {
            errors.Add("La fecha de inicio no puede ser posterior a la fecha de fin.");
        }

        return errors;
    }
}
