using Microsoft.EntityFrameworkCore;
using ZooTech.Domain.Module_Sanidad.Entities;
using ZooTech.Domain.Module_Sanidad.Interfaces;
using ZooTech.Infrastructure.Context;

namespace ZooTech.Infrastructure.Persistence.Modules.Module_Sanidad.Repositories;

public sealed class TipoPesoRepository : ITipoPesoRepository
{
    private readonly ZootechContext _context;

    public TipoPesoRepository(ZootechContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TipoPeso>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.CatTipoPesos
            .AsNoTracking()
            .Where(t => t.activo)
            .Select(t => new TipoPeso { Code = t.code, Nombre = t.nombre })
            .ToListAsync(cancellationToken);
    }
}
