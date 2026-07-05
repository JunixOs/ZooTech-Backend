namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.DownloadReporte;


public sealed record DownloadReporteCommand(
    string Formato,
    string? Fecha = null,
    string? Codigo = null,
    string? Nombre = null,
    string? TipoPeso = null,
    decimal? PesoKg = null);
