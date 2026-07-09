namespace ZooTech.Application.Modules.Module_Celo.UseCases.GetComparacionCelosRealVsEstandarPorVacuno;

public interface IGetComparacionCelosRealVsEstandarPorVacunoInputPort
{
    Task<GetComparacionCelosRealVsEstandarPorVacunoOutput> HandleAsync(
        GetComparacionCelosRealVsEstandarPorVacunoCommand cmd,
        CancellationToken cancellationToken = default);
}
