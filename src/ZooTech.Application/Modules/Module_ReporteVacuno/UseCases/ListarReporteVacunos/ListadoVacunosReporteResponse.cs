namespace ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ListarReporteVacunos;

public sealed record ListadoVacunosReporteResponse(
    IReadOnlyCollection<VacunoListadoItem> Data,
    ReporteVacunoResumen Resumen,
    ReporteVacunoFiltros Filtros,
    string? DownloadUrl);

public sealed record VacunoListadoItem(
    long Id,
    string Codigo,
    DateOnly FechaRegistro,
    string Nombre,
    string? Raza,
    string? Procedencia,
    string? Estado);

public sealed record ReporteVacunoResumen(int TotalVacunos);

public sealed record ReporteVacunoFiltros(
    DateOnly FechaDesde,
    DateOnly FechaHasta,
    string? Q,
    string? Raza,
    string? Procedencia,
    string? Estado,
    string? AptoPara,
    string Formato);

public sealed record ReporteVacunoListadoPage(
    IReadOnlyCollection<VacunoListadoItem> Items,
    int TotalRegistros);
