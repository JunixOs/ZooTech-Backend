namespace ZooTech.Application.Modules.Module_Celo.UseCases.GetComparacionCelosRealVsEstandarPorVacuno;

public sealed class ComparacionCelosPorVacunoItemDto
{
    public DateOnly Periodo { get; set; }

    public int RegistrosReales { get; set; }

    public int RegistrosEstandar { get; set; }
}
