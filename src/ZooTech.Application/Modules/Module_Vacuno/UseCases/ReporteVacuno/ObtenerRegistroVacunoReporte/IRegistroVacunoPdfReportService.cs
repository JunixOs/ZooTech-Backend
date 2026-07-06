namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ObtenerRegistroVacunoReporte;

public interface IRegistroVacunoPdfReportService
{
    Task<RegistroVacunoPdfReportResult> GenerateAsync(
        RegistroVacunoDetalle vacuno,
        CancellationToken cancellationToken = default);
}

public sealed record RegistroVacunoPdfReportResult(
    string FileName,
    string DownloadUrl);
