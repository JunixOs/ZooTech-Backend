using ZooTech.Application.Common.Models;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.DeleteTriaje;

public interface IDeleteTriajeInputPort
{
    Task<EmptyOutput> Handle(DeleteTriajeCommand command, CancellationToken cancellationToken);
}
