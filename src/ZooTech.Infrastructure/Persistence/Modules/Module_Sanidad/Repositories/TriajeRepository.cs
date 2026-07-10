using System.Globalization;
using Microsoft.EntityFrameworkCore;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Domain.Module_Sanidad.Entities;
using ZooTech.Domain.Module_Sanidad.Interfaces;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Entities;

namespace ZooTech.Infrastructure.Persistence.Modules.Module_Sanidad.Repositories;

public class TriajeRepository : ITriajeRepository
{
    private readonly GanaderiaDbContext _ganaderiaDbContext;
    private readonly IDateTimeProvider _dateTimeProvider;

    public TriajeRepository(
        IGanaderiaDbContextFactory ganaderiaDbContextFactory,
        IDateTimeProvider dateTimeProvider
    )
    {
        _ganaderiaDbContext = ganaderiaDbContextFactory.CreateDbContextByTenantContext();
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Triaje?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _ganaderiaDbContext.triajes
            .Include(t => t.vacuno)
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.id == id && t.deleted_at == null, cancellationToken);

        return entity is null ? null : ToTriaje(entity);
    }

    public async Task<(IEnumerable<Triaje> Items, int Total)> GetAllAsync(
        int pagina,
        int tamano,
        string? fecha = null,
        string? fechaDesde = null,
        string? fechaHasta = null,
        string? codigo = null,
        string? nombre = null,
        string? tipoPeso = null,
        decimal? pesoKg = null,
        long? vacunoId = null,
        bool? uniqueVacuno = null,
        CancellationToken cancellationToken = default)
    {
        var query = _ganaderiaDbContext.triajes
            .AsNoTracking()
            .Include(t => t.vacuno)
            .Where(t => t.deleted_at == null)
            .AsQueryable();

        if (vacunoId.HasValue)
            query = query.Where(t => t.vacuno_id == vacunoId.Value);

        if (!string.IsNullOrEmpty(codigo))
            query = query.Where(t => t.codigo.Contains(codigo));

        if (!string.IsNullOrEmpty(nombre))
            query = query.Where(t => t.vacuno.nombre.Contains(nombre));

        if (!string.IsNullOrEmpty(fecha) &&
    DateTime.TryParseExact(fecha, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fechaExacta))
        {
            var desdeExacta = fechaExacta.Date;
            var hastaExacta = desdeExacta.AddDays(1);
            query = query.Where(t => t.fecha_hora >= desdeExacta && t.fecha_hora < hastaExacta);
        }

        if (!string.IsNullOrEmpty(fechaDesde) &&
            DateTime.TryParseExact(fechaDesde, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var desde))
        {
            query = query.Where(t => t.fecha_hora >= desde.Date);
        }

        if (!string.IsNullOrEmpty(fechaHasta) &&
            DateTime.TryParseExact(fechaHasta, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var hasta))
        {
            query = query.Where(t => t.fecha_hora < hasta.Date.AddDays(1));
        }

        if (!string.IsNullOrEmpty(tipoPeso))
            query = query.Where(t => t.tipo_peso_code == tipoPeso);

        if (pesoKg.HasValue)
            query = query.Where(t => t.peso_kg == pesoKg);

        if (uniqueVacuno == true)
        {
            query = query.Where(t => t.id == _ganaderiaDbContext.triajes
                .Where(inner => inner.vacuno_id == t.vacuno_id && inner.deleted_at == null)
                .OrderByDescending(inner => inner.fecha_hora)
                .Select(inner => inner.id)
                .FirstOrDefault());
        }



        query = query.OrderByDescending(t => t.fecha_hora);

        var total = await query.CountAsync(cancellationToken);

        var entities = await query
            .Skip((pagina - 1) * tamano)
            .Take(tamano)
            .ToListAsync(cancellationToken);

        return (entities.Select(ToTriaje), total);
    }

    public async Task<Triaje> AddAsync(Triaje triaje, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = ToEntity(triaje);
            _ganaderiaDbContext.triajes.Add(entity);
            await _ganaderiaDbContext.SaveChangesAsync(cancellationToken);
            return ToTriaje(entity);
        }
        catch (DbUpdateException)
        {
            throw new InvalidOperationException("No se pudo registrar el triaje. Verifique que no exista un registro con el mismo vacuno, tipo de peso y fecha.");
        }
    }

    public async Task<Triaje> UpdateAsync(Triaje triaje, CancellationToken cancellationToken = default)
    {
        var entity = ToEntity(triaje);
        _ganaderiaDbContext.triajes.Update(entity);
        await _ganaderiaDbContext.SaveChangesAsync(cancellationToken);
        return ToTriaje(entity);
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _ganaderiaDbContext.triajes
            .FirstOrDefaultAsync(t => t.id == id && t.deleted_at == null, cancellationToken);

        if (entity is null) return;

        var now = _dateTimeProvider.ServerNow;
        entity.deleted_at = now;
        entity.updated_at = now;

        await _ganaderiaDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<string> GenerateCodigoAsync(CancellationToken cancellationToken = default)
    {
        var codigos = await _ganaderiaDbContext.triajes
            .Where(t => t.codigo.StartsWith("TRI"))
            .Select(t => t.codigo)
            .ToListAsync(cancellationToken);

        var maxNumber = codigos
            .Select(codigo =>
                int.TryParse(codigo[3..], out var number) ? number : 0)
            .DefaultIfEmpty(0)
            .Max();

        return $"TRI{maxNumber + 1:D3}";
    }

    public async Task<IEnumerable<TriajeHistorialItem>> GetHistorialByVacunoIdAsync(long vacunoId, string? fechaDesde = null, string? fechaHasta = null, CancellationToken cancellationToken = default)
    {
        var query = _ganaderiaDbContext.triajes
            .AsNoTracking()
            .Where(t => t.vacuno_id == vacunoId && t.deleted_at == null);

        if (!string.IsNullOrEmpty(fechaDesde) &&
            DateTime.TryParseExact(fechaDesde, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var desde))
        {
            query = query.Where(t => t.fecha_hora >= desde.Date);
        }

        if (!string.IsNullOrEmpty(fechaHasta) &&
            DateTime.TryParseExact(fechaHasta, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var hasta))
        {
            query = query.Where(t => t.fecha_hora < hasta.Date.AddDays(1));
        }

        return await query
            .OrderBy(t => t.fecha_hora)
            .Select(t => new TriajeHistorialItem
            {
                Id = t.id,
                FechaHora = t.fecha_hora,
                TipoPesoCode = t.tipo_peso_code,
                PesoKg = t.peso_kg
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TriajeHistorialItem>> GetHistorialGeneralAsync(string? fechaDesde = null, string? fechaHasta = null, CancellationToken cancellationToken = default)
    {
        var query = _ganaderiaDbContext.triajes
            .AsNoTracking()
            .Where(t => t.deleted_at == null);

        if (!string.IsNullOrEmpty(fechaDesde) &&
            DateTime.TryParseExact(fechaDesde, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var desde))
        {
            query = query.Where(t => t.fecha_hora >= desde.Date);
        }

        if (!string.IsNullOrEmpty(fechaHasta) &&
            DateTime.TryParseExact(fechaHasta, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var hasta))
        {
            query = query.Where(t => t.fecha_hora < hasta.Date.AddDays(1));
        }

        return await query
            .OrderBy(t => t.fecha_hora)
            .Select(t => new TriajeHistorialItem
            {
                Id = t.id,
                FechaHora = t.fecha_hora,
                TipoPesoCode = t.tipo_peso_code,
                PesoKg = t.peso_kg
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsVacunoAsync(long vacunoId, CancellationToken cancellationToken = default)
    {
        return await _ganaderiaDbContext.vacunos.AnyAsync(v => v.id == vacunoId, cancellationToken);
    }

    public async Task<bool> ExistsUsuarioAsync(long usuarioId, CancellationToken cancellationToken = default)
    {
        return await _ganaderiaDbContext.usuarios.AnyAsync(u => u.id == usuarioId, cancellationToken);
    }

    public async Task<bool> ExistsTipoPesoAsync(string tipoPesoCode, CancellationToken cancellationToken = default)
    {
        return await _ganaderiaDbContext.cat_tipo_pesos.AnyAsync(tp => tp.code == tipoPesoCode, cancellationToken);
    }

    public async Task<IEnumerable<TriajeDetallePorVacunoItem>> GetDetallesByVacunoIdAsync(long vacunoId, CancellationToken cancellationToken = default)
    {
        return await _ganaderiaDbContext.triajes
            .AsNoTracking()
            .Include(t => t.tipo_peso_codeNavigation)
            .Where(t => t.vacuno_id == vacunoId && t.deleted_at == null)
            .OrderByDescending(t => t.fecha_hora)
            .Select(t => new TriajeDetallePorVacunoItem
            {
                CodigoRegistro = t.codigo,
                FechaHora = t.fecha_hora,
                TipoPesoMedido = t.tipo_peso_codeNavigation.nombre,
                PesoKg = t.peso_kg,
                Observaciones = t.observaciones
            })
            .ToListAsync(cancellationToken);
    }


    // Mappers
    private static Triaje ToTriaje(triaje e) => Triaje.Rehydrate(
        id: e.id,
        codigo: e.codigo,
        fechaHora: e.fecha_hora,
        vacunoId: e.vacuno_id,
        vacunoNombre: e.vacuno?.nombre ?? string.Empty,
        tipoPesoCode: e.tipo_peso_code,
        pesoKg: e.peso_kg,
        observaciones: e.observaciones,
        estadoRegistroCode: e.estado_registro_code,
        encargadoUsuarioId: e.encargado_usuario_id,
        createdBy: e.created_by,
        updatedBy: e.updated_by,
        deletedBy: e.deleted_by,
        createdAt: e.created_at,
        updatedAt: e.updated_at,
        deletedAt: e.deleted_at,
        motivoEliminacion: e.motivo_eliminacion);

    private static triaje ToEntity(Triaje t) => new()
    {
        id = t.Id,
        codigo = t.Codigo,
        fecha_hora = t.FechaHora,
        vacuno_id = t.VacunoId,
        tipo_peso_code = t.TipoPesoCode,
        peso_kg = t.PesoKg,
        observaciones = t.Observaciones,
        estado_registro_code = t.EstadoRegistroCode,
        encargado_usuario_id = t.EncargadoUsuarioId,
        created_by = t.CreatedBy,
        updated_by = t.UpdatedBy,
        deleted_by = t.DeletedBy,
        created_at = t.CreatedAt,
        updated_at = t.UpdatedAt,
        deleted_at = t.DeletedAt,
        motivo_eliminacion = t.MotivoEliminacion
    };
}
