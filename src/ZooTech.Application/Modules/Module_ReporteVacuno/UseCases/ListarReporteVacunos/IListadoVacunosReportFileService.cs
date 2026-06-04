namespace ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ListarReporteVacunos;

public interface IListadoVacunosReportFileService
{
    Task<ListadoVacunosReportFileResult> GenerateExcelAsync(
        IReadOnlyCollection<VacunoListadoItem> data,
        ReporteVacunoResumen resumen,
        ReporteVacunoFiltros filtros,
        CancellationToken cancellationToken = default);

    Task<ListadoVacunosReportFileResult> GeneratePdfAsync(
        IReadOnlyCollection<VacunoListadoItem> data,
        ReporteVacunoResumen resumen,
        ReporteVacunoFiltros filtros,
        CancellationToken cancellationToken = default);
}

public sealed record ListadoVacunosReportFileResult(
    string FileName,
    string DownloadUrl);
