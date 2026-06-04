namespace ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ObtenerRegistroVacunoReporte;

public interface IRegistroVacunoPdfReportService
{
    Task<RegistroVacunoPdfReportResult> GenerateAsync(
        RegistroVacunoDetalle vacuno,
        CancellationToken cancellationToken = default);
}

public sealed record RegistroVacunoPdfReportResult(
    string FileName,
    string DownloadUrl);
