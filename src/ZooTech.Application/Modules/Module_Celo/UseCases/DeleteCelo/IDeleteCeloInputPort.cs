using ZooTech.Application.Common.Models;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.DeleteCelo;

public interface IDeleteCeloInputPort
{
    Task<EmptyOutput> Handle(DeleteCeloCommand command, CancellationToken cancellationToken = default);
}
