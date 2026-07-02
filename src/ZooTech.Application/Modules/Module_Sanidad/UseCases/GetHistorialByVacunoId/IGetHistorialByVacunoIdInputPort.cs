namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetHistorialByVacunoId;

public interface IGetHistorialByVacunoIdInputPort
{
    Task<GetHistorialByVacunoIdOutput> HandleAsync(long vacunoId, string? desde = null, string? hasta = null, CancellationToken cancellationToken = default);
}
