namespace ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.ReporteVacuno.Responses;

public sealed record ListadoVacunosReporteResponseDto(
    IReadOnlyCollection<VacunoListadoItemDto> Data,
    ReporteVacunoResumenDto Resumen,
    ReporteVacunoFiltrosDto Filtros,
    string? DownloadUrl);

public sealed record VacunoListadoItemDto(
    long Id,
    string Codigo,
    DateOnly FechaRegistro,
    string Nombre,
    string? Raza,
    string? Procedencia,
    string? Estado);

public sealed record ReporteVacunoResumenDto(int TotalVacunos);

public sealed record ReporteVacunoFiltrosDto(
    DateOnly FechaDesde,
    DateOnly FechaHasta,
    string? Q,
    string? Raza,
    string? Procedencia,
    string? Estado,
    string? AptoPara,
    string Formato);

