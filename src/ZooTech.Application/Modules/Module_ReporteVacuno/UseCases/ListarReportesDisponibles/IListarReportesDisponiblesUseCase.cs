namespace ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ListarReportesDisponibles;

public interface IListarReportesDisponiblesUseCase
{
    ReportesDisponiblesResponse Handle(ListarReportesDisponiblesQuery query);
}
