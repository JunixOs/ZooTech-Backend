namespace ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ObtenerRegistroVacunoReporte;

public interface IRegistroVacunoReadRepository
{
    Task<RegistroVacunoDetalle?> ObtenerRegistroAsync(
        long vacunoId,
        CancellationToken cancellationToken = default);
}
