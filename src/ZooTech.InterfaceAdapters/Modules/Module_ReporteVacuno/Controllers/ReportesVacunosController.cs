using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ListarReporteVacunos;
using ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ListarReportesDisponibles;
using ZooTech.InterfaceAdapters.Modules.Module_ReporteVacuno.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_ReporteVacuno.DTOs.Responses;
using ZooTech.InterfaceAdapters.Modules.Module_ReporteVacuno.Mappers;

namespace ZooTech.InterfaceAdapters.Modules.Module_ReporteVacuno.Controllers;

[ApiController]
[Route("vacunos/reportes")]
[Tags("Reportes Vacunos")]
public sealed class ReportesVacunosController : ControllerBase
{
    private readonly IListarReportesDisponiblesUseCase _listarReportesDisponiblesUseCase;
    private readonly IListarReporteVacunosUseCase _listarReporteVacunosUseCase;

    public ReportesVacunosController(
        IListarReportesDisponiblesUseCase listarReportesDisponiblesUseCase,
        IListarReporteVacunosUseCase listarReporteVacunosUseCase)
    {
        _listarReportesDisponiblesUseCase = listarReportesDisponiblesUseCase;
        _listarReporteVacunosUseCase = listarReporteVacunosUseCase;
    }

    /// Lista los reportes y graficos disponibles del modulo Vacuno.
    /// Aplica por defecto el rango de los ultimos 30 dias cuando no llegan fechas.
    [HttpGet]
    [ProducesResponseType(typeof(ReportesDisponiblesResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Disponibles([FromQuery] ReportesDisponiblesQueryDto query, CancellationToken cancellationToken)
    {
        var applicationQuery = ReportesDisponiblesMapper.ToApplicationQuery(query);
        var response = await _listarReportesDisponiblesUseCase.HandleAsync(applicationQuery, cancellationToken);

        return Ok(ReportesDisponiblesMapper.ToDto(response));
    }

    /// Genera el reporte listado de vacunos con filtros, rango de fechas y paginacion.
    [HttpGet("listado")]
    [ProducesResponseType(typeof(ListadoVacunosReporteResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status501NotImplemented)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Listado(
        [FromQuery] ListadoVacunosReporteQueryDto query,
        CancellationToken cancellationToken)
    {
        var applicationQuery = ListadoVacunosReporteMapper.ToApplicationQuery(query);
        var response = await _listarReporteVacunosUseCase.HandleAsync(applicationQuery, cancellationToken);

        return Ok(ListadoVacunosReporteMapper.ToDto(response));
    }
}
