using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosPdf;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosExcel;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GetOrdenioById;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.ListOrdenios;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunos;
using ZooTech.InterfaceAdapters.DTOs;
using ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.DTOs.Responses;
using ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.Mappers;
using ZooTech.Application.Common.Behaviors.Module_ProduccionLeche.Ordenios.CreateOrdenio;
using ZooTech.Application.Common.Behaviors.Module_ProduccionLeche.Ordenios.DeleteOrdenio;
using ZooTech.Application.Common.Behaviors.Module_ProduccionLeche.Ordenios.GenerateOrdeniosExcel;
using ZooTech.Application.Common.Behaviors.Module_ProduccionLeche.Ordenios.GenerateOrdeniosPdf;
using ZooTech.Application.Common.Behaviors.Module_ProduccionLeche.Ordenios.GetOrdenioById;
using ZooTech.Application.Common.Behaviors.Module_ProduccionLeche.Ordenios.ListOrdenios;
using ZooTech.Application.Common.Behaviors.Module_ProduccionLeche.Ordenios.UpdateOrdenio;

namespace ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.Controllers;

[ApiController]
[Route("api/v1/produccion-leche")]
[ApiExplorerSettings(GroupName = "produccion_leche")]
public sealed class ProduccionLecheController : ControllerBase
{
    // TODO: Modificar aqui "IListarVacunosInputPort" por un Pipeline
    private readonly IListarVacunosInputPort _listarVacunosInputPort;
    private readonly ICreateOrdenioBehaviorPipelineFactory _createOrdenioBehaviorPipelineFactory;
    private readonly IGetOrdenioByIdBehaviorPipelineFactory _getOrdenioByIdBehaviorPipelineFactory;
    private readonly IGenerateOrdeniosPdfBehaviorPipelineFactory _generateOrdeniosPdfBehaviorPipelineFactory;
    private readonly IGenerateOrdeniosExcelBehaviorPipelineFactory _generateOrdeniosExcelBehaviorPipelineFactory;
    private readonly IListOrdeniosBehaviorPipelineFactory _listOrdeniosBehaviorPipelineFactory;
    private readonly IUpdateOrdenioBehaviorPipelineFactory _updateOrdenioBehaviorPipelineFactory;
    private readonly IDeleteOrdenioBehaviorPipelineFactory _deleteOrdenioBehaviorPipelineFactory;

    public ProduccionLecheController(
        IListarVacunosInputPort listarVacunosInputPort,
        ICreateOrdenioBehaviorPipelineFactory createOrdenioBehaviorPipelineFactory,
        IGetOrdenioByIdBehaviorPipelineFactory getOrdenioByIdBehaviorPipelineFactory,
        IGenerateOrdeniosPdfBehaviorPipelineFactory generateOrdeniosPdfBehaviorPipelineFactory,
        IGenerateOrdeniosExcelBehaviorPipelineFactory generateOrdeniosExcelBehaviorPipelineFactory,
        IListOrdeniosBehaviorPipelineFactory listOrdeniosBehaviorPipelineFactory,
        IUpdateOrdenioBehaviorPipelineFactory updateOrdenioBehaviorPipelineFactory,
        IDeleteOrdenioBehaviorPipelineFactory deleteOrdenioBehaviorPipelineFactory

    )
    {
        _listarVacunosInputPort = listarVacunosInputPort;
        
        _createOrdenioBehaviorPipelineFactory = createOrdenioBehaviorPipelineFactory;
        _getOrdenioByIdBehaviorPipelineFactory = getOrdenioByIdBehaviorPipelineFactory;
        _generateOrdeniosPdfBehaviorPipelineFactory = generateOrdeniosPdfBehaviorPipelineFactory;
        _generateOrdeniosExcelBehaviorPipelineFactory = generateOrdeniosExcelBehaviorPipelineFactory;
        _listOrdeniosBehaviorPipelineFactory = listOrdeniosBehaviorPipelineFactory;
        _updateOrdenioBehaviorPipelineFactory = updateOrdenioBehaviorPipelineFactory;
        _deleteOrdenioBehaviorPipelineFactory = deleteOrdenioBehaviorPipelineFactory;
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
        var behaviorPipeline = _createOrdenioBehaviorPipelineFactory.Create();

        var data = ProduccionLecheMapper.ToResponse(
            await behaviorPipeline.Execute(ProduccionLecheMapper.ToCommand(request), cancellationToken));
        return Created($"/api/v1/produccion-leche/{data.Id}", GeneralResponseDTO<OrdenioResponse>.Ok(data));
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(GeneralResponseDTO<OrdenioResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] long id, CancellationToken cancellationToken)
    {
        var behaviorPipeline = _getOrdenioByIdBehaviorPipelineFactory.Create();

        var data = ProduccionLecheMapper.ToResponse(await behaviorPipeline.Execute(
                new GetOrdenioByIdCommand
                {
                    Id = id
                }, 
                cancellationToken
            ));
        return Ok(GeneralResponseDTO<OrdenioResponse>.Ok(data));
    }

    [HttpGet]
    [ProducesResponseType(typeof(GeneralResponseDTO<ListOrdeniosResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> List(
        [FromQuery] long? vacunoId,
        [FromQuery] string? estadoOrdenioCode,
        [FromQuery] DateTime? fechaDesde,
        [FromQuery] DateTime? fechaHasta,
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        CancellationToken cancellationToken)
    {
        var behaviorPipeline = _listOrdeniosBehaviorPipelineFactory.Create();

        var currentPage = page ?? 1;
        var currentPageSize = pageSize ?? 20;
        var data = ProduccionLecheMapper.ToResponse(
            await behaviorPipeline.Execute(
                new ListOrdeniosQuery
                {
                    VacunoId = vacunoId, 
                    EstadoOrdenioCode = estadoOrdenioCode, 
                    FechaDesde = fechaDesde, 
                    FechaHasta = fechaHasta, 
                    Page = currentPage, 
                    PageSize = currentPageSize
                },
                cancellationToken
            ),
            currentPage, currentPageSize);
        return Ok(GeneralResponseDTO<ListOrdeniosResponse>.Ok(data));
    }

    [HttpGet("reporte/pdf")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GeneratePdf(
        [FromQuery] long? vacunoId,
        [FromQuery] string? estadoOrdenioCode,
        [FromQuery] DateTime? fechaDesde,
        [FromQuery] DateTime? fechaHasta,
        [FromQuery] bool comparativo,
        CancellationToken cancellationToken)
    {
        var behaviorPipeline = _generateOrdeniosPdfBehaviorPipelineFactory.Create();

        var report = await behaviorPipeline.Execute(
            new GenerateOrdeniosComparationPdfQuery
            {
                VacunoId = vacunoId, 
                EstadoOrdenioCode = estadoOrdenioCode, 
                FechaDesde = fechaDesde, 
                FechaHasta = fechaHasta, 
                Comparativo = comparativo
            },
            cancellationToken
        );

        return File(report.Content, report.ContentType, report.FileName);
    }

    [HttpGet("reporte/excel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GenerateExcel(
        [FromQuery] long? vacunoId,
        [FromQuery] string? estadoOrdenioCode,
        [FromQuery] DateTime? fechaDesde,
        [FromQuery] DateTime? fechaHasta,
        CancellationToken cancellationToken)
    {
        var behaviorPipeline = _generateOrdeniosExcelBehaviorPipelineFactory.Create();

        var report = await behaviorPipeline.Execute(
            new GenerateOrdeniosComparationExcelQuery
            {
                VacunoId = vacunoId, 
                EstadoOrdenioCode = estadoOrdenioCode, 
                FechaDesde = fechaDesde, 
                FechaHasta = fechaHasta
            },
            cancellationToken
        );

        return File(report.Content, report.ContentType, report.FileName);
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
        var behaviorPipeline = _updateOrdenioBehaviorPipelineFactory.Create();

        var command = ProduccionLecheMapper.ToCommand(request);
        command.Id = id;

        var data = ProduccionLecheMapper.ToResponse(
            await behaviorPipeline.Execute(command, cancellationToken));
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
        var behaviorPipeline = _deleteOrdenioBehaviorPipelineFactory.Create();

        var command = ProduccionLecheMapper.ToCommand(request);
        command.Id = id;

        await behaviorPipeline.Execute(command, cancellationToken);
        return NoContent();
    }
}
