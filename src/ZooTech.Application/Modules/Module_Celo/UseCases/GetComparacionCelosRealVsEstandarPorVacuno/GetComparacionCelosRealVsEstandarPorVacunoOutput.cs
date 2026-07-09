namespace ZooTech.Application.Modules.Module_Celo.UseCases.GetComparacionCelosRealVsEstandarPorVacuno;

public sealed class GetComparacionCelosRealVsEstandarPorVacunoOutput
{
    public GetComparacionCelosRealVsEstandarPorVacunoOutput(
        List<ComparacionCelosPorVacunoItemDto> items)
    {
        Items = items;
    }

    public List<ComparacionCelosPorVacunoItemDto> Items { get; }
}
