using System.Threading;
using System.Threading.Tasks;

namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.DeleteFecundacion;

public interface IDeleteFecundacionInputPort
{
    Task HandleAsync(long id, DeleteFecundacionCommand command, CancellationToken cancellationToken);
}
