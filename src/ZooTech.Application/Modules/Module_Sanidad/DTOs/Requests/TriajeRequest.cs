namespace ZooTech.InterfaceAdapters.Modules.Module_Sanidad.DTOs.Requests;

public class TriajeRequest
{
    public DateTime FechaHora { get; set; } = DateTime.Now;
    public long VacunoId { get; set; }
    public string TipoPesoCode { get; set; } = string.Empty;
    public decimal PesoKg { get; set; }
    public string? Observaciones { get; set; }
    public string EstadoRegistroCode { get; set; } = string.Empty;
    public long? EncargadoUsuarioId { get; set; }
}