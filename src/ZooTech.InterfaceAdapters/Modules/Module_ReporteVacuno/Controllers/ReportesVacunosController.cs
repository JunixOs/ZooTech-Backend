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

    /// <summary>
    /// Lista los reportes y graficos disponibles del modulo Vacuno.
    /// Aplica por defecto el rango de los ultimos 30 dias cuando no llegan fechas.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ReportesDisponiblesResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
    public IActionResult Disponibles([FromQuery] ReportesDisponiblesQueryDto query)
    {
        var applicationQuery = ReportesDisponiblesMapper.ToApplicationQuery(query);
        var response = _listarReportesDisponiblesUseCase.Handle(applicationQuery);

        return Ok(ReportesDisponiblesMapper.ToDto(response));
    }

    /// <summary>
    /// Genera el reporte listado de vacunos con filtros, rango de fechas y paginacion.
    /// </summary>
    /// <remarks>
    /// Ejemplo:
    /// GET /vacunos/reportes/listado?fechaDesde=2024-03-01&amp;fechaHasta=2024-03-31&amp;q=VACA001&amp;formato=json&amp;page=1&amp;limit=20
    /// </remarks>
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
