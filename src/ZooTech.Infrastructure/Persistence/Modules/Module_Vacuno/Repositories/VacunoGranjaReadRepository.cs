using Microsoft.EntityFrameworkCore;
using ZooTech.Domain.Module_Vacuno.Interfaces;
using ZooTech.Domain.Module_Vacuno.ReadModels;
using ZooTech.Infrastructure.Persistence.Context;

namespace ZooTech.Infrastructure.Persistence.Modules.Module_Vacuno.Repositories;

public sealed class VacunoGranjaReadRepository : IVacunoGranjaReadRepository
{
    private readonly GanaderiaDbContext _context;

    public VacunoGranjaReadRepository(GanaderiaDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<VacunoGranjaListItem>> ListarActivasAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.granjas
            .AsNoTracking()
            .Where(g => g.activo)
            .OrderBy(g => g.nombre)
            .Select(g => new VacunoGranjaListItem(
                g.id,
                g.nombre,
                g.distrito_codigo,
                g.distrito_codigoNavigation != null ? g.distrito_codigoNavigation.nombre : null,
                g.distrito_codigoNavigation != null && g.distrito_codigoNavigation.provincia_codigoNavigation != null
                    ? g.distrito_codigoNavigation.provincia_codigoNavigation.nombre
                    : null,
                g.distrito_codigoNavigation != null
                    && g.distrito_codigoNavigation.provincia_codigoNavigation != null
                    && g.distrito_codigoNavigation.provincia_codigoNavigation.departamento_codigoNavigation != null
                        ? g.distrito_codigoNavigation.provincia_codigoNavigation.departamento_codigoNavigation.nombre
                        : null))
            .ToListAsync(cancellationToken);
    }
}
