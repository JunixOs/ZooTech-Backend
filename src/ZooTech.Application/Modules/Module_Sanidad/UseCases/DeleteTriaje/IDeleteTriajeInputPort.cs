using ZooTech.Application.Common.Behaviors;
using ZooTech.Application.Common.Models;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.DeleteTriaje;

public interface IDeleteTriajeInputPort
    : IRequestHandler<DeleteTriajeCommand , EmptyOutput>
{
}
