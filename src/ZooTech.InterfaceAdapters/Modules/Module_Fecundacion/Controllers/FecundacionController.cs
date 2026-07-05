using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.CreateFecundacion;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.ListarFecundacion;
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
    private readonly IListarFecundacionInputPort _listarFecundacionInputPort;

    public FecundacionController
    (ICreateFecundacionInputPort createInputPort,
    IListarFecundacionInputPort listarFecundacionInputPort)
    {
        _createInputPort = createInputPort;
        _listarFecundacionInputPort = listarFecundacionInputPort;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<List<FecundacionItemResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarFecundacion(
    [FromQuery] string? query,
    [FromQuery] DateTime? fechaDesde,
    [FromQuery] DateTime? fechaHasta,
    [FromQuery] string? resultado,
    [FromQuery] int page = 1,
    [FromQuery] int limit = 20,
    CancellationToken cancellationToken = default)
    {
        var command = new ListarFecundacionCommand(
            Query: query,
            FechaDesde: fechaDesde,
            FechaHasta: fechaHasta,
            Resultado: resultado,
            Page: page,
            Limit: limit);

        var output = await _listarFecundacionInputPort.HandleAsync(command, cancellationToken);
        var response = output.Items.Select(FecundacionMapper.ToListItemResponse).ToList();
        return Ok(PagedResponse<List<FecundacionItemResponse>>.OkPaged(response, page, limit, output.TotalCount));
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
