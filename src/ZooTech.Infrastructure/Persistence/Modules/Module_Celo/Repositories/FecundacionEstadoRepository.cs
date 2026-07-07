using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.Common;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Entities;

namespace ZooTech.Infrastructure.Persistence.Modules.Module_Celo.Repositories;

public sealed class FecundacionEstadoRepository : IFecundacionEstadoRepository
{
    private const string HembraCode = "H";
    private const string HembraName = "Hembra";
    private const string Fallida = FecundacionEstadoConstants.Fallida;
    private const string SinEstado = FecundacionEstadoConstants.SinEstado;
    private readonly GanaderiaDbContext context;
    private readonly IDateTimeProvider dateTimeProvider;

    public FecundacionEstadoRepository(
        GanaderiaDbContext context,
        IDateTimeProvider dateTimeProvider)
    {
        this.context = context;
        this.dateTimeProvider = dateTimeProvider;
    }

    public async Task<FecundacionEstadoSnapshot?> GetByVacunoIdAsync(
        long vacunoId,
        CancellationToken cancellationToken = default)
    {
        var animal = await GetAnimalInfoAsync(vacunoId, cancellationToken);
        if (animal is null)
        {
            return null;
        }

        var fecundacionId = await context.fecundacions.AsNoTracking()
            .Where(item => item.vacuno_receptor_id == vacunoId &&
                (item.observaciones_veterinarias == null ||
                 !item.observaciones_veterinarias.StartsWith("ANULADO_FECUNDACION:")))
            .OrderByDescending(item => item.fecha_procedimiento)
            .ThenByDescending(item => item.created_at)
            .ThenByDescending(item => item.id)
            .Select(item => (long?)item.id)
            .FirstOrDefaultAsync(cancellationToken);

        return fecundacionId.HasValue
            ? await BuildSnapshotAsync(animal, fecundacionId.Value, cancellationToken)
            : CreateSinEstadoSnapshot(animal);
    }

    public async Task<FecundacionEstadoSnapshot?> GetByFecundacionIdAsync(
        long fecundacionId,
        CancellationToken cancellationToken = default)
    {
        var fecundacionInfo = await context.fecundacions.AsNoTracking()
            .Where(item => item.id == fecundacionId)
            .Where(item => item.observaciones_veterinarias == null ||
                !item.observaciones_veterinarias.StartsWith("ANULADO_FECUNDACION:"))
            .Select(item => new
            {
                item.vacuno_receptor_id
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (fecundacionInfo is null)
        {
            return null;
        }

        var animal = await GetAnimalInfoAsync(fecundacionInfo.vacuno_receptor_id, cancellationToken);
        if (animal is null)
        {
            return null;
        }

        return await BuildSnapshotAsync(animal, fecundacionId, cancellationToken);
    }

    public async Task<string?> GetEstadoCodeByNameAsync(
        string estado,
        CancellationToken cancellationToken = default)
    {
        var lookupValues = FecundacionEstadoConstants.GetLookupValues(estado);

        return await context.cat_estado_fecundacion_vacunos.AsNoTracking()
            .Where(item => lookupValues.Contains(item.nombre) || lookupValues.Contains(item.code))
            .Select(item => item.code)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> HasOtherActiveFecundacionAsync(
        long vacunoId,
        long fecundacionId,
        CancellationToken cancellationToken = default)
    {
        var fecundacionIds = await context.fecundacions.AsNoTracking()
            .Where(item => item.vacuno_receptor_id == vacunoId && item.id != fecundacionId)
            .Where(item => item.observaciones_veterinarias == null ||
                !item.observaciones_veterinarias.StartsWith("ANULADO_FECUNDACION:"))
            .Select(item => item.id)
            .ToArrayAsync(cancellationToken);

        foreach (var otherFecundacionId in fecundacionIds)
        {
            var estado = await GetLatestEstadoNameByFecundacionIdAsync(
                otherFecundacionId,
                cancellationToken);

            if (FecundacionEstadoConstants.IsActive(estado))
            {
                return true;
            }
        }

        return false;
    }

    public async Task UpdateEstadoAsync(
        long fecundacionId,
        string estadoCode,
        long updatedBy,
        CancellationToken cancellationToken = default)
    {
        var fecundacion = await context.fecundacions
            .FirstAsync(item => item.id == fecundacionId, cancellationToken);

        var now = dateTimeProvider.ServerNow;
        var previous = await GetLatestEstadoNameByFecundacionIdAsync(fecundacionId, cancellationToken);

        context.vacuno_estado_fecundacion_historials.Add(new vacuno_estado_fecundacion_historial
        {
            vacuno_id = fecundacion.vacuno_receptor_id,
            fecundacion_id = fecundacion.id,
            estado_fecundacion_code = estadoCode,
            fecha_actualizacion = DateOnly.FromDateTime(now),
            created_by = updatedBy,
            created_at = now
        });

        fecundacion.updated_by = updatedBy;
        fecundacion.updated_at = now;

        await TryAddAuditAsync(
            fecundacion.id,
            previous,
            await GetEstadoNameByCodeAsync(estadoCode, cancellationToken),
            updatedBy,
            now,
            cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }

    private async Task<AnimalInfo?> GetAnimalInfoAsync(
        long vacunoId,
        CancellationToken cancellationToken)
    {
        return await (
            from animal in context.vacunos.AsNoTracking()
            join sexo in context.cat_sexos.AsNoTracking()
                on animal.sexo_code equals sexo.code into sexoJoin
            from sexo in sexoJoin.DefaultIfEmpty()
            where animal.id == vacunoId && animal.deleted_at == null
            select new AnimalInfo(
                animal.id,
                animal.codigo,
                animal.nombre,
                animal.sexo_code == HembraCode || (sexo != null && sexo.nombre == HembraName)))
            .FirstOrDefaultAsync(cancellationToken);
    }

    private async Task<FecundacionEstadoSnapshot> BuildSnapshotAsync(
        AnimalInfo animal,
        long fecundacionId,
        CancellationToken cancellationToken)
    {
        var fecundacion = await (
            from fec in context.fecundacions.AsNoTracking()
            join tipo in context.cat_tipo_fecundacions.AsNoTracking()
                on fec.tipo_fecundacion_code equals tipo.code into tipoJoin
            from tipo in tipoJoin.DefaultIfEmpty()
            join resultado in context.cat_resultado_fecundacions.AsNoTracking()
                on fec.resultado_code equals resultado.code into resultadoJoin
            from resultado in resultadoJoin.DefaultIfEmpty()
            join responsable in context.responsables.AsNoTracking()
                on fec.responsable_id equals responsable.id into responsableJoin
            from responsable in responsableJoin.DefaultIfEmpty()
            where fec.id == fecundacionId
            select new
            {
                fec.id,
                fec.codigo,
                TipoFecundacion = tipo != null ? tipo.nombre : fec.tipo_fecundacion_code,
                Responsable = responsable != null ? responsable.nombre_completo : null,
                fec.fecha_procedimiento,
                Resultado = resultado != null ? resultado.nombre : fec.resultado_code,
                Observaciones = fec.observaciones_veterinarias
            })
            .FirstAsync(cancellationToken);

        var estado = await GetLatestEstadoByFecundacionIdAsync(fecundacionId, cancellationToken);
        var estadoActual = estado?.Nombre ?? SinEstado;

        return new FecundacionEstadoSnapshot(
            animal.Id,
            fecundacion.id,
            animal.Codigo,
            animal.Nombre,
            estadoActual,
            IsDisponible(estadoActual),
            estado?.UltimaActualizacion,
            fecundacion.codigo,
            fecundacion.TipoFecundacion,
            await GetToroDonanteAsync(fecundacion.id, cancellationToken),
            fecundacion.Responsable,
            fecundacion.fecha_procedimiento,
            fecundacion.Resultado,
            fecundacion.Observaciones,
            animal.EsHembra);
    }

    private static FecundacionEstadoSnapshot CreateSinEstadoSnapshot(AnimalInfo animal)
    {
        return new FecundacionEstadoSnapshot(
            animal.Id,
            null,
            animal.Codigo,
            animal.Nombre,
            SinEstado,
            true,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            animal.EsHembra);
    }

    private async Task<string?> GetToroDonanteAsync(
        long fecundacionId,
        CancellationToken cancellationToken)
    {
        return await (
            from donante in context.fecundacion_donantes.AsNoTracking()
            join vacunoDonante in context.vacunos.AsNoTracking()
                on donante.vacuno_donante_id equals vacunoDonante.id into vacunoJoin
            from vacunoDonante in vacunoJoin.DefaultIfEmpty()
            join externoDonante in context.reproductor_externos.AsNoTracking()
                on donante.externo_donante_id equals externoDonante.id into externoJoin
            from externoDonante in externoJoin.DefaultIfEmpty()
            where donante.fecundacion_id == fecundacionId
            select vacunoDonante != null
                ? vacunoDonante.nombre
                : externoDonante != null
                    ? externoDonante.nombre
                    : null)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private async Task<(string Nombre, DateTime UltimaActualizacion)?> GetLatestEstadoByFecundacionIdAsync(
        long fecundacionId,
        CancellationToken cancellationToken)
    {
        var estado = await (
            from historial in context.vacuno_estado_fecundacion_historials.AsNoTracking()
            join catalogo in context.cat_estado_fecundacion_vacunos.AsNoTracking()
                on historial.estado_fecundacion_code equals catalogo.code
            where historial.fecundacion_id == fecundacionId && historial.deleted_at == null
            orderby historial.fecha_actualizacion descending,
                historial.created_at descending,
                historial.id descending
            select new
            {
                catalogo.nombre,
                historial.created_at
            })
            .FirstOrDefaultAsync(cancellationToken);

        return estado is null ? null : (estado.nombre, estado.created_at);
    }

    private async Task<string?> GetLatestEstadoNameByFecundacionIdAsync(
        long fecundacionId,
        CancellationToken cancellationToken)
    {
        var estado = await GetLatestEstadoByFecundacionIdAsync(fecundacionId, cancellationToken);
        return estado?.Nombre;
    }

    private async Task<string?> GetEstadoNameByCodeAsync(
        string estadoCode,
        CancellationToken cancellationToken)
    {
        return await context.cat_estado_fecundacion_vacunos.AsNoTracking()
            .Where(item => item.code == estadoCode)
            .Select(item => item.nombre)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private async Task TryAddAuditAsync(
        long fecundacionId,
        string? previousEstado,
        string? nextEstado,
        long updatedBy,
        DateTime now,
        CancellationToken cancellationToken)
    {
        var moduloCode = await context.cat_modulos.AsNoTracking()
            .Where(item => item.activo &&
                (item.code == "REPRODUCCION" ||
                 item.code == "CELO" ||
                 item.nombre.Contains("Reproduccion") ||
                 item.nombre.Contains("Reproducción")))
            .Select(item => item.code)
            .FirstOrDefaultAsync(cancellationToken);

        if (moduloCode is null)
        {
            return;
        }

        context.bitacora_auditoria.Add(new bitacora_auditorium
        {
            modulo_code = moduloCode,
            entidad = "fecundacion",
            entidad_id = fecundacionId.ToString(),
            accion = "UPDATE_ESTADO_FECUNDACION",
            datos_anteriores = JsonSerializer.Serialize(new { estadoFecundacion = previousEstado }),
            datos_nuevos = JsonSerializer.Serialize(new { estadoFecundacion = nextEstado }),
            motivo = "Actualizacion de estado de fecundacion",
            usuario_id = updatedBy,
            created_at = now
        });
    }

    private static bool IsDisponible(string estado)
    {
        return FecundacionEstadoConstants.IsDisponible(estado);
    }

    private sealed record AnimalInfo(
        long Id,
        string Codigo,
        string Nombre,
        bool EsHembra);
}
