namespace ZooTech.InterfaceAdapters.Modules.Module_Celo.DTOs.Responses;

public sealed class VacaEnCeloResponse
{
    public long Id { get; set; }
    public string Codigo { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public int DiasRestante { get; set; }
    public string Estado { get; set; } = null!;
    public int VecesEnCelo { get; set; }
    public int Crias { get; set; }
}
