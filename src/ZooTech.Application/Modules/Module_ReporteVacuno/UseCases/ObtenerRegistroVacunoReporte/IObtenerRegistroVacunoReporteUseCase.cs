namespace ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ObtenerRegistroVacunoReporte;

public interface IObtenerRegistroVacunoReporteUseCase
{
    Task<RegistroVacunoReporteResponse> HandleAsync(
        ObtenerRegistroVacunoReporteQuery query,
        CancellationToken cancellationToken = default);
}
