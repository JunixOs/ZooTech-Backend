using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.CreateVacuno;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.DeleteVacuno;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GetVacunoById;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunos;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GenerarArbolGenealogico;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.UpdateVacuno;
using ZooTech.InterfaceAdapters.DTOs;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Responses;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Mappers;

namespace ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Controllers;

[ApiController]
[Route("api/v1/vacuno")]
[ApiExplorerSettings(GroupName = "public")]
public sealed class VacunoController : ControllerBase
{
    private readonly IListarVacunosInputPort _listarInputPort;
    private readonly ICreateVacunoInputPort _createInputPort;
    private readonly IGetVacunoByIdInputPort _getByIdInputPort;
    private readonly IUpdateVacunoInputPort _updateInputPort;
    private readonly IDeleteVacunoInputPort _deleteInputPort;
    private readonly IGenerarArbolGenealogicoInputPort _generarArbolInputPort;

    public VacunoController(
        IListarVacunosInputPort listarInputPort,
        ICreateVacunoInputPort createInputPort,
        IGetVacunoByIdInputPort getByIdInputPort,
        IUpdateVacunoInputPort updateInputPort,
        IDeleteVacunoInputPort deleteInputPort,
        IGenerarArbolGenealogicoInputPort generarArbolInputPort)
    {
        _listarInputPort = listarInputPort;
        _createInputPort = createInputPort;
        _getByIdInputPort = getByIdInputPort;
        _updateInputPort = updateInputPort;
        _deleteInputPort = deleteInputPort;
        _generarArbolInputPort = generarArbolInputPort;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<List<VacunoItemResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarVacunos([FromQuery] string? query, [FromQuery] DateTime? fechaDesde, [FromQuery] DateTime? fechaHasta, [FromQuery] int page = 1, [FromQuery] int limit = 20, CancellationToken cancellationToken = default)
    {
        var command = new ListarVacunosCommand(query, fechaDesde, fechaHasta, page, limit);
        var output = await _listarInputPort.HandleAsync(command, cancellationToken);
        var response = output.Items.Select(VacunoMapper.ToResponse).ToList();
        return Ok(PagedResponse<List<VacunoItemResponse>>.OkPaged(response, page, limit, output.TotalCount));
    }

    [HttpGet("{id:long}/genealogia")]
    [ProducesResponseType(typeof(GeneralResponseDTO<IReadOnlyList<ArbolVacunoDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetArbolGenealogico([FromRoute] long id, [FromQuery] int niveles = 4, CancellationToken cancellationToken = default)
    {
        var command = new GenerarArbolGenealogicoCommand(niveles);
        var output = await _generarArbolInputPort.HandleAsync(id, command, cancellationToken);
        return Ok(GeneralResponseDTO<IReadOnlyList<ArbolVacunoDto>>.Ok(output.Arbol));
    }

    [HttpPost]
    [ProducesResponseType(typeof(GeneralResponseDTO<VacunoResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateVacunoRequest request, CancellationToken cancellationToken)
    {
        var data = VacunoMapper.ToResponse(
            await _createInputPort.HandleAsync(VacunoMapper.ToCommand(request), cancellationToken));
        return Created($"/api/v1/vacuno/{data.Id}", GeneralResponseDTO<VacunoResponse>.Ok(data));
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(GeneralResponseDTO<VacunoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] long id, CancellationToken cancellationToken)
    {
        var data = VacunoMapper.ToResponse(await _getByIdInputPort.HandleAsync(id, cancellationToken));
        return Ok(GeneralResponseDTO<VacunoResponse>.Ok(data));
    }

    [HttpPatch("{id:long}")]
    [ProducesResponseType(typeof(GeneralResponseDTO<VacunoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromRoute] long id, [FromBody] UpdateVacunoRequest request, CancellationToken cancellationToken)
    {
        var data = VacunoMapper.ToResponse(
            await _updateInputPort.HandleAsync(id, VacunoMapper.ToCommand(request), cancellationToken));
        return Ok(GeneralResponseDTO<VacunoResponse>.Ok(data));
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] long id, [FromBody] DeleteVacunoRequest request, CancellationToken cancellationToken)
    {
        await _deleteInputPort.HandleAsync(id, VacunoMapper.ToCommand(request), cancellationToken);
        return NoContent();
    }
}
