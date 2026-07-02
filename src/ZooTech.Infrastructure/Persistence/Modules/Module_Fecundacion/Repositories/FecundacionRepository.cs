using Microsoft.EntityFrameworkCore;
using ZooTech.Domain.Module_Fecundacion.Entities;
using ZooTech.Domain.Module_Fecundacion.Interfaces;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Entities;

namespace ZooTech.Infrastructure.Persistence.Modules.Module_Fecundacion.Repositories;

public sealed class FecundacionRepository : IFecundacionRepository
{
    private readonly GanaderiaDbContext _context;

    public FecundacionRepository(GanaderiaDbContext context)
    {
        _context = context;
    }

    public async Task<Fecundacion> AddAsync(Fecundacion fecundacion, CancellationToken cancellationToken = default)
    {
        // Crear entidad de persistencia para Fecundación
        var entity = new fecundacion
        {
            codigo = fecundacion.Codigo,
            tipo_fecundacion_code = fecundacion.TipoFecundacionCode,
            vacuno_receptor_id = fecundacion.VacunoReceptorId,
            celo_registro_id = fecundacion.CeloRegistroId,
            fecha_procedimiento = DateOnly.FromDateTime(fecundacion.FechaProcedimiento),
            responsable_id = fecundacion.ResponsableId,
            resultado_code = fecundacion.ResultadoCode,
            observaciones_veterinarias = fecundacion.ObservacionesVeterinarias,
            created_by = fecundacion.CreatedBy,
            updated_by = fecundacion.UpdatedBy,
            created_at = fecundacion.CreatedAt,
            updated_at = fecundacion.UpdatedAt
        };

        await _context.fecundacions.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        // Procesar Donante
        if (fecundacion.MachoExterno)
        {
            // Buscar o registrar reproductor externo
            var ext = await _context.reproductor_externos
                .FirstOrDefaultAsync(e => e.nombre.ToLower() == fecundacion.MachoExternoNombre!.ToLower() && e.activo, cancellationToken);
            long extId;
            if (ext != null)
            {
                extId = ext.id;
            }
            else
            {
                var sexo = await _context.cat_sexos.FirstOrDefaultAsync(s => s.code.StartsWith("M") || s.nombre.ToLower().Contains("macho"), cancellationToken) 
                           ?? await _context.cat_sexos.FirstOrDefaultAsync(cancellationToken);
                
                var newExt = new reproductor_externo
                {
                    nombre = fecundacion.MachoExternoNombre!,
                    activo = true,
                    created_at = DateTime.UtcNow,
                    sexo_code = sexo?.code
                };
                
                await _context.reproductor_externos.AddAsync(newExt, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                extId = newExt.id;
            }

            var donante = new fecundacion_donante
            {
                fecundacion_id = entity.id,
                tipo_donante = "EXTERNO",
                externo_donante_id = extId,
                vacuno_donante_id = null
            };
            await _context.fecundacion_donantes.AddAsync(donante, cancellationToken);
        }
        else
        {
            var donante = new fecundacion_donante
            {
                fecundacion_id = entity.id,
                tipo_donante = "INTERNO",
                vacuno_donante_id = fecundacion.VacunoDonanteId,
                externo_donante_id = null
            };
            await _context.fecundacion_donantes.AddAsync(donante, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);

        // Retornar entidad de dominio reconstruida con el ID asignado
        return Fecundacion.CreateNew(
            codigo: entity.codigo,
            tipoFecundacionCode: entity.tipo_fecundacion_code,
            vacunoReceptorId: entity.vacuno_receptor_id,
            celoRegistroId: entity.celo_registro_id,
            fechaProcedimiento: entity.fecha_procedimiento.ToDateTime(TimeOnly.MinValue),
            responsableId: entity.responsable_id,
            resultadoCode: entity.resultado_code,
            observacionesVeterinarias: entity.observaciones_veterinarias,
            actorUsuarioId: entity.created_by,
            utcNow: entity.created_at,
            machoExterno: fecundacion.MachoExterno,
            machoExternoNombre: fecundacion.MachoExternoNombre,
            vacunoDonanteId: fecundacion.VacunoDonanteId);
    }

    public async Task<bool> ExistsVacunoAsync(long vacunoId, CancellationToken cancellationToken = default)
    {
        return await _context.vacunos.AnyAsync(v => v.id == vacunoId, cancellationToken);
    }

    public async Task<bool> ExistsCeloAsync(long celoId, CancellationToken cancellationToken = default)
    {
        return await _context.celo_registros.AnyAsync(c => c.id == celoId, cancellationToken);
    }

    public async Task<bool> ExistsCodigoAsync(string codigo, CancellationToken cancellationToken = default)
    {
        return await _context.fecundacions.AnyAsync(f => f.codigo == codigo, cancellationToken);
    }

    public async Task<long> GetOrCreateResponsableByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var normalizedName = name.Trim();
        var existing = await _context.responsables
            .FirstOrDefaultAsync(r => r.nombre_completo != null && r.nombre_completo.ToLower() == normalizedName.ToLower(), cancellationToken);
        
        if (existing != null)
        {
            return existing.id;
        }

        // Obtener el primer tipo de responsable del catálogo para evitar violar FK
        var tipo = await _context.cat_tipo_responsables.FirstOrDefaultAsync(cancellationToken);
        var tipoCode = tipo?.code ?? "VET";

        var nuevo = new responsable
        {
            nombre_completo = normalizedName,
            tipo_responsable_code = tipoCode,
            activo = true,
            created_at = DateTime.UtcNow
        };

        await _context.responsables.AddAsync(nuevo, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        return nuevo.id;
    }
}
