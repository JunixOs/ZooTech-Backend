using ZooTech.Application.Common.Behaviors;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.CreateTriaje;

public interface ICreateTriajeInputPort
    : IRequestHandler<CreateTriajeCommand , CreateTriajeOutput>
{
}
