namespace ZooTech.InterfaceAdapters.Modules.Module_Celo.DTOs.Responses;

public sealed class ComparacionCelosPorVacunoResponse
{
    public DateOnly Periodo { get; set; }

    public int RegistrosReales { get; set; }

    public int RegistrosEstandar { get; set; }
}
