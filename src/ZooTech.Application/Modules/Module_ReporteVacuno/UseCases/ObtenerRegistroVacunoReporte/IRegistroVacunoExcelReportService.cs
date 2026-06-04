namespace ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ObtenerRegistroVacunoReporte;

public interface IRegistroVacunoExcelReportService
{
    Task<RegistroVacunoExcelReportResult> GenerateAsync(
        RegistroVacunoDetalle vacuno,
        CancellationToken cancellationToken = default);
}

public sealed record RegistroVacunoExcelReportResult(
    string FileName,
    string DownloadUrl);
