using ZooTech.Application.Common.Gateway.Reports;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GetActivityStats;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.Common;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ExportarActividadVacunos;

public interface IExportarActividadVacunosInputPort
{
    Task<GeneratedReportDocument> HandleAsync(
        ExportarActividadVacunosQuery query,
        CancellationToken cancellationToken = default);
}

public sealed record ActividadVacunosReportModel(GetActivityStatsOutput Stats);

public sealed class ExportarActividadVacunosInteractor : IExportarActividadVacunosInputPort
{
    private readonly IVacunoActivityStatsService _statsService;
    private readonly IVacunoReportFormatPolicy _formatPolicy;
    private readonly IReportStrategyResolver<ActividadVacunosReportModel> _strategyResolver;

    public ExportarActividadVacunosInteractor(
        IVacunoActivityStatsService statsService,
        IVacunoReportFormatPolicy formatPolicy,
        IReportStrategyResolver<ActividadVacunosReportModel> strategyResolver)
    {
        _statsService = statsService;
        _formatPolicy = formatPolicy;
        _strategyResolver = strategyResolver;
    }

    public async Task<GeneratedReportDocument> HandleAsync(
        ExportarActividadVacunosQuery query,
        CancellationToken cancellationToken = default)
    {
        var format = await _formatPolicy.EnsureAllowedAsync(query.Formato);
        var stats = await _statsService.GetAsync(
            query.FechaInicio,
            query.FechaFin,
            cancellationToken);

        return await _strategyResolver
            .Resolve(format)
            .GenerateAsync(new ActividadVacunosReportModel(stats), cancellationToken);
    }
}
