using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.CreateOrdenio;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.DeleteOrdenio;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GetOrdenioById;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.ListOrdenios;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.UpdateOrdenio;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunos;
using ZooTech.InterfaceAdapters.DTOs;
using ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.DTOs.Responses;
using ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.Mappers;

namespace ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.Controllers;

[ApiController]
[Route("api/v1/produccion-leche")]
public sealed class ProduccionLecheController : ControllerBase
{
    private readonly IListarVacunosInputPort _listarVacunosInputPort;
    private readonly ICreateOrdenioInputPort _createInputPort;
    private readonly IGetOrdenioByIdInputPort _getByIdInputPort;
    private readonly IListOrdeniosInputPort _listInputPort;
    private readonly IUpdateOrdenioInputPort _updateInputPort;
    private readonly IDeleteOrdenioInputPort _deleteInputPort;

    public ProduccionLecheController(
        IListarVacunosInputPort listarVacunosInputPort,
        ICreateOrdenioInputPort createInputPort,
        IGetOrdenioByIdInputPort getByIdInputPort,
        IListOrdeniosInputPort listInputPort,
        IUpdateOrdenioInputPort updateInputPort,
        IDeleteOrdenioInputPort deleteInputPort)
    {
        _listarVacunosInputPort = listarVacunosInputPort;
        _createInputPort = createInputPort;
        _getByIdInputPort = getByIdInputPort;
        _listInputPort = listInputPort;
        _updateInputPort = updateInputPort;
        _deleteInputPort = deleteInputPort;
    }

    [HttpGet("health")]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status200OK)]
    public IActionResult Health()
    {
        return Ok(new { status = "Api funcionando de manera correcta", model = "modulo produccion leche" });
    }

    [HttpGet("vacunos")]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetVacunos(CancellationToken cancellationToken)
    {
        var output = await _listarVacunosInputPort.HandleAsync(cancellationToken);
        var data = output.Items.Select(x => new { id = x.Id, codigo = x.Codigo, nombre = x.Nombre, raza = x.RazaCode });
        return Ok(GeneralResponseDTO<object>.Ok(data));
    }

    [HttpPost]
    [ProducesResponseType(typeof(GeneralResponseDTO<OrdenioResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreateOrdenioRequest request,
        CancellationToken cancellationToken)
    {
        var data = ProduccionLecheMapper.ToResponse(
            await _createInputPort.HandleAsync(ProduccionLecheMapper.ToCommand(request), cancellationToken));
        return Created($"/api/v1/produccion-leche/{data.Id}", GeneralResponseDTO<OrdenioResponse>.Ok(data));
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(GeneralResponseDTO<OrdenioResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] long id, CancellationToken cancellationToken)
    {
        var data = ProduccionLecheMapper.ToResponse(await _getByIdInputPort.HandleAsync(id, cancellationToken));
        return Ok(GeneralResponseDTO<OrdenioResponse>.Ok(data));
    }

    [HttpGet]
    [ProducesResponseType(typeof(GeneralResponseDTO<ListOrdeniosResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        [FromQuery] long? vacunoId,
        [FromQuery] string? estadoOrdenioCode,
        [FromQuery] DateTime? fechaDesde,
        [FromQuery] DateTime? fechaHasta,
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        CancellationToken cancellationToken)
    {
        var currentPage = page ?? 1;
        var currentPageSize = pageSize ?? 20;
        var data = ProduccionLecheMapper.ToResponse(
            await _listInputPort.HandleAsync(
                new ListOrdeniosQuery(vacunoId, estadoOrdenioCode, fechaDesde, fechaHasta, currentPage, currentPageSize),
                cancellationToken),
            currentPage, currentPageSize);
        return Ok(GeneralResponseDTO<ListOrdeniosResponse>.Ok(data));
    }

    [HttpPatch("{id:long}")]
    [ProducesResponseType(typeof(GeneralResponseDTO<OrdenioResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        [FromRoute] long id,
        [FromBody] UpdateOrdenioRequest request,
        CancellationToken cancellationToken)
    {
        var data = ProduccionLecheMapper.ToResponse(
            await _updateInputPort.HandleAsync(id, ProduccionLecheMapper.ToCommand(request), cancellationToken));
        return Ok(GeneralResponseDTO<OrdenioResponse>.Ok(data));
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        [FromRoute] long id,
        [FromBody] DeleteOrdenioRequest request,
        CancellationToken cancellationToken)
    {
        await _deleteInputPort.HandleAsync(id, ProduccionLecheMapper.ToCommand(request), cancellationToken);
        return NoContent();
    }
}
