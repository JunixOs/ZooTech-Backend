namespace ZooTech.Application.Modules.Module_Celo.UseCases.GetComparacionCelosRealVsEstandar;

public sealed class ComparacionCelosItemDto
{
    public DateOnly Fecha { get; set; }

    public int RegistrosReales { get; set; }

    public int RegistrosEstandar { get; set; }
}