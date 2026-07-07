using System.Collections.Generic;

namespace ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Responses;

public sealed record VacunoActivityStatsResponse(
    string FechaInicio,
    string FechaFin,
    List<VacunoActivityPointResponse> Points,
    int Mayor,
    int Menor);
