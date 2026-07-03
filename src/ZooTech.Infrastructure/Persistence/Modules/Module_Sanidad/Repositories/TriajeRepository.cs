using Microsoft.EntityFrameworkCore;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Domain.Module_Sanidad.Entities;
using ZooTech.Domain.Module_Sanidad.Interfaces;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Entities;

namespace ZooTech.Infrastructure.Persistence.Modules.Module_Sanidad.Repositories;

public class TriajeRepository : ITriajeRepository
{
    private readonly GanaderiaDbContext _context;
    private readonly IDateTimeProvider _dateTimeProvider;

    public TriajeRepository(GanaderiaDbContext context, IDateTimeProvider dateTimeProvider)
    {
        _context = context;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Triaje?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.triajes
            .Include(t => t.vacuno)
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.id == id && t.deleted_at == null, cancellationToken);

        return entity is null ? null : ToTriaje(entity);
    }

    public async Task<(IEnumerable<Triaje> Items, int Total)> GetAllAsync(
        int pagina,
        int tamano,
        string? fechaInicio = null,
        string? fechaFin = null,
        string? codigo = null,
        string? nombre = null,
        string? tipoPeso = null,
        decimal? pesoKg = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.triajes
            .AsNoTracking()
            .Include(t => t.vacuno)
            .Where(t => t.deleted_at == null)
            .AsQueryable();

        if (!string.IsNullOrEmpty(codigo))
            query = query.Where(t => t.codigo.Contains(codigo));

        if (!string.IsNullOrEmpty(nombre))
            query = query.Where(t => t.vacuno.nombre.Contains(nombre));

        if (!string.IsNullOrEmpty(tipoPeso))
            query = query.Where(t => t.tipo_peso_code == tipoPeso);

        if (pesoKg.HasValue)
            query = query.Where(t => t.peso_kg == pesoKg);

        if (!string.IsNullOrEmpty(fechaInicio) && DateTime.TryParse(fechaInicio, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var parsedInicio))
            query = query.Where(t => t.fecha_hora >= parsedInicio.Date);

        if (!string.IsNullOrEmpty(fechaFin) && DateTime.TryParse(fechaFin, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var parsedFin))
            query = query.Where(t => t.fecha_hora <= parsedFin.Date.AddDays(1).AddTicks(-1));

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
            _context.triajes.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return ToTriaje(entity);
        }
        catch (DbUpdateException)
        {
            throw new InvalidOperationException("No se pudo registrar el triaje. Verifique que no exista un registro con el mismo vacuno, tipo de peso y fecha.");
        }
    }

    public async Task UpdateAsync(Triaje triaje, CancellationToken cancellationToken = default)
    {
        var entity = ToEntity(triaje);
        _context.triajes.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.triajes
            .FirstOrDefaultAsync(t => t.id == id && t.deleted_at == null, cancellationToken);

        if (entity is null) return;

        var now = _dateTimeProvider.ServerNow;
        entity.deleted_at = now;
        entity.updated_at = now;

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<string> GenerateCodigoAsync(CancellationToken cancellationToken = default)
    {
        var codigos = await _context.triajes
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

    public async Task<IEnumerable<TriajeHistorialItem>> GetHistorialByVacunoIdAsync(long vacunoId, string? desde = null, string? hasta = null, CancellationToken cancellationToken = default)
    {
        var query = _context.triajes
            .AsNoTracking()
            .Where(t => t.vacuno_id == vacunoId && t.deleted_at == null);

        if (!string.IsNullOrEmpty(desde) && DateTime.TryParse(desde, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var fechaDesde))
        {
            query = query.Where(t => t.fecha_hora >= fechaDesde.Date);
        }

        if (!string.IsNullOrEmpty(hasta) && DateTime.TryParse(hasta, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var fechaHasta))
        {
            // Set to end of day to include all records on the 'hasta' date
            query = query.Where(t => t.fecha_hora <= fechaHasta.Date.AddDays(1).AddTicks(-1));
        }

        return await query
            .OrderByDescending(t => t.fecha_hora)
            .Select(t => new TriajeHistorialItem
            {
                Id = t.id,
                FechaHora = t.fecha_hora,
                TipoPesoCode = t.tipo_peso_code,
                PesoKg = t.peso_kg
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TriajeHistorialItem>> GetHistorialGeneralAsync(string? desde = null, string? hasta = null, CancellationToken cancellationToken = default)
    {
        var query = _context.triajes
            .AsNoTracking()
            .Where(t => t.deleted_at == null);

        if (!string.IsNullOrEmpty(desde) && DateTime.TryParse(desde, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var fechaDesde))
        {
            query = query.Where(t => t.fecha_hora >= fechaDesde.Date);
        }

        if (!string.IsNullOrEmpty(hasta) && DateTime.TryParse(hasta, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var fechaHasta))
        {
            // Set to end of day to include all records on the 'hasta' date
            query = query.Where(t => t.fecha_hora <= fechaHasta.Date.AddDays(1).AddTicks(-1));
        }

        return await query
            .OrderByDescending(t => t.fecha_hora)
            .Select(t => new TriajeHistorialItem
            {
                Id = t.id,
                FechaHora = t.fecha_hora,
                TipoPesoCode = t.tipo_peso_code,
                PesoKg = t.peso_kg
            })
            .ToListAsync(cancellationToken);
    }

    // Validaciones de existencia ultra rápidas
    public async Task<bool> ExisteVacunoAsync(long vacunoId)
    {
        return await _context.vacunos.AnyAsync(v => v.id == vacunoId && v.deleted_at == null);
    }

    public async Task<bool> ExisteTipoPesoAsync(string tipoPesoCode)
    {
        return await _context.cat_tipo_pesos.AnyAsync(tp => tp.code == tipoPesoCode && tp.activo);
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