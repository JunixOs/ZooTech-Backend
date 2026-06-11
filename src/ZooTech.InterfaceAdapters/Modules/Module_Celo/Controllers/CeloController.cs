using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Modules.Module_Celo.UseCases.EditarCelo;
using ZooTech.Application.Modules.Module_Celo.UseCases.EliminarCelo;
using ZooTech.Application.Modules.Module_Celo.UseCases.ListarCelos;
using ZooTech.Application.Modules.Module_Celo.UseCases.RegistrarCelo;
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
    private readonly IListarCelosInputPort _listarCelosInputPort;
    private readonly IRegistrarCeloInputPort _registrarCeloInputPort;
    private readonly IEditarCeloInputPort _editarCeloInputPort;
    private readonly IEliminarCeloInputPort _eliminarCeloInputPort;

    public CeloController(
        IListarCelosInputPort listarCelosInputPort,
        IRegistrarCeloInputPort registrarCeloInputPort,
        IEditarCeloInputPort editarCeloInputPort,
        IEliminarCeloInputPort eliminarCeloInputPort)
    {
        _listarCelosInputPort = listarCelosInputPort;
        _registrarCeloInputPort = registrarCeloInputPort;
        _editarCeloInputPort = editarCeloInputPort;
        _eliminarCeloInputPort = eliminarCeloInputPort;
    }

    [HttpGet]
    [ProducesResponseType(typeof(GeneralResponseDTO<List<CeloItemResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarCelos(CancellationToken cancellationToken)
    {
        var output = await _listarCelosInputPort.HandleAsync(cancellationToken);

        var response = output.Items.Select(CeloMapper.ToResponse).ToList();

        return Ok(GeneralResponseDTO<List<CeloItemResponse>>.Ok(response));
    }

    [HttpGet("vacas-en-celo")]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarVacasEnCelo(CancellationToken cancellationToken)
    {
        var output = await _listarCelosInputPort.HandleAsync(cancellationToken);

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
    [ProducesResponseType(typeof(GeneralResponseDTO<RegistrarCeloResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegistrarCelo(
        [FromBody] RegistrarCeloRequest request,
        CancellationToken cancellationToken)
    {
        var command = CeloMapper.ToCommand(request);
        var output = await _registrarCeloInputPort.HandleAsync(command, cancellationToken);
        var response = CeloMapper.ToResponse(output);

        return CreatedAtAction(
            nameof(ListarCelos),
            null,
            GeneralResponseDTO<RegistrarCeloResponse>.Ok(response));
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(GeneralResponseDTO<EditarCeloResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EditarCelo(
        long id,
        [FromBody] EditarCeloRequest request,
        CancellationToken cancellationToken)
    {
        var command = CeloMapper.ToCommand(request) with { Id = id };
        var output = await _editarCeloInputPort.HandleAsync(command, cancellationToken);
        var response = CeloMapper.ToResponse(output);

        return Ok(GeneralResponseDTO<EditarCeloResponse>.Ok(response));
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EliminarCelo(
        long id,
        [FromBody] EliminarCeloRequest request,
        CancellationToken cancellationToken)
    {
        var command = CeloMapper.ToCommand(request, id);
        await _eliminarCeloInputPort.HandleAsync(command, cancellationToken);

        return Ok(GeneralResponseDTO<object>.Ok(new { mensaje = "Celo eliminado con éxito" }));
    }
}
