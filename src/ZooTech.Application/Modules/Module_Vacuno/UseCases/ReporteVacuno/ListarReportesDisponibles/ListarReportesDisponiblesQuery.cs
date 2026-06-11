namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ListarReportesDisponibles;

public sealed record ListarReportesDisponiblesQuery(
    string? FechaDesde,
    string? FechaHasta,
    string? Q);

