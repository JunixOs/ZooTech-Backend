namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.UpdateTriaje;

public interface IUpdateTriajeInputPort
{
    Task<UpdateTriajeOutput> Handle(UpdateTriajeCommand command, CancellationToken cancellationToken = default);
}
