namespace ZooTech.InterfaceAdapters.Modules.Module_Celo.DTOs.Responses;

public sealed class ReporteCeloPorVacunoResponse
{
    public string CodigoVacuno { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public DateOnly UltimoCelo { get; set; }
    public int VecesEnCelo { get; set; }
}