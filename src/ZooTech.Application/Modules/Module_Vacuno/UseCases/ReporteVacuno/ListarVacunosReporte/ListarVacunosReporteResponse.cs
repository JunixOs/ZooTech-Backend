namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ListarVacunosReporte;

public sealed record ListarVacunosReporteResponse(
    IReadOnlyCollection<VacunoListadoReporteItem> Data,
    int TotalCount,
    int Page,
    int PageSize,
    ReporteVacunoListadoResumen Resumen,
    ReporteVacunoListadoFiltros Filtros,
    string? DownloadUrl);

public sealed record VacunoListadoReporteItem(
    long Id,
    string Codigo,
    DateOnly FechaRegistro,
    string Nombre,
    string? Raza,
    string? Procedencia,
    string Estado,
    string EstadoRegistro);

public sealed record ReporteVacunoListadoResumen(int TotalVacunos);

public sealed record ReporteVacunoListadoFiltros(
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
