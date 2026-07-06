namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetHistorialByVacunoId;

public interface IGetHistorialByVacunoIdInputPort
{
    Task<GetHistorialByVacunoIdOutput> HandleAsync(long vacunoId, string? fechaDesde = null, string? fechaHasta = null, CancellationToken cancellationToken = default);
}
