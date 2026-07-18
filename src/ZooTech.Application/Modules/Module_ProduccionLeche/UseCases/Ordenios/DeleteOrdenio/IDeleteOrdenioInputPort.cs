using ZooTech.Application.Common.Models;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.DeleteOrdenio;

public interface IDeleteOrdenioInputPort
{
    Task<EmptyOutput> Handle(DeleteOrdenioCommand command, CancellationToken cancellationToken);
}
