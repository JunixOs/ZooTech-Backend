namespace ZooTech.Application.Modules.Module_Sanidad.DTOs.Responses;

public class TriajeHistorialResponse
{
    public long Id { get; set; }
    public DateTime FechaHora { get; set; }
    public string TipoPesoCode { get; set; } = string.Empty;
    public decimal PesoKg { get; set; }
}