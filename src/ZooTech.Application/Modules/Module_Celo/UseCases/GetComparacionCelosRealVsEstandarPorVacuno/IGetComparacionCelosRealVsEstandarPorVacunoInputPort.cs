namespace ZooTech.Application.Modules.Module_Celo.UseCases.GetComparacionCelosRealVsEstandarPorVacuno;

public interface IGetComparacionCelosRealVsEstandarPorVacunoInputPort
{
    Task<GetComparacionCelosRealVsEstandarPorVacunoOutput> HandleAsync(
        string codigoVacuno,
        DateTime? fechaInicio,
        DateTime? fechaFin,
        CancellationToken cancellationToken = default);
}
