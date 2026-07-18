namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ListarVacunosReporte;

public sealed record ListarVacunosReporteResponse(
    IReadOnlyCollection<VacunoListadoReporteItem> Data,
    ReporteVacunoListadoResumen Resumen,
    ReporteVacunoListadoFiltros Filtros,
    string? DownloadUrl,
    int TotalCount,
    int Page,
    int PageSize);

public sealed record VacunoListadoReporteItem(
    long Id,
    string Codigo,
    DateOnly FechaNacimiento,
    DateOnly FechaRegistro,
    string Nombre,
    string? TipoAdquisicion,
    string? Raza,
    string? Color,
    string? Sexo,
    string? Granja,
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
