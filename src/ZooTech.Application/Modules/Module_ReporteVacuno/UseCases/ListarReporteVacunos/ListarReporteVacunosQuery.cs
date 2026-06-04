namespace ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ListarReporteVacunos;

public sealed record ListarReporteVacunosQuery(
    string? FechaDesde,
    string? FechaHasta,
    string? Q,
    string? Codigo,
    string? FechaRegistro,
    string? Nombre,
    string? Raza,
    string? Procedencia,
    string? Estado,
    string? AptoPara,
    string? Formato,
    string? Page,
    string? Limit);
