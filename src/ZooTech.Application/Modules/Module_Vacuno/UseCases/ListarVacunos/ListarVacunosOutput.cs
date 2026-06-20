namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunos;

public sealed record ListarVacunosOutput(
    IReadOnlyCollection<VacunoListadoItem> Data,
    ListarVacunosResumen Resumen,
    ListarVacunosFiltros Filtros,
    string? DownloadUrl);

public sealed record VacunoListadoItem(
    long Id,
    string Codigo,
    DateOnly FechaRegistro,
    string Nombre,
    DateOnly FechaNacimiento,
    string RazaCode,
    string SexoCode,
    string? Raza,
    string? Procedencia,
    string? Estado);

public sealed record ListarVacunosResumen(int TotalVacunos);

public sealed record ListarVacunosFiltros(
    DateOnly? FechaDesde,
    DateOnly? FechaHasta,
    string? Q,
    string? Raza,
    string? Procedencia,
    string? Estado,
    string? AptoPara,
    string Formato);

public sealed record ListarVacunosPage(
    IReadOnlyCollection<VacunoListadoItem> Items,
    int TotalRegistros);
