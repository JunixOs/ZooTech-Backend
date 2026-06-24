namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.UpdateTriaje;

public interface IUpdateTriajeInputPort
{
    Task HandleAsync(long id, UpdateTriajeCommand command, CancellationToken cancellationToken = default);
}
