namespace ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ListarReporteVacunos;

public sealed record ListarReporteVacunosQuery(
    string? FechaDesde,
    string? FechaHasta,
    string? Q,
    string? Raza,
    string? Procedencia,
    string? Estado,
    string? AptoPara,
    string? Formato,
    string? Page,
    string? Limit);
