namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetTriajeById;

public interface IGetTriajeByIdInputPort
{
    Task<GetTriajeByIdOutput> Handle(GetTriajeByIdCommand cmd, CancellationToken cancellationToken = default);
}
