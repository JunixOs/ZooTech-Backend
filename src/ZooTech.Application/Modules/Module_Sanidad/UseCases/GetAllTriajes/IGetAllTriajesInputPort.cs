using ZooTech.Application.Common.Behaviors;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllTriajes;

public interface IGetAllTriajesInputPort
    : IRequestHandler<GetAllTriajesQuery , GetAllTriajesOutput>
{
}
