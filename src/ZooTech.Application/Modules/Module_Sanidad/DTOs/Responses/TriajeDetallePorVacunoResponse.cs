namespace ZooTech.Application.Modules.Module_Sanidad.DTOs.Responses;

public class TriajeDetallePorVacunoResponse
{
    public string CodigoRegistro { get; set; } = string.Empty;
    public string Fecha { get; set; } = string.Empty;
    public string Hora { get; set; } = string.Empty;
    public string TipoPesoMedido { get; set; } = string.Empty;
    public decimal PesoKg { get; set; }
    public string? Observaciones { get; set; }
}
