namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ListarReportesDisponibles;

public sealed record ReportesDisponiblesResponse(
    IReadOnlyCollection<ReporteDisponibleItem> Data,
    ReportesDisponiblesFiltros Filtros);

public sealed record ReporteDisponibleItem(
    string Tipo,
    string Nombre,
    string Descripcion);

public sealed record ReportesDisponiblesFiltros(
    DateOnly? FechaDesde,
    DateOnly? FechaHasta,
    string? Q);

