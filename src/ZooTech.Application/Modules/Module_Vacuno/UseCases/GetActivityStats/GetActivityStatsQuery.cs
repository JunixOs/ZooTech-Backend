using System;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.GetActivityStats;

public sealed record GetActivityStatsQuery(
    DateOnly? FechaInicio,
    DateOnly? FechaFin);
