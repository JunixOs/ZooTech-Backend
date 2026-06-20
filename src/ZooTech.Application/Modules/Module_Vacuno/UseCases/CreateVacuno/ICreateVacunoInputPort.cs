namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.CreateVacuno;

public interface ICreateVacunoInputPort
{
    Task<CreateVacunoOutput> HandleAsync(CreateVacunoCommand command, CancellationToken cancellationToken);
}
