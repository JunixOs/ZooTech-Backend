namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.DeleteTriaje;

public interface IDeleteTriajeInputPort
{
    Task HandleAsync(long id, CancellationToken cancellationToken = default);
}
