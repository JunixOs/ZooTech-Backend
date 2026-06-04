namespace ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ListarReporteVacunos;

public sealed record ReporteVacunoListadoCriteria(
    DateOnly FechaDesde,
    DateOnly FechaHasta,
    string? Q,
    string? Codigo,
    string? FechaRegistro,
    string? Nombre,
    string? Raza,
    string? Procedencia,
    string? Estado,
    string? AptoPara,
    int Page,
    int Limit);
