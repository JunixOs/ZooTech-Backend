namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.DownloadReporte;


public interface IDownloadReporteInputPort
{
    Task<DownloadReporteOutput> HandleAsync(DownloadReporteCommand command, CancellationToken cancellationToken = default);
}
