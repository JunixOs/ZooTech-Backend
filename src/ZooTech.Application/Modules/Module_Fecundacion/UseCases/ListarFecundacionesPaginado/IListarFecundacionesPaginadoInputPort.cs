using System.Threading;
using System.Threading.Tasks;

namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.ListarFecundacionesPaginado;

public interface IListarFecundacionesPaginadoInputPort
{
    Task<ListarFecundacionesPaginadoOutput> HandleAsync(
        ListarFecundacionesPaginadoCommand command,
        CancellationToken cancellationToken);
}
