namespace ZooTech.Application.Modules.Module_Celo.UseCases.GetReporteCelos;

public sealed class CeloReporteItemDto
{
    public string CodigoRegistro { get; set; } = null!;
    public DateOnly Fecha { get; set; }
    public TimeOnly Hora { get; set; }
    public string CodigoVacuno { get; set; } = null!;
    public string NombreVacuno { get; set; } = null!;
    public int VecesEnCelo { get; set; }
    public int Caracteristicas { get; set; }
    public List<string> ListaCaracteristicas { get; set; } = new();
    public string Observaciones { get; set; } = string.Empty;
    public int Crias { get; set; }
}
