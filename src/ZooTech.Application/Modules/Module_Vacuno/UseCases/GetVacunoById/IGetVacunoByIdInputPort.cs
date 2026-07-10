namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.GetVacunoById;

public interface IGetVacunoByIdInputPort
{
    Task<GetVacunoByIdOutput> HandleAsync(GetVacunoByIdCommand cmd, CancellationToken cancellationToken);
}
