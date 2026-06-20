namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.CreateTriaje;

public interface ICreateTriajeInputPort
{
    Task<CreateTriajeOutput> HandleAsync(CreateTriajeCommand command, CancellationToken cancellationToken = default);
}
