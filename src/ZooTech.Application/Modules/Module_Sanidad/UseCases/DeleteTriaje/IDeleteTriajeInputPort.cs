namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.DeleteTriaje;

public interface IDeleteTriajeInputPort
{
    Task HandleAsync(long id, DeleteTriajeCommand command, CancellationToken cancellationToken);
}
