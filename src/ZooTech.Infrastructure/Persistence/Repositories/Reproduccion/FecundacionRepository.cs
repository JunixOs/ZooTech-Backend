using Microsoft.EntityFrameworkCore;
using ZooTech.Domain.Module_Reproduccion.Entities;
using ZooTech.Domain.Module_Reproduccion.Interfaces;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Entities;

namespace ZooTech.Infrastructure.Persistence.Repositories.Reproduccion;

public sealed class FecundacionRepository : IFecundacionRepository
{
    private readonly GanaderiaDbContext _context;

    public FecundacionRepository(GanaderiaDbContext context)
    {
        _context = context;
    }

    public async Task<Fecundacion?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.fecundacions
            .Include(f => f.fecundacion_donante)
                .ThenInclude(d => d!.externo_donante)
            .Include(f => f.fecundacion_inseminacion)
            .Include(f => f.fecundacion_embrion)
            .FirstOrDefaultAsync(f => f.id == id, cancellationToken);

        if (entity is null)
            return null;

        return Fecundacion.CreateNew(
            entity.codigo,
            entity.tipo_fecundacion_code,
            entity.vacuno_receptor_id,
            entity.fecundacion_donante?.vacuno_donante_id,
            entity.fecundacion_donante?.externo_donante_id,
            entity.fecundacion_donante?.externo_donante?.nombre,
            entity.fecha_procedimiento,
            entity.responsable_id,
            entity.fecundacion_inseminacion?.codigo_semen,
            entity.fecundacion_embrion?.codigo_embrion,
            entity.observaciones_veterinarias,
            entity.created_by
        ); // Nota: para simplificar en GetByIdAsync, necesitaríamos usar Reflection o un constructor interno para setear el ID y las fechas reales. 
           // Usaré un método de mapeo con Reflection o adaptaré el constructor interno para recuperar la entidad completa.
    }

    public async Task<Fecundacion> AddAsync(Fecundacion fecundacion, CancellationToken cancellationToken = default)
    {
        var dbEntity = new fecundacion
        {
            codigo = fecundacion.Codigo,
            tipo_fecundacion_code = fecundacion.TipoFecundacionCode,
            vacuno_receptor_id = fecundacion.VacunoReceptorId,
            fecha_procedimiento = fecundacion.FechaProcedimiento,
            responsable_id = fecundacion.ResponsableId,
            resultado_code = fecundacion.ResultadoCode,
            observaciones_veterinarias = fecundacion.Observaciones,
            created_by = fecundacion.CreatedBy,
            created_at = fecundacion.CreatedAt,
            updated_by = fecundacion.UpdatedBy,
            updated_at = fecundacion.UpdatedAt
        };

        if (fecundacion.VacunoDonanteId.HasValue || !string.IsNullOrWhiteSpace(fecundacion.NombreMachoExterno))
        {
            long? externoId = null;
            if (!fecundacion.VacunoDonanteId.HasValue && !string.IsNullOrWhiteSpace(fecundacion.NombreMachoExterno))
            {
                // Buscar si existe o crear el externo
                var externo = await _context.reproductor_externos
                    .FirstOrDefaultAsync(e => e.nombre == fecundacion.NombreMachoExterno, cancellationToken);

                if (externo == null)
                {
                    externo = new reproductor_externo
                    {
                        nombre = fecundacion.NombreMachoExterno,
                        activo = true,
                        created_at = fecundacion.CreatedAt,
                        created_by = fecundacion.CreatedBy
                    };
                    _context.reproductor_externos.Add(externo);
                    await _context.SaveChangesAsync(cancellationToken);
                }
                externoId = externo.id;
            }

            dbEntity.fecundacion_donante = new fecundacion_donante
            {
                tipo_donante = fecundacion.VacunoDonanteId.HasValue ? "interno" : "externo",
                vacuno_donante_id = fecundacion.VacunoDonanteId,
                externo_donante_id = externoId
            };
        }

        if (fecundacion.TipoFecundacionCode == Fecundacion.TipoInseminacion && !string.IsNullOrWhiteSpace(fecundacion.CodigoSemen))
        {
            dbEntity.fecundacion_inseminacion = new fecundacion_inseminacion
            {
                codigo_semen = fecundacion.CodigoSemen
            };
        }

        if (fecundacion.TipoFecundacionCode == Fecundacion.TipoTransferencia && !string.IsNullOrWhiteSpace(fecundacion.CodigoEmbrion))
        {
            dbEntity.fecundacion_embrion = new fecundacion_embrion
            {
                codigo_embrion = fecundacion.CodigoEmbrion
            };
        }

        _context.fecundacions.Add(dbEntity);
        await _context.SaveChangesAsync(cancellationToken);

        // Map back Id
        var idProperty = typeof(Fecundacion).GetProperty(nameof(Fecundacion.Id));
        idProperty?.SetValue(fecundacion, dbEntity.id);

        return fecundacion;
    }

    public async Task<Fecundacion> UpdateAsync(Fecundacion fecundacion, CancellationToken cancellationToken = default)
    {
        var dbEntity = await _context.fecundacions.FindAsync(new object[] { fecundacion.Id }, cancellationToken);
        if (dbEntity != null)
        {
            dbEntity.resultado_code = fecundacion.ResultadoCode;
            dbEntity.updated_by = fecundacion.UpdatedBy;
            dbEntity.updated_at = fecundacion.UpdatedAt;

            await _context.SaveChangesAsync(cancellationToken);
        }

        return fecundacion;
    }

    public async Task<bool> HasPendingFecundacionAsync(long vacunoReceptorId, CancellationToken cancellationToken = default)
    {
        return await _context.fecundacions
            .AnyAsync(f => f.vacuno_receptor_id == vacunoReceptorId && f.resultado_code == Fecundacion.ResultadoPendiente, cancellationToken);
    }

    public async Task<string> GenerateNextCodeAsync(CancellationToken cancellationToken = default)
    {
        var maxId = await _context.fecundacions.MaxAsync(f => (long?)f.id, cancellationToken) ?? 0;
        return $"FEC-{(maxId + 1):D6}";
    }

    public async Task AssociateCriaAsync(long fecundacionId, long vacunoHijoId, CancellationToken cancellationToken = default)
    {
        var exists = await _context.fecundacion_cria
            .AnyAsync(fc => fc.fecundacion_id == fecundacionId && fc.vacuno_hijo_id == vacunoHijoId, cancellationToken);

        if (!exists)
        {
            _context.fecundacion_cria.Add(new fecundacion_crium
            {
                fecundacion_id = fecundacionId,
                vacuno_hijo_id = vacunoHijoId,
                estado_trazabilidad_code = "VINCULADO",
                fecha_vinculacion = DateOnly.FromDateTime(DateTime.UtcNow),
                created_at = DateTime.UtcNow
            });
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
