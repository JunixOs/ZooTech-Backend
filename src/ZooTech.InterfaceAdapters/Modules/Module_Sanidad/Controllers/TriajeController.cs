using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.CreateTriaje;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.DeleteTriaje;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllTipoPesos;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllTriajes;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllVacunosSanidad;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetHistorialByVacunoId;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetHistorialGeneral;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetTriajeById;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.UpdateTriaje;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GenerateTriajesPdf;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GenerateTriajesExcel;
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
    private readonly IGetHistorialGeneralInputPort _getHistorialGeneralInputPort;
    private readonly IGenerateTriajesPdfInputPort _generatePdfInputPort;
    private readonly IGenerateTriajesExcelInputPort _generateExcelInputPort;

    public TriajeController(
        IGetAllTriajesInputPort getAllInputPort,
        IGetTriajeByIdInputPort getByIdInputPort,
        ICreateTriajeInputPort createInputPort,
        IUpdateTriajeInputPort updateInputPort,
        IDeleteTriajeInputPort deleteInputPort,
        IGetAllTipoPesosInputPort getTipoPesosInputPort,
        IGetAllVacunosSanidadInputPort getVacunosInputPort,
        IGetHistorialByVacunoIdInputPort getHistorialInputPort,
        IGetHistorialGeneralInputPort getHistorialGeneralInputPort,
        IGenerateTriajesPdfInputPort generatePdfInputPort,
        IGenerateTriajesExcelInputPort generateExcelInputPort)
    {
        _getAllInputPort = getAllInputPort;
        _getByIdInputPort = getByIdInputPort;
        _createInputPort = createInputPort;
        _updateInputPort = updateInputPort;
        _deleteInputPort = deleteInputPort;
        _getTipoPesosInputPort = getTipoPesosInputPort;
        _getVacunosInputPort = getVacunosInputPort;
        _getHistorialInputPort = getHistorialInputPort;
        _getHistorialGeneralInputPort = getHistorialGeneralInputPort;
        _generatePdfInputPort = generatePdfInputPort;
        _generateExcelInputPort = generateExcelInputPort;
    }

    [HttpGet]
    [ProducesResponseType(typeof(GeneralResponseDTO<PagedTriajeResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pagina = 1,
        [FromQuery] int tamano = 10,
        [FromQuery] string? fecha = null,
        [FromQuery] string? fechaDesde = null,
        [FromQuery] string? fechaHasta = null,
        [FromQuery] string? codigo = null,
        [FromQuery] string? nombre = null,
        [FromQuery] string? tipoPeso = null,
        [FromQuery] decimal? pesoKg = null,
        CancellationToken cancellationToken = default)
    {
        var currentPage = pagina <= 0 ? 1 : pagina;
        var currentTamano = tamano <= 0 ? 10 : Math.Min(tamano, 100);
        var query = new GetAllTriajesQuery(currentPage, currentTamano, fecha, fechaDesde, fechaHasta, codigo, nombre, tipoPeso, pesoKg); 
        var output = await _getAllInputPort.HandleAsync(query, cancellationToken);
        return Ok(GeneralResponseDTO<PagedTriajeResponse>.Ok(TriajeMapper.ToPagedResponse(output, currentPage, currentTamano)));
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

    [HttpPatch("{id:long}")]
    [ProducesResponseType(typeof(GeneralResponseDTO<TriajeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateTriajeRequest request, CancellationToken cancellationToken = default)
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

    [HttpGet("reporte/pdf")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GeneratePdf(
        [FromQuery] string? fecha,
        [FromQuery] string? fechaDesde,
        [FromQuery] string? fechaHasta,
        [FromQuery] string? codigo,
        [FromQuery] string? nombre,
        [FromQuery] string? tipoPeso,
        [FromQuery] decimal? pesoKg,
        [FromQuery] long? vacunoId,
        CancellationToken cancellationToken)
    {
        var report = await _generatePdfInputPort.HandleAsync(
            new GenerateTriajesPdfQuery(fecha, fechaDesde, fechaHasta, codigo, nombre, tipoPeso, pesoKg, vacunoId),
            cancellationToken);

        return File(report.Content, report.ContentType, report.FileName);
    }

    [HttpGet("reporte/excel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GenerateExcel(
        [FromQuery] string? fecha,
        [FromQuery] string? fechaDesde,
        [FromQuery] string? fechaHasta,
        [FromQuery] string? codigo,
        [FromQuery] string? nombre,
        [FromQuery] string? tipoPeso,
        [FromQuery] decimal? pesoKg,
        [FromQuery] long? vacunoId,
        CancellationToken cancellationToken)
    {
        var report = await _generateExcelInputPort.HandleAsync(
            new GenerateTriajesExcelQuery(fecha, fechaDesde, fechaHasta, codigo, nombre, tipoPeso, pesoKg, vacunoId),
            cancellationToken);

        return File(report.Content, report.ContentType, report.FileName);
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
        [FromQuery] string? desde = null,
        [FromQuery] string? hasta = null,
        CancellationToken cancellationToken = default)
    {
        var output = await _getHistorialGeneralInputPort.HandleAsync(desde, hasta, cancellationToken);
        return Ok(GeneralResponseDTO<object>.Ok(output.Items.Select(t => new { id = t.Id, fechaHora = t.FechaHora, tipoPesoCode = t.TipoPesoCode, pesoKg = t.PesoKg })));
    }
}
