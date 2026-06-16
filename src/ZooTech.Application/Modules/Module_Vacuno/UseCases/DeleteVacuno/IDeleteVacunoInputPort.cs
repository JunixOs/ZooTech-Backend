namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.DeleteVacuno;

public interface IDeleteVacunoInputPort
{
    Task HandleAsync(long id, DeleteVacunoCommand command, CancellationToken cancellationToken);
}
