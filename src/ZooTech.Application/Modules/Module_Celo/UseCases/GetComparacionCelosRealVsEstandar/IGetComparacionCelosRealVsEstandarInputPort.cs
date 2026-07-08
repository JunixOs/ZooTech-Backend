namespace ZooTech.Application.Modules.Module_Celo.UseCases.GetComparacionCelosRealVsEstandar;

public interface IGetComparacionCelosRealVsEstandarInputPort
{
    Task<GetComparacionCelosRealVsEstandarOutput> HandleAsync(
        GetComparacionCelosRealVsEstandarCommand cmd,
        CancellationToken cancellationToken = default);
}