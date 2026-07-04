using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.GetFecundacionEstado;
using ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.UpdateFecundacionEstado;
using ZooTech.InterfaceAdapters.DTOs;
using ZooTech.InterfaceAdapters.Modules.Module_Celo.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Celo.DTOs.Responses;
using ZooTech.InterfaceAdapters.Modules.Module_Celo.Mappers;

namespace ZooTech.InterfaceAdapters.Modules.Module_Celo.Controllers;

[ApiController]
[Route("api/reproduccion/fecundacion/estado")]
[ApiExplorerSettings(GroupName = "public")]
public sealed class FecundacionEstadoController : ControllerBase
{
    private const string UpdatedByHeaderName = "X-User-Id";
    private readonly IGetFecundacionEstadoInputPort getInputPort;
    private readonly IUpdateFecundacionEstadoInputPort updateInputPort;

    public FecundacionEstadoController(
        IGetFecundacionEstadoInputPort getInputPort,
        IUpdateFecundacionEstadoInputPort updateInputPort)
    {
        this.getInputPort = getInputPort;
        this.updateInputPort = updateInputPort;
    }

    [HttpGet("{vacunoId:long}")]
    [Tags("Reproduccion")]
    [ProducesResponseType(typeof(GeneralResponseDTO<FecundacionEstadoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> GetEstado(
        [FromRoute] long vacunoId,
        CancellationToken cancellationToken)
    {
        var output = await getInputPort.HandleAsync(
            new GetFecundacionEstadoCommand(vacunoId),
            cancellationToken);

        return Ok(GeneralResponseDTO<FecundacionEstadoResponse>.Ok(
            FecundacionEstadoMapper.ToResponse(output)));
    }

    [HttpPut("{fecundacionId:long}")]
    [Tags("Reproduccion")]
    [ProducesResponseType(typeof(GeneralResponseDTO<FecundacionEstadoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateEstado(
        [FromRoute] long fecundacionId,
        [FromBody] UpdateFecundacionEstadoRequest? request,
        CancellationToken cancellationToken)
    {
        var output = await updateInputPort.HandleAsync(
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
