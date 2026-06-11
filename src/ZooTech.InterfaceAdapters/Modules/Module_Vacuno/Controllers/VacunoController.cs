using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.CreateVacuno;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.DeleteVacuno;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GetVacunoById;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunos;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ListarReportesDisponibles;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ObtenerRegistroVacunoReporte;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.UpdateVacuno;
using ZooTech.InterfaceAdapters.DTOs;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.ReporteVacuno.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.ReporteVacuno.Responses;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Responses;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Mappers;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Mappers.ReporteVacuno;

namespace ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Controllers;

[ApiController]
[Route("api/v1/vacunos")] // Cambiado a plural para estandarizar, pero agregamos alias "vacuno"
[Route("api/v1/vacuno")]
[ApiExplorerSettings(GroupName = "public")]
public sealed class VacunoController : ControllerBase
{
    private readonly IListarVacunosInputPort _listarInputPort;
    private readonly ICreateVacunoInputPort _createInputPort;
    private readonly IGetVacunoByIdInputPort _getByIdInputPort;
    private readonly IUpdateVacunoInputPort _updateInputPort;
    private readonly IDeleteVacunoInputPort _deleteInputPort;
    private readonly IListarReportesDisponiblesUseCase _listarReportesDisponiblesUseCase;
    private readonly IObtenerRegistroVacunoReporteUseCase _obtenerRegistroVacunoReporteUseCase;

    public VacunoController(
        IListarVacunosInputPort listarInputPort,
        ICreateVacunoInputPort createInputPort,
        IGetVacunoByIdInputPort getByIdInputPort,
        IUpdateVacunoInputPort updateInputPort,
        IDeleteVacunoInputPort deleteInputPort,
        IListarReportesDisponiblesUseCase listarReportesDisponiblesUseCase,
        IObtenerRegistroVacunoReporteUseCase obtenerRegistroVacunoReporteUseCase)
    {
        _listarInputPort = listarInputPort;
        _createInputPort = createInputPort;
        _getByIdInputPort = getByIdInputPort;
        _updateInputPort = updateInputPort;
        _deleteInputPort = deleteInputPort;
        _listarReportesDisponiblesUseCase = listarReportesDisponiblesUseCase;
        _obtenerRegistroVacunoReporteUseCase = obtenerRegistroVacunoReporteUseCase;
    }

    [HttpGet]
    [ProducesResponseType(typeof(GeneralResponseDTO<List<VacunoItemResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarVacunos([FromQuery] string? q, CancellationToken cancellationToken)
    {
        // Llamada básica mapeada al comando con valores por defecto para retornar todos
        var command = new ListarVacunosCommand(null, null, q, null, null, null, null, null, "1", "10000");
        var output = await _listarInputPort.HandleAsync(command, cancellationToken);
        var response = output.Data.Select(x => new VacunoItemResponse(
            x.Id,
            x.Codigo,
            x.Nombre,
            x.FechaNacimiento,
            x.RazaCode,
            x.SexoCode,
            x.Procedencia
        )).ToList();
        return Ok(GeneralResponseDTO<List<VacunoItemResponse>>.Ok(response));
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

    // ==========================================
    // ENDPOINTS DE REPORTES FUSIONADOS AL MÓDULO
    // ==========================================

    [HttpGet("reportes")]
    [ProducesResponseType(typeof(ReportesDisponiblesResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ReportesDisponibles([FromQuery] ReportesDisponiblesQueryDto query, CancellationToken cancellationToken)
    {
        var applicationQuery = ReportesDisponiblesMapper.ToApplicationQuery(query);
        var response = await _listarReportesDisponiblesUseCase.HandleAsync(applicationQuery, cancellationToken);
        return Ok(ReportesDisponiblesMapper.ToDto(response));
    }

    [HttpGet("reportes/listado")]
    [ProducesResponseType(typeof(ListadoVacunosReporteResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status501NotImplemented)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ReportesListado([FromQuery] ListadoVacunosReporteQueryDto query, CancellationToken cancellationToken)
    {
        var command = new ListarVacunosCommand(query.FechaDesde, query.FechaHasta, query.Q, query.Raza, query.Procedencia, query.Estado, query.AptoPara, query.Formato, query.Page, query.Limit);
        var response = await _listarInputPort.HandleAsync(command, cancellationToken);
        return Ok(ListadoVacunosReporteMapper.ToDto(response));
    }

    [HttpGet("{vacunoId:long}/reporte")]
    [ProducesResponseType(typeof(RegistroVacunoReporteResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status501NotImplemented)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ReporteIndividual([FromRoute] long vacunoId, [FromQuery] RegistroVacunoReporteQueryDto query, CancellationToken cancellationToken)
    {
        var applicationQuery = RegistroVacunoReporteMapper.ToApplicationQuery(vacunoId, query);
        var response = await _obtenerRegistroVacunoReporteUseCase.HandleAsync(applicationQuery, cancellationToken);
        return Ok(RegistroVacunoReporteMapper.ToDto(response));
    }
}
