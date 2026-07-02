using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.CreateFecundacion;
using ZooTech.InterfaceAdapters.DTOs;
using ZooTech.InterfaceAdapters.Modules.Module_Fecundacion.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Fecundacion.DTOs.Responses;
using ZooTech.InterfaceAdapters.Modules.Module_Fecundacion.Mappers;

namespace ZooTech.InterfaceAdapters.Modules.Module_Fecundacion.Controllers;

[ApiController]
[Route("api/v1/fecundacion")]
[ApiExplorerSettings(GroupName = "public")]
public sealed class FecundacionController : ControllerBase
{
    private readonly ICreateFecundacionInputPort _createInputPort;

    public FecundacionController(ICreateFecundacionInputPort createInputPort)
    {
        _createInputPort = createInputPort;
    }

    [HttpPost]
    [ProducesResponseType(typeof(GeneralResponseDTO<CreateFecundacionResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateFecundacion(
        [FromBody] CreateFecundacionRequest request,
        CancellationToken cancellationToken)
    {
        // En producción se resolvería el ID de usuario desde Claims, usamos 1 por defecto
        long? actorUsuarioId = 1;

        var command = FecundacionMapper.ToCommand(request, actorUsuarioId);
        var output = await _createInputPort.HandleAsync(command, cancellationToken);
        var response = FecundacionMapper.ToResponse(output);

        return StatusCode(StatusCodes.Status201Created, GeneralResponseDTO<CreateFecundacionResponse>.Ok(response));
    }
}
