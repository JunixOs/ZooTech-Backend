namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.DeleteOrdenio;

public interface IDeleteOrdenioInputPort
{
    Task HandleAsync(long id, DeleteOrdenioCommand command, CancellationToken cancellationToken);
}
