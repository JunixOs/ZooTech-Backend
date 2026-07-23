using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Common.Behaviors.Module_Celo.FecundacionEstado.GetFecundacionEstado;
using ZooTech.Application.Common.Behaviors.Module_Celo.FecundacionEstado.UpdateFecundacionEstado;
using ZooTech.Application.Common.Gateway.Identity;
using ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.GetFecundacionEstado;
using ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.UpdateFecundacionEstado;
using ZooTech.InterfaceAdapters.DTOs;
using ZooTech.InterfaceAdapters.Modules.Module_Celo.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Celo.DTOs.Responses;
using ZooTech.InterfaceAdapters.Modules.Module_Celo.Mappers;
using ApiErrorResponse = ZooTech.InterfaceAdapters.DTOs.Responses.ErrorResponse;

namespace ZooTech.InterfaceAdapters.Modules.Module_Celo.Controllers;

[ApiController]
[Route("api/v1/reproduccion/fecundacion/estado")]
[Route("api/reproduccion/fecundacion/estado")]
[ApiExplorerSettings(GroupName = "celo - fecundacion_estado")]
public sealed class FecundacionEstadoController : ControllerBase
{
    private readonly IGetFecundacionEstadoBehaviorPipelineFactory _getFecundacionEstadoBehaviorPipelineFactory;
    private readonly IUpdateFecundacionEstadoBehaviorPipelineFactory _updateFecundacionEstadoBehaviorPipelineFactory;
    private readonly ICurrentUserService _currentUserService;

    public FecundacionEstadoController(
        IGetFecundacionEstadoBehaviorPipelineFactory getFecundacionEstadoBehaviorPipelineFactory,
        IUpdateFecundacionEstadoBehaviorPipelineFactory updateFecundacionEstadoBehaviorPipelineFactory,
        ICurrentUserService currentUserService
    )
    {
        _getFecundacionEstadoBehaviorPipelineFactory = getFecundacionEstadoBehaviorPipelineFactory;
        _updateFecundacionEstadoBehaviorPipelineFactory = updateFecundacionEstadoBehaviorPipelineFactory;
        _currentUserService = currentUserService;
    }

    [HttpGet("{vacunoId:long}")]
    [Tags("Reproduccion")]
    [ProducesResponseType(typeof(GeneralResponseDTO<FecundacionEstadoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> GetEstado(
        [FromRoute] long vacunoId,
        CancellationToken cancellationToken)
    {
        var behaviorPipeline = _getFecundacionEstadoBehaviorPipelineFactory.Create();

        var output = await behaviorPipeline.Execute(
            new GetFecundacionEstadoCommand(vacunoId),
            cancellationToken);

        return Ok(GeneralResponseDTO<FecundacionEstadoResponse>.Ok(
            FecundacionEstadoMapper.ToResponse(output)));
    }

    [HttpPut("{fecundacionId:long}")]
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
                _currentUserService.UserId),
            cancellationToken);

        return Ok(GeneralResponseDTO<FecundacionEstadoResponse>.Ok(
            FecundacionEstadoMapper.ToResponse(output)));
    }

}
