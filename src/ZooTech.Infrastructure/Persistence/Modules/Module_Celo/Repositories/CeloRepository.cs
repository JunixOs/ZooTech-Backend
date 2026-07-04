using Microsoft.EntityFrameworkCore;
using ZooTech.Domain.Module_Celo.Entities;
using ZooTech.Domain.Module_Celo.Interfaces;
using ZooTech.Infrastructure.Persistence.Context;

namespace ZooTech.Infrastructure.Persistence.Modules.Module_Celo.Repositories;

public sealed class CeloRepository : ICeloRepository
{
    private readonly GanaderiaDbContext _context;

    public CeloRepository(GanaderiaDbContext context)
    {
        _context = context;
    }

    public async Task<List<Celo>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _context.celo_registros
            .AsNoTracking()
            .Include(c => c.vacuno)
            .Include(c => c.caracteristica_codes)
            .Where(c => c.deleted_at == null)
            .OrderByDescending(c => c.fecha_hora)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain).ToList();
    }

    public async Task<Dictionary<long, int>> GetVecesEnCeloCountsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.celo_registros
            .AsNoTracking()
            .Where(c => c.deleted_at == null)
            .GroupBy(c => c.vacuno_id)
            .Select(g => new { VacunoId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.VacunoId, x => x.Count, cancellationToken);
    }

    public async Task<Celo?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.celo_registros
            .AsNoTracking()
            .Include(c => c.caracteristica_codes)
            .Include(c => c.vacuno)
            .FirstOrDefaultAsync(c => c.id == id && c.deleted_at == null, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<Celo> AddAsync(Celo celo, CancellationToken cancellationToken = default)
    {
        var entity = ToEntity(celo);
        await _context.celo_registros.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        await _context.Entry(entity)
            .Reference(e => e.vacuno)
            .LoadAsync(cancellationToken);

        return ToDomain(entity);
    }

    public async Task<Celo> UpdateAsync(Celo celo, CancellationToken cancellationToken = default)
    {
        var entity = await _context.celo_registros
            .Include(c => c.caracteristica_codes)
            .FirstOrDefaultAsync(c => c.id == celo.Id && c.deleted_at == null, cancellationToken);

        if (entity is null)
            throw new InvalidOperationException($"No se encontró el registro de celo con ID {celo.Id}.");

        entity.observaciones = celo.Observaciones;
        entity.updated_at = celo.UpdatedAt;
        entity.updated_by = celo.UpdatedBy;

        entity.caracteristica_codes.Clear();
        if (celo.CaracteristicaCodes.Count > 0)
        {
            var caracteristicas = await _context.cat_caracteristica_celos
                .Where(c => celo.CaracteristicaCodes.Contains(c.code))
                .ToListAsync(cancellationToken);

            foreach (var caracteristica in caracteristicas)
                entity.caracteristica_codes.Add(caracteristica);
        }

        entity.deleted_at = celo.DeletedAt;
        entity.deleted_by = celo.DeletedBy;
        entity.motivo_eliminacion = celo.MotivoEliminacion;

        await _context.SaveChangesAsync(cancellationToken);

        return ToDomain(entity);
    }

    public async Task<bool> ExistsVacunoAsync(long vacunoId, CancellationToken cancellationToken = default)
    {
        return await _context.vacunos
            .AnyAsync(v => v.id == vacunoId, cancellationToken);
    }

    public async Task<bool> ExistsCodigoAsync(string codigo, CancellationToken cancellationToken = default)
    {
        return await _context.celo_registros
            .AnyAsync(c => c.codigo == codigo && c.deleted_at == null, cancellationToken);
    }

    public async Task<List<Celo>> GetByDateRangeAsync(
        DateTime? fechaInicio,
        DateTime? fechaFin,
        CancellationToken cancellationToken = default)
    {
        var query = _context.celo_registros
            .AsNoTracking()
            .Include(c => c.vacuno)
            .Include(c => c.caracteristica_codes)
            .Where(c => c.deleted_at == null);

        if (fechaInicio.HasValue)
        {
            query = query.Where(c => c.fecha_hora >= fechaInicio.Value);
        }

        if (fechaFin.HasValue)
        {
            query = query.Where(c => c.fecha_hora <= fechaFin.Value);
        }

        var entities = await query
            .OrderBy(c => c.fecha_hora)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain).ToList();
    }

    private static Celo ToDomain(Entities.celo_registro entity)
    {
        return Celo.Rehydrate(
            id: entity.id,
            codigo: entity.codigo,
            fechaHora: entity.fecha_hora,
            vacunoId: entity.vacuno_id,
            vacunoCodigo: entity.vacuno?.codigo ?? string.Empty,
            nombreVacuno: entity.vacuno?.nombre ?? string.Empty,
            encargadoUsuarioId: entity.encargado_usuario_id,
            observaciones: entity.observaciones,
            estadoRegistroCode: entity.estado_registro_code,
            caracteristicaCodes: entity.caracteristica_codes
                .Select(c => c.code)
                .ToList(),
            createdAt: entity.created_at,
            updatedAt: entity.updated_at,
            deletedAt: entity.deleted_at,
            motivoEliminacion: entity.motivo_eliminacion,
            createdBy: entity.created_by,
            updatedBy: entity.updated_by,
            deletedBy: entity.deleted_by);
    }

    private static Entities.celo_registro ToEntity(Celo celo)
    {
        return new Entities.celo_registro
        {
            id = celo.Id,
            codigo = celo.Codigo,
            fecha_hora = celo.FechaHora,
            vacuno_id = celo.VacunoId,
            encargado_usuario_id = celo.EncargadoUsuarioId,
            observaciones = celo.Observaciones,
            estado_registro_code = celo.EstadoRegistroCode,
            created_at = celo.CreatedAt,
            updated_at = celo.UpdatedAt,
            deleted_at = celo.DeletedAt,
            motivo_eliminacion = celo.MotivoEliminacion,
            created_by = celo.CreatedBy,
            updated_by = celo.UpdatedBy,
            deleted_by = celo.DeletedBy
        };
    }
}
