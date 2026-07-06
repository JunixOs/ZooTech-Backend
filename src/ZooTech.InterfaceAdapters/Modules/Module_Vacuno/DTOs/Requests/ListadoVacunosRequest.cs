using Microsoft.AspNetCore.Mvc;

namespace ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Requests;

public sealed class ListadoVacunosRequest
{
    [FromQuery(Name = "fechaDesde")]
    public string? FechaDesde { get; init; }

    [FromQuery(Name = "fechaHasta")]
    public string? FechaHasta { get; init; }

    [FromQuery(Name = "search")]
    public string? Search { get; init; }

    [FromQuery(Name = "q")]
    public string? Q { get; init; }

    [FromQuery(Name = "codigo")]
    public string? Codigo { get; init; }

    [FromQuery(Name = "fechaRegistro")]
    public string? FechaRegistro { get; init; }

    [FromQuery(Name = "nombre")]
    public string? Nombre { get; init; }

    [FromQuery(Name = "raza")]
    public string? Raza { get; init; }

    [FromQuery(Name = "procedencia")]
    public string? Procedencia { get; init; }

    [FromQuery(Name = "estado")]
    public string? Estado { get; init; }

    [FromQuery(Name = "estadoRegistro")]
    public string? EstadoRegistro { get; init; }

    [FromQuery(Name = "aptoPara")]
    public string? AptoPara { get; init; }

    [FromQuery(Name = "formato")]
    public string? Formato { get; init; }

    [FromQuery(Name = "page")]
    public string? Page { get; init; }

    [FromQuery(Name = "pageSize")]
    public string? PageSize { get; init; }

    [FromQuery(Name = "limit")]
    public string? Limit { get; init; }
}

