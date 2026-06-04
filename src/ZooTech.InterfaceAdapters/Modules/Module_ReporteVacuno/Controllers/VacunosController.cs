using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ListarReporteVacunos;
using ZooTech.InterfaceAdapters.Modules.Module_ReporteVacuno.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_ReporteVacuno.DTOs.Responses;
using ZooTech.InterfaceAdapters.Modules.Module_ReporteVacuno.Mappers;

namespace ZooTech.InterfaceAdapters.Modules.Module_ReporteVacuno.Controllers;

[ApiController]
[Route("vacunos")]
[Tags("Vacunos")]
public sealed class VacunosController : ControllerBase
{
    private readonly IListarReporteVacunosUseCase _listarReporteVacunosUseCase;

    public VacunosController(IListarReporteVacunosUseCase listarReporteVacunosUseCase)
    {
        _listarReporteVacunosUseCase = listarReporteVacunosUseCase;
    }

    /// Lista vacunos con filtros y paginacion.
    /// Aplica por defecto el rango de los ultimos 30 dias cuando no llegan fechas.
    [HttpGet]
    [ProducesResponseType(typeof(VacunoListResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Listar(
        [FromQuery] ListadoVacunosReporteQueryDto query,
        CancellationToken cancellationToken)
    {
        var applicationQuery = VacunoListMapper.ToApplicationQuery(query);
        var response = await _listarReporteVacunosUseCase.HandleAsync(applicationQuery, cancellationToken);

        return Ok(VacunoListMapper.ToDto(response, query));
    }
}
