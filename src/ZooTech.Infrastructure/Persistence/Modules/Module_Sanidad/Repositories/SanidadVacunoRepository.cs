using Microsoft.EntityFrameworkCore;
using ZooTech.Domain.Module_Sanidad.Entities;
using ZooTech.Domain.Module_Sanidad.Interfaces;
using ZooTech.Infrastructure.Persistence.Context;

namespace ZooTech.Infrastructure.Persistence.Modules.Module_Sanidad.Repositories;

public sealed class SanidadVacunoRepository : ISanidadVacunoRepository
{
    private readonly GanaderiaDbContext _context;

    public SanidadVacunoRepository(GanaderiaDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<VacunoOption>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.vacunos
            .AsNoTracking()
            .Where(v => v.deleted_at == null)
            .Select(v => new VacunoOption { Id = v.id, Codigo = v.codigo, Nombre = v.nombre })
            .ToListAsync(cancellationToken);
    }
}
