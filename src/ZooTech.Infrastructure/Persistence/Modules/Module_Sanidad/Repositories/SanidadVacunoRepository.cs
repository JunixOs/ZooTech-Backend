using Microsoft.EntityFrameworkCore;
using ZooTech.Domain.Module_Sanidad.Entities;
using ZooTech.Domain.Module_Sanidad.Interfaces;
using ZooTech.Infrastructure.Context;

namespace ZooTech.Infrastructure.Persistence.Modules.Module_Sanidad.Repositories;

public sealed class SanidadVacunoRepository : ISanidadVacunoRepository
{
    private readonly ZootechContext _context;

    public SanidadVacunoRepository(ZootechContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<VacunoOption>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Vacunos
            .AsNoTracking()
            .Where(v => v.deleted_at == null)
            .Select(v => new VacunoOption { Id = v.id, Codigo = v.codigo, Nombre = v.nombre })
            .ToListAsync(cancellationToken);
    }
}
