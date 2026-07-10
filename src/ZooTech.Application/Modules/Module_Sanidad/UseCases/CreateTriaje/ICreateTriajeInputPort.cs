namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.CreateTriaje;

public interface ICreateTriajeInputPort
{
    Task<CreateTriajeOutput> Handle(CreateTriajeCommand command, CancellationToken cancellationToken = default);
}
