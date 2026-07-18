namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetHistorialByVacunoId;

public interface IGetHistorialByVacunoIdInputPort
{
    Task<GetHistorialByVacunoIdOutput> Handle(GetHistorialByVacunoIdCommand cmd, CancellationToken cancellationToken = default);
}
