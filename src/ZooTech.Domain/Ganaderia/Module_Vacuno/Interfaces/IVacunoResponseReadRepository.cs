using ZooTech.Domain.Ganaderia.Module_Vacuno.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace ZooTech.Domain.Ganaderia.Module_Vacuno.Interfaces;

public interface IVacunoResponseReadRepository
{
    Task<Dictionary<long, string>> GetActiveCodesByIdsAsync(
        IEnumerable<long> ids,
        CancellationToken cancellationToken = default);

    Task<VacunoGranjaDetails?> GetGranjaDetailsAsync(
        long granjaId,
        CancellationToken cancellationToken = default);

    Task<decimal?> GetPrecioCompraAsync(
        long vacunoId,
        CancellationToken cancellationToken = default);

    Task<VacunoUtilizacionDetails?> GetLatestUtilizacionAsync(
        long vacunoId,
        CancellationToken cancellationToken = default);
        
    Task<ActivityAggregatesOutput> GetActivityAggregatesAsync(
        DateOnly start,
        DateOnly end,
        CancellationToken cancellationToken = default);
}
