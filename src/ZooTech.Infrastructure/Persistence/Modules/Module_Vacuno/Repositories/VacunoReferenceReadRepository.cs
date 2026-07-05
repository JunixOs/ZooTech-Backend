using Microsoft.EntityFrameworkCore;
using ZooTech.Domain.Module_Vacuno.Interfaces;
using ZooTech.Domain.Module_Vacuno.ReadModels;
using ZooTech.Infrastructure.Persistence.Context;

namespace ZooTech.Infrastructure.Persistence.Modules.Module_Vacuno.Repositories;

public sealed class VacunoReferenceReadRepository : IVacunoReferenceReadRepository
{
    private readonly GanaderiaDbContext _context;

    public VacunoReferenceReadRepository(GanaderiaDbContext context)
    {
        _context = context;
    }

    public Task<VacunoCodeLookup?> GetActiveByIdAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        return _context.vacunos
            .AsNoTracking()
            .Where(v => v.id == id && v.deleted_at == null)
            .Select(v => new VacunoCodeLookup(v.id, v.codigo))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<VacunoCodeLookup?> GetActiveByCodigoAsync(
        string codigo,
        CancellationToken cancellationToken = default)
    {
        return _context.vacunos
            .AsNoTracking()
            .Where(v => v.codigo == codigo && v.deleted_at == null)
            .Select(v => new VacunoCodeLookup(v.id, v.codigo))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<bool> ExistsActiveGranjaAsync(
        long granjaId,
        CancellationToken cancellationToken = default)
    {
        return _context.granjas
            .AsNoTracking()
            .AnyAsync(g => g.id == granjaId && g.activo, cancellationToken);
    }

    public Task<bool> ExistsDistritoAsync(
        string codigoDistrito,
        CancellationToken cancellationToken = default)
    {
        return _context.geo_distritos
            .AsNoTracking()
            .AnyAsync(d => d.codigo == codigoDistrito, cancellationToken);
    }

    public Task<long?> FindGranjaIdAsync(
        string nombre,
        string codigoDistrito,
        CancellationToken cancellationToken = default)
    {
        return _context.granjas
            .AsNoTracking()
            .Where(g => g.nombre == nombre && g.distrito_codigo == codigoDistrito)
            .Select(g => (long?)g.id)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
