using Microsoft.EntityFrameworkCore;
using ZooTech.Domain.Module_Fecundacion.Entities;
using ZooTech.Domain.Module_Fecundacion.Interfaces;
using ZooTech.Domain.Module_Fecundacion.ReadModels;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Entities;
using ZooTech.Application.Modules.Module_Fecundacion.Common;
using ZooTech.Domain.Module_Fecundacion.Rules;

namespace ZooTech.Infrastructure.Persistence.Modules.Module_Fecundacion.Repositories;

public sealed class FecundacionRepository : IFecundacionRepository
{
    private readonly GanaderiaDbContext _context;

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
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var pattern = $"%{query}%";
            q = q.Where(f =>
                EF.Functions.Like(f.codigo, pattern)
                || EF.Functions.Like(f.vacuno_receptor.nombre, pattern)
                || EF.Functions.Like(f.tipo_fecundacion_code, pattern)
                || EF.Functions.Like(f.resultado_code, pattern)
                || (f.responsable != null && EF.Functions.Like(f.responsable.nombre_completo, pattern))
                || (f.fecundacion_donante != null && (
                       (f.fecundacion_donante.vacuno_donante != null && EF.Functions.Like(f.fecundacion_donante.vacuno_donante.nombre, pattern))
                    || (f.fecundacion_donante.externo_donante != null && EF.Functions.Like(f.fecundacion_donante.externo_donante.nombre, pattern))
                   ))
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
                f.id,
                f.codigo,
                f.fecha_procedimiento,
                NombreVacunoReceptor = f.vacuno_receptor.nombre,
                Responsable = f.responsable != null ? f.responsable.nombre_completo : null,
                f.tipo_fecundacion_code,
                f.resultado_code,
                NombreDonante = f.fecundacion_donante != null
                    ? (f.fecundacion_donante.vacuno_donante != null
                        ? f.fecundacion_donante.vacuno_donante.nombre
                        : (f.fecundacion_donante.externo_donante != null
                            ? f.fecundacion_donante.externo_donante.nombre
                            : "Sin Donante Registrado"))
                    : "Sin Donante Registrado"
            })
            .ToListAsync(cancellationToken);

        var items = rows.Select(r => new FecundacionListItem(
            Id: r.id,
            Codigo: r.codigo,
            Tipo: r.tipo_fecundacion_code,
            VacunoReceptor: r.NombreVacunoReceptor,
            FechaProcedimiento: r.fecha_procedimiento,
            Responsable: r.Responsable,
            Resultado: r.resultado_code,
            Observaciones: null
        )).ToList();

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

    public async Task<FecundacionEditData?> GetForEditAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        return await _context.fecundacions
            .AsNoTracking()
            .Where(f => f.id == id && (f.observaciones_veterinarias == null || !f.observaciones_veterinarias.StartsWith("ANULADO_FECUNDACION:")))
            .Select(f => new FecundacionEditData(
                f.id,
                f.codigo,
                f.tipo_fecundacion_code,
                f.vacuno_receptor_id,
                f.vacuno_receptor.codigo,
                f.vacuno_receptor.nombre,
                f.fecundacion_donante != null ? f.fecundacion_donante.tipo_donante : "INTERNO",
                f.fecundacion_donante != null ? f.fecundacion_donante.vacuno_donante_id : null,
                f.fecundacion_donante != null && f.fecundacion_donante.vacuno_donante != null ? f.fecundacion_donante.vacuno_donante.codigo : null,
                f.fecundacion_donante != null && f.fecundacion_donante.vacuno_donante != null ? f.fecundacion_donante.vacuno_donante.nombre : null,
                f.fecundacion_donante != null ? f.fecundacion_donante.externo_donante_id : null,
                f.fecundacion_donante != null && f.fecundacion_donante.externo_donante != null ? f.fecundacion_donante.externo_donante.nombre : null,
                f.fecha_procedimiento,
                f.responsable.nombre_completo ?? string.Empty,
                f.resultado_code,
                f.vacuno_estado_fecundacion_historials
                    .Where(h => h.deleted_at == null)
                    .OrderByDescending(h => h.fecha_actualizacion)
                    .ThenByDescending(h => h.id)
                    .Select(h => h.estado_fecundacion_code)
                    .FirstOrDefault() ?? string.Empty,
                f.observaciones_veterinarias,
                f.fecundacion_inseminacion != null ? f.fecundacion_inseminacion.codigo_semen : null,
                f.fecundacion_embrion != null ? f.fecundacion_embrion.codigo_embrion : null,
                f.created_at,
                f.updated_at
            ))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<FecundacionOptionsData> GetOptionsAsync(CancellationToken cancellationToken = default)
    {
        var tipos = await _context.cat_tipo_fecundacions
            .AsNoTracking()
            .OrderBy(t => t.nombre)
            .Select(t => new FecundacionOptionData(t.code, t.nombre, t.descripcion))
            .ToListAsync(cancellationToken);

        var resultados = await _context.cat_resultado_fecundacions
            .AsNoTracking()
            .OrderBy(r => r.nombre)
            .Select(r => new FecundacionOptionData(r.code, r.nombre, r.descripcion))
            .ToListAsync(cancellationToken);

        var estados = await _context.cat_estado_fecundacion_vacunos
            .AsNoTracking()
            .OrderBy(e => e.nombre)
            .Select(e => new FecundacionOptionData(e.code, e.nombre, e.descripcion))
            .ToListAsync(cancellationToken);

        return new FecundacionOptionsData(tipos, resultados, estados);
    }

    public async Task<IReadOnlyList<FecundacionVacunoOptionData>> SearchVacunosAsync(
        string? sexo,
        string? query,
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

        if (!string.IsNullOrWhiteSpace(text))
        {
            vacunos = vacunos.Where(v =>
                v.codigo.ToLower().Contains(text) ||
                v.nombre.ToLower().Contains(text));
        }

        return await vacunos
            .OrderBy(v => v.codigo)
            .Take(20)
            .Select(v => new FecundacionVacunoOptionData(
                v.id,
                v.codigo,
                v.nombre,
                v.sexo_codeNavigation.nombre))
            .ToListAsync(cancellationToken);
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
        entity.responsable_id = responsable.id;
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
            throw new ArgumentException("El estado de fecundación indicado no existe.");
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
        var query = _context.fecundacions
            .AsNoTracking()
            .Where(f => f.vacuno_receptor_id == receptorId && 
                       (f.observaciones_veterinarias == null || !f.observaciones_veterinarias.StartsWith("ANULADO_FECUNDACION:")));

        if (fecundacionId.HasValue)
        {
            query = query.Where(f => f.id != fecundacionId.Value);
        }

        var otherIds = await query.Select(f => f.id).ToListAsync(cancellationToken);

        if (otherIds.Count == 0)
            return false;

        var latestStates = await _context.vacuno_estado_fecundacion_historials
            .AsNoTracking()
            .Where(h => h.fecundacion_id != null && otherIds.Contains(h.fecundacion_id.Value) && h.deleted_at == null)
            .GroupBy(h => h.fecundacion_id!.Value)
            .Select(g => g.OrderByDescending(h => h.fecha_actualizacion).ThenByDescending(h => h.id).First().estado_fecundacion_code)
            .ToListAsync(cancellationToken);

        return latestStates.Any(FecundacionRules.EsEstadoPendienteOConfirmacion);
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

    private static bool IsSexo(string code, string nombre, string expected)
        => Normalize(code) == expected || Normalize(nombre) == expected;

    private static string Normalize(string? value)
        => value?.Trim().ToLowerInvariant() ?? string.Empty;



    public async Task DeleteAsync(long id, string razon, CancellationToken cancellationToken = default)
    {
        var entity = await _context.fecundacions
            .Include(x => x.fecundacion_cria)
            .FirstOrDefaultAsync(x => x.id == id, cancellationToken);

        if (entity is null)
            return;

        var now = DateTime.UtcNow;
        var reason = razon.Trim();
        var deleteNote = BuildDeleteNote(now, reason, entity.observaciones_veterinarias);

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
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

        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
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
}
