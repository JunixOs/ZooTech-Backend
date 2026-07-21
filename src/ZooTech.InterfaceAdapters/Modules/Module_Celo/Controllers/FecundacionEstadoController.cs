using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Common.Behaviors.Module_Celo.FecundacionEstado.GetFecundacionEstado;
using ZooTech.Application.Common.Behaviors.Module_Celo.FecundacionEstado.UpdateFecundacionEstado;
using ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.GetFecundacionEstado;
using ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.UpdateFecundacionEstado;
using ZooTech.InterfaceAdapters.DTOs;
using ZooTech.InterfaceAdapters.Modules.Module_Celo.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Celo.DTOs.Responses;
using ZooTech.InterfaceAdapters.Modules.Module_Celo.Mappers;
using ApiErrorResponse = ZooTech.InterfaceAdapters.DTOs.Responses.ErrorResponse;

namespace ZooTech.InterfaceAdapters.Modules.Module_Celo.Controllers;

[ApiController]
[Route("api/v1")]
[ApiExplorerSettings(GroupName = "public")]
public sealed class FecundacionEstadoController : ControllerBase
{
    private const string UpdatedByHeaderName = "X-User-Id";
    private readonly IGetFecundacionEstadoBehaviorPipelineFactory _getFecundacionEstadoBehaviorPipelineFactory;
    private readonly IUpdateFecundacionEstadoBehaviorPipelineFactory _updateFecundacionEstadoBehaviorPipelineFactory;

    public FecundacionEstadoController(
        IGetFecundacionEstadoBehaviorPipelineFactory getFecundacionEstadoBehaviorPipelineFactory,
        IUpdateFecundacionEstadoBehaviorPipelineFactory updateFecundacionEstadoBehaviorPipelineFactory
    )
    {
        _getFecundacionEstadoBehaviorPipelineFactory = getFecundacionEstadoBehaviorPipelineFactory;
        _updateFecundacionEstadoBehaviorPipelineFactory = updateFecundacionEstadoBehaviorPipelineFactory;
    }

    [HttpGet("vacunos/fecundacion-estado")]
    [HttpGet("vacunos/{vacunoId:long}/fecundacion-estado")]
    [Tags("Reproduccion")]
    [ProducesResponseType(typeof(GeneralResponseDTO<FecundacionEstadoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> GetEstado(
        [FromRoute] long? vacunoId,
        [FromQuery] long? id,
        CancellationToken cancellationToken)
    {
        var resolvedVacunoId = vacunoId ?? id;
        if (!resolvedVacunoId.HasValue)
        {
            return BadRequest(ApiErrorResponse.Create(
                "VALIDATION_ERROR",
                "El id del vacuno es obligatorio.",
                null));
        }

        var behaviorPipeline = _getFecundacionEstadoBehaviorPipelineFactory.Create();

        var output = await behaviorPipeline.Execute(
            new GetFecundacionEstadoCommand(resolvedVacunoId.Value),
            cancellationToken);

        return Ok(GeneralResponseDTO<FecundacionEstadoResponse>.Ok(
            FecundacionEstadoMapper.ToResponse(output)));
    }

    [HttpPut("fecundaciones/{fecundacionId:long}/estado")]
    [Tags("Reproduccion")]
    [ProducesResponseType(typeof(GeneralResponseDTO<FecundacionEstadoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateEstado(
        [FromRoute] long fecundacionId,
        [FromBody] UpdateFecundacionEstadoRequest? request,
        CancellationToken cancellationToken)
    {
        var behaviorPipeline = _updateFecundacionEstadoBehaviorPipelineFactory.Create();

        var output = await behaviorPipeline.Execute(
            new UpdateFecundacionEstadoCommand(
                fecundacionId,
                request?.EstadoFecundacion,
                GetUpdatedByFromHeader()),
            cancellationToken);

        return Ok(GeneralResponseDTO<FecundacionEstadoResponse>.Ok(
            FecundacionEstadoMapper.ToResponse(output)));
    }

    private long? GetUpdatedByFromHeader()
    {
        if (!Request.Headers.TryGetValue(UpdatedByHeaderName, out var values))
        {
            return null;
        }

        return long.TryParse(values.FirstOrDefault(), out var updatedBy)
            ? updatedBy
            : -1;
    }
}
