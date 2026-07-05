namespace ZooTech.Application.Modules.Module_Sanidad.DTOs.Responses;

public class TriajeReporteResponse
{
    public long Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public DateTime FechaHora { get; set; }
    public long VacunoId { get; set; }
    public string? VacunoNombre { get; set; }
    public string TipoPesoCode { get; set; } = string.Empty;
    public decimal PesoKg { get; set; }
    public string? Observaciones { get; set; }
    public string EstadoRegistroCode { get; set; } = string.Empty;
    public long? EncargadoUsuarioId { get; set; }
    public DateTime CreatedAt { get; set; }
}
