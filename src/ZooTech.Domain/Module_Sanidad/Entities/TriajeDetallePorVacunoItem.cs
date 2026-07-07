namespace ZooTech.Domain.Module_Sanidad.Entities;

public class TriajeDetallePorVacunoItem
{
    public string CodigoRegistro { get; set; } = string.Empty;
    public DateTime FechaHora { get; set; }
    public string TipoPesoMedido { get; set; } = string.Empty;
    public decimal PesoKg { get; set; }
    public string? Observaciones { get; set; }
}
