namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ListarVacunosReporte;

public sealed record ListadoVacunosReportModel(
    IReadOnlyCollection<VacunoListadoReporteItem> Items,
    ReporteVacunoListadoFiltros Filtros);
