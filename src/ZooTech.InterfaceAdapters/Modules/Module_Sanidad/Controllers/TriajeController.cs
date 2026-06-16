using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.CreateTriaje;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.DeleteTriaje;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllTipoPesos;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllTriajes;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllVacunosSanidad;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetDistribucionTipoPesoReport;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetHistorialByVacunoId;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetPesoPromedioReport;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetResumenReport;
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
    private readonly IGetResumenReportInputPort _getResumenInputPort;
    private readonly IGetPesoPromedioReportInputPort _getPesoPromedioInputPort;
    private readonly IGetDistribucionTipoPesoReportInputPort _getDistribucionInputPort;

    public TriajeController(
        IGetAllTriajesInputPort getAllInputPort,
        IGetTriajeByIdInputPort getByIdInputPort,
        ICreateTriajeInputPort createInputPort,
        IUpdateTriajeInputPort updateInputPort,
        IDeleteTriajeInputPort deleteInputPort,
        IGetAllTipoPesosInputPort getTipoPesosInputPort,
        IGetAllVacunosSanidadInputPort getVacunosInputPort,
        IGetHistorialByVacunoIdInputPort getHistorialInputPort,
        IGetResumenReportInputPort getResumenInputPort,
        IGetPesoPromedioReportInputPort getPesoPromedioInputPort,
        IGetDistribucionTipoPesoReportInputPort getDistribucionInputPort)
    {
        _getAllInputPort = getAllInputPort;
        _getByIdInputPort = getByIdInputPort;
        _createInputPort = createInputPort;
        _updateInputPort = updateInputPort;
        _deleteInputPort = deleteInputPort;
        _getTipoPesosInputPort = getTipoPesosInputPort;
        _getVacunosInputPort = getVacunosInputPort;
        _getHistorialInputPort = getHistorialInputPort;
        _getResumenInputPort = getResumenInputPort;
        _getPesoPromedioInputPort = getPesoPromedioInputPort;
        _getDistribucionInputPort = getDistribucionInputPort;
    }

    [HttpGet]
    [ProducesResponseType(typeof(GeneralResponseDTO<PagedTriajeResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pagina = 1,
        [FromQuery] int tamano = 10,
        [FromQuery] string? fecha = null,
        [FromQuery] string? codigo = null,
        [FromQuery] string? nombre = null,
        [FromQuery] string? tipoPeso = null,
        [FromQuery] decimal? pesoKg = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllTriajesQuery(pagina, tamano, fecha, codigo, nombre, tipoPeso, pesoKg);
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
    public async Task<IActionResult> GetHistorial(long vacunoId, CancellationToken cancellationToken = default)
    {
        var output = await _getHistorialInputPort.HandleAsync(vacunoId, cancellationToken);
        return Ok(GeneralResponseDTO<object>.Ok(output.Items.Select(t => new { id = t.Id, fechaHora = t.FechaHora, tipoPesoCode = t.TipoPesoCode, pesoKg = t.PesoKg })));
    }

    [HttpGet("reportes/resumen")]
    [ProducesResponseType(typeof(GeneralResponseDTO<ResumenSanidadResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetResumenReport(CancellationToken cancellationToken = default)
    {
        var output = await _getResumenInputPort.HandleAsync(cancellationToken);
        return Ok(GeneralResponseDTO<ResumenSanidadResponse>.Ok(new ResumenSanidadResponse(
            output.TotalTriajes,
            output.TotalVacunosConTriajes,
            output.PesoPromedioGeneral,
            output.DistribucionTipoPeso.Select(d => new TipoPesoDistribucionResponse(d.TipoPesoCode, d.Cantidad, d.PesoPromedio)).ToList().AsReadOnly())));
    }

    [HttpGet("reportes/peso-promedio")]
    [ProducesResponseType(typeof(GeneralResponseDTO<PesoPromedioResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPesoPromedioReport([FromQuery] int meses = 12, CancellationToken cancellationToken = default)
    {
        var output = await _getPesoPromedioInputPort.HandleAsync(meses, cancellationToken);
        return Ok(GeneralResponseDTO<PesoPromedioResponse>.Ok(new PesoPromedioResponse(
            output.Items.Select(i => new PesoPromedioItemResponse(i.Anio, i.Mes, i.PesoPromedio)).ToList().AsReadOnly())));
    }

    [HttpGet("reportes/distribucion-tipo-peso")]
    [ProducesResponseType(typeof(GeneralResponseDTO<DistribucionTipoPesoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDistribucionTipoPesoReport(CancellationToken cancellationToken = default)
    {
        var output = await _getDistribucionInputPort.HandleAsync(cancellationToken);
        return Ok(GeneralResponseDTO<DistribucionTipoPesoResponse>.Ok(new DistribucionTipoPesoResponse(
            output.Items.Select(i => new DistribucionItemResponse(i.TipoPesoCode, i.Cantidad, i.PesoPromedio)).ToList().AsReadOnly())));
    }
}
