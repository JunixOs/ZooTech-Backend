namespace ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.ReporteVacuno.Responses;

public sealed record ReportesDisponiblesResponseDto(
    IReadOnlyCollection<ReporteDisponibleDto> Data,
    ReportesDisponiblesFiltrosDto Filtros);

public sealed record ReporteDisponibleDto(
    string Tipo,
    string Nombre,
    string Descripcion);

public sealed record ReportesDisponiblesFiltrosDto(
    DateOnly FechaDesde,
    DateOnly FechaHasta,
    string? Q);

