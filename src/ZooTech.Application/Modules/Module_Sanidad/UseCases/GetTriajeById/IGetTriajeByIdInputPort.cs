namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetTriajeById;

public interface IGetTriajeByIdInputPort
{
    Task<GetTriajeByIdOutput> HandleAsync(long id, CancellationToken cancellationToken = default);
}
