using Microsoft.AspNetCore.Mvc;

namespace ZooTech.InterfaceAdapters.Modules.Module_ReporteVacuno.DTOs.Requests;

public sealed class ListadoVacunosReporteQueryDto
{
    [FromQuery(Name = "fechaDesde")]
    public string? FechaDesde { get; init; }

    [FromQuery(Name = "fechaHasta")]
    public string? FechaHasta { get; init; }

    [FromQuery(Name = "q")]
    public string? Q { get; init; }

    [FromQuery(Name = "raza")]
    public string? Raza { get; init; }

    [FromQuery(Name = "procedencia")]
    public string? Procedencia { get; init; }

    [FromQuery(Name = "estado")]
    public string? Estado { get; init; }

    [FromQuery(Name = "aptoPara")]
    public string? AptoPara { get; init; }

    [FromQuery(Name = "formato")]
    public string? Formato { get; init; }

    [FromQuery(Name = "page")]
    public string? Page { get; init; }

    [FromQuery(Name = "limit")]
    public string? Limit { get; init; }
}
