using ZooTech.Application.Common.Models;

namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.DeleteFecundacion;

public interface IDeleteFecundacionInputPort
{
    Task<EmptyOutput> HandleAsync(DeleteFecundacionCommand command, CancellationToken cancellationToken);
}
