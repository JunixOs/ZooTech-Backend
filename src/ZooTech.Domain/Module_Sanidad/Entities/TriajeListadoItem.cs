namespace ZooTech.Domain.Module_Sanidad.Entities;

public class TriajeListadoItem
{
    public long Id { get; set; }
    public string Codigo { get; set; } = null!;
    public DateTime FechaHora { get; set; }
    public long VacunoId { get; set; }
    public string VacunoNombre { get; set; } = null!;
    public string TipoPesoCode { get; set; } = null!;
    public decimal PesoKg { get; set; }
    public string? Observaciones { get; set; }
    public string EstadoRegistroCode { get; set; } = null!;
    public long? EncargadoUsuarioId { get; set; }
    public DateTime CreatedAt { get; set; }
}