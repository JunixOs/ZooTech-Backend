using ZooTech.Application.Common.Behaviors;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.UpdateTriaje;

public interface IUpdateTriajeInputPort
    : IRequestHandler<UpdateTriajeCommand , UpdateTriajeOutput>
{
}
