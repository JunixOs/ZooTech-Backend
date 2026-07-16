using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Common.Behaviors.Module_Sanidad.CreateTriaje;
using ZooTech.Application.Common.Behaviors.Module_Sanidad.DeleteTriaje;
using ZooTech.Application.Common.Behaviors.Module_Sanidad.GetAllTipoPesos;
using ZooTech.Application.Common.Behaviors.Module_Sanidad.GetAllTriajes;
using ZooTech.Application.Common.Behaviors.Module_Sanidad.GetAllVacunosSanidad;
using ZooTech.Application.Common.Behaviors.Module_Sanidad.GetHistorialByVacunoId;
using ZooTech.Application.Common.Behaviors.Module_Sanidad.GetTriajeById;
using ZooTech.Application.Common.Behaviors.Module_Sanidad.UpdateTriaje;
using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.DeleteTriaje;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllTriajes;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetHistorialByVacunoId;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetHistorialGeneral;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetTriajeById;
using ZooTech.Domain.Shared.Enums;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GenerateTriajesPdf;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GenerateTriajesExcel;
using ZooTech.InterfaceAdapters.DTOs;

using ZooTech.InterfaceAdapters.Modules.Module_Sanidad.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Sanidad.DTOs.Responses;
using ZooTech.InterfaceAdapters.Modules.Module_Sanidad.Mappers;
using ZooTech.Application.Common.Behaviors.Module_Sanidad.GetHistorialGeneral;
using ZooTech.Application.Common.Behaviors.Module_Sanidad.GenerateTriajesPdf;
using ZooTech.Application.Common.Behaviors.Module_Sanidad.GenerateTriajesExcel;

namespace ZooTech.InterfaceAdapters.Module_Sanidad.Controllers;

[ApiController]
[Route("api/v1/triaje")]
[ApiExplorerSettings(GroupName = "public")]
public sealed class TriajeController : ControllerBase
{
    private readonly IGetAllTriajesBehaviorPipelineFactory _getAllTriajesBehaviorPipelineFactory;
    private readonly IGetTriajeByIdBehaviorPipelineFactory _getTriajeByIdBehaviorPipelineFactory;
    private readonly ICreateTriajeBehaviorPipelineFactory _createTriajeBehaviorPipelineFactory;
    private readonly IUpdateTriajeBehaviorPipelineFactory _updateTriajeBehaviorPipelineFactory;
    private readonly IDeleteTriajeBehaviorPipelineFactory _deleteTriajeBehaviorPipelineFactory;
    private readonly IGetAllTipoPesosBehaviorPipelineFactory _getAllTipoPesosBehaviorPipelineFactory;
    private readonly IGetAllVacunosSanidadBehaviorPipelineFactory _getAllVacunosSanidadBehaviorPipelineFactory;
    private readonly IGetHistorialByVacunoIdBehaviorPipelineFactory _getHistorialByVacunoIdBehaviorPipelineFactory;

    private readonly IGetHistorialGeneralBehaviorPipelineFactory _getHistorialGeneralBehaviorPipelineFactory;
    private readonly IGenerateTriajesPdfBehaviorPipelineFactory _generateTriajesPdfBehaviorPipelineFactory;
    private readonly IGenerateTriajesExcelBehaviorPipelineFactory _generateTriajesExcelBehaviorPipelineFactory;    

    public TriajeController(
        IGetAllTriajesBehaviorPipelineFactory getAllTriajesBehaviorPipelineFactory,
        IGetTriajeByIdBehaviorPipelineFactory getTriajeByIdBehaviorPipelineFactory,
        ICreateTriajeBehaviorPipelineFactory createTriajeBehaviorPipelineFactory,
        IUpdateTriajeBehaviorPipelineFactory updateTriajeBehaviorPipelineFactory,
        IDeleteTriajeBehaviorPipelineFactory deleteTriajeBehaviorPipelineFactory,
        IGetAllTipoPesosBehaviorPipelineFactory getAllTipoPesosBehaviorPipelineFactory,
        IGetAllVacunosSanidadBehaviorPipelineFactory getAllVacunosSanidadBehaviorPipelineFactory,
        IGetHistorialByVacunoIdBehaviorPipelineFactory getHistorialByVacunoIdBehaviorPipelineFactory,

        IGetHistorialGeneralBehaviorPipelineFactory getHistorialGeneralBehaviorPipelineFactory,
        IGenerateTriajesPdfBehaviorPipelineFactory generateTriajesPdfBehaviorPipelineFactory,
        IGenerateTriajesExcelBehaviorPipelineFactory generateTriajesExcelBehaviorPipelineFactory    

    )
    {
        _getAllTriajesBehaviorPipelineFactory = getAllTriajesBehaviorPipelineFactory;
        _getTriajeByIdBehaviorPipelineFactory = getTriajeByIdBehaviorPipelineFactory;
        _createTriajeBehaviorPipelineFactory = createTriajeBehaviorPipelineFactory;
        _updateTriajeBehaviorPipelineFactory = updateTriajeBehaviorPipelineFactory;
        _deleteTriajeBehaviorPipelineFactory = deleteTriajeBehaviorPipelineFactory;
        _getAllTipoPesosBehaviorPipelineFactory = getAllTipoPesosBehaviorPipelineFactory;
        _getAllVacunosSanidadBehaviorPipelineFactory = getAllVacunosSanidadBehaviorPipelineFactory;
        _getHistorialByVacunoIdBehaviorPipelineFactory = getHistorialByVacunoIdBehaviorPipelineFactory;

        _getHistorialGeneralBehaviorPipelineFactory = getHistorialGeneralBehaviorPipelineFactory;
        _generateTriajesPdfBehaviorPipelineFactory = generateTriajesPdfBehaviorPipelineFactory;
        _generateTriajesExcelBehaviorPipelineFactory = generateTriajesExcelBehaviorPipelineFactory;
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
        [FromQuery] long? vacunoId = null,
        CancellationToken cancellationToken = default)
    {
        var behaviorPipeline = _getAllTriajesBehaviorPipelineFactory.Create();

        var currentPage = pagina <= 0 ? 1 : pagina;
        var currentTamano = tamano <= 0 ? 10 : Math.Min(tamano, 100);

        var query = new GetAllTriajesQuery
        {
            Pagina = pagina, 
            Tamano = tamano, 
            Fecha = fecha, 
            Codigo = codigo, 
            Nombre = nombre, 
            TipoPeso = tipoPeso, 
            PesoKg = pesoKg
        };

        var output = await behaviorPipeline.Execute(query, cancellationToken);
        return Ok(GeneralResponseDTO<PagedTriajeResponse>.Ok(TriajeMapper.ToPagedResponse(output, currentPage, currentTamano)));
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(GeneralResponseDTO<TriajeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken = default)
    {
        var behaviorPipeline = _getTriajeByIdBehaviorPipelineFactory.Create();

        var output = await behaviorPipeline.Execute(
            new GetTriajeByIdCommand
            {
                Id = id
            },
            cancellationToken
        );

        return Ok(GeneralResponseDTO<TriajeResponse>.Ok(TriajeMapper.ToResponse(output)));
    }

    [HttpPost]
    [ProducesResponseType(typeof(GeneralResponseDTO<TriajeResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] TriajeRequest request, CancellationToken cancellationToken = default)
    {
        var behaviorPipeline = _createTriajeBehaviorPipelineFactory.Create();

        var command = TriajeMapper.ToCreateCommand(request);
        var output = await behaviorPipeline.Execute(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = output.Id },
            GeneralResponseDTO<TriajeResponse>.Ok(TriajeMapper.ToResponse(output)));
    }

    [HttpPatch("{id:long}")]
    [ProducesResponseType(typeof(GeneralResponseDTO<TriajeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateTriajeRequest request, CancellationToken cancellationToken = default)
    {
        var behaviorPipeline = _updateTriajeBehaviorPipelineFactory.Create();

        var command = TriajeMapper.ToUpdateCommand(request);
        command.Id = id;

        var output = await behaviorPipeline.Execute(command, cancellationToken);
        
        return Ok(GeneralResponseDTO<TriajeResponse>.Ok(TriajeMapper.ToResponse(output)));
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id, [FromBody] DeleteTriajeRequest request, CancellationToken cancellationToken = default)
    {
        var behaviorPipeline = _deleteTriajeBehaviorPipelineFactory.Create();

        await behaviorPipeline.Execute(
            new DeleteTriajeCommand{
                Id = id,
                MotivoEliminacion = request.MotivoEliminacion
            }, 
            cancellationToken
        );

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
        var behaviorPipeline = _generateTriajesPdfBehaviorPipelineFactory.Create();

        var report = await behaviorPipeline.Execute(
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
        var behaviorPipeline = _generateTriajesExcelBehaviorPipelineFactory.Create();

        var report = await behaviorPipeline.Execute(
            new GenerateTriajesExcelQuery(fecha, fechaDesde, fechaHasta, codigo, nombre, tipoPeso, pesoKg, vacunoId),
            cancellationToken);

        return File(report.Content, report.ContentType, report.FileName);
    }

    [HttpGet("tipos-peso")]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTiposPeso(CancellationToken cancellationToken = default)
    {
        var behaviorPipeline = _getAllTipoPesosBehaviorPipelineFactory.Create();

        var output = await behaviorPipeline.Execute(
            EmptyCommand.Value(
                AuditEventType.Read,
                "Get all tipos peso"
            ),
            cancellationToken
        );
        return Ok(GeneralResponseDTO<object>.Ok(output.Items.Select(t => new { code = t.Code, nombre = t.Nombre })));
    }

    [HttpGet("vacunos")]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetVacunos(CancellationToken cancellationToken = default)
    {
        var behaviorPipeline = _getAllVacunosSanidadBehaviorPipelineFactory.Create();

        var output = await behaviorPipeline.Execute(
            EmptyCommand.Value(
                AuditEventType.Read,
                "Get all vacunos sanidad"
            ),
            cancellationToken
        );

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
        var behaviorPipeline = _getHistorialByVacunoIdBehaviorPipelineFactory.Create();

        var output = await behaviorPipeline.Execute(
            new GetHistorialByVacunoIdCommand
            {
                Vacunoid = vacunoId,
                FechaDesde = desde,
                FechaHasta = hasta
            },
            cancellationToken
        );
        return Ok(GeneralResponseDTO<object>.Ok(output.Items.Select(t => new { id = t.Id, fechaHora = t.FechaHora, tipoPesoCode = t.TipoPesoCode, pesoKg = t.PesoKg })));
    }

    [HttpGet("historial-general")]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHistorialGeneral(
        [FromQuery] string? desde = null,
        [FromQuery] string? hasta = null,
        CancellationToken cancellationToken = default)
    {
        var behaviorPipeline = _getHistorialGeneralBehaviorPipelineFactory.Create();

        var output = await behaviorPipeline.Execute(
            new GetHistorialGeneralCommand
            {
                FechaDesde = desde, 
                FechaHasta = hasta
            }, 
            cancellationToken
        );
        return Ok(GeneralResponseDTO<object>.Ok(output.Items.Select(t => new { id = t.Id, fechaHora = t.FechaHora, tipoPesoCode = t.TipoPesoCode, pesoKg = t.PesoKg })));
    }
}
