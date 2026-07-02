using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Entities;
using ZooTech.InterfaceAdapters.DTOs;
using ZooTech.InterfaceAdapters.Modules.Module_Fecundacion.DTOs;
using ZooTech.Domain.Module_Fecundacion.ReadModels;

namespace ZooTech.InterfaceAdapters.Modules.Module_Fecundacion.Controllers;

[ApiController]
[Route("api/v1/fecundacion")]
[ApiExplorerSettings(GroupName = "public")]
public sealed class FecundacionController : ControllerBase
{
    private const string DeleteMarker = "ANULADO_FECUNDACION:";
    private const int ObservacionesMaxLength = 250;

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromServices] GanaderiaDbContext db,
        CancellationToken cancellationToken)
    {
        var items = await db.fecundacions
            .AsNoTracking()
            .Where(x => x.observaciones_veterinarias == null || !x.observaciones_veterinarias.StartsWith(DeleteMarker))
            .Include(x => x.tipo_fecundacion_codeNavigation)
            .Include(x => x.vacuno_receptor)
            .Include(x => x.responsable)
            .Include(x => x.resultado_codeNavigation)
            .OrderByDescending(x => x.fecha_procedimiento)
            .ThenByDescending(x => x.id)
            .Select(x => new FecundacionListItem(
                x.id,
                x.codigo,
                x.tipo_fecundacion_codeNavigation.nombre,
                x.vacuno_receptor.codigo + " - " + x.vacuno_receptor.nombre,
                x.fecha_procedimiento,
                x.responsable.nombre_completo ?? string.Empty,
                x.resultado_codeNavigation.nombre,
                x.observaciones_veterinarias))
            .ToListAsync(cancellationToken);

        return Ok(GeneralResponseDTO<List<FecundacionResponse>>.Ok(items.Select(ToResponse).ToList()));
    }

    [HttpGet("{id:long}")]
    [HttpGet("/api/v1/fecundaciones/{id:long}")]
    public async Task<IActionResult> Obtener(
        long id,
        [FromServices] GanaderiaDbContext db,
        CancellationToken cancellationToken)
    {
        var item = await db.fecundacions
            .AsNoTracking()
            .Where(x => x.id == id)
            .Where(x => x.observaciones_veterinarias == null || !x.observaciones_veterinarias.StartsWith(DeleteMarker))
            .Include(x => x.tipo_fecundacion_codeNavigation)
            .Include(x => x.vacuno_receptor)
            .Include(x => x.responsable)
            .Include(x => x.resultado_codeNavigation)
            .Select(x => new FecundacionListItem(
                x.id,
                x.codigo,
                x.tipo_fecundacion_codeNavigation.nombre,
                x.vacuno_receptor.codigo + " - " + x.vacuno_receptor.nombre,
                x.fecha_procedimiento,
                x.responsable.nombre_completo ?? string.Empty,
                x.resultado_codeNavigation.nombre,
                x.observaciones_veterinarias))
            .FirstOrDefaultAsync(cancellationToken);

        if (item is null)
            return NotFound(new { error = new { code = "NOT_FOUND", message = "El registro de fecundacion no existe." } });

        return Ok(GeneralResponseDTO<FecundacionResponse>.Ok(ToResponse(item)));
    }

    [HttpPost]
    public async Task<IActionResult> Registrar(
        [FromBody] CreateFecundacionRequest request,
        [FromServices] GanaderiaDbContext db,
        CancellationToken cancellationToken)
    {
        var errors = Validate(request);
        if (errors.Count > 0)
            return ValidationError(errors);

        var receptorExists = await db.vacunos.AnyAsync(
            x => x.id == request.VacunoReceptorId && x.deleted_at == null,
            cancellationToken);
        if (!receptorExists)
            errors.Add(new("vacunoReceptorId", "El vacuno receptor no existe o no esta activo."));

        var tipo = await db.cat_tipo_fecundacions.FirstOrDefaultAsync(
            x => x.code.ToLower() == request.TipoFecundacionCode.Trim().ToLower(),
            cancellationToken);
        if (tipo is null)
            errors.Add(new("tipoFecundacionCode", "El tipo de fecundacion no es valido."));

        var resultado = await db.cat_resultado_fecundacions.FirstOrDefaultAsync(
            x => x.code.ToLower() == request.ResultadoCode.Trim().ToLower(),
            cancellationToken);
        if (resultado is null)
            errors.Add(new("resultadoCode", "El resultado de fecundacion no es valido."));

        if (!request.MachoExterno && request.VacunoDonanteId.HasValue)
        {
            var donanteExists = await db.vacunos.AnyAsync(
                x => x.id == request.VacunoDonanteId.Value && x.deleted_at == null,
                cancellationToken);
            if (!donanteExists)
                errors.Add(new("vacunoDonanteId", "El vacuno donante no existe o no esta activo."));
        }

        if (errors.Count > 0)
            return ValidationError(errors);

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        var responsableName = request.ResponsableName.Trim();
        var responsable = await db.responsables.FirstOrDefaultAsync(
            x => x.activo && x.nombre_completo == responsableName,
            cancellationToken);

        if (responsable is null)
        {
            var tipoResponsable = await db.cat_tipo_responsables
                .Select(x => x.code)
                .FirstOrDefaultAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(tipoResponsable))
                return ValidationError([new("responsableName", "No existe un tipo de responsable configurado.")]);

            responsable = new responsable
            {
                nombre_completo = responsableName,
                tipo_responsable_code = tipoResponsable,
                activo = true,
                created_at = DateTime.UtcNow
            };
            db.responsables.Add(responsable);
            await db.SaveChangesAsync(cancellationToken);
        }

        var entity = new fecundacion
        {
            codigo = $"FEC{Guid.NewGuid():N}"[..15].ToUpperInvariant(),
            tipo_fecundacion_code = tipo!.code,
            vacuno_receptor_id = request.VacunoReceptorId,
            fecha_procedimiento = request.FechaProcedimiento,
            responsable_id = responsable.id,
            resultado_code = resultado!.code,
            observaciones_veterinarias = request.Observaciones?.Trim(),
            created_at = DateTime.UtcNow,
            updated_at = DateTime.UtcNow
        };
        db.fecundacions.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        if (request.MachoExterno)
        {
            var sexoMacho = await db.cat_sexos
                .Where(x => x.nombre.ToLower().Contains("macho"))
                .Select(x => x.code)
                .FirstOrDefaultAsync(cancellationToken);
            var externo = new reproductor_externo
            {
                nombre = request.MachoExternoNombre!.Trim(),
                sexo_code = sexoMacho,
                activo = true,
                created_at = DateTime.UtcNow
            };
            db.reproductor_externos.Add(externo);
            await db.SaveChangesAsync(cancellationToken);
            db.fecundacion_donantes.Add(new fecundacion_donante
            {
                fecundacion_id = entity.id,
                tipo_donante = "EXTERNO",
                externo_donante_id = externo.id
            });
        }
        else if (request.VacunoDonanteId.HasValue)
        {
            db.fecundacion_donantes.Add(new fecundacion_donante
            {
                fecundacion_id = entity.id,
                tipo_donante = "VACUNO",
                vacuno_donante_id = request.VacunoDonanteId
            });
        }

        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return Created($"/api/v1/fecundacion/{entity.id}", GeneralResponseDTO<object>.Ok(new { entity.id, entity.codigo }));
    }

    [HttpDelete("{id:long}")]
    [HttpDelete("/api/v1/fecundaciones/{id:long}")]
    public async Task<IActionResult> Eliminar(
        long id,
        [FromBody] DeleteFecundacionRequest? request,
        [FromServices] GanaderiaDbContext db,
        CancellationToken cancellationToken)
    {
        var entity = await db.fecundacions
            .Include(x => x.fecundacion_cria)
            .FirstOrDefaultAsync(x => x.id == id, cancellationToken);

        if (entity is null || IsDeleted(entity))
            return NotFound(new { error = new { code = "NOT_FOUND", message = "El registro de fecundacion no existe." } });

        if (entity.fecundacion_cria.Count > 0)
        {
            return BadRequest(new
            {
                error = new
                {
                    code = "FECUNDACION_WITH_TRACEABILITY",
                    message = "No se puede eliminar una fecundacion con cria registrada en la trazabilidad."
                }
            });
        }

        var now = DateTime.UtcNow;
        var reason = request?.Razon?.Trim();
        var deleteNote = BuildDeleteNote(now, reason, entity.observaciones_veterinarias);

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        entity.observaciones_veterinarias = deleteNote;
        entity.updated_at = now;

        var activeHistory = await db.vacuno_estado_fecundacion_historials
            .Where(x => x.fecundacion_id == entity.id && x.deleted_at == null)
            .ToListAsync(cancellationToken);

        foreach (var history in activeHistory)
        {
            history.deleted_at = now;
            history.motivo_eliminacion = Truncate(reason ?? "Registro de fecundacion eliminado.", ObservacionesMaxLength);
        }

        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return NoContent();
    }

    private static List<FieldError> Validate(CreateFecundacionRequest request)
    {
        var errors = new List<FieldError>();
        if (string.IsNullOrWhiteSpace(request.TipoFecundacionCode)) errors.Add(new("tipoFecundacionCode", "El tipo es obligatorio."));
        if (request.VacunoReceptorId <= 0) errors.Add(new("vacunoReceptorId", "El receptor es obligatorio."));
        if (request.MachoExterno && string.IsNullOrWhiteSpace(request.MachoExternoNombre)) errors.Add(new("vacunoDonanteId", "Ingrese el macho externo."));
        if (string.IsNullOrWhiteSpace(request.ResponsableName)) errors.Add(new("responsableName", "El responsable es obligatorio."));
        if (string.IsNullOrWhiteSpace(request.ResultadoCode)) errors.Add(new("resultadoCode", "El resultado es obligatorio."));
        if (request.Observaciones?.Length > 250) errors.Add(new("observaciones", "Las observaciones no pueden superar 250 caracteres."));
        return errors;
    }

    private static bool IsDeleted(fecundacion entity) =>
        entity.observaciones_veterinarias?.StartsWith(DeleteMarker, StringComparison.OrdinalIgnoreCase) == true;

    private static string BuildDeleteNote(DateTime date, string? reason, string? originalObservations)
    {
        var parts = new List<string> { $"{DeleteMarker} {date:yyyy-MM-dd HH:mm:ss}" };
        if (!string.IsNullOrWhiteSpace(reason))
            parts.Add($"Motivo: {reason}");
        if (!string.IsNullOrWhiteSpace(originalObservations))
            parts.Add($"Obs: {originalObservations}");

        return Truncate(string.Join(" | ", parts), ObservacionesMaxLength);
    }

    private static string Truncate(string value, int maxLength) =>
        value.Length <= maxLength ? value : value[..maxLength];

    private static FecundacionResponse ToResponse(FecundacionListItem item)
        => new(
            item.Id,
            item.Codigo,
            item.Tipo,
            item.VacunoReceptor,
            item.FechaProcedimiento,
            item.Responsable,
            item.Resultado,
            item.Observaciones);

    private IActionResult ValidationError(IEnumerable<FieldError> errors) => BadRequest(new
    {
        error = new { code = "VALIDATION_ERROR", message = "Los datos enviados no son validos.", details = errors }
    });

    private sealed record FieldError(string Field, string Message);
}
