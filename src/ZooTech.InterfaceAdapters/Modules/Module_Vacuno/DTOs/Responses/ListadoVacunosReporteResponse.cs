namespace ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Responses;

public sealed record ListadoVacunosReporteResponse(
    IReadOnlyCollection<VacunoListadoItemResponse> Data,
    ReporteVacunoResumenResponse Resumen,
    ReporteVacunoFiltrosResponse Filtros,
    string? DownloadUrl,
    int TotalCount,
    int Page,
    int PageSize);

public sealed record VacunoListadoItemResponse(
    long Id,
    string Codigo,
    DateOnly FechaRegistro,
    string Nombre,
    string? Raza,
    string? Procedencia,
    string? Estado,
    string? EstadoRegistro);

public sealed record ReporteVacunoResumenResponse(int TotalVacunos);

public sealed record ReporteVacunoFiltrosResponse(
    DateOnly? FechaDesde,
    DateOnly? FechaHasta,
    string? Q,
    string? Codigo,
    DateOnly? FechaRegistro,
    string? Nombre,
    string? Raza,
    string? Procedencia,
    string? Estado,
    string? EstadoRegistro,
    string? AptoPara,
    string Formato);

