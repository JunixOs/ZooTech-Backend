namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ListarVacunosReporte;

public sealed record ListarVacunosReporteQuery(
    string? FechaDesde,
    string? FechaHasta,
    string? Q,
    string? Codigo,
    string? FechaRegistro,
    string? Nombre,
    string? Raza,
    string? Procedencia,
    string? Estado,
    string? EstadoRegistro,
    string? AptoPara,
    string? Formato,
    string? Page,
    string? Limit);
