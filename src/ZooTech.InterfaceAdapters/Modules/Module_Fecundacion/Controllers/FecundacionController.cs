using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.CreateFecundacion;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.GetFecundacionForEdit;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.GetFecundacionOptions;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.SearchFecundacionVacunos;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.UpdateFecundacion;
using ZooTech.Domain.Module_Fecundacion.ReadModels;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Entities;
using ZooTech.InterfaceAdapters.DTOs;
using ZooTech.InterfaceAdapters.Modules.Module_Fecundacion.DTOs.Responses;
using ZooTech.InterfaceAdapters.Modules.Module_Fecundacion.Mappers;
using CreateFecundacionRequest = ZooTech.InterfaceAdapters.Modules.Module_Fecundacion.DTOs.Requests.CreateFecundacionRequest;
using DeleteFecundacionRequest = ZooTech.InterfaceAdapters.Modules.Module_Fecundacion.DTOs.DeleteFecundacionRequest;
using FecundacionListResponse = ZooTech.InterfaceAdapters.Modules.Module_Fecundacion.DTOs.FecundacionResponse;
using UpdateFecundacionRequest = ZooTech.InterfaceAdapters.Modules.Module_Fecundacion.DTOs.Requests.UpdateFecundacionRequest;

namespace ZooTech.InterfaceAdapters.Modules.Module_Fecundacion.Controllers;

[ApiController]
[Route("api/v1")]
[ApiExplorerSettings(GroupName = "public")]
public sealed class FecundacionController : ControllerBase
{
    private const string DeleteMarker = "ANULADO_FECUNDACION:";
    private const int ObservacionesMaxLength = 250;

    private readonly ICreateFecundacionInputPort _createInputPort;
    private readonly IGetFecundacionForEditInputPort _getForEditInputPort;
    private readonly IGetFecundacionOptionsInputPort _getOptionsInputPort;
    private readonly ISearchFecundacionVacunosInputPort _searchVacunosInputPort;
    private readonly IUpdateFecundacionInputPort _updateInputPort;

    public FecundacionController(
        ICreateFecundacionInputPort createInputPort,
        IGetFecundacionForEditInputPort getForEditInputPort,
        IGetFecundacionOptionsInputPort getOptionsInputPort,
        ISearchFecundacionVacunosInputPort searchVacunosInputPort,
        IUpdateFecundacionInputPort updateInputPort)
    {
        _createInputPort = createInputPort;
        _getForEditInputPort = getForEditInputPort;
        _getOptionsInputPort = getOptionsInputPort;
        _searchVacunosInputPort = searchVacunosInputPort;
        _updateInputPort = updateInputPort;
    }

    [HttpGet("fecundaciones")]
    [HttpGet("fecundacion")]
    [ProducesResponseType(typeof(GeneralResponseDTO<List<FecundacionListResponse>>), StatusCodes.Status200OK)]
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

        return Ok(GeneralResponseDTO<List<FecundacionListResponse>>.Ok(items.Select(ToListResponse).ToList()));
    }

    [HttpPost("fecundaciones")]
    [HttpPost("fecundacion")]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Registrar(
        [FromBody] CreateFecundacionRequest request,
        CancellationToken cancellationToken)
    {
        var output = await _createInputPort.HandleAsync(
            new CreateFecundacionCommand(
                request.TipoFecundacionCode,
                request.VacunoReceptorId,
                request.CeloRegistroId,
                request.FechaProcedimiento,
                request.ResponsableName,
                request.ResultadoCode,
                request.Observaciones,
                request.MachoExterno,
                request.MachoExternoNombre,
                request.VacunoDonanteId,
                null),
            cancellationToken);

        return Created(
            $"/api/v1/fecundaciones/{output.Id}",
            GeneralResponseDTO<object>.Ok(new
            {
                output.Id,
                output.Codigo,
                output.FechaProcedimiento
            }));
    }

    [HttpGet("fecundaciones/{fecundacionId:long}")]
    [HttpGet("fecundacion/{fecundacionId:long}")]
    [ProducesResponseType(typeof(GeneralResponseDTO<FecundacionEditResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetForEdit(long fecundacionId, CancellationToken cancellationToken)
    {
        var output = await _getForEditInputPort.HandleAsync(fecundacionId, cancellationToken);
        return Ok(GeneralResponseDTO<FecundacionEditResponse>.Ok(FecundacionMapper.ToResponse(output)));
    }

    [HttpGet("fecundacion/opciones")]
    [ProducesResponseType(typeof(GeneralResponseDTO<FecundacionOptionsResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOptions(CancellationToken cancellationToken)
    {
        var output = await _getOptionsInputPort.HandleAsync(cancellationToken);
        return Ok(GeneralResponseDTO<FecundacionOptionsResponse>.Ok(FecundacionMapper.ToResponse(output)));
    }

    [HttpGet("fecundacion/vacunos")]
    [ProducesResponseType(typeof(GeneralResponseDTO<IReadOnlyList<FecundacionVacunoOptionResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchVacunos(
        [FromQuery] string? sexo,
        [FromQuery(Name = "q")] string? query,
        CancellationToken cancellationToken)
    {
        var output = await _searchVacunosInputPort.HandleAsync(
            new SearchFecundacionVacunosQuery(sexo, query),
            cancellationToken);

        var response = output.Select(FecundacionMapper.ToResponse).ToList();
        return Ok(GeneralResponseDTO<IReadOnlyList<FecundacionVacunoOptionResponse>>.Ok(response));
    }

    [HttpPatch("fecundaciones/{fecundacionId:long}")]
    [HttpPut("fecundacion/{fecundacionId:long}")]
    [ProducesResponseType(typeof(GeneralResponseDTO<FecundacionUpdateResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        long fecundacionId,
        [FromBody] UpdateFecundacionRequest request,
        CancellationToken cancellationToken)
    {
        var current = await _getForEditInputPort.HandleAsync(fecundacionId, cancellationToken);
        var output = await _updateInputPort.HandleAsync(
            FecundacionMapper.ToCommand(request, current, fecundacionId),
            cancellationToken);

        return Ok(GeneralResponseDTO<FecundacionUpdateResponse>.Ok(FecundacionMapper.ToResponse(output)));
    }

    [HttpDelete("fecundaciones/{id:long}")]
    [HttpDelete("fecundacion/{id:long}")]
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

    private static FecundacionListResponse ToListResponse(FecundacionListItem item)
        => new(
            item.Id,
            item.Codigo,
            item.Tipo,
            item.VacunoReceptor,
            item.FechaProcedimiento,
            item.Responsable,
            item.Resultado,
            item.Observaciones);
}
