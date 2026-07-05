using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ZooTech.Application.Modules.Module_Fecundacion.Common;
using ZooTech.Domain.Module_Fecundacion.Entities;
using ZooTech.Domain.Module_Fecundacion.Interfaces;
using ZooTech.Domain.Module_Fecundacion.Rules;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Entities;

namespace ZooTech.Infrastructure.Persistence.Modules.Module_Fecundacion.Repositories;

public sealed class FecundacionRepository : IFecundacionRepository, IFecundacionQueryRepository
{
    private readonly GanaderiaDbContext _context;

    public FecundacionRepository(GanaderiaDbContext context)
    {
        _context = context;
    }

    public Task<bool> ExistsVacunoAsync(long id, CancellationToken cancellationToken = default)
    {
        return _context.vacunos
            .AsNoTracking()
            .AnyAsync(v => v.id == id && v.deleted_at == null, cancellationToken);
    }

    public Task<bool> ExistsCeloAsync(long id, CancellationToken cancellationToken = default)
    {
        return _context.celo_registros
            .AsNoTracking()
            .AnyAsync(c => c.id == id && c.deleted_at == null, cancellationToken);
    }

    public async Task<long> GetOrCreateResponsableByNameAsync(
        string nombre,
        CancellationToken cancellationToken = default)
    {
        var responsable = await GetOrCreateResponsableAsync(nombre.Trim(), cancellationToken);

        if (responsable.id == 0)
            await _context.SaveChangesAsync(cancellationToken);

        return responsable.id;
    }

    public Task<bool> ExistsCodigoAsync(string codigo, CancellationToken cancellationToken = default)
    {
        return _context.fecundacions
            .AsNoTracking()
            .AnyAsync(f => f.codigo == codigo, cancellationToken);
    }

    public async Task<Fecundacion> AddAsync(
        Fecundacion fecundacion,
        CancellationToken cancellationToken = default)
    {
        var utcNow = DateTime.UtcNow;
        var entity = new fecundacion
        {
            codigo = fecundacion.Codigo.Trim(),
            tipo_fecundacion_code = fecundacion.TipoFecundacionCode.Trim(),
            vacuno_receptor_id = fecundacion.VacunoReceptorId,
            celo_registro_id = fecundacion.CeloRegistroId,
            fecha_procedimiento = DateOnly.FromDateTime(fecundacion.FechaProcedimiento),
            responsable_id = fecundacion.ResponsableId,
            resultado_code = fecundacion.ResultadoCode.Trim(),
            observaciones_veterinarias = string.IsNullOrWhiteSpace(fecundacion.ObservacionesVeterinarias)
                ? null
                : fecundacion.ObservacionesVeterinarias.Trim(),
            created_by = fecundacion.ActorUsuarioId,
            updated_by = fecundacion.ActorUsuarioId,
            created_at = utcNow,
            updated_at = utcNow
        };

        _context.fecundacions.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        if (fecundacion.MachoExterno)
        {
            var externo = await GetOrCreateReproductorExternoAsync(
                fecundacion.MachoExternoNombre!.Trim(),
                cancellationToken);

            if (externo.id == 0)
                await _context.SaveChangesAsync(cancellationToken);

            _context.fecundacion_donantes.Add(new fecundacion_donante
            {
                fecundacion_id = entity.id,
                tipo_donante = FecundacionRules.TipoDonanteExterno,
                externo_donante_id = externo.id
            });
        }
        else if (fecundacion.VacunoDonanteId.HasValue)
        {
            _context.fecundacion_donantes.Add(new fecundacion_donante
            {
                fecundacion_id = entity.id,
                tipo_donante = FecundacionRules.TipoDonanteInterno,
                vacuno_donante_id = fecundacion.VacunoDonanteId.Value
            });
        }

        await _context.SaveChangesAsync(cancellationToken);

        return new Fecundacion(
            entity.id,
            entity.codigo,
            entity.tipo_fecundacion_code,
            entity.vacuno_receptor_id,
            entity.fecha_procedimiento.ToDateTime(TimeOnly.MinValue),
            entity.responsable_id,
            entity.resultado_code,
            entity.observaciones_veterinarias,
            entity.celo_registro_id,
            entity.created_by,
            fecundacion.MachoExterno,
            fecundacion.MachoExternoNombre,
            fecundacion.VacunoDonanteId);
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

    public async Task<(List<FecundacionItemDto> Data, int Total)> GetPagedAsync(
        int page,
        int limit,
        DateOnly? fechaDesde,
        DateOnly? fechaHasta,
        string? q,
        string? tipoFecundacion,
        string? estado,
        string? responsable,
        CancellationToken cancellationToken = default)
    {
        var queryable = _context.fecundacions
            .AsNoTracking()
            .Where(x => x.observaciones_veterinarias == null || !x.observaciones_veterinarias.StartsWith("ANULADO_FECUNDACION:"));

        if (fechaDesde == null && fechaHasta == null)
        {
            var limitDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-30));
            queryable = queryable.Where(x => x.fecha_procedimiento >= limitDate);
        }
        else
        {
            if (fechaDesde.HasValue)
            {
                queryable = queryable.Where(x => x.fecha_procedimiento >= fechaDesde.Value);
            }
            if (fechaHasta.HasValue)
            {
                queryable = queryable.Where(x => x.fecha_procedimiento <= fechaHasta.Value);
            }
        }

        if (!string.IsNullOrWhiteSpace(q))
        {
            var keyword = q.Trim().ToLower();
            queryable = queryable.Where(x => x.vacuno_receptor.codigo.ToLower().Contains(keyword) || 
                                             x.vacuno_receptor.nombre.ToLower().Contains(keyword));
        }

        if (!string.IsNullOrWhiteSpace(tipoFecundacion))
        {
            var normalizedTipo = tipoFecundacion.Trim().ToLower();
            queryable = queryable.Where(x => x.tipo_fecundacion_code.ToLower() == normalizedTipo);
        }

        if (!string.IsNullOrWhiteSpace(estado))
        {
            var normalizedEstado = estado.Trim().ToLower();
            queryable = queryable.Where(x => x.resultado_code.ToLower() == normalizedEstado);
        }

        if (!string.IsNullOrWhiteSpace(responsable))
        {
            var respName = responsable.Trim().ToLower();
            queryable = queryable.Where(x => x.responsable.nombre_completo.ToLower().Contains(respName));
        }

        var total = await queryable.CountAsync(cancellationToken);

        var rawData = await queryable
            .Include(x => x.tipo_fecundacion_codeNavigation)
            .Include(x => x.vacuno_receptor)
            .Include(x => x.responsable)
            .Include(x => x.resultado_codeNavigation)
            .Include(x => x.fecundacion_donante)
                .ThenInclude(d => d!.vacuno_donante)
            .Include(x => x.fecundacion_donante)
                .ThenInclude(d => d!.externo_donante)
            .OrderByDescending(x => x.fecha_procedimiento)
            .ThenByDescending(x => x.id)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync(cancellationToken);

        var mappedData = rawData.Select(x => {
            var donanteNombre = string.Empty;
            if (x.fecundacion_donante != null)
            {
                if (string.Equals(x.fecundacion_donante.tipo_donante, FecundacionRules.TipoDonanteExterno, StringComparison.OrdinalIgnoreCase))
                {
                    donanteNombre = x.fecundacion_donante.externo_donante?.nombre ?? string.Empty;
                }
                else
                {
                    donanteNombre = x.fecundacion_donante.vacuno_donante?.nombre ?? x.fecundacion_donante.vacuno_donante?.codigo ?? string.Empty;
                }
            }

            return new FecundacionItemDto(
                x.id,
                x.codigo,
                x.fecha_procedimiento,
                $"{x.vacuno_receptor.codigo} - {x.vacuno_receptor.nombre}",
                x.tipo_fecundacion_codeNavigation.nombre,
                donanteNombre,
                x.responsable.nombre_completo ?? string.Empty,
                x.resultado_codeNavigation.nombre
            );
        }).ToList();

        return (mappedData, total);
    }

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
