namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.GetVacunoById;

public interface IGetVacunoByIdInputPort
{
    Task<GetVacunoByIdOutput> HandleAsync(long id, CancellationToken cancellationToken);
}
