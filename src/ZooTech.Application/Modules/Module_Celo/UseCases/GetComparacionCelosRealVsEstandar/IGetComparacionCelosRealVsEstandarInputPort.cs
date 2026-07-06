namespace ZooTech.Application.Modules.Module_Celo.UseCases.GetComparacionCelosRealVsEstandar;

public interface IGetComparacionCelosRealVsEstandarInputPort
{
    Task<GetComparacionCelosRealVsEstandarOutput> HandleAsync(
        DateTime? fechaInicio,
        DateTime? fechaFin,
        CancellationToken cancellationToken = default);
}