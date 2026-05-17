using Microsoft.EntityFrameworkCore;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Domain.Module_Sanidad.Entities;
using ZooTech.Domain.Module_Sanidad.Interfaces;
using ZooTech.Infrastructure.Common.Time;
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
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.id == id && t.deleted_at == null);

        return entity is null ? null : ToTriaje(entity);
    }

    public async Task<IEnumerable<Triaje>> GetAllAsync()
    {
        var entities = await _context.Triajes
            .AsNoTracking()
            .Where(t => t.deleted_at == null)
            .OrderByDescending(t => t.fecha_hora)
            .ToListAsync();

        return entities.Select(ToTriaje);
    }

    public async Task AddAsync(Triaje triaje)
    {
        try
        {
            var entity = ToEntity(triaje);
            _context.Triajes.Add(entity);
            await _context.SaveChangesAsync();
            triaje.Id = entity.id;
        }
        catch (DbUpdateException)
        {
            throw new InvalidOperationException("No se pudo registrar el triaje. Verifique que no exista un registro con el mismo vacuno, tipo de peso y fecha.");
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
        var lastCodigo = await _context.Triajes
            .Where(t => t.codigo.StartsWith("TRI"))
            .OrderByDescending(t => t.codigo)
            .Select(t => t.codigo)
            .FirstOrDefaultAsync();

        var nextNumber = 1;
        if (!string.IsNullOrEmpty(lastCodigo) && lastCodigo.Length > 3
            && int.TryParse(lastCodigo[3..], out var lastNumber))
        {
            nextNumber = lastNumber + 1;
        }

        return $"TRI{nextNumber:D3}";
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


    // Mappers
    private static Triaje ToTriaje(triaje e) => new()
    {
        Id = e.id,
        Codigo = e.codigo,
        FechaHora = e.fecha_hora,
        VacunoId = e.vacuno_id,
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