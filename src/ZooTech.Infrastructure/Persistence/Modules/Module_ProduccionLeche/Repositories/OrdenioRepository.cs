using Microsoft.EntityFrameworkCore;
using ZooTech.Domain.Module_ProduccionLeche.Entities;
using ZooTech.Domain.Module_ProduccionLeche.Interfaces;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Entities;

namespace ZooTech.Infrastructure.Persistence.Modules.Module_ProduccionLeche.Repositories;

public sealed class OrdenioRepository : IOrdenioRepository
{
    private readonly GanaderiaDbContext _ganaderiaDbContext;

    public OrdenioRepository(IGanaderiaDbContextFactory ganaderiaDbContextFactory)
        : this(ganaderiaDbContextFactory.CreateDbContextByTenantContext())
    {
    }

    public OrdenioRepository(GanaderiaDbContext ganaderiaDbContext)
    {
        _ganaderiaDbContext = ganaderiaDbContext;
    }

    public Task<bool> ExistsCodigoAsync(string codigo, CancellationToken cancellationToken)
        => _ganaderiaDbContext.ordenios.AnyAsync(x => x.deleted_at == null && x.codigo == codigo.Trim(), cancellationToken);

    public async Task<bool> ExistsVacunoFechaAsync(long vacunoId, DateTime fechaHora, long? excludeOrdenioId, CancellationToken cancellationToken)
    {
        return await _ganaderiaDbContext.ordenios.AnyAsync(
            x => x.deleted_at == null
                && x.vacuno_id == vacunoId
                && x.fecha_hora == fechaHora
                && (!excludeOrdenioId.HasValue || x.id != excludeOrdenioId.Value),
            cancellationToken);
    }

    public Task<bool> ExistsVacunoAsync(long vacunoId, CancellationToken cancellationToken)
        => _ganaderiaDbContext.vacunos.AnyAsync(x => x.id == vacunoId, cancellationToken);

    public Task<bool> HasActiveRecordsByVacunoAsync(long vacunoId, CancellationToken cancellationToken)
        => _ganaderiaDbContext.ordenios.AnyAsync(x => x.vacuno_id == vacunoId && x.deleted_at == null, cancellationToken);

    public Task<bool> ExistsUsuarioAsync(long usuarioId, CancellationToken cancellationToken)
        =>_ganaderiaDbContext.usuarios.AnyAsync(x => x.id == usuarioId, cancellationToken);

    public Task<bool> ExistsEstadoAsync(string estadoOrdenioCode, CancellationToken cancellationToken) 
        => _ganaderiaDbContext.cat_estado_ordenios.AnyAsync(x => x.code == estadoOrdenioCode.Trim(), cancellationToken);

    public async Task<Ordenio?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        var entity = await _ganaderiaDbContext.ordenios
            .Include(x => x.vacuno)
            .Include(x => x.encargado_usuario)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.id == id && x.deleted_at == null, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<Ordenio?> GetByCodigoAsync(string codigo, CancellationToken cancellationToken)
    {
        var entity = await _ganaderiaDbContext.ordenios
            .Include(x => x.vacuno)
            .Include(x => x.encargado_usuario)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.codigo == codigo.Trim() && x.deleted_at == null, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<(IReadOnlyList<OrdenioList> Items, int TotalCount)> ListAsync(
        long? vacunoId,
        string? estadoOrdenioCode,
        DateTime? fechaDesde,
        DateTime? fechaHasta,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var queryable = BuildListQuery(vacunoId, estadoOrdenioCode, fechaDesde, fechaHasta);


        var totalCount = await queryable.CountAsync(cancellationToken);


        var entities = await queryable
            .OrderByDescending(x => x.fecha_hora)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(ToListProjection())
            .ToListAsync(cancellationToken);

        return (entities.Select(ListOrdenioToDomain).ToList(), totalCount);
    }

    public async Task<Ordenio> AddAsync(Ordenio ordenio, CancellationToken cancellationToken)
    {
        var entity = ToEntity(ordenio);
        _ganaderiaDbContext.ordenios.Add(entity);
        await Task.CompletedTask;
        return ToDomain(entity);
    }

    public async Task<Ordenio> UpdateAsync(Ordenio ordenio, CancellationToken cancellationToken)
    {
        var entity = await _ganaderiaDbContext.ordenios
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

        await Task.CompletedTask;
        return ToDomain(entity);
    }
    public async Task<IReadOnlyList<OrdenioList>> ListReportAsync(
        long? vacunoId,
        string? estadoOrdenioCode,
        DateTime? fechaDesde,
        DateTime? fechaHasta,
        CancellationToken cancellationToken)
    {
        var entities = await BuildListQuery(vacunoId, estadoOrdenioCode, fechaDesde, fechaHasta)
            .OrderByDescending(x => x.fecha_hora)
            .Select(ToListProjection())
            .ToListAsync(cancellationToken);

        return entities.Select(ListOrdenioToDomain).ToList();
    }

    private IQueryable<ordenio> BuildListQuery(
        long? vacunoId,
        string? estadoOrdenioCode,
        DateTime? fechaDesde,
        DateTime? fechaHasta
    )
    {
        var queryable = _ganaderiaDbContext.ordenios
            .AsNoTracking()
            .Where(x => x.deleted_at == null)
            .AsQueryable();

        if (vacunoId.HasValue)
            queryable = queryable.Where(x => x.vacuno_id == vacunoId.Value);

        if (!string.IsNullOrWhiteSpace(estadoOrdenioCode))
            queryable = queryable.Where(x => x.estado_ordenio_code == estadoOrdenioCode.Trim());

        if (fechaDesde.HasValue)
            queryable = queryable.Where(x => x.fecha_hora >= fechaDesde.Value);

        if (fechaHasta.HasValue)
            queryable = queryable.Where(x => x.fecha_hora <= fechaHasta.Value);

        return queryable;
    }

    private static System.Linq.Expressions.Expression<Func<ordenio, ordenio>> ToListProjection()
        => entity => new ordenio
        {
            id = entity.id,
            codigo = entity.codigo,
            fecha_hora = entity.fecha_hora,
            vacuno_id = entity.vacuno_id,
            encargado_usuario_id = entity.encargado_usuario_id,
            litros = entity.litros,
            estado_ordenio_code = entity.estado_ordenio_code,
            observaciones = entity.observaciones,
            created_at = entity.created_at,
            updated_at = entity.updated_at,
            deleted_at = entity.deleted_at,
            motivo_eliminacion = entity.motivo_eliminacion,
            vacuno = new vacuno
            {
                id = entity.vacuno.id,
                codigo = entity.vacuno.codigo,
                nombre = entity.vacuno.nombre
            },
            encargado_usuario = new usuario
            {
                id = entity.encargado_usuario.id,
                nombre_completo = entity.encargado_usuario.nombre_completo
            }
        };


    public static OrdenioList ListOrdenioToDomain(ordenio entity)
        => OrdenioList.Rehydrate(

            entity.id,
            entity.codigo,
            entity.fecha_hora,
            entity.vacuno_id,
            entity.vacuno?.nombre ?? string.Empty,
            entity.vacuno?.codigo ?? string.Empty,
            entity.encargado_usuario_id,
            entity.encargado_usuario?.nombre_completo ?? string.Empty,
            entity.litros,
            entity.estado_ordenio_code,
            entity.observaciones,
            entity.created_at,
            entity.updated_at,
            entity.deleted_at,
            entity.motivo_eliminacion);

    private static Ordenio ToDomain(ordenio entity)
        => Ordenio.Rehydrate(
            entity.id,
            entity.codigo,
            entity.fecha_hora,
            entity.vacuno_id,
            entity.vacuno?.nombre ?? string.Empty,
            entity.encargado_usuario_id,
            entity.encargado_usuario?.nombre_completo ?? string.Empty,
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
