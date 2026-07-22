using Microsoft.EntityFrameworkCore;
using ZooTech.Domain.Module_Sanidad.Entities;
using ZooTech.Domain.Module_Sanidad.Interfaces;
using ZooTech.Infrastructure.Persistence.Context;

namespace ZooTech.Infrastructure.Persistence.Modules.Module_Sanidad.Repositories;

public sealed class TipoPesoRepository : ITipoPesoRepository
{
    private readonly GanaderiaDbContext _ganaderiaDbContext;

    public TipoPesoRepository(
        IGanaderiaDbContextFactory ganaderiaDbContextFactory
    )
    {
        _ganaderiaDbContext = ganaderiaDbContextFactory.CreateDbContextByTenantContext();
    }

    public async Task<IEnumerable<TipoPeso>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _ganaderiaDbContext.cat_tipo_pesos
            .AsNoTracking()
            .Where(t => t.activo)
            .Select(t => new TipoPeso { Code = t.code, Nombre = t.nombre })
            .ToListAsync(cancellationToken);
    }
}
