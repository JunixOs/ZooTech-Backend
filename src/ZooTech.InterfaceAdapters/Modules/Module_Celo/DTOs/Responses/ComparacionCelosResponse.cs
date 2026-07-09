namespace ZooTech.InterfaceAdapters.Modules.Module_Celo.DTOs.Responses;

public sealed class ComparacionCelosResponse
{
    public DateOnly Fecha { get; set; }

    public int RegistrosReales { get; set; }

    public int RegistrosEstandar { get; set; }
}