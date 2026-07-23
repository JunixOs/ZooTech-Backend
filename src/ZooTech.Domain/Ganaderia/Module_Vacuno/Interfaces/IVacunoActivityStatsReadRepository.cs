using ZooTech.Domain.Ganaderia.Module_Vacuno.Entities;

namespace ZooTech.Domain.Ganaderia.Module_Vacuno.Interfaces;

public interface IVacunoActivityStatsReadRepository
{
    Task<ActivityAggregatesOutput> GetActivityAggregatesAsync(
        DateOnly fechaInicio,
        DateOnly fechaFin,
        CancellationToken cancellationToken = default);
}
