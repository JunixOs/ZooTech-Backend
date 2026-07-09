using ZooTech.Application.Common.Models;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.DeleteVacuno;

public interface IDeleteVacunoInputPort
{
    Task<EmptyOutput> HandleAsync(DeleteVacunoCommand command, CancellationToken cancellationToken);
}
