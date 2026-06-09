namespace ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ListarReportesDisponibles;

public interface IListarReportesDisponiblesUseCase
{
    Task<ReportesDisponiblesResponse> HandleAsync(ListarReportesDisponiblesQuery query, CancellationToken cancellationToken = default);
}
