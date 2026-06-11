namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunos;

public interface IListadoVacunosReportFileService
{
    Task<ListadoVacunosReportFileResult> GenerateExcelAsync(
        IReadOnlyCollection<VacunoListadoItem> data,
        ListarVacunosResumen resumen,
        ListarVacunosFiltros filtros,
        CancellationToken cancellationToken = default);

    Task<ListadoVacunosReportFileResult> GeneratePdfAsync(
        IReadOnlyCollection<VacunoListadoItem> data,
        ListarVacunosResumen resumen,
        ListarVacunosFiltros filtros,
        CancellationToken cancellationToken = default);
}

public sealed record ListadoVacunosReportFileResult(
    string FileName,
    string DownloadUrl);

