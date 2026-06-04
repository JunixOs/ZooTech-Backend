using Microsoft.AspNetCore.Mvc;

namespace ZooTech.InterfaceAdapters.Modules.Module_ReporteVacuno.DTOs.Requests;

public sealed class ReportesDisponiblesQueryDto
{
    [FromQuery(Name = "fechaDesde")]
    public string? FechaDesde { get; init; }

    [FromQuery(Name = "fechaHasta")]
    public string? FechaHasta { get; init; }

    [FromQuery(Name = "q")]
    public string? Q { get; init; }
}
