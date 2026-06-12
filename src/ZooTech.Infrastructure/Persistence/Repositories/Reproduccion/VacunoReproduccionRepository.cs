using Microsoft.EntityFrameworkCore;
using ZooTech.Domain.Module_Reproduccion.Interfaces;
using ZooTech.Infrastructure.Persistence.Context;

namespace ZooTech.Infrastructure.Persistence.Repositories.Reproduccion;

public sealed class VacunoReproduccionRepository : IVacunoReproduccionRepository
{
    private readonly GanaderiaDbContext _context;

    public VacunoReproduccionRepository(GanaderiaDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.vacunos.AnyAsync(v => v.id == id && v.deleted_at == null, cancellationToken);
    }

    public async Task<bool> IsHembraAsync(long id, CancellationToken cancellationToken = default)
    {
        var vacuno = await _context.vacunos
            .Where(v => v.id == id && v.deleted_at == null)
            .Select(v => v.sexo_code)
            .FirstOrDefaultAsync(cancellationToken);

        return string.Equals(vacuno, "HEMBRA", StringComparison.OrdinalIgnoreCase) || 
               string.Equals(vacuno, "H", StringComparison.OrdinalIgnoreCase);
    }

    public async Task<bool> IsVivoAsync(long id, CancellationToken cancellationToken = default)
    {
        var estado = await _context.v_vacuno_estado_vigentes
            .Where(e => e.vacuno_id == id)
            .Select(e => e.estado_code)
            .FirstOrDefaultAsync(cancellationToken);

        return string.Equals(estado, "VIVO", StringComparison.OrdinalIgnoreCase);
    }
}
