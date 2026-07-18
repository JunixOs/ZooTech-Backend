using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Common.Behaviors.Module_Celo.CreateCelo;
using ZooTech.Application.Common.Behaviors.Module_Celo.DeleteCelo;
using ZooTech.Application.Common.Behaviors.Module_Celo.GetCelos;
using ZooTech.Application.Common.Behaviors.Module_Celo.UpdateCelo;
using ZooTech.Application.Common.Models;
using ZooTech.Domain.Shared.Enums;
using ZooTech.Application.Modules.Module_Celo.UseCases.GetComparacionCelosRealVsEstandar;
using ZooTech.Application.Modules.Module_Celo.UseCases.GetComparacionCelosRealVsEstandarPorVacuno;
using ZooTech.Application.Modules.Module_Celo.UseCases.GetVacasEnCelo;
using ZooTech.Application.Modules.Module_Celo.UseCases.ListReporteCeloGeneral;
using ZooTech.InterfaceAdapters.DTOs;
using ZooTech.InterfaceAdapters.Modules.Module_Celo.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Celo.DTOs.Responses;
using ZooTech.InterfaceAdapters.Modules.Module_Celo.Mappers;
using ZooTech.Application.Common.Behaviors.Module_Celo.GetComparacionCelosRealVsEstandar;
using ZooTech.Application.Common.Behaviors.Module_Celo.GetVacasEnCelo;
using ZooTech.Application.Common.Behaviors.Module_Celo.ListCelos;
using ZooTech.Application.Common.Behaviors.Module_Celo.ListReporteCeloGeneral;
using ZooTech.Application.Common.Behaviors.Module_Celo.GetReporteCelos;
using ZooTech.Application.Common.Behaviors.Module_Celo.GetComparacionCelosRealVsEstandarPorVacuno;

namespace ZooTech.InterfaceAdapters.Modules.Module_Celo.Controllers;

[ApiController]
[Route("api/v1/celo")]
[ApiExplorerSettings(GroupName = "celo")]
public sealed class CeloController : ControllerBase
{
    private readonly IGetCelosBehaviorPipelineFactory _getCelosBehaviorPipelineFactory;
    private readonly ICreateCeloBehaviorPipelineFactory _createCeloBehaviorPipelineFactory;
    private readonly IUpdateCeloBehaviorPipelineFactory _updateCeloBehaviorPipelineFactory;
    private readonly IDeleteCeloBehaviorPipelineFactory _deleteCeloBehaviorPipelineFactory;

    private readonly IGetComparacionCelosRealVsEstandarBehaviorPipelineFactory _getComparacionCelosRealVsEstandarBehaviorPipelineFactory;
    private readonly IGetComparacionCelosRealVsEstandarPorVacunoBehaviorPipelineFactory _getComparacionCelosRealVsEstandarPorVacunoBehaviorPipelineFactory;
    private readonly IGetVacasEnCeloBehaviorPipelineFactory _getVacasEnCeloBehaviorPipelineFactory;
    private readonly IListCelosBehaviorPipelineFactory _listCelosBehaviorPipelineFactory;
    private readonly IListReporteCeloGeneralBehaviorPipelineFactory _listReporteCeloGeneralBehaviorPipelineFactory;
    private readonly IGetReporteCelosBehaviorPipelineFactory _getReporteCelosBehaviorPipelineFactory;

    public CeloController(
        IGetCelosBehaviorPipelineFactory getCelosBehaviorPipelineFactory,
        ICreateCeloBehaviorPipelineFactory createCeloBehaviorPipelineFactory,
        IUpdateCeloBehaviorPipelineFactory updateCeloBehaviorPipelineFactory,
        IDeleteCeloBehaviorPipelineFactory deleteCeloBehaviorPipelineFactory,
        IGetComparacionCelosRealVsEstandarBehaviorPipelineFactory getComparacionCelosRealVsEstandarBehaviorPipelineFactory,
        IGetComparacionCelosRealVsEstandarPorVacunoBehaviorPipelineFactory getComparacionCelosRealVsEstandarPorVacunoBehaviorPipelineFactory,
        IGetVacasEnCeloBehaviorPipelineFactory getVacasEnCeloBehaviorPipelineFactory,
        IListCelosBehaviorPipelineFactory listCelosBehaviorPipelineFactory,
        IListReporteCeloGeneralBehaviorPipelineFactory listReporteCeloGeneralBehaviorPipelineFactory,
        IGetReporteCelosBehaviorPipelineFactory getReporteCelosBehaviorPipelineFactory
    )
    {
        _getCelosBehaviorPipelineFactory = getCelosBehaviorPipelineFactory;
        _createCeloBehaviorPipelineFactory = createCeloBehaviorPipelineFactory;;
        _updateCeloBehaviorPipelineFactory = updateCeloBehaviorPipelineFactory;
        _deleteCeloBehaviorPipelineFactory = deleteCeloBehaviorPipelineFactory;

        _getComparacionCelosRealVsEstandarBehaviorPipelineFactory = getComparacionCelosRealVsEstandarBehaviorPipelineFactory;
        _getComparacionCelosRealVsEstandarPorVacunoBehaviorPipelineFactory = getComparacionCelosRealVsEstandarPorVacunoBehaviorPipelineFactory;
        _getVacasEnCeloBehaviorPipelineFactory = getVacasEnCeloBehaviorPipelineFactory;
        _listCelosBehaviorPipelineFactory = listCelosBehaviorPipelineFactory;
        _listReporteCeloGeneralBehaviorPipelineFactory = listReporteCeloGeneralBehaviorPipelineFactory;
        _getReporteCelosBehaviorPipelineFactory = getReporteCelosBehaviorPipelineFactory;
    }

    [HttpGet]
    [ProducesResponseType(typeof(GeneralResponseDTO<ListCelosResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCelos(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] DateTime? fechaInicio = null,
        [FromQuery] DateTime? fechaFin = null,
        [FromQuery] Dictionary<string, string>? columnFilters = null,
        CancellationToken cancellationToken = default)
    {
        var behaviorPipeline = _listCelosBehaviorPipelineFactory.Create();

        var output = await behaviorPipeline.Execute(
            CeloMapper.ToCommand(
                search,
                page,
                pageSize,
                fechaInicio,
                fechaFin,
                columnFilters
            ),
            cancellationToken
        );
        var response = CeloMapper.ToResponse(output);

        return Ok(GeneralResponseDTO<ListCelosResponse>.Ok(response));
    }

    [HttpGet("reportes/general")]
    [ProducesResponseType(typeof(GeneralResponseDTO<ListReporteCeloGeneralResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReporteCeloGeneral(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] DateTime? fechaInicio = null,
        [FromQuery] DateTime? fechaFin = null,
        [FromQuery] Dictionary<string, string>? columnFilters = null,
        CancellationToken cancellationToken = default)
    {
        var behaviorPipeline = _listReporteCeloGeneralBehaviorPipelineFactory.Create();

        var output = await behaviorPipeline.Execute(
            new ListReporteCeloGeneralCommand
            {
                Search = search, 
                Page = page, 
                PageSize = pageSize, 
                FechaInicio = fechaInicio, 
                FechaFin =  fechaFin, 
                ColumnFilters = columnFilters
            }, 
            cancellationToken
        );
        var response = CeloMapper.ToResponse(output);

        return Ok(GeneralResponseDTO<ListReporteCeloGeneralResponse>.Ok(response));
    }

    [HttpGet("reportes/por-vacuno")]
    [ProducesResponseType(typeof(GeneralResponseDTO<List<ReporteCeloPorVacunoResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReporteCeloPorVacuno(
        [FromQuery] DateTime? fechaInicio,
        [FromQuery] DateTime? fechaFin,
        CancellationToken cancellationToken)
    {
        var behaviorPipeline = _getCelosBehaviorPipelineFactory.Create();

        var output = await behaviorPipeline.Execute(
            EmptyCommand.Value(AuditEventType.Read , "Get celos"),
            cancellationToken
        );

        var inicio = fechaInicio.HasValue ? DateOnly.FromDateTime(fechaInicio.Value) : (DateOnly?)null;
        var fin = fechaFin.HasValue ? DateOnly.FromDateTime(fechaFin.Value) : (DateOnly?)null;

        var items = output.Items.Where(item =>
            (!inicio.HasValue || item.Fecha >= inicio.Value) &&
            (!fin.HasValue || item.Fecha <= fin.Value));

        var response = items
            .GroupBy(item => new
            {
                item.CodigoVacuno,
                item.NombreVacuno
            })
            .Select(group => new ReporteCeloPorVacunoResponse
            {
                CodigoVacuno = group.Key.CodigoVacuno,
                Nombre = group.Key.NombreVacuno,
                UltimoCelo = group.Max(item => item.Fecha),
                VecesEnCelo = group.Count()
            })
            .OrderByDescending(item => item.UltimoCelo)
            .ToList();

        return Ok(GeneralResponseDTO<List<ReporteCeloPorVacunoResponse>>.Ok(response));
    }

    [HttpGet("reportes/por-vacuno/{codigoVacuno}")]
    [ProducesResponseType(typeof(GeneralResponseDTO<List<CeloReporteItemResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDetalleCeloPorVacuno(
    string codigoVacuno,
    CancellationToken cancellationToken)
    {
        var behaviorPipeline = _getReporteCelosBehaviorPipelineFactory.Create();

        var output = await behaviorPipeline.Execute(
            EmptyCommand.Value(
                AuditEventType.Read,
                "Get reporte celos por vacuno"
            ),
            cancellationToken
        );

        var response = output.Items
            .Where(item => item.CodigoVacuno == codigoVacuno)
            .OrderByDescending(item => item.Fecha)
            .ThenByDescending(item => item.Hora)
            .Select(item => CeloMapper.ToResponse(item))
            .ToList();

        return Ok(GeneralResponseDTO<List<CeloReporteItemResponse>>.Ok(response));
    }

    [HttpGet("vacas-en-celo")]
    [ProducesResponseType(typeof(GeneralResponseDTO<List<VacaEnCeloResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetVacasEnCelo(
        [FromQuery] DateTime? fechaInicio,
        [FromQuery] DateTime? fechaFin,
        CancellationToken cancellationToken)
    {
        var behaviorPipeline = _getVacasEnCeloBehaviorPipelineFactory.Create();

        var output = await behaviorPipeline.Execute(
            new GetVacasEnCeloCommand
            {
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            },
            cancellationToken
        );

        var response = output.Items.Select(item => CeloMapper.ToResponse(item)).ToList();

        return Ok(GeneralResponseDTO<List<VacaEnCeloResponse>>.Ok(response));
    }

    [HttpGet("reportes/comparacion-real-vs-estandar")]
    [ProducesResponseType(typeof(GeneralResponseDTO<List<ComparacionCelosResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetComparacionRealVsEstandar(
        [FromQuery] DateTime? fechaInicio,
        [FromQuery] DateTime? fechaFin,
        CancellationToken cancellationToken)
    {
        var behaviorPipeline = _getComparacionCelosRealVsEstandarBehaviorPipelineFactory.Create();

        var output = await behaviorPipeline.Execute(
            new GetComparacionCelosRealVsEstandarCommand
            {
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            },
            cancellationToken
        );

        var response = output.Items
            .Select(CeloMapper.ToResponse)
            .ToList();

        return Ok(
            GeneralResponseDTO<List<ComparacionCelosResponse>>.Ok(response));
    }

    [HttpGet("reportes/comparacion-por-vacuno/{codigoVacuno}")]
    [ProducesResponseType(typeof(GeneralResponseDTO<List<ComparacionCelosPorVacunoResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetComparacionPorVacuno(
        string codigoVacuno,
        [FromQuery] DateTime? fechaInicio,
        [FromQuery] DateTime? fechaFin,
        CancellationToken cancellationToken)
    {
        var behaviorPipeline = _getComparacionCelosRealVsEstandarPorVacunoBehaviorPipelineFactory.Create();

        var output = await behaviorPipeline.Execute(
            new GetComparacionCelosRealVsEstandarPorVacunoCommand
            {
                CodigoVacuno = codigoVacuno,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            },
            cancellationToken);

        var response = output.Items
            .Select(CeloMapper.ToResponse)
            .ToList();

        return Ok(
            GeneralResponseDTO<List<ComparacionCelosPorVacunoResponse>>.Ok(response));
    }

    [HttpPost]
    [ProducesResponseType(typeof(GeneralResponseDTO<CreateCeloResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateCelo(
        [FromBody] CreateCeloRequest request,
        CancellationToken cancellationToken)
    {
        var behaviorPipeline = _createCeloBehaviorPipelineFactory.Create();

        var command = CeloMapper.ToCommand(request);
        var output = await behaviorPipeline.Execute(command, cancellationToken);
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
        var behaviorPipeline = _updateCeloBehaviorPipelineFactory.Create();

        var command = CeloMapper.ToCommand(request);
        var output = await behaviorPipeline.Execute(command, cancellationToken);
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
        var behaviorPipeline = _deleteCeloBehaviorPipelineFactory.Create();

        var command = CeloMapper.ToCommand(request, id);
        await behaviorPipeline.Execute(command, cancellationToken);

        return Ok(GeneralResponseDTO<object>.Ok(new
        {
            mensaje = "Celo eliminado con éxito"
        }));
    }
}