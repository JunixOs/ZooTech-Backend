using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.DeleteVacuno;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GetVacunoById;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunos;
using ZooTech.Domain.Module_Vacuno.Interfaces;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.InterfaceAdapters.DTOs;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Responses;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Mappers;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ObtenerRegistroVacunoReporte;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ListarVacunosReporte;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Mappers.ReporteVacuno;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Services;
using ListadoVacunosReporteApiResponse = ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Responses.ListadoVacunosReporteResponse;
using RegistroVacunoReporteApiResponse = ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Responses.RegistroVacunoReporteResponse;

namespace ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Controllers;

[ApiController]
[Route("api/v1/vacunos")]
[ApiExplorerSettings(GroupName = "public")]
public sealed class VacunoController : ControllerBase
{
    private readonly IGetVacunoByIdInputPort _getByIdInputPort;
    private readonly IDeleteVacunoInputPort _deleteInputPort;
    private readonly IVacunoRepository _vacunoRepository;
    private readonly IVacunoActivityStatsReadRepository _activityStatsReadRepository;
    private readonly IObtenerRegistroVacunoReporteUseCase _reporteUseCase;
    private readonly IListarVacunosReporteUseCase _listarVacunosReporteUseCase;
    private readonly IVacunoMutationService _mutationService;
    private readonly IVacunoResponseEnricher _responseEnricher;

    public VacunoController(
        IGetVacunoByIdInputPort getByIdInputPort,
        IDeleteVacunoInputPort deleteInputPort,
        IVacunoRepository vacunoRepository,
        IVacunoActivityStatsReadRepository activityStatsReadRepository,
        IObtenerRegistroVacunoReporteUseCase reporteUseCase,
        IListarVacunosReporteUseCase listarVacunosReporteUseCase,
        IVacunoMutationService mutationService,
        IVacunoResponseEnricher responseEnricher)
    {
        _getByIdInputPort = getByIdInputPort;
        _deleteInputPort = deleteInputPort;
        _vacunoRepository = vacunoRepository;
        _activityStatsReadRepository = activityStatsReadRepository;
        _reporteUseCase = reporteUseCase;
        _listarVacunosReporteUseCase = listarVacunosReporteUseCase;
        _mutationService = mutationService;
        _responseEnricher = responseEnricher;
    }

    [HttpPost]
    [ProducesResponseType(typeof(GeneralResponseDTO<VacunoResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreateVacunoRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mutationService.CreateAsync(request, cancellationToken);
        if (!result.Success)
        {
            return ToReferenceErrorResult(result.Error!);
        }

        var response = result.Response!;
        return Created($"/api/v1/vacunos/{response.Id}", GeneralResponseDTO<VacunoResponse>.Ok(response));
    }

    [HttpGet("{identifier}")]
    [ProducesResponseType(typeof(GeneralResponseDTO<VacunoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        [FromRoute] string identifier,
        CancellationToken cancellationToken)
    {
        long id = await ResolveIdAsync(identifier, cancellationToken);
        var output = await _getByIdInputPort.HandleAsync(id, cancellationToken);
        var response = await _responseEnricher.EnrichAsync(output.Data, cancellationToken);
        return Ok(GeneralResponseDTO<VacunoResponse>.Ok(response));
    }

    [HttpPatch("{identifier}")]
    [ProducesResponseType(typeof(GeneralResponseDTO<VacunoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        [FromRoute] string identifier,
        [FromBody] UpdateVacunoRequest request,
        CancellationToken cancellationToken)
    {
        long id = await ResolveIdAsync(identifier, cancellationToken);
        var result = await _mutationService.UpdateAsync(id, request, cancellationToken);
        if (!result.Success)
        {
            return ToReferenceErrorResult(result.Error!);
        }

        return Ok(GeneralResponseDTO<VacunoResponse>.Ok(result.Response!));
    }

    [HttpDelete("{identifier}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] string identifier, [FromBody] DeleteVacunoRequest request, CancellationToken cancellationToken)
    {
        long id = await ResolveIdAsync(identifier, cancellationToken);
        await _deleteInputPort.HandleAsync(id, VacunoMapper.ToCommand(request), cancellationToken);
        return NoContent();
    }

    private IActionResult ToReferenceErrorResult(VacunoReferenceError error)
    {
        if (error.Kind == VacunoReferenceErrorKind.NotFound)
        {
            return NotFound(GeneralResponseDTO<object>.Fail(error.Message));
        }

        return BadRequest(new
        {
            error = new
            {
                code = "VALIDATION_ERROR",
                message = "Los datos enviados no son válidos.",
                details = new[]
                {
                    new { field = error.Field, message = error.Message }
                }
            }
        });
    }

    [HttpGet("granjas")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarGranjas(
        [FromServices] GanaderiaDbContext db,
        CancellationToken cancellationToken)
    {
        var granjas = await db.granjas
            .Where(g => g.activo)
            .Include(g => g.distrito_codigoNavigation)
                .ThenInclude(d => d.provincia_codigoNavigation)
                    .ThenInclude(p => p.departamento_codigoNavigation)
            .OrderBy(g => g.nombre)
            .Select(g => new
            {
                id = g.id,
                nombre = g.nombre,
                codigoDistrito = g.distrito_codigo,
                distrito = g.distrito_codigoNavigation != null ? g.distrito_codigoNavigation.nombre : null,
                provincia = g.distrito_codigoNavigation != null && g.distrito_codigoNavigation.provincia_codigoNavigation != null
                    ? g.distrito_codigoNavigation.provincia_codigoNavigation.nombre : null,
                departamento = g.distrito_codigoNavigation != null && g.distrito_codigoNavigation.provincia_codigoNavigation != null
                    && g.distrito_codigoNavigation.provincia_codigoNavigation.departamento_codigoNavigation != null
                    ? g.distrito_codigoNavigation.provincia_codigoNavigation.departamento_codigoNavigation.nombre : null
            })
            .ToListAsync(cancellationToken);

        return Ok(granjas);
    }

    [HttpGet("estadisticas/actividad")]
    [ProducesResponseType(typeof(VacunoActivityStatsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActivityStats(
        [FromQuery] System.DateOnly? fechaInicio,
        [FromQuery] System.DateOnly? fechaFin,
        CancellationToken cancellationToken)
    {
        var end = fechaFin ?? System.DateOnly.FromDateTime(System.DateTime.UtcNow);
        var start = fechaInicio ?? end.AddDays(-30);

        if (start > end)
        {
            return BadRequest("La fecha de inicio no puede ser posterior a la fecha de fin.");
        }

        var vacunos = await _activityStatsReadRepository.ListarHastaAsync(end, cancellationToken);
        var points = new List<VacunoActivityPointResponse>();

        for (var date = start; date <= end; date = date.AddDays(1))
        {
            var count = vacunos.Count(v =>
            {
                var isRegistered = v.FechaRegistro <= date;
                if (!isRegistered) return false;

                if (v.DeletedAt.HasValue)
                {
                    var deletionDate = System.DateOnly.FromDateTime(v.DeletedAt.Value);
                    return deletionDate > date;
                }

                return true;
            });

            points.Add(new VacunoActivityPointResponse(date.ToString("yyyy-MM-dd"), count));
        }

        var mayor = points.Any() ? points.Max(p => p.Cantidad) : 0;
        var menor = points.Any() ? points.Min(p => p.Cantidad) : 0;

        return Ok(new VacunoActivityStatsResponse(
            start.ToString("yyyy-MM-dd"),
            end.ToString("yyyy-MM-dd"),
            points,
            mayor,
            menor
        ));
    }

    private async Task<long> ResolveIdAsync(string identifier, CancellationToken cancellationToken)
    {
        if (long.TryParse(identifier, out long id))
        {
            return id;
        }

        var vacuno = await _vacunoRepository.GetByCodigoAsync(identifier, cancellationToken);
        if (vacuno != null)
        {
            return vacuno.Id;
        }

        return -1;
    }

    [HttpGet("{vacunoId:long}/reporte")]
    [ProducesResponseType(typeof(RegistroVacunoReporteApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetReporteIndividual(
        [FromRoute] long vacunoId,
        [FromQuery] RegistroVacunoReporteRequest request,
        CancellationToken cancellationToken)
    {
        var query = RegistroVacunoReporteMapper.ToApplicationQuery(vacunoId, request);
        var response = await _reporteUseCase.HandleAsync(query, cancellationToken);
        
        return Ok(GeneralResponseDTO<RegistroVacunoReporteApiResponse>.Ok(
            RegistroVacunoReporteMapper.ToResponse(response)));
    }

    [HttpGet("reportes/listado")]
    [ProducesResponseType(typeof(GeneralResponseDTO<ListadoVacunosReporteApiResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ReportesListado(
        [FromQuery] ListadoVacunosRequest request, 
        CancellationToken cancellationToken)
    {
        var response = await _listarVacunosReporteUseCase.HandleAsync(
            RegistroVacunoReporteMapper.ToApplicationQuery(request),
            cancellationToken);

        return Ok(GeneralResponseDTO<ListadoVacunosReporteApiResponse>.Ok(
            RegistroVacunoReporteMapper.ToResponse(response)));
    }
}
