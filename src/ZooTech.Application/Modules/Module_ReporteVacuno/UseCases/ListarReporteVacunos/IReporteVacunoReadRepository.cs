namespace ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ListarReporteVacunos;

public interface IReporteVacunoReadRepository
{
    Task<ReporteVacunoListadoPage> ListarAsync(
        ReporteVacunoListadoCriteria criteria,
        CancellationToken cancellationToken = default);
}
