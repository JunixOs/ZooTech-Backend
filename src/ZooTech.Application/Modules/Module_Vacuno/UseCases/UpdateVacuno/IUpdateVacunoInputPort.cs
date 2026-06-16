namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.UpdateVacuno;

public interface IUpdateVacunoInputPort
{
    Task<UpdateVacunoOutput> HandleAsync(long id, UpdateVacunoCommand command, CancellationToken cancellationToken);
}
