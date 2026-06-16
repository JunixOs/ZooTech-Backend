namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllTriajes;

public interface IGetAllTriajesInputPort
{
    Task<GetAllTriajesOutput> HandleAsync(GetAllTriajesQuery query, CancellationToken cancellationToken = default);
}
