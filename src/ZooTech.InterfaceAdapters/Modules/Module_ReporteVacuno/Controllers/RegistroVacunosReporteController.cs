using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ObtenerRegistroVacunoReporte;
using ZooTech.InterfaceAdapters.Modules.Module_ReporteVacuno.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_ReporteVacuno.DTOs.Responses;
using ZooTech.InterfaceAdapters.Modules.Module_ReporteVacuno.Mappers;

namespace ZooTech.InterfaceAdapters.Modules.Module_ReporteVacuno.Controllers;

[ApiController]
[Route("vacunos")]
[Tags("Reportes Vacunos")]
public sealed class RegistroVacunosReporteController : ControllerBase
{
    private readonly IObtenerRegistroVacunoReporteUseCase _obtenerRegistroVacunoReporteUseCase;

    public RegistroVacunosReporteController(
        IObtenerRegistroVacunoReporteUseCase obtenerRegistroVacunoReporteUseCase)
    {
        _obtenerRegistroVacunoReporteUseCase = obtenerRegistroVacunoReporteUseCase;
    }

    /// <summary>
    /// Genera el reporte individual de registro por vacuno.
    /// </summary>
    /// <remarks>
    /// Ejemplo:
    /// GET /vacunos/1/reporte?formato=json
    /// </remarks>
    [HttpGet("{vacunoId:long}/reporte")]
    [ProducesResponseType(typeof(RegistroVacunoReporteResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status501NotImplemented)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Registro(
        [FromRoute] long vacunoId,
        [FromQuery] RegistroVacunoReporteQueryDto query,
        CancellationToken cancellationToken)
    {
        var applicationQuery = RegistroVacunoReporteMapper.ToApplicationQuery(vacunoId, query);
        var response = await _obtenerRegistroVacunoReporteUseCase.HandleAsync(applicationQuery, cancellationToken);

        return Ok(RegistroVacunoReporteMapper.ToDto(response));
    }
}
