using System.Collections.Generic;
using ZooTech.Application.Common.Validator;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.GetActivityStats;

public sealed class GetActivityStatsQueryValidator : ICommandQueryValidator<GetActivityStatsQuery>
{
    public ModuleName ModuleName => ModuleName.Vacuno;

    public List<string> Validate(GetActivityStatsQuery request)
    {
        var errors = new List<string>();

        if (request.FechaInicio.HasValue && request.FechaFin.HasValue && request.FechaInicio.Value > request.FechaFin.Value)
        {
            errors.Add("La fecha de inicio no puede ser posterior a la fecha de fin.");
        }

        return errors;
    }
}
