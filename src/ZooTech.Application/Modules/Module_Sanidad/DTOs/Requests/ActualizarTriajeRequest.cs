public class ActualizarTriajeRequest
{
    public string TipoPesoCode { get; set; } = string.Empty;
    public decimal PesoKg { get; set; }
    public string? Observaciones { get; set; }
}
