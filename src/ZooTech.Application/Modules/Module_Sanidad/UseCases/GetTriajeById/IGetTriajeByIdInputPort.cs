using ZooTech.Application.Common.Behaviors;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetTriajeById;

public interface IGetTriajeByIdInputPort
    : IRequestHandler<GetTriajeByIdQuery , GetTriajeByIdOutput>
{
}
