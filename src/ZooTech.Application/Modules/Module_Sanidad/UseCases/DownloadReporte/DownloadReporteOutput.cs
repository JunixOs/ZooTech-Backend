namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.DownloadReporte;


public sealed record DownloadReporteOutput(
    byte[] Content,
    string ContentType,
    string FileName,
    string Message);
