namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.UpdateOrdenio;

public interface IUpdateOrdenioInputPort
{
    Task<UpdateOrdenioOutput> HandleAsync(long id, UpdateOrdenioCommand command, CancellationToken cancellationToken);
}
