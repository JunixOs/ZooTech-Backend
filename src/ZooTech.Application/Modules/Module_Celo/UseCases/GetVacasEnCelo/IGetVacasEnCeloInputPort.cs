namespace ZooTech.Application.Modules.Module_Celo.UseCases.GetVacasEnCelo;

public interface IGetVacasEnCeloInputPort
{
    Task<GetVacasEnCeloOutput> HandleAsync(
        GetVacasEnCeloCommand cmd,
        CancellationToken cancellationToken = default);
}
