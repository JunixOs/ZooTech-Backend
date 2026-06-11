namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ObtenerRegistroVacunoReporte;

public sealed record ObtenerRegistroVacunoReporteQuery(
    long VacunoId,
    string? Formato);

