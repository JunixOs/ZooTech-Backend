using Microsoft.EntityFrameworkCore;
using ZooTech.Application.Modules.Module_Fecundacion.Exceptions;
using ZooTech.Domain.Ganaderia.Module_Fecundacion.Entities;
using ZooTech.Domain.Ganaderia.Module_Fecundacion.Interfaces;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Entities;
using ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.Common;
using ZooTech.Domain.Ganaderia.Module_Fecundacion.Rules;

namespace ZooTech.Infrastructure.Persistence.Modules.Module_Fecundacion.Repositories;

public sealed class FecundacionRepository : IFecundacionRepository
{
    private readonly GanaderiaDbContext _context;

    public FecundacionRepository(IGanaderiaDbContextFactory ganaderiaDbContextFactory)
        : this(ganaderiaDbContextFactory.CreateDbContextByTenantContext())
    {
    }

    public FecundacionRepository(GanaderiaDbContext context)
    {
        _context = context;
    }

    public async Task<(List<FecundacionListItem> Items, int TotalCount)> GetPagedAsync(
    string? query, DateTime? fechaDesde, DateTime? fechaHasta, string? resultado,
    int page, int limit, CancellationToken cancellationToken = default)
    {
        var q = _context.fecundacions
            .AsNoTracking()
            .Where(f => f.observaciones_veterinarias == null ||
                        !f.observaciones_veterinarias.StartsWith("ANULADO_FECUNDACION:"))
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var pattern = $"%{query.Trim()}%";
            q = q.Where(f =>
                EF.Functions.Like(f.codigo, pattern)
                || EF.Functions.Like(f.vacuno_receptor.nombre, pattern)
                || EF.Functions.Like(f.tipo_fecundacion_code, pattern)
                || EF.Functions.Like(f.resultado_code, pattern)
                || EF.Functions.Like(f.responsable.nombre_completo, pattern)
                || (f.fecundacion_donante != null
                    && ((f.fecundacion_donante.vacuno_donante != null
                            && EF.Functions.Like(f.fecundacion_donante.vacuno_donante.nombre, pattern))
                        || (f.fecundacion_donante.externo_donante != null
                            && EF.Functions.Like(f.fecundacion_donante.externo_donante.nombre, pattern))))
            );
        }

        if (!string.IsNullOrWhiteSpace(resultado))
        {
            var resultadoNormalizado = resultado.Trim().ToUpperInvariant();
            q = q.Where(f => f.resultado_code == resultadoNormalizado);
        }

        if (fechaDesde.HasValue)
            q = q.Where(f => f.fecha_procedimiento >= DateOnly.FromDateTime(fechaDesde.Value));

        if (fechaHasta.HasValue)
            q = q.Where(f => f.fecha_procedimiento <= DateOnly.FromDateTime(fechaHasta.Value));

        var totalCount = await q.CountAsync(cancellationToken);

        var rows = await q
            .OrderByDescending(f => f.fecha_procedimiento)
            .ThenByDescending(f => f.id)
            .Skip((page - 1) * limit)
            .Take(limit)
            .Select(f => new
            {
                Id = f.id,
                Codigo = f.codigo,
                FechaProcedimiento = f.fecha_procedimiento,
                NombreVacunoReceptor = f.vacuno_receptor.nombre,
                Responsable = f.responsable.nombre_completo,
                TipoFecundacionCode = f.tipo_fecundacion_code,
                ResultadoCode = f.resultado_code,
                TieneDonante = f.fecundacion_donante != null,
                VacunoDonanteNombre = f.fecundacion_donante != null
                    ? f.fecundacion_donante.vacuno_donante != null
                        ? f.fecundacion_donante.vacuno_donante.nombre
                        : null
                    : null,
                ExternoDonanteNombre = f.fecundacion_donante != null
                    ? f.fecundacion_donante.externo_donante != null
                        ? f.fecundacion_donante.externo_donante.nombre
                        : null
                    : null
            })
            .ToListAsync(cancellationToken);

        var items = rows.Select(r =>
        {
            string nombreDonante = "Sin Donante Registrado";
            if (r.TieneDonante)
            {
                if (!string.IsNullOrWhiteSpace(r.VacunoDonanteNombre))
                    nombreDonante = r.VacunoDonanteNombre;
                else if (!string.IsNullOrWhiteSpace(r.ExternoDonanteNombre))
                    nombreDonante = r.ExternoDonanteNombre;
            }

            return new FecundacionListItem(
                Id: r.Id,
                Codigo: r.Codigo,
                Tipo: r.TipoFecundacionCode,
                VacunoReceptor: r.NombreVacunoReceptor,
                FechaProcedimiento: r.FechaProcedimiento,
                Responsable: r.Responsable ?? string.Empty,
                Resultado: r.ResultadoCode,
                NombreDonante: nombreDonante,
                Observaciones: null
            );
        }).ToList();

        return (items, totalCount);
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
            created_by = fecundacion.ActorUsuarioId,
            updated_by = fecundacion.ActorUsuarioId,
            created_at = DateTime.UtcNow,
            updated_at = DateTime.UtcNow
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

        // Guardar técnica (Inseminación/Transferencia de embrión)
        if (FecundacionRules.EsInseminacionArtificial(fecundacion.TipoFecundacionCode) && !string.IsNullOrWhiteSpace(fecundacion.CodigoSemen))
        {
            var inseminacion = new fecundacion_inseminacion
            {
                fecundacion_id = entity.id,
                codigo_semen = fecundacion.CodigoSemen.Trim()
            };
            await _context.fecundacion_inseminacions.AddAsync(inseminacion, cancellationToken);
        }
        else if (FecundacionRules.EsTransferenciaEmbriones(fecundacion.TipoFecundacionCode) && !string.IsNullOrWhiteSpace(fecundacion.CodigoEmbrion))
        {
            var embrion = new fecundacion_embrion
            {
                fecundacion_id = entity.id,
                codigo_embrion = fecundacion.CodigoEmbrion.Trim()
            };
            await _context.fecundacion_embrions.AddAsync(embrion, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);
        await AddInitialEstadoHistorialAsync(entity, fecundacion.ActorUsuarioId, cancellationToken);

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
            vacunoDonanteId: fecundacion.VacunoDonanteId,
            codigoSemen: fecundacion.CodigoSemen,
            codigoEmbrion: fecundacion.CodigoEmbrion);
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

    public async Task<Fecundacion?> GetByCodigoAsync(string codigo, CancellationToken cancellationToken = default)
    {
        var entity = await _context.fecundacions
            .Include(f => f.vacuno_receptor)
            .Include(f => f.responsable)
            .Include(f => f.fecundacion_donante)
            .Include(f => f.fecundacion_inseminacion)
            .Include(f => f.fecundacion_embrion)
            .FirstOrDefaultAsync(x => x.codigo == codigo, cancellationToken);

        if (entity is null)
            return null;

        return new Fecundacion(
            id: entity.id,
            codigo: entity.codigo,
            tipoFecundacionCode: entity.tipo_fecundacion_code,
            vacunoReceptorId: entity.vacuno_receptor_id,
            celoRegistroId: entity.celo_registro_id,
            fechaProcedimiento: entity.fecha_procedimiento.ToDateTime(TimeOnly.MinValue),
            responsableId: entity.responsable_id,
            resultadoCode: entity.resultado_code,
            observacionesVeterinarias: entity.observaciones_veterinarias,
            actorUsuarioId: entity.created_by,
            machoExterno: entity.fecundacion_donante?.tipo_donante == FecundacionRules.TipoDonanteExterno,
            machoExternoNombre: null, // Si es necesario mapear el nombre del reproductor externo, habría que hacer el include
            vacunoDonanteId: entity.fecundacion_donante?.vacuno_donante_id,
            codigoSemen: entity.fecundacion_inseminacion?.codigo_semen,
            codigoEmbrion: entity.fecundacion_embrion?.codigo_embrion);
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

    public async Task<FecundacionEditData?> GetForEditAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        var projection = await _context.fecundacions
            .AsNoTracking()
            .Where(f => f.id == id && (f.observaciones_veterinarias == null || !f.observaciones_veterinarias.StartsWith("ANULADO_FECUNDACION:")))
            .Select(f => new FecundacionEditProjection
            {
                Id = f.id,
                Codigo = f.codigo,
                TipoFecundacionCode = f.tipo_fecundacion_code,
                VacunoReceptorId = f.vacuno_receptor_id,
                VacunoReceptorCodigo = f.vacuno_receptor.codigo,
                VacunoReceptorNombre = f.vacuno_receptor.nombre,
                TipoDonante = f.fecundacion_donante != null ? f.fecundacion_donante.tipo_donante : "INTERNO",
                VacunoDonanteId = f.fecundacion_donante != null ? f.fecundacion_donante.vacuno_donante_id : null,
                VacunoDonanteCodigo = f.fecundacion_donante != null && f.fecundacion_donante.vacuno_donante != null ? f.fecundacion_donante.vacuno_donante.codigo : null,
                VacunoDonanteNombre = f.fecundacion_donante != null && f.fecundacion_donante.vacuno_donante != null ? f.fecundacion_donante.vacuno_donante.nombre : null,
                ExternoDonanteId = f.fecundacion_donante != null ? f.fecundacion_donante.externo_donante_id : null,
                ExternoDonanteNombre = f.fecundacion_donante != null && f.fecundacion_donante.externo_donante != null ? f.fecundacion_donante.externo_donante.nombre : null,
                FechaProcedimiento = f.fecha_procedimiento,
                ResponsableNombre = f.responsable.nombre_completo ?? string.Empty,
                ResultadoCode = f.resultado_code,
                EstadoFecundacionCode = f.vacuno_estado_fecundacion_historials
                    .Where(h => h.deleted_at == null)
                    .OrderByDescending(h => h.fecha_actualizacion)
                    .ThenByDescending(h => h.id)
                    .Select(h => h.estado_fecundacion_code)
                    .FirstOrDefault() ?? string.Empty,
                ObservacionesVeterinarias = f.observaciones_veterinarias,
                CodigoSemen = f.fecundacion_inseminacion != null ? f.fecundacion_inseminacion.codigo_semen : null,
                CodigoEmbrion = f.fecundacion_embrion != null ? f.fecundacion_embrion.codigo_embrion : null,
                CreadoEn = f.created_at,
                ActualizadoEn = f.updated_at
            })
            .FirstOrDefaultAsync(cancellationToken);

        return projection is null ? null : ToEditData(projection);
    }

    public async Task<FecundacionOptionsData> GetOptionsAsync(CancellationToken cancellationToken = default)
    {
        var tipos = await _context.cat_tipo_fecundacions
            .AsNoTracking()
            .OrderBy(t => t.nombre)
            .Select(t => new FecundacionOptionProjection
            {
                Code = t.code,
                Nombre = t.nombre,
                Descripcion = t.descripcion
            })
            .ToListAsync(cancellationToken);

        var resultados = await _context.cat_resultado_fecundacions
            .AsNoTracking()
            .OrderBy(r => r.nombre)
            .Select(r => new FecundacionOptionProjection
            {
                Code = r.code,
                Nombre = r.nombre,
                Descripcion = r.descripcion
            })
            .ToListAsync(cancellationToken);

        var estados = await _context.cat_estado_fecundacion_vacunos
            .AsNoTracking()
            .OrderBy(e => e.nombre)
            .Select(e => new FecundacionOptionProjection
            {
                Code = e.code,
                Nombre = e.nombre,
                Descripcion = e.descripcion
            })
            .ToListAsync(cancellationToken);

        return new FecundacionOptionsData(
            tipos.Select(ToOptionData).ToList(),
            resultados.Select(ToOptionData).ToList(),
            estados.Select(ToOptionData).ToList());
    }

    public async Task<IReadOnlyList<FecundacionVacunoOptionData>> SearchVacunosAsync(
        string? sexo,
        string? query,
        bool soloDisponibles = false,
        long? excluirFecundacionId = null,
        CancellationToken cancellationToken = default)
    {
        var sexoFilter = Normalize(sexo);
        var text = query?.Trim().ToLowerInvariant();

        var vacunos = _context.vacunos
            .AsNoTracking()
            .Where(v => v.deleted_at == null);

        if (!string.IsNullOrWhiteSpace(sexoFilter))
        {
            vacunos = vacunos.Where(v =>
                v.sexo_code.ToLower() == sexoFilter ||
                v.sexo_codeNavigation.nombre.ToLower() == sexoFilter);
        }

        if (soloDisponibles && IsSexoFilterHembra(sexoFilter))
        {
            var activeReceptorIds = await GetActiveFecundacionReceptorIdsAsync(excluirFecundacionId, cancellationToken);
            vacunos = vacunos.Where(v => !activeReceptorIds.Contains(v.id));
        }

        if (!string.IsNullOrWhiteSpace(text))
        {
            vacunos = vacunos.Where(v =>
                v.codigo.ToLower().Contains(text) ||
                v.nombre.ToLower().Contains(text));
        }

        var rows = await vacunos
            .OrderBy(v => v.codigo)
            .Take(20)
            .Select(v => new FecundacionVacunoOptionProjection
            {
                Id = v.id,
                Codigo = v.codigo,
                Nombre = v.nombre,
                Sexo = v.sexo_codeNavigation.nombre
            })
            .ToListAsync(cancellationToken);

        return rows.Select(ToVacunoOptionData).ToList();
    }

    public async Task<FecundacionUpdateData?> UpdateAsync(
        long id,
        FecundacionUpdateValues values,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.fecundacions
            .Include(f => f.fecundacion_donante)
            .Include(f => f.fecundacion_inseminacion)
            .Include(f => f.fecundacion_embrion)
            .FirstOrDefaultAsync(f => f.id == id, cancellationToken);

        if (entity is null)
            return null;

        await ValidateCatalogsAsync(values, cancellationToken);
        await ValidateVacunosAsync(id, values, cancellationToken);

        var previousResult = entity.resultado_code;
        var responsable = await GetOrCreateResponsableAsync(values.ResponsableNombre.Trim(), cancellationToken);

        entity.tipo_fecundacion_code = values.TipoFecundacionCode.Trim();
        entity.vacuno_receptor_id = values.VacunoReceptorId;
        entity.fecha_procedimiento = values.FechaProcedimiento;
        entity.responsable = responsable;
        entity.resultado_code = values.ResultadoCode.Trim();
        entity.observaciones_veterinarias = string.IsNullOrWhiteSpace(values.ObservacionesVeterinarias)
            ? null
            : values.ObservacionesVeterinarias.Trim();
        entity.updated_at = DateTime.UtcNow;

        await UpsertDonanteAsync(entity, values, cancellationToken);
        UpsertTecnica(entity, values);
        await AddEstadoHistorialAsync(entity, values, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        var warning = FecundacionRules.EsResultadoExitoso(previousResult) &&
            FecundacionRules.EsResultadoFallido(values.ResultadoCode)
                ? "El cambio de Exitosa a Fallida impacta la trazabilidad existente."
                : null;

        return new FecundacionUpdateData(
            entity.id,
            entity.codigo,
            entity.resultado_code,
            values.EstadoFecundacionCode,
            warning);
    }

    private async Task ValidateCatalogsAsync(FecundacionUpdateValues values, CancellationToken cancellationToken)
    {
        var tipoExists = await _context.cat_tipo_fecundacions
            .AnyAsync(t => t.code == values.TipoFecundacionCode, cancellationToken);
        if (!tipoExists)
            throw new ArgumentException("El tipo de fecundación indicado no existe.");

        var resultadoExists = await _context.cat_resultado_fecundacions
            .AnyAsync(r => r.code == values.ResultadoCode, cancellationToken);
        if (!resultadoExists)
            throw new ArgumentException("El resultado de fecundación indicado no existe.");

        var estadoExists = await _context.cat_estado_fecundacion_vacunos
            .AnyAsync(e => e.code == values.EstadoFecundacionCode, cancellationToken);
        if (!estadoExists)
            throw new FecundacionInvalidEstadoException();
    }

    private async Task ValidateVacunosAsync(
        long fecundacionId,
        FecundacionUpdateValues values,
        CancellationToken cancellationToken)
    {
        var receptor = await _context.vacunos
            .AsNoTracking()
            .Include(v => v.sexo_codeNavigation)
            .FirstOrDefaultAsync(v => v.id == values.VacunoReceptorId && v.deleted_at == null, cancellationToken);

        if (receptor is null || !IsSexo(receptor.sexo_code, receptor.sexo_codeNavigation.nombre, "hembra"))
            throw new ArgumentException("El receptor debe ser una hembra activa.");

        if (string.Equals(values.TipoDonante, FecundacionRules.TipoDonanteInterno, StringComparison.OrdinalIgnoreCase))
        {
            var donante = await _context.vacunos
                .AsNoTracking()
                .Include(v => v.sexo_codeNavigation)
                .FirstOrDefaultAsync(v => v.id == values.VacunoDonanteId && v.deleted_at == null, cancellationToken);

            if (donante is null || !IsSexo(donante.sexo_code, donante.sexo_codeNavigation.nombre, "macho"))
                throw new ArgumentException("El donante interno debe ser un macho activo.");
        }

        if (await HasActiveFecundacionAsync(fecundacionId, values.VacunoReceptorId, cancellationToken))
            throw new ArgumentException("La hembra ya tiene otra fecundación activa pendiente o en confirmación.");
    }

    public async Task<bool> HasActiveFecundacionAsync(
        long? fecundacionId,
        long receptorId,
        CancellationToken cancellationToken = default)
    {
        var activeReceptorIds = await GetActiveFecundacionReceptorIdsAsync(fecundacionId, cancellationToken);
        return activeReceptorIds.Contains(receptorId);
    }

    private async Task<responsable> GetOrCreateResponsableAsync(string nombre, CancellationToken cancellationToken)
    {
        var existing = await _context.responsables
            .FirstOrDefaultAsync(r => r.activo && r.nombre_completo == nombre, cancellationToken);

        if (existing is not null)
            return existing;

        var tipoCode = await _context.cat_tipo_responsables
            .AsNoTracking()
            .OrderByDescending(t => t.code.Contains("VET"))
            .ThenBy(t => t.code)
            .Select(t => t.code)
            .FirstOrDefaultAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(tipoCode))
            throw new ArgumentException("No existe un tipo de responsable para registrar al encargado.");

        var responsable = new responsable
        {
            nombre_completo = nombre,
            tipo_responsable_code = tipoCode,
            activo = true,
            created_at = DateTime.UtcNow
        };

        _context.responsables.Add(responsable);
        return responsable;
    }

    private async Task UpsertDonanteAsync(
        fecundacion entity,
        FecundacionUpdateValues values,
        CancellationToken cancellationToken)
    {
        entity.fecundacion_donante ??= new fecundacion_donante { fecundacion_id = entity.id };

        if (string.Equals(values.TipoDonante, FecundacionRules.TipoDonanteInterno, StringComparison.OrdinalIgnoreCase))
        {
            entity.fecundacion_donante.tipo_donante = FecundacionRules.TipoDonanteInterno;
            entity.fecundacion_donante.vacuno_donante_id = values.VacunoDonanteId;
            entity.fecundacion_donante.externo_donante_id = null;
            return;
        }

        var externo = await GetOrCreateReproductorExternoAsync(values.ExternoDonanteNombre!.Trim(), cancellationToken);
        entity.fecundacion_donante.tipo_donante = FecundacionRules.TipoDonanteExterno;
        entity.fecundacion_donante.vacuno_donante_id = null;
        entity.fecundacion_donante.externo_donante_id = externo.id;
    }

    private async Task<reproductor_externo> GetOrCreateReproductorExternoAsync(
        string nombre,
        CancellationToken cancellationToken)
    {
        var existing = await _context.reproductor_externos
            .FirstOrDefaultAsync(r => r.activo && r.nombre == nombre, cancellationToken);

        if (existing is not null)
            return existing;

        var machoCode = await _context.cat_sexos
            .AsNoTracking()
            .Where(s => s.code.ToLower().Contains("macho") || s.nombre.ToLower().Contains("macho"))
            .OrderBy(s => s.code)
            .Select(s => s.code)
            .FirstOrDefaultAsync(cancellationToken);

        var externo = new reproductor_externo
        {
            nombre = nombre,
            sexo_code = machoCode,
            activo = true,
            created_at = DateTime.UtcNow
        };

        _context.reproductor_externos.Add(externo);
        return externo;
    }

    private void UpsertTecnica(fecundacion entity, FecundacionUpdateValues values)
    {
        if (FecundacionRules.EsInseminacionArtificial(values.TipoFecundacionCode))
        {
            entity.fecundacion_inseminacion ??= new fecundacion_inseminacion { fecundacion_id = entity.id };
            entity.fecundacion_inseminacion.codigo_semen = values.CodigoSemen!.Trim();
            RemoveEmbrion(entity);
            return;
        }

        if (FecundacionRules.EsTransferenciaEmbriones(values.TipoFecundacionCode))
        {
            entity.fecundacion_embrion ??= new fecundacion_embrion { fecundacion_id = entity.id };
            entity.fecundacion_embrion.codigo_embrion = values.CodigoEmbrion!.Trim();
            RemoveInseminacion(entity);
            return;
        }

        RemoveInseminacion(entity);
        RemoveEmbrion(entity);
    }

    private void RemoveInseminacion(fecundacion entity)
    {
        if (entity.fecundacion_inseminacion is null)
            return;

        _context.fecundacion_inseminacions.Remove(entity.fecundacion_inseminacion);
        entity.fecundacion_inseminacion = null;
    }

    private void RemoveEmbrion(fecundacion entity)
    {
        if (entity.fecundacion_embrion is null)
            return;

        _context.fecundacion_embrions.Remove(entity.fecundacion_embrion);
        entity.fecundacion_embrion = null;
    }

    private async Task AddEstadoHistorialAsync(
        fecundacion entity,
        FecundacionUpdateValues values,
        CancellationToken cancellationToken)
    {
        var latest = await _context.vacuno_estado_fecundacion_historials
            .Where(h => h.fecundacion_id == entity.id && h.deleted_at == null)
            .OrderByDescending(h => h.fecha_actualizacion)
            .ThenByDescending(h => h.id)
            .FirstOrDefaultAsync(cancellationToken);

        if (latest is not null &&
            latest.estado_fecundacion_code == values.EstadoFecundacionCode &&
            latest.vacuno_id == values.VacunoReceptorId)
        {
            return;
        }

        _context.vacuno_estado_fecundacion_historials.Add(new vacuno_estado_fecundacion_historial
        {
            vacuno_id = values.VacunoReceptorId,
            fecundacion_id = entity.id,
            estado_fecundacion_code = values.EstadoFecundacionCode,
            fecha_actualizacion = DateOnly.FromDateTime(DateTime.Today),
            observaciones = values.ObservacionesVeterinarias?.Trim(),
            created_at = DateTime.UtcNow
        });
    }

    private async Task AddInitialEstadoHistorialAsync(
        fecundacion entity,
        long? actorUsuarioId,
        CancellationToken cancellationToken)
    {
        var estadoInicialCode = await _context.cat_estado_fecundacion_vacunos
            .AsNoTracking()
            .Where(e =>
                e.nombre == FecundacionEstadoConstants.EnEspera ||
                e.code == "en_espera" ||
                e.code == "EN_ESPERA" ||
                e.nombre == FecundacionEstadoConstants.Pendiente ||
                e.code == "PENDIENTE" ||
                e.code == "PEND")
            .OrderByDescending(e => e.nombre == FecundacionEstadoConstants.EnEspera || e.code == "en_espera")
            .ThenByDescending(e => e.nombre == FecundacionEstadoConstants.Pendiente)
            .ThenBy(e => e.code)
            .Select(e => e.code)
            .FirstOrDefaultAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(estadoInicialCode))
            return;

        var now = DateTime.UtcNow;
        _context.vacuno_estado_fecundacion_historials.Add(new vacuno_estado_fecundacion_historial
        {
            vacuno_id = entity.vacuno_receptor_id,
            fecundacion_id = entity.id,
            estado_fecundacion_code = estadoInicialCode,
            fecha_actualizacion = DateOnly.FromDateTime(now),
            observaciones = entity.observaciones_veterinarias,
            created_by = actorUsuarioId,
            created_at = now
        });

        await _context.SaveChangesAsync(cancellationToken);
    }

    private static bool IsSexo(string code, string nombre, string expected)
        => Normalize(code) == expected || Normalize(nombre) == expected;

    private static string Normalize(string? value)
        => value?.Trim().ToLowerInvariant() ?? string.Empty;

    private static bool IsSexoFilterHembra(string sexoFilter)
        => sexoFilter is "hembra" or "h";

    private async Task<HashSet<long>> GetActiveFecundacionReceptorIdsAsync(
        long? excludedFecundacionId,
        CancellationToken cancellationToken)
    {
        var fecundaciones = _context.fecundacions
            .AsNoTracking()
            .Where(f => f.observaciones_veterinarias == null ||
                        !f.observaciones_veterinarias.StartsWith("ANULADO_FECUNDACION:"));

        if (excludedFecundacionId.HasValue)
        {
            fecundaciones = fecundaciones.Where(f => f.id != excludedFecundacionId.Value);
        }

        var candidates = await fecundaciones
            .Select(f => new ActiveFecundacionProjection
            {
                Id = f.id,
                VacunoReceptorId = f.vacuno_receptor_id,
                ResultadoCode = f.resultado_code
            })
            .ToListAsync(cancellationToken);

        if (candidates.Count == 0)
        {
            return [];
        }

        var candidateIds = candidates.Select(item => item.Id).ToArray();

        var latestStates = await _context.vacuno_estado_fecundacion_historials
            .AsNoTracking()
            .Where(h => h.fecundacion_id != null &&
                        candidateIds.Contains(h.fecundacion_id.Value) &&
                        h.deleted_at == null)
            .GroupBy(h => h.fecundacion_id!.Value)
            .Select(g => new LatestFecundacionStateProjection
            {
                FecundacionId = g.Key,
                EstadoCode = g.OrderByDescending(h => h.fecha_actualizacion)
                    .ThenByDescending(h => h.id)
                    .First()
                    .estado_fecundacion_code
            })
            .ToListAsync(cancellationToken);

        var latestStateByFecundacionId = latestStates.ToDictionary(
            item => item.FecundacionId,
            item => item.EstadoCode);

        return candidates
            .Where(item =>
                latestStateByFecundacionId.TryGetValue(item.Id, out var estadoCode)
                    ? FecundacionRules.EsEstadoPendienteOConfirmacion(estadoCode)
                    : FecundacionRules.EsEstadoPendienteOConfirmacion(item.ResultadoCode))
            .Select(item => item.VacunoReceptorId)
            .ToHashSet();
    }

    public async Task DeleteAsync(long id, string razon, CancellationToken cancellationToken = default)
    {
        var entity = await _context.fecundacions
            .FirstOrDefaultAsync(x => x.id == id, cancellationToken);

        if (entity is null)
            return;

        var now = DateTime.UtcNow;
        var reason = razon.Trim();
        var deleteNote = BuildDeleteNote(now, reason, entity.observaciones_veterinarias);

        entity.observaciones_veterinarias = deleteNote;
        entity.updated_at = now;

        var activeHistory = await _context.vacuno_estado_fecundacion_historials
            .Where(x => x.fecundacion_id == entity.id && x.deleted_at == null)
            .ToListAsync(cancellationToken);

        foreach (var history in activeHistory)
        {
            history.deleted_at = now;
            history.motivo_eliminacion = Truncate(reason, 250);
        }

    }

    private static string BuildDeleteNote(DateTime date, string reason, string? originalObservations)
    {
        var parts = new List<string> { $"ANULADO_FECUNDACION: {date:yyyy-MM-dd HH:mm:ss}" };
        if (!string.IsNullOrWhiteSpace(reason))
            parts.Add($"Motivo: {reason}");
        if (!string.IsNullOrWhiteSpace(originalObservations))
            parts.Add($"Obs: {originalObservations}");

        return Truncate(string.Join(" | ", parts), 250);
    }

    private static string Truncate(string value, int maxLength) =>
        value.Length <= maxLength ? value : value[..maxLength];

    public async Task<bool> HasCriaAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.fecundacion_cria
            .AnyAsync(x => x.fecundacion_id == id, cancellationToken);
    }

    private static FecundacionListItem ToListItem(FecundacionListProjection projection)
        => new(
            Id: projection.Id,
            Codigo: projection.Codigo,
            Tipo: projection.TipoFecundacionCode,
            VacunoReceptor: projection.NombreVacunoReceptor,
            FechaProcedimiento: projection.FechaProcedimiento,
            Responsable: projection.Responsable ?? string.Empty,
            Resultado: projection.ResultadoCode,
            NombreDonante: projection.NombreDonante,
            Observaciones: null);

    private static FecundacionEditData ToEditData(FecundacionEditProjection projection)
        => new(
            projection.Id,
            projection.Codigo,
            projection.TipoFecundacionCode,
            projection.VacunoReceptorId,
            projection.VacunoReceptorCodigo,
            projection.VacunoReceptorNombre,
            projection.TipoDonante,
            projection.VacunoDonanteId,
            projection.VacunoDonanteCodigo,
            projection.VacunoDonanteNombre,
            projection.ExternoDonanteId,
            projection.ExternoDonanteNombre,
            projection.FechaProcedimiento,
            projection.ResponsableNombre,
            projection.ResultadoCode,
            projection.EstadoFecundacionCode,
            projection.ObservacionesVeterinarias,
            projection.CodigoSemen,
            projection.CodigoEmbrion,
            projection.CreadoEn,
            projection.ActualizadoEn);

    private static FecundacionOptionData ToOptionData(FecundacionOptionProjection projection)
        => new(projection.Code, projection.Nombre, projection.Descripcion);

    private static FecundacionVacunoOptionData ToVacunoOptionData(FecundacionVacunoOptionProjection projection)
        => new(projection.Id, projection.Codigo, projection.Nombre, projection.Sexo);

    private sealed class FecundacionListProjection
    {
        public long Id { get; init; }
        public string Codigo { get; init; } = string.Empty;
        public DateOnly FechaProcedimiento { get; init; }
        public string NombreVacunoReceptor { get; init; } = string.Empty;
        public string? Responsable { get; init; }
        public string TipoFecundacionCode { get; init; } = string.Empty;
        public string ResultadoCode { get; init; } = string.Empty;
        public string NombreDonante { get; init; } = string.Empty;
    }

    private sealed class FecundacionEditProjection
    {
        public long Id { get; init; }
        public string Codigo { get; init; } = string.Empty;
        public string TipoFecundacionCode { get; init; } = string.Empty;
        public long VacunoReceptorId { get; init; }
        public string VacunoReceptorCodigo { get; init; } = string.Empty;
        public string VacunoReceptorNombre { get; init; } = string.Empty;
        public string TipoDonante { get; init; } = string.Empty;
        public long? VacunoDonanteId { get; init; }
        public string? VacunoDonanteCodigo { get; init; }
        public string? VacunoDonanteNombre { get; init; }
        public long? ExternoDonanteId { get; init; }
        public string? ExternoDonanteNombre { get; init; }
        public DateOnly FechaProcedimiento { get; init; }
        public string ResponsableNombre { get; init; } = string.Empty;
        public string ResultadoCode { get; init; } = string.Empty;
        public string EstadoFecundacionCode { get; init; } = string.Empty;
        public string? ObservacionesVeterinarias { get; init; }
        public string? CodigoSemen { get; init; }
        public string? CodigoEmbrion { get; init; }
        public DateTime CreadoEn { get; init; }
        public DateTime ActualizadoEn { get; init; }
    }

    private sealed class FecundacionOptionProjection
    {
        public string Code { get; init; } = string.Empty;
        public string Nombre { get; init; } = string.Empty;
        public string? Descripcion { get; init; }
    }

    private sealed class FecundacionVacunoOptionProjection
    {
        public long Id { get; init; }
        public string Codigo { get; init; } = string.Empty;
        public string Nombre { get; init; } = string.Empty;
        public string Sexo { get; init; } = string.Empty;
    }

    private sealed class ActiveFecundacionProjection
    {
        public long Id { get; init; }
        public long VacunoReceptorId { get; init; }
        public string ResultadoCode { get; init; } = string.Empty;
    }

    private sealed class LatestFecundacionStateProjection
    {
        public long FecundacionId { get; init; }
        public string EstadoCode { get; init; } = string.Empty;
    }
}
