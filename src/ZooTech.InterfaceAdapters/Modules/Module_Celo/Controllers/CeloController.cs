using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Modules.Module_Celo.UseCases.CreateCelo;
using ZooTech.Application.Modules.Module_Celo.UseCases.DeleteCelo;
using ZooTech.Application.Modules.Module_Celo.UseCases.GetCelos;
using ZooTech.Application.Modules.Module_Celo.UseCases.UpdateCelo;
using ZooTech.InterfaceAdapters.DTOs;
using ZooTech.InterfaceAdapters.Modules.Module_Celo.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Celo.DTOs.Responses;
using ZooTech.InterfaceAdapters.Modules.Module_Celo.Mappers;

namespace ZooTech.InterfaceAdapters.Modules.Module_Celo.Controllers;

[ApiController]
[Route("api/v1/celo")]
[ApiExplorerSettings(GroupName = "public")]
public sealed class CeloController : ControllerBase
{
    private readonly IGetCelosInputPort _getCelosInputPort;
    private readonly ICreateCeloInputPort _createCeloInputPort;
    private readonly IUpdateCeloInputPort _updateCeloInputPort;
    private readonly IDeleteCeloInputPort _deleteCeloInputPort;

    public CeloController(
        IGetCelosInputPort getCelosInputPort,
        ICreateCeloInputPort createCeloInputPort,
        IUpdateCeloInputPort updateCeloInputPort,
        IDeleteCeloInputPort deleteCeloInputPort)
    {
        _getCelosInputPort = getCelosInputPort;
        _createCeloInputPort = createCeloInputPort;
        _updateCeloInputPort = updateCeloInputPort;
        _deleteCeloInputPort = deleteCeloInputPort;
    }

    [HttpGet]
    [ProducesResponseType(typeof(GeneralResponseDTO<List<CeloItemResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCelos(CancellationToken cancellationToken)
    {
        var output = await _getCelosInputPort.HandleAsync(cancellationToken);
        var response = output.Items.Select(CeloMapper.ToResponse).ToList();
        return Ok(GeneralResponseDTO<List<CeloItemResponse>>.Ok(response));
    }

    [HttpGet("vacas-en-celo")]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetVacasEnCelo(CancellationToken cancellationToken)
    {
        var output = await _getCelosInputPort.HandleAsync(cancellationToken);

        var response = output.Items.Select((item, index) => new
        {
            id = index + 1,
            codigo = item.CodigoVacuno,
            nombre = item.NombreVacuno,
            diasRestante = 0,
            estado = "En celo",
            vecesEnCelo = item.VecesEnCelo,
            crias = 0
        }).ToList();

        return Ok(GeneralResponseDTO<object>.Ok(response));
    }

    [HttpPost]
    [ProducesResponseType(typeof(GeneralResponseDTO<CreateCeloResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateCelo(
        [FromBody] CreateCeloRequest request,
        CancellationToken cancellationToken)
    {
        var command = CeloMapper.ToCommand(request);
        var output = await _createCeloInputPort.HandleAsync(command, cancellationToken);
        var response = CeloMapper.ToResponse(output);

        return CreatedAtAction(
            nameof(GetCelos),
            null,
            GeneralResponseDTO<CreateCeloResponse>.Ok(response));
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(GeneralResponseDTO<UpdateCeloResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCelo(
        long id,
        [FromBody] UpdateCeloRequest request,
        CancellationToken cancellationToken)
    {
        var command = CeloMapper.ToCommand(request) with { Id = id };
        var output = await _updateCeloInputPort.HandleAsync(command, cancellationToken);
        var response = CeloMapper.ToResponse(output);

        return Ok(GeneralResponseDTO<UpdateCeloResponse>.Ok(response));
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCelo(
        long id,
        [FromBody] DeleteCeloRequest request,
        CancellationToken cancellationToken)
    {
        var command = CeloMapper.ToCommand(request, id);
        await _deleteCeloInputPort.HandleAsync(command, cancellationToken);

        return Ok(GeneralResponseDTO<object>.Ok(new { mensaje = "Celo eliminado con éxito" }));
    }
}
