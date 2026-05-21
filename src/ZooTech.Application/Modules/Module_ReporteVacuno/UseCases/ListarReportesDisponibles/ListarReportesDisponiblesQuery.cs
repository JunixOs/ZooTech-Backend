namespace ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ListarReportesDisponibles;

public sealed record ListarReportesDisponiblesQuery(
    string? FechaDesde,
    string? FechaHasta,
    string? Q);
