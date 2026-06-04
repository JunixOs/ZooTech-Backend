namespace ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ObtenerRegistroVacunoReporte;

public sealed record ObtenerRegistroVacunoReporteQuery(
    long VacunoId,
    string? Formato);
