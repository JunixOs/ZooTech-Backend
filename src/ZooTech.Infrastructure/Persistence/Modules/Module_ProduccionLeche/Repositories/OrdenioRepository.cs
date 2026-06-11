using Microsoft.EntityFrameworkCore;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Common;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.ListOrdenios;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Ports;
using ZooTech.Domain.Module_ProduccionLeche.Entities;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Entities;
using ZooTech.Infrastructure.Persistence.Mappers;

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

        return entity is null ? null : OrdenioMapper.ToDomain(entity);
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

    public async Task<IReadOnlyList<ProduccionComparativaDiariaItem>> GetProduccionComparativaDiariaAsync(DateTime? fechaDesde, DateTime? fechaHasta, long? vacunoId, CancellationToken cancellationToken)
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

        var produccionRealPorDiaVacuno = await queryable
            .GroupBy(x => new { Fecha = x.fecha_hora.Date, x.vacuno_id })
            .Select(g => new
            {
                g.Key.Fecha,
                g.Key.vacuno_id,
                LitrosReales = g.Sum(x => x.litros),
                CantidadOrdenios = g.Count()
            })
            .ToListAsync(cancellationToken);

        if (produccionRealPorDiaVacuno.Count == 0)
        {
            return [];
        }

        var fechaMinima = DateOnly.FromDateTime(produccionRealPorDiaVacuno.Min(x => x.Fecha));
        var fechaMaxima = DateOnly.FromDateTime(produccionRealPorDiaVacuno.Max(x => x.Fecha));
        var vacunoIds = produccionRealPorDiaVacuno
            .Select(x => x.vacuno_id)
            .Distinct()
            .ToList();

        var produccionEstandar = await _dbContext.produccion_leche_estandars
            .AsNoTracking()
            .Where(x => x.vacuno_id.HasValue
                        && vacunoIds.Contains(x.vacuno_id.Value)
                        && x.fecha_inicio <= fechaMaxima
                        && (!x.fecha_fin.HasValue || x.fecha_fin.Value >= fechaMinima))
            .Select(x => new
            {
                VacunoId = x.vacuno_id!.Value,
                x.fecha_inicio,
                x.fecha_fin,
                x.litros_esperados_dia
            })
            .ToListAsync(cancellationToken);

        var items = produccionRealPorDiaVacuno
            .Select(x =>
            {
                var fecha = DateOnly.FromDateTime(x.Fecha);
                var litrosEstandar = produccionEstandar
                    .Where(e => e.VacunoId == x.vacuno_id
                                && e.fecha_inicio <= fecha
                                && (!e.fecha_fin.HasValue || e.fecha_fin.Value >= fecha))
                    .OrderByDescending(e => e.fecha_inicio)
                    .Select(e => e.litros_esperados_dia)
                    .FirstOrDefault();

                return new
                {
                    x.Fecha,
                    x.LitrosReales,
                    LitrosEstandar = litrosEstandar,
                    x.CantidadOrdenios
                };
            })
            .GroupBy(x => x.Fecha)
            .Select(g => new ProduccionComparativaDiariaItem(
                g.Key,
                g.Sum(x => x.LitrosReales),
                g.Sum(x => x.LitrosEstandar),
                g.Sum(x => x.CantidadOrdenios)))
            .OrderByDescending(x => x.Fecha)
            .ToList();

        return items;
    }

    public async Task<Ordenio> AddAsync(Ordenio ordenio, CancellationToken cancellationToken)
    {
        var entity = OrdenioMapper.ToEntity(ordenio);
        _dbContext.ordenios.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return OrdenioMapper.ToDomain(entity);
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
        return OrdenioMapper.ToDomain(entity);
    }

    public async Task<IReadOnlyList<VacunoSimpleOutput>> ListVacunosAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.vacunos
            .AsNoTracking()
            .Select(x => new VacunoSimpleOutput(x.id, x.codigo, x.nombre, x.raza_code))
            .ToListAsync(cancellationToken);
    }

   
}
