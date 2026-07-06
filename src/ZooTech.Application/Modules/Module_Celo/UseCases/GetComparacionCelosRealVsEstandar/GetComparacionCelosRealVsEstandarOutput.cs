namespace ZooTech.Application.Modules.Module_Celo.UseCases.GetComparacionCelosRealVsEstandar;

public sealed class GetComparacionCelosRealVsEstandarOutput
{
    public GetComparacionCelosRealVsEstandarOutput(
        List<ComparacionCelosItemDto> items)
    {
        Items = items;
    }

    public List<ComparacionCelosItemDto> Items { get; }
}