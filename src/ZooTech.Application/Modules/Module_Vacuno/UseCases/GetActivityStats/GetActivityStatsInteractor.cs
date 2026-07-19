using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Interfaces;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.GetActivityStats;

public sealed class GetActivityStatsInteractor : IGetActivityStatsInputPort
{
    private readonly IVacunoResponseReadRepository _vacunoReadRepository;

    public GetActivityStatsInteractor(IVacunoResponseReadRepository vacunoReadRepository)
    {
        _vacunoReadRepository = vacunoReadRepository;
    }

    public async Task<GetActivityStatsOutput> HandleAsync(GetActivityStatsQuery command, CancellationToken cancellationToken = default)
    {
        var end = command.FechaFin ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var start = command.FechaInicio ?? end.AddDays(-30);



        var aggregates = await _vacunoReadRepository.GetActivityAggregatesAsync(start, end, cancellationToken);
        var points = new List<GetActivityPointOutput>();
        int activosActuales = aggregates.InventarioInicial;

        for (var date = start; date <= end; date = date.AddDays(1))
        {
            var altasHoy = aggregates.AltasPorDia.GetValueOrDefault(date, 0);
            var bajasHoy = aggregates.BajasPorDia.GetValueOrDefault(date, 0);
            
            activosActuales = activosActuales + altasHoy - bajasHoy;

            points.Add(new GetActivityPointOutput(date.ToString("yyyy-MM-dd"), activosActuales));
        }

        var mayor = points.Any() ? points.Max(p => p.Cantidad) : 0;
        var menor = points.Any() ? points.Min(p => p.Cantidad) : 0;

        return new GetActivityStatsOutput(
            start.ToString("yyyy-MM-dd"),
            end.ToString("yyyy-MM-dd"),
            points,
            mayor,
            menor
        );
    }
}
