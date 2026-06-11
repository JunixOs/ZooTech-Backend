namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ObtenerRegistroVacunoReporte;

public interface IRegistroVacunoReadRepository
{
    Task<RegistroVacunoDetalle?> ObtenerRegistroAsync(
        long vacunoId,
        CancellationToken cancellationToken = default);
}

