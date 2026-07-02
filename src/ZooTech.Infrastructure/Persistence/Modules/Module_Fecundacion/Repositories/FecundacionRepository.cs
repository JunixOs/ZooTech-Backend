using Microsoft.EntityFrameworkCore;
using ZooTech.Domain.Module_Fecundacion.Interfaces;
using ZooTech.Domain.Module_Fecundacion.Rules;
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

    public async Task<FecundacionEditData?> GetForEditAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.fecundacions
            .AsNoTracking()
            .Include(f => f.vacuno_receptor)
            .Include(f => f.responsable)
            .Include(f => f.fecundacion_donante)
                .ThenInclude(d => d!.vacuno_donante)
            .Include(f => f.fecundacion_donante)
                .ThenInclude(d => d!.externo_donante)
            .Include(f => f.fecundacion_inseminacion)
            .Include(f => f.fecundacion_embrion)
            .FirstOrDefaultAsync(f => f.id == id, cancellationToken);

        if (entity is null)
            return null;

        var estadoActual = await _context.vacuno_estado_fecundacion_historials
            .AsNoTracking()
            .Where(h => h.fecundacion_id == id && h.deleted_at == null)
            .OrderByDescending(h => h.fecha_actualizacion)
            .ThenByDescending(h => h.id)
            .Select(h => h.estado_fecundacion_code)
            .FirstOrDefaultAsync(cancellationToken) ?? string.Empty;

        var donante = entity.fecundacion_donante;

        return new FecundacionEditData(
            entity.id,
            entity.codigo,
            entity.tipo_fecundacion_code,
            entity.vacuno_receptor_id,
            entity.vacuno_receptor.codigo,
            entity.vacuno_receptor.nombre,
            donante?.tipo_donante ?? FecundacionRules.TipoDonanteInterno,
            donante?.vacuno_donante_id,
            donante?.vacuno_donante?.codigo,
            donante?.vacuno_donante?.nombre,
            donante?.externo_donante_id,
            donante?.externo_donante?.nombre,
            entity.fecha_procedimiento,
            entity.responsable.nombre_completo ?? string.Empty,
            entity.resultado_code,
            estadoActual,
            entity.observaciones_veterinarias,
            entity.fecundacion_inseminacion?.codigo_semen,
            entity.fecundacion_embrion?.codigo_embrion,
            entity.created_at,
            entity.updated_at);
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

        if (await HasOtherActiveFecundacionAsync(fecundacionId, values.VacunoReceptorId, cancellationToken))
            throw new ArgumentException("La hembra ya tiene otra fecundación activa pendiente o en confirmación.");
    }

    private async Task<bool> HasOtherActiveFecundacionAsync(
        long fecundacionId,
        long receptorId,
        CancellationToken cancellationToken)
    {
        var otherIds = await _context.fecundacions
            .AsNoTracking()
            .Where(f => f.id != fecundacionId && f.vacuno_receptor_id == receptorId)
            .Select(f => f.id)
            .ToListAsync(cancellationToken);

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
}
