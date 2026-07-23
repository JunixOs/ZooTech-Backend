using Microsoft.EntityFrameworkCore;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Entities;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Interfaces;
using ZooTech.Infrastructure.Persistence.Context;

namespace ZooTech.Infrastructure.Persistence.Modules.Module_Vacuno.Repositories;

public sealed class VacunoActivityStatsReadRepository : IVacunoActivityStatsReadRepository
{
    private readonly GanaderiaDbContext _context;

    public VacunoActivityStatsReadRepository(IGanaderiaDbContextFactory ganaderiaDbContextFactory)
    {
        _context = ganaderiaDbContextFactory.CreateDbContextByTenantContext();
    }

    public async Task<ActivityAggregatesOutput> GetActivityAggregatesAsync(
        DateOnly fechaInicio,
        DateOnly fechaFin,
        CancellationToken cancellationToken = default)
    {
        var source = _context.vacunos.AsNoTracking();

        var inventarioInicial = await source.CountAsync(
            vacuno =>
                vacuno.fecha_registro < fechaInicio &&
                (vacuno.deleted_at == null ||
                 DateOnly.FromDateTime(vacuno.deleted_at.Value) >= fechaInicio),
            cancellationToken);

        var altasPorDia = await source
            .Where(vacuno =>
                vacuno.fecha_registro >= fechaInicio &&
                vacuno.fecha_registro <= fechaFin)
            .GroupBy(vacuno => vacuno.fecha_registro)
            .Select(group => new { Fecha = group.Key, Cantidad = group.Count() })
            .ToDictionaryAsync(item => item.Fecha, item => item.Cantidad, cancellationToken);

        var bajasPorDia = await source
            .Where(vacuno =>
                vacuno.deleted_at != null &&
                DateOnly.FromDateTime(vacuno.deleted_at.Value) >= fechaInicio &&
                DateOnly.FromDateTime(vacuno.deleted_at.Value) <= fechaFin)
            .GroupBy(vacuno => DateOnly.FromDateTime(vacuno.deleted_at!.Value))
            .Select(group => new { Fecha = group.Key, Cantidad = group.Count() })
            .ToDictionaryAsync(item => item.Fecha, item => item.Cantidad, cancellationToken);

        return new ActivityAggregatesOutput(
            inventarioInicial,
            altasPorDia,
            bajasPorDia);
    }
}
