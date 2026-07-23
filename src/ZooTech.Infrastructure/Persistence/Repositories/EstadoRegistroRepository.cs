using Microsoft.EntityFrameworkCore;
using ZooTech.Domain.Common.Interfaces;
using ZooTech.Infrastructure.Persistence.Context;

namespace ZooTech.Infrastructure.Persistence.Repositories;

public sealed class EstadoRegistroRepository : IEstadoRegistroRepository
{
    private readonly GanaderiaDbContext _context;

    public EstadoRegistroRepository(IGanaderiaDbContextFactory ganaderiaDbContextFactory)
    {
        _context = ganaderiaDbContextFactory.CreateDbContextByTenantContext();
    }

    public async Task<string> GetActiveCodeAsync(CancellationToken cancellationToken = default)
    {
        return await _context.cat_estado_registros
            .AsNoTracking()
            .Where(e => e.code == "ACTIVO")
            .Select(e => e.code)
            .FirstAsync(cancellationToken);
    }

    public async Task<string> GetDeletedCodeAsync(CancellationToken cancellationToken = default)
    {
        return await _context.cat_estado_registros
            .AsNoTracking()
            .Where(e => e.code == "ELIMINADO")
            .Select(e => e.code)
            .FirstAsync(cancellationToken);
    }
}
