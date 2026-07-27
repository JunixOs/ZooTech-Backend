using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Common.Behaviors;
using ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.GetFecundacionEstado;
using ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.UpdateFecundacionEstado;
using ZooTech.Domain.Shared.Enums;
using ZooTech.InterfaceAdapters.DTOs;
using ZooTech.InterfaceAdapters.Filters;
using ZooTech.InterfaceAdapters.Modules.Module_Celo.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Celo.DTOs.Responses;
using ZooTech.InterfaceAdapters.Modules.Module_Celo.Mappers;
using ApiErrorResponse = ZooTech.InterfaceAdapters.DTOs.Responses.ErrorResponse;

namespace ZooTech.InterfaceAdapters.Modules.Module_Celo.Controllers;

[ApiController]
[Route("api/reproduccion/fecundacion/estado")]
[ApiExplorerSettings(GroupName = "celo - fecundacion_estado")]
public sealed class FecundacionEstadoController : ControllerBase
{
    private const string UpdatedByHeaderName = "X-User-Id";

    private readonly IBehaviorDispatcher _behaviorDispatcher;

    public FecundacionEstadoController(
        IBehaviorDispatcher behaviorDispatcher
    )
    {
        _behaviorDispatcher = behaviorDispatcher;
    }

    [HttpGet("{vacunoId:long}")]
    [ServiceFilter(typeof(TenantHeaderFilter))]
    [RestrictTenantType(TenantType.Tenant)]
    [Authorize(Roles = AuthorizationRoles.Regular)]
    [Tags("Reproduccion")]
    [ProducesResponseType(typeof(GeneralResponseDTO<FecundacionEstadoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> GetEstado(
        [FromRoute] long vacunoId,
        CancellationToken cancellationToken)
    {
        var output = await _behaviorDispatcher.Send<GetFecundacionEstadoQuery , GetFecundacionEstadoOutput>(
            new GetFecundacionEstadoQuery(vacunoId),
            cancellationToken);

        return Ok(GeneralResponseDTO<FecundacionEstadoResponse>.Ok(
            FecundacionEstadoMapper.ToResponse(output)));
    }

    [HttpPut("{fecundacionId:long}")]
    [ServiceFilter(typeof(TenantHeaderFilter))]
    [RestrictTenantType(TenantType.Tenant)]
    [Authorize(Roles = AuthorizationRoles.Regular)]
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
        var output = await _behaviorDispatcher.Send<UpdateFecundacionEstadoCommand , UpdateFecundacionEstadoOutput>(
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
