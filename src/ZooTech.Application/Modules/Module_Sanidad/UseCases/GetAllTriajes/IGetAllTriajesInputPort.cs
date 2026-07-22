namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllTriajes;

public interface IGetAllTriajesInputPort
{
    Task<GetAllTriajesOutput> Handle(GetAllTriajesQuery query, CancellationToken cancellationToken = default);
}
