namespace ZooTech.Domain.Module_Sanidad.Entities;

public class Triaje
{
    public long Id { get; set; }
    public string Codigo { get; set; } = null!;
    public DateTime FechaHora { get; set; }
    public long VacunoId { get; set; }
    public string TipoPesoCode { get; set; } = null!;
    public decimal PesoKg { get; set; }
    public string? Observaciones { get; set; }
    public string EstadoRegistroCode { get; set; } = null!;
    public long? EncargadoUsuarioId { get; set; }
    public long? CreatedBy { get; set; }
    public long? UpdatedBy { get; set; }
    public long? DeletedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? MotivoEliminacion { get; set; }
}