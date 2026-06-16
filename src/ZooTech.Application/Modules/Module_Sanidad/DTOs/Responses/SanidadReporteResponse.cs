namespace ZooTech.Application.Modules.Module_Sanidad.DTOs.Responses;

public class ResumenSanidadResponse
{
    public int TotalTriajes { get; set; }
    public decimal PesoPromedioKg { get; set; }
    public decimal PesoMinimoKg { get; set; }
    public decimal PesoMaximoKg { get; set; }
    public int TotalVacunosEvaluados { get; set; }
    public List<TipoPesoCountItem> DistribucionTipoPeso { get; set; } = new();
}

public class TipoPesoCountItem
{
    public string Code { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int Cantidad { get; set; }
}

public class PesoPromedioItem
{
    public DateTime Fecha { get; set; }
    public decimal PesoPromedio { get; set; }
    public int CantidadRegistros { get; set; }
}
