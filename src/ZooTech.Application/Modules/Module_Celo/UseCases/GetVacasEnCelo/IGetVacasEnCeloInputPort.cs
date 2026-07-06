namespace ZooTech.Application.Modules.Module_Celo.UseCases.GetVacasEnCelo;

public interface IGetVacasEnCeloInputPort
{
    Task<GetVacasEnCeloOutput> HandleAsync(
        DateTime? fechaInicio,
        DateTime? fechaFin,
        CancellationToken cancellationToken = default);
}
