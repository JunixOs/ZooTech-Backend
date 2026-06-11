namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ObtenerRegistroVacunoReporte;

public interface IRegistroVacunoExcelReportService
{
    Task<RegistroVacunoExcelReportResult> GenerateAsync(
        RegistroVacunoDetalle vacuno,
        CancellationToken cancellationToken = default);
}

public sealed record RegistroVacunoExcelReportResult(
    string FileName,
    string DownloadUrl);

