namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ListarVacunosReporte;

public interface IListarVacunosReporteUseCase
{
    Task<ListarVacunosReporteResponse> HandleAsync(
        ListarVacunosReporteQuery query,
        CancellationToken cancellationToken = default);
}
