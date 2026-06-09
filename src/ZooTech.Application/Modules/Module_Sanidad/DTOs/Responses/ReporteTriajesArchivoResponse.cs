namespace ZooTech.Application.Modules.Module_Sanidad.DTOs.Responses;

public class ReporteTriajesArchivoResponse
{
    public byte[] Content { get; set; } = Array.Empty<byte>();
    public string ContentType { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
