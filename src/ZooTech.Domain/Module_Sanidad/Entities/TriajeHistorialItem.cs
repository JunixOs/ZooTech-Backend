namespace ZooTech.Domain.Module_Sanidad.Entities;

public class TriajeHistorialItem
{
    public long Id { get; set; }
    public DateTime FechaHora { get; set; }
    public string TipoPesoCode { get; set; } = null!;
    public decimal PesoKg { get; set; }
}