namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ObtenerRegistroVacunoReporte;

public interface IObtenerRegistroVacunoReporteUseCase
{
    Task<RegistroVacunoReporteResponse> HandleAsync(
        ObtenerRegistroVacunoReporteQuery query,
        CancellationToken cancellationToken = default);
}
