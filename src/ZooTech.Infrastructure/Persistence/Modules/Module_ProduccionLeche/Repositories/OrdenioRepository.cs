using Microsoft.EntityFrameworkCore;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Common;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.ListOrdenios;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Ports;
using ZooTech.Domain.Module_ProduccionLeche.Entities;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Entities;

namespace ZooTech.Infrastructure.Persistence.Modules.Module_ProduccionLeche.Repositories;

public sealed class OrdenioRepository : IOrdenioRepository
{
    private readonly GanaderiaDbContext _dbContext;

    public OrdenioRepository(GanaderiaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> ExistsCodigoAsync(string codigo, CancellationToken cancellationToken)
        => _dbContext.ordenios.AnyAsync(x => x.deleted_at == null && x.codigo == codigo.Trim(), cancellationToken);

    public Task<bool> ExistsVacunoFechaAsync(long vacunoId, DateTime fechaHora, long? excludeOrdenioId, CancellationToken cancellationToken)
        => _dbContext.ordenios.AnyAsync(
            x => x.deleted_at == null
                 && x.vacuno_id == vacunoId
                 && x.fecha_hora == fechaHora
                 && (!excludeOrdenioId.HasValue || x.id != excludeOrdenioId.Value),
            cancellationToken);

    public Task<bool> ExistsVacunoAsync(long vacunoId, CancellationToken cancellationToken)
        => _dbContext.vacunos.AnyAsync(x => x.id == vacunoId, cancellationToken);

    public Task<bool> ExistsUsuarioAsync(long usuarioId, CancellationToken cancellationToken)
        => _dbContext.usuarios.AnyAsync(x => x.id == usuarioId, cancellationToken);

    public Task<bool> ExistsEstadoAsync(string estadoOrdenioCode, CancellationToken cancellationToken)
        => _dbContext.cat_estado_ordenios.AnyAsync(x => x.code == estadoOrdenioCode.Trim(), cancellationToken);

    public async Task<Ordenio?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.ordenios
            .Include(x => x.vacuno)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.id == id && x.deleted_at == null, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<(IReadOnlyList<OrdenioOutput> Items, int TotalCount)> ListAsync(ListOrdeniosQuery query, CancellationToken cancellationToken)
    {
        var queryable = _dbContext.ordenios
            .AsNoTracking()
            .Where(x => x.deleted_at == null)
            .AsQueryable();

        if (query.VacunoId.HasValue)
        {
            queryable = queryable.Where(x => x.vacuno_id == query.VacunoId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.EstadoOrdenioCode))
        {
            var estado = query.EstadoOrdenioCode.Trim();
            queryable = queryable.Where(x => x.estado_ordenio_code == estado);
        }

        if (query.FechaDesde.HasValue)
        {
            queryable = queryable.Where(x => x.fecha_hora >= query.FechaDesde.Value);
        }

        if (query.FechaHasta.HasValue)
        {
            queryable = queryable.Where(x => x.fecha_hora <= query.FechaHasta.Value);
        }

        var totalCount = await queryable.CountAsync(cancellationToken);

        var items = await queryable
            .OrderByDescending(x => x.fecha_hora)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(x => new OrdenioOutput(
                x.id,
                x.codigo,
                x.fecha_hora,
                x.vacuno_id,
                x.vacuno.nombre,
                x.encargado_usuario_id,
                x.litros,
                x.estado_ordenio_code,
                x.observaciones,
                x.created_at,
                x.updated_at))
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IReadOnlyList<ProduccionDiariaItem>> GetProduccionDiariaAsync(DateTime? fechaDesde, DateTime? fechaHasta, long? vacunoId, CancellationToken cancellationToken)
    {
        var queryable = _dbContext.ordenios
            .AsNoTracking()
            .Where(x => x.deleted_at == null)
            .AsQueryable();

        if (vacunoId.HasValue)
        {
            queryable = queryable.Where(x => x.vacuno_id == vacunoId.Value);
        }

        if (fechaDesde.HasValue)
        {
            queryable = queryable.Where(x => x.fecha_hora >= fechaDesde.Value);
        }

        if (fechaHasta.HasValue)
        {
            queryable = queryable.Where(x => x.fecha_hora <= fechaHasta.Value);
        }

        var grouped = await queryable
            .GroupBy(x => x.fecha_hora.Date)
            .ToListAsync(cancellationToken);

        var items = grouped
            .Select(g => new ProduccionDiariaItem(g.Key, g.Sum(x => x.litros), g.Count()))
            .OrderByDescending(x => x.Fecha)
            .ToList();

        return items;
    }

    public async Task<Ordenio> AddAsync(Ordenio ordenio, CancellationToken cancellationToken)
    {
        var entity = ToEntity(ordenio);
        _dbContext.ordenios.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return ToDomain(entity);
    }

    public async Task<Ordenio> UpdateAsync(Ordenio ordenio, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.ordenios
            .FirstOrDefaultAsync(x => x.id == ordenio.Id, cancellationToken)
            ?? throw new InvalidOperationException("No se encontró el ordeño para actualizar.");

        entity.fecha_hora = ordenio.FechaHora;
        entity.encargado_usuario_id = ordenio.EncargadoUsuarioId;
        entity.litros = ordenio.Litros;
        entity.estado_ordenio_code = ordenio.EstadoOrdenioCode;
        entity.observaciones = ordenio.Observaciones;
        entity.updated_by = ordenio.UpdatedBy;
        entity.updated_at = ordenio.UpdatedAt;
        entity.deleted_at = ordenio.DeletedAt;
        entity.deleted_by = ordenio.DeletedBy;
        entity.motivo_eliminacion = ordenio.MotivoEliminacion;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return ToDomain(entity);
    }

    public async Task<IReadOnlyList<VacunoSimpleOutput>> ListVacunosAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.vacunos
            .AsNoTracking()
            .Select(x => new VacunoSimpleOutput(x.id, x.codigo, x.nombre, x.raza_code))
            .ToListAsync(cancellationToken);
    }

    private static Ordenio ToDomain(ordenio entity)
        => Ordenio.Rehydrate(
            entity.id,
            entity.codigo,
            entity.fecha_hora,
            entity.vacuno_id,
            entity.vacuno?.nombre ?? string.Empty,
            entity.encargado_usuario_id,
            entity.litros,
            entity.estado_ordenio_code,
            entity.observaciones,
            entity.created_at,
            entity.updated_at,
            entity.deleted_at,
            entity.motivo_eliminacion,
            entity.created_by,
            entity.updated_by,
            entity.deleted_by);

    private static ordenio ToEntity(Ordenio domain)
        => new()
        {
            codigo = domain.Codigo,
            fecha_hora = domain.FechaHora,
            vacuno_id = domain.VacunoId,
            encargado_usuario_id = domain.EncargadoUsuarioId,
            litros = domain.Litros,
            estado_ordenio_code = domain.EstadoOrdenioCode,
            observaciones = domain.Observaciones,
            created_by = domain.CreatedBy,
            updated_by = domain.UpdatedBy,
            deleted_by = domain.DeletedBy,
            created_at = domain.CreatedAt,
            updated_at = domain.UpdatedAt,
            deleted_at = domain.DeletedAt,
            motivo_eliminacion = domain.MotivoEliminacion
        };
}
