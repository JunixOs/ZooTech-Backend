namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.VacasSequia;

public interface IGetVacasSequiaInputPort
{
    Task<GetVacasSequiaOutput> HandleAsync(CancellationToken cancellationToken);
}
