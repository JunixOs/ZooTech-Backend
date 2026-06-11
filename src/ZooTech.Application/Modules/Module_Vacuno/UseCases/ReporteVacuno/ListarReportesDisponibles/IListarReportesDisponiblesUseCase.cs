namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ListarReportesDisponibles;

public interface IListarReportesDisponiblesUseCase
{
    Task<ReportesDisponiblesResponse> HandleAsync(ListarReportesDisponiblesQuery query, CancellationToken cancellationToken = default);
}

