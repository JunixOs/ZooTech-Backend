using Microsoft.EntityFrameworkCore;
using ZooTech.Domain.Module_Vacuno.Interfaces;
using ZooTech.Infrastructure.Persistence.Context;

namespace ZooTech.Infrastructure.Persistence.Modules.Module_Vacuno.Repositories;

public sealed class VacunoActivityStatsReadRepository : IVacunoActivityStatsReadRepository
{
    private readonly GanaderiaDbContext _context;

    public VacunoActivityStatsReadRepository(IGanaderiaDbContextFactory ganaderiaDbContextFactory)
    {
        _context = ganaderiaDbContextFactory.CreateDbContextByTenantContext();
    }

    public async Task<IReadOnlyList<VacunoActivityStatsReadItem>> ListarHastaAsync(
        DateOnly fechaFin,
        CancellationToken cancellationToken = default)
    {
        return await _context.vacunos
            .AsNoTracking()
            .Where(v => v.fecha_registro <= fechaFin)
            .Select(v => new VacunoActivityStatsReadItem(v.fecha_registro, v.deleted_at))
            .ToListAsync(cancellationToken);
    }
}
