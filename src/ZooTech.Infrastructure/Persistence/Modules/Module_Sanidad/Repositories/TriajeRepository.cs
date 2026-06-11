using Microsoft.EntityFrameworkCore;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Domain.Module_Sanidad.Entities;
using ZooTech.Domain.Module_Sanidad.Interfaces;
using ZooTech.Infrastructure.Context;
using ZooTech.Infrastructure.Persistence.Entities;

namespace ZooTech.Infrastructure.Persistence.Modules.Module_Sanidad.Repositories;

public class TriajeRepository : ITriajeRepository
{
    private readonly ZootechContext _context;
    private readonly IDateTimeProvider _dateTimeProvider;

    public TriajeRepository(ZootechContext context, IDateTimeProvider dateTimeProvider)
    {
        _context = context;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Triaje?> GetByIdAsync(long id)
    {
        var entity = await _context.Triajes
            .Include(t => t.vacuno)
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.id == id && t.deleted_at == null);

        return entity is null ? null : ToTriaje(entity);
    }

    public async Task<(IEnumerable<Triaje> Items, int Total)> GetAllAsync(
        int pagina,
        int tamano,
        string? fecha = null,
        string? codigo = null,
        string? nombre = null,
        string? tipoPeso = null,
        decimal? pesoKg = null)
    {
        var query = _context.Triajes
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

        query = query.OrderByDescending(t => t.fecha_hora);

        var total = await query.CountAsync();

        var entities = await query
            .Skip((pagina - 1) * tamano)
            .Take(tamano)
            .ToListAsync();

        return (entities.Select(ToTriaje), total);
    }

    public async Task AddAsync(Triaje triaje)
    {
        var entity = ToEntity(triaje);
        _context.Triajes.Add(entity);

        var maxRetries = 10;
        var attempt = 0;
        while (attempt < maxRetries)
        {
            try
            {
                await _context.SaveChangesAsync();
                triaje.Id = entity.id;
                return;
            }
            catch (DbUpdateException ex) when (ex.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx && sqlEx.Number == 2627 && sqlEx.Message.Contains("uq_triaje_codigo"))
            {
                attempt++;
                if (attempt >= maxRetries)
                {
                    throw new InvalidOperationException("No se pudo generar un código único de triaje tras varios intentos.");
                }

                // Generar un nuevo código e intentar de nuevo
                var newCodigo = await GenerateCodigoAsync();
                triaje.Codigo = newCodigo;
                entity.codigo = newCodigo;
            }
            catch (DbUpdateException)
            {
                throw new InvalidOperationException("No se pudo registrar el triaje. Verifique que no exista un registro con el mismo vacuno, tipo de peso y fecha.");
            }
        }
    }

    public async Task UpdateAsync(Triaje triaje)
    {
        var entity = ToEntity(triaje);
        _context.Triajes.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var entity = await _context.Triajes
            .FirstOrDefaultAsync(t => t.id == id && t.deleted_at == null);

        if (entity is null) return;

        var now = _dateTimeProvider.ServerNow;
        entity.deleted_at = now;
        entity.updated_at = now;

        await _context.SaveChangesAsync();
    }

    public async Task<string> GenerateCodigoAsync()
    {
        var codes = await _context.Triajes
            .Where(t => t.codigo.StartsWith("TRI"))
            .Select(t => t.codigo)
            .ToListAsync();

        var maxNumber = codes
            .Select(c => c.Length > 3 && int.TryParse(c[3..], out var num) ? num : 0)
            .DefaultIfEmpty(0)
            .Max();

        return $"TRI{maxNumber + 1:D3}";
    }

    // Metodo get para traer tipo peso 
    public async Task<IEnumerable<TipoPeso>> GetAllTipoPesosAsync()
    {
        return await _context.CatTipoPesos
            .Where(t => t.activo)
            .Select(t => new TipoPeso
            {
                Code = t.code,
                Nombre = t.nombre
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<VacunoOption>> GetAllVacunosAsync()
    {
        return await _context.Vacunos
            .Where(v => v.deleted_at == null)
            .Select(v => new VacunoOption
            {
                Id = v.id,
                Codigo = v.codigo,
                Nombre = v.nombre
            })
            .ToListAsync();
    }
    public async Task<IEnumerable<TriajeHistorialItem>> GetHistorialByVacunoIdAsync(long vacunoId)
    {
        return await _context.Triajes
            .AsNoTracking()
            .Where(t => t.vacuno_id == vacunoId && t.deleted_at == null)
            .OrderByDescending(t => t.fecha_hora)
            .Select(t => new TriajeHistorialItem
            {
                Id = t.id,
                FechaHora = t.fecha_hora,
                TipoPesoCode = t.tipo_peso_code,
                PesoKg = t.peso_kg
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<Triaje>> GetGeneralReportAsync(DateTime? startDate, DateTime? endDate)
    {
        var query = _context.Triajes
            .AsNoTracking()
            .Include(t => t.vacuno)
            .Where(t => t.deleted_at == null)
            .AsQueryable();

        if (startDate.HasValue)
        {
            query = query.Where(t => t.fecha_hora >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(t => t.fecha_hora <= endDate.Value);
        }

        query = query.OrderBy(t => t.fecha_hora);

        var entities = await query.ToListAsync();
        return entities.Select(ToTriaje);
    }


    // Mappers
    private static Triaje ToTriaje(triaje e) => new()
    {
        Id = e.id,
        Codigo = e.codigo,
        FechaHora = e.fecha_hora,
        VacunoId = e.vacuno_id,
        VacunoNombre = e.vacuno.nombre,
        TipoPesoCode = e.tipo_peso_code,
        PesoKg = e.peso_kg,
        Observaciones = e.observaciones,
        EstadoRegistroCode = e.estado_registro_code,
        EncargadoUsuarioId = e.encargado_usuario_id,
        CreatedBy = e.created_by,
        UpdatedBy = e.updated_by,
        DeletedBy = e.deleted_by,
        CreatedAt = e.created_at,
        UpdatedAt = e.updated_at,
        DeletedAt = e.deleted_at,
        MotivoEliminacion = e.motivo_eliminacion
    };

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