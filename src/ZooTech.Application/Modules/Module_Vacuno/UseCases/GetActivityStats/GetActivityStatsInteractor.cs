namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.GetActivityStats;

public sealed class GetActivityStatsInteractor : IGetActivityStatsInputPort
{
    private readonly IVacunoActivityStatsService _statsService;

    public GetActivityStatsInteractor(IVacunoActivityStatsService statsService)
    {
        _statsService = statsService;
    }

    public Task<GetActivityStatsOutput> HandleAsync(
        GetActivityStatsQuery command,
        CancellationToken cancellationToken = default)
        => _statsService.GetAsync(
            command.FechaInicio,
            command.FechaFin,
            cancellationToken);
}
