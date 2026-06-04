namespace ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ListarReporteVacunos;

public interface IListarReporteVacunosUseCase
{
    Task<ListadoVacunosReporteResponse> HandleAsync(
        ListarReporteVacunosQuery query,
        CancellationToken cancellationToken = default);
}
