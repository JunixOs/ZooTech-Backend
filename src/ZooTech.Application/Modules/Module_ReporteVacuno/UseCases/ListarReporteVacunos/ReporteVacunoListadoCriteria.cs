namespace ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ListarReporteVacunos;

public sealed record ReporteVacunoListadoCriteria(
    DateOnly FechaDesde,
    DateOnly FechaHasta,
    string? Q,
    string? Raza,
    string? Procedencia,
    string? Estado,
    string? AptoPara,
    int Page,
    int Limit);
