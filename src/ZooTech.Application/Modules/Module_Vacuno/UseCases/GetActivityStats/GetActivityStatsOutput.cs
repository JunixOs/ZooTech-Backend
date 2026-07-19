using System.Collections.Generic;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.GetActivityStats;

public sealed record GetActivityStatsOutput(
    string FechaInicio,
    string FechaFin,
    List<GetActivityPointOutput> Points,
    int Mayor,
    int Menor);

public sealed record GetActivityPointOutput(
    string Fecha,
    int Cantidad);
