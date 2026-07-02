using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.CreateTriaje;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.DeleteTriaje;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllTipoPesos;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllTriajes;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllVacunosSanidad;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetHistorialByVacunoId;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetTriajeById;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.UpdateTriaje;
using ZooTech.InterfaceAdapters.DTOs;
using ZooTech.InterfaceAdapters.Modules.Module_Sanidad.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Sanidad.DTOs.Responses;
using ZooTech.InterfaceAdapters.Modules.Module_Sanidad.Mappers;

namespace ZooTech.InterfaceAdapters.Module_Sanidad.Controllers;

[ApiController]
[Route("api/v1/triaje")]
public sealed class TriajeController : ControllerBase
{
    private readonly IGetAllTriajesInputPort _getAllInputPort;
    private readonly IGetTriajeByIdInputPort _getByIdInputPort;
    private readonly ICreateTriajeInputPort _createInputPort;
    private readonly IUpdateTriajeInputPort _updateInputPort;
    private readonly IDeleteTriajeInputPort _deleteInputPort;
    private readonly IGetAllTipoPesosInputPort _getTipoPesosInputPort;
    private readonly IGetAllVacunosSanidadInputPort _getVacunosInputPort;
    private readonly IGetHistorialByVacunoIdInputPort _getHistorialInputPort;

    public TriajeController(
        IGetAllTriajesInputPort getAllInputPort,
        IGetTriajeByIdInputPort getByIdInputPort,
        ICreateTriajeInputPort createInputPort,
        IUpdateTriajeInputPort updateInputPort,
        IDeleteTriajeInputPort deleteInputPort,
        IGetAllTipoPesosInputPort getTipoPesosInputPort,
        IGetAllVacunosSanidadInputPort getVacunosInputPort,
        IGetHistorialByVacunoIdInputPort getHistorialInputPort)
    {
        _getAllInputPort = getAllInputPort;
        _getByIdInputPort = getByIdInputPort;
        _createInputPort = createInputPort;
        _updateInputPort = updateInputPort;
        _deleteInputPort = deleteInputPort;
        _getTipoPesosInputPort = getTipoPesosInputPort;
        _getVacunosInputPort = getVacunosInputPort;
        _getHistorialInputPort = getHistorialInputPort;
    }

    [HttpGet]
    [ProducesResponseType(typeof(GeneralResponseDTO<PagedTriajeResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pagina = 1,
        [FromQuery] int tamano = 10,
        [FromQuery] string? fechaInicio = null,
        [FromQuery] string? fechaFin = null,
        [FromQuery] string? codigo = null,
        [FromQuery] string? nombre = null,
        [FromQuery] string? tipoPeso = null,
        [FromQuery] decimal? pesoKg = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllTriajesQuery(pagina, tamano, fechaInicio, fechaFin, codigo, nombre, tipoPeso, pesoKg);
        var output = await _getAllInputPort.HandleAsync(query, cancellationToken);
        return Ok(GeneralResponseDTO<PagedTriajeResponse>.Ok(TriajeMapper.ToPagedResponse(output)));
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(GeneralResponseDTO<TriajeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken = default)
    {
        var output = await _getByIdInputPort.HandleAsync(id, cancellationToken);
        return Ok(GeneralResponseDTO<TriajeResponse>.Ok(TriajeMapper.ToResponse(output)));
    }

    [HttpPost]
    [ProducesResponseType(typeof(GeneralResponseDTO<TriajeResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] TriajeRequest request, CancellationToken cancellationToken = default)
    {
        var command = TriajeMapper.ToCreateCommand(request);
        var output = await _createInputPort.HandleAsync(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = output.Id },
            GeneralResponseDTO<TriajeResponse>.Ok(TriajeMapper.ToResponse(output)));
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(GeneralResponseDTO<TriajeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(long id, [FromBody] TriajeRequest request, CancellationToken cancellationToken = default)
    {
        var command = TriajeMapper.ToUpdateCommand(request);
        var output = await _updateInputPort.HandleAsync(id, command, cancellationToken);
        return Ok(GeneralResponseDTO<TriajeResponse>.Ok(TriajeMapper.ToResponse(output)));
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id, [FromBody] DeleteTriajeRequest request, CancellationToken cancellationToken = default)
    {
        await _deleteInputPort.HandleAsync(id, new DeleteTriajeCommand(request.MotivoEliminacion), cancellationToken);
        return NoContent();
    }

    [HttpGet("tipos-peso")]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTiposPeso(CancellationToken cancellationToken = default)
    {
        var output = await _getTipoPesosInputPort.HandleAsync(cancellationToken);
        return Ok(GeneralResponseDTO<object>.Ok(output.Items.Select(t => new { code = t.Code, nombre = t.Nombre })));
    }

    [HttpGet("vacunos")]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetVacunos(CancellationToken cancellationToken = default)
    {
        var output = await _getVacunosInputPort.HandleAsync(cancellationToken);
        return Ok(GeneralResponseDTO<object>.Ok(output.Items.Select(v => new { id = v.Id, codigo = v.Codigo, nombre = v.Nombre })));
    }

    [HttpGet("historial/{vacunoId:long}")]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHistorial(
        long vacunoId,
        [FromQuery] string? desde = null,
        [FromQuery] string? hasta = null,
        CancellationToken cancellationToken = default)
    {
        var output = await _getHistorialInputPort.HandleAsync(vacunoId, desde, hasta, cancellationToken);
        return Ok(GeneralResponseDTO<object>.Ok(output.Items.Select(t => new { id = t.Id, fechaHora = t.FechaHora, tipoPesoCode = t.TipoPesoCode, pesoKg = t.PesoKg })));
    }
    [HttpGet("historial-general")]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHistorialGeneral(
        [FromServices] ZooTech.Application.Modules.Module_Sanidad.UseCases.GetHistorialGeneral.IGetHistorialGeneralInputPort getHistorialGeneralInputPort,
        [FromQuery] string? desde = null,
        [FromQuery] string? hasta = null,
        CancellationToken cancellationToken = default)
    {
        var output = await getHistorialGeneralInputPort.HandleAsync(desde, hasta, cancellationToken);
        return Ok(GeneralResponseDTO<object>.Ok(output.Items.Select(t => new { id = t.Id, fechaHora = t.FechaHora, tipoPesoCode = t.TipoPesoCode, pesoKg = t.PesoKg })));
    }
}
