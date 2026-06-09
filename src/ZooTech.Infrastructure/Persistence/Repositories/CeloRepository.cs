using Microsoft.EntityFrameworkCore;
using ZooTech.Application.Common.Gateway.Repositories;
using ZooTech.Application.Modules.Module_Celo.UseCases.ListarCelos;
using ZooTech.Infrastructure.Persistence.Context;

namespace ZooTech.Infrastructure.Persistence.Repositories;

public class CeloRepository : ICeloRepository
{
    private readonly GanaderiaDbContext _context;

    public CeloRepository(GanaderiaDbContext context)
    {
        _context = context;
    }

    public async Task<List<CeloListItemDto>> ListarCelosAsync(
        CancellationToken cancellationToken = default)
    {
        var resultado = await (
            from cr in _context.celo_registros
            join v in _context.vacunos on cr.vacuno_id equals v.id
            where cr.deleted_at == null
            orderby cr.fecha_hora descending
            select new CeloListItemDto
            {
                CodigoRegistro = cr.codigo,
                Fecha = DateOnly.FromDateTime(cr.fecha_hora),
                Hora = TimeOnly.FromDateTime(cr.fecha_hora),
                CodigoVacuno = v.codigo,
                NombreVacuno = v.nombre,
                VecesEnCelo = _context.celo_registros
                    .Count(x => x.vacuno_id == cr.vacuno_id && x.deleted_at == null)
            }
        ).ToListAsync(cancellationToken);

        return resultado;
    }

    public async Task<bool> EditarCeloAsync(
        long id,
        string? observaciones,
        List<string> caracteristicaCodes,
        CancellationToken cancellationToken = default)
    {
        var celo = await _context.celo_registros
            .Include(c => c.caracteristica_codes)
            .FirstOrDefaultAsync(
                c => c.id == id && c.deleted_at == null,
                cancellationToken);

        if (celo == null)
            return false;

        // Actualizar observaciones
        celo.observaciones = observaciones;

        // Limpiar características actuales
        celo.caracteristica_codes.Clear();

        // Obtener nuevas características
        var caracteristicas = await _context.cat_caracteristica_celos
            .Where(c => caracteristicaCodes.Contains(c.code))
            .ToListAsync(cancellationToken);

        // Agregar nuevas características
        foreach (var caracteristica in caracteristicas)
        {
            celo.caracteristica_codes.Add(caracteristica);
        }

        celo.updated_at = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> EliminarCeloAsync(
    long id,
    string motivoEliminacion,
    CancellationToken cancellationToken = default)
    {
        var celo = await _context.celo_registros
            .FirstOrDefaultAsync(
                c => c.id == id && c.deleted_at == null,
                cancellationToken);

        if (celo == null)
            return false;

        celo.deleted_at = DateTime.UtcNow;
        celo.motivo_eliminacion = motivoEliminacion;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}