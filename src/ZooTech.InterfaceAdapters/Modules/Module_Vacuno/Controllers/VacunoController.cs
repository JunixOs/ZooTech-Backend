using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Modules.Animals.UseCases.ReportAnimalList;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.DeleteVacuno;
using ZooTech.Application.Modules.Module_Vacuno.Common;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GenerarArbolGenealogico;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ExportarArbolGenealogico;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GetVacunoById;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GetVacunoCatalogs;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunosPaginado;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ListarVacunosReporte;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ObtenerRegistroVacunoReporte;
using ZooTech.Domain.Enums;
using ZooTech.Domain.Module_Vacuno.Interfaces;
using ZooTech.InterfaceAdapters.DTOs;
using ZooTech.InterfaceAdapters.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Responses;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Mappers;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Mappers.ReporteVacuno;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Presenters;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Services;
using ZooTech.InterfaceAdapters.Presenters;
using ApiErrorDetail = ZooTech.InterfaceAdapters.DTOs.Responses.ErrorDetail;
using ApiErrorResponse = ZooTech.InterfaceAdapters.DTOs.Responses.ErrorResponse;
using ListadoVacunosReporteApiResponse = ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Responses.ListadoVacunosReporteResponse;
using RegistroVacunoReporteApiResponse = ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Responses.RegistroVacunoReporteResponse;

namespace ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Controllers;

[ApiController]
[Route("api/v1/vacunos")]
[ApiExplorerSettings(GroupName = "public")]
public sealed class VacunoController : ControllerBase
{
    private const string DeletedByHeaderName = "X-User-Id";

    private readonly IGetVacunoByIdInputPort _getByIdInputPort;
    private readonly IGetVacunoCatalogsInputPort _getCatalogsInputPort;
    private readonly IDeleteVacunoInputPort _deleteVacunoInputPort;
    private readonly IVacunoRepository _vacunoRepository;
    private readonly IVacunoActivityStatsReadRepository _activityStatsReadRepository;
    private readonly IVacunoGranjaReadRepository _granjaReadRepository;
    private readonly IObtenerRegistroVacunoReporteUseCase _reporteUseCase;
    private readonly IListarVacunosReporteUseCase _listarVacunosReporteUseCase;
    private readonly IVacunoMutationService _mutationService;
    private readonly IListarVacunosPaginadoInputPort _listarPaginadoInputPort;
    private readonly ListarVacunosPaginadoPresenter _listarPaginadoPresenter;
    private readonly IReportAnimalListInputPort _reportAnimalListInputPort;
    private readonly IExportarArbolGenealogicoInputPort _exportarArbolInputPort;

    public VacunoController(
        IGetVacunoByIdInputPort getByIdInputPort,
        IGetVacunoCatalogsInputPort getCatalogsInputPort,
        IDeleteVacunoInputPort deleteVacunoInputPort,
        IVacunoRepository vacunoRepository,
        IVacunoActivityStatsReadRepository activityStatsReadRepository,
        IVacunoGranjaReadRepository granjaReadRepository,
        IObtenerRegistroVacunoReporteUseCase reporteUseCase,
        IListarVacunosReporteUseCase listarVacunosReporteUseCase,
        IVacunoMutationService mutationService,
        IListarVacunosPaginadoInputPort listarPaginadoInputPort,
        ListarVacunosPaginadoPresenter listarPaginadoPresenter,
        IReportAnimalListInputPort reportAnimalListInputPort,
        IExportarArbolGenealogicoInputPort exportarArbolInputPort)
    {
        _getByIdInputPort = getByIdInputPort;
        _getCatalogsInputPort = getCatalogsInputPort;
        _deleteVacunoInputPort = deleteVacunoInputPort;
        _vacunoRepository = vacunoRepository;
        _activityStatsReadRepository = activityStatsReadRepository;
        _granjaReadRepository = granjaReadRepository;
        _reporteUseCase = reporteUseCase;
        _listarVacunosReporteUseCase = listarVacunosReporteUseCase;
        _mutationService = mutationService;
        _listarPaginadoInputPort = listarPaginadoInputPort;
        _listarPaginadoPresenter = listarPaginadoPresenter;
        _reportAnimalListInputPort = reportAnimalListInputPort;
        _exportarArbolInputPort = exportarArbolInputPort;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Listar(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 20,
        [FromQuery] DateTime? fechaDesde = null,
        [FromQuery] DateTime? fechaHasta = null,
        [FromQuery] string? estado = null,
        [FromQuery] string? q = null)
    {
        EstadoAnimal? estadoEnum = null;
        if (!string.IsNullOrEmpty(estado))
        {
            if (Enum.TryParse<EstadoAnimal>(estado, true, out var parsed))
            {
                estadoEnum = parsed;
            }
            else
            {
                return BadRequest(ApiErrorResponse.Create(
                    "VALIDATION_ERROR",
                    "Los datos enviados no son validos.",
                    new[]
                    {
                        new ApiErrorDetail
                        {
                            Field = "estado",
                            Message = "Valores permitidos: sano, enfermo, cuarentena, muerto."
                        }
                    }));
            }
        }

        var command = new ListarVacunosPaginadoCommand
        {
            Page = page,
            Limit = limit,
            FechaDesde = fechaDesde,
            FechaHasta = fechaHasta,
            Estado = estadoEnum,
            Q = q
        };

        await _listarPaginadoInputPort.Handle(command);
        return StatusCode(_listarPaginadoPresenter.StatusCode, _listarPaginadoPresenter.Response);
    }

    [HttpGet("catalogos")]
    [ProducesResponseType(typeof(GeneralResponseDTO<VacunoCatalogsResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCatalogos(CancellationToken cancellationToken)
    {
        var catalogs = await _getCatalogsInputPort.HandleAsync(cancellationToken);
        return Ok(GeneralResponseDTO<VacunoCatalogsResponse>.Ok(VacunoMapper.ToResponse(catalogs)));
    }

    [HttpGet("referencias")]
    [ProducesResponseType(typeof(GeneralResponseDTO<List<VacunoReferenceResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarReferencias(CancellationToken cancellationToken)
    {
        var items = await _vacunoRepository.ListReferencesAsync(cancellationToken);
        return Ok(GeneralResponseDTO<List<VacunoReferenceResponse>>.Ok(items.Select(VacunoMapper.ToResponse).ToList()));
    }

    [HttpPost]
    [ProducesResponseType(typeof(GeneralResponseDTO<VacunoResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
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
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        [FromRoute] string identifier,
        CancellationToken cancellationToken)
    {
        long id = await ResolveIdAsync(identifier, cancellationToken);
        if (id == -1)
        {
            return NotFound(ApiErrorResponse.Create("VACUNO_NOT_FOUND", $"No existe el vacuno con el identificador '{identifier}'."));
        }

        var output = await _getByIdInputPort.HandleAsync(id, cancellationToken);
        var response = VacunoMapper.ToResponse(output.Data);
        return Ok(GeneralResponseDTO<VacunoResponse>.Ok(response));
    }

    [HttpPatch("{identifier}")]
    [ProducesResponseType(typeof(GeneralResponseDTO<VacunoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        [FromRoute] string identifier,
        [FromBody] UpdateVacunoRequest request,
        CancellationToken cancellationToken)
    {
        long id = await ResolveIdAsync(identifier, cancellationToken);
        if (id == -1)
        {
            return NotFound(ApiErrorResponse.Create("VACUNO_NOT_FOUND", $"No existe el vacuno con el identificador '{identifier}'."));
        }

        var result = await _mutationService.UpdateAsync(id, request, cancellationToken);
        if (!result.Success)
        {
            return ToReferenceErrorResult(result.Error!);
        }

        return Ok(GeneralResponseDTO<VacunoResponse>.Ok(result.Response!));
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(
        [FromRoute] long id,
        [FromBody] DeleteVacunoRequest request,
        CancellationToken cancellationToken)
    {
        var command = VacunoMapper.ToCommand(request);
        await _deleteVacunoInputPort.HandleAsync(id, command, cancellationToken);
        return NoContent();
    }

    private long? GetDeletedByFromHeader()
    {
        if (!Request.Headers.TryGetValue(DeletedByHeaderName, out var values))
        {
            return null;
        }

        return long.TryParse(values.FirstOrDefault(), out var deletedBy)
            ? deletedBy
            : -1;
    }

    private IActionResult ToReferenceErrorResult(VacunoReferenceError error)
    {
        if (error.Kind == VacunoReferenceErrorKind.NotFound)
        {
            return NotFound(ApiErrorResponse.Create(
                "NOT_FOUND",
                error.Message));
        }

        return BadRequest(ApiErrorResponse.Create(
            "VALIDATION_ERROR",
            "Los datos enviados no son validos.",
            new[]
            {
                new ApiErrorDetail
                {
                    Field = error.Field ?? string.Empty,
                    Message = error.Message
                }
            }));
    }

    [HttpGet("granjas")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarGranjas(
        CancellationToken cancellationToken)
    {
        var granjas = await _granjaReadRepository.ListarActivasAsync(cancellationToken);

        return Ok(granjas.Select(g => new
        {
            id = g.Id,
            nombre = g.Nombre,
            codigoDistrito = g.CodigoDistrito,
            distrito = g.Distrito,
            provincia = g.Provincia,
            departamento = g.Departamento
        }));
    }

    [HttpGet("actividad")]
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
            return BadRequest(ApiErrorResponse.Create(
                "VALIDATION_ERROR",
                "Los datos enviados no son validos.",
                new[]
                {
                    new ApiErrorDetail
                    {
                        Field = "fechaInicio",
                        Message = "La fecha de inicio no puede ser posterior a la fecha de fin."
                    }
                }));
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
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
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

    [HttpGet("{id:long}/genealogia")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GenerarArbolGenealogico(
        [FromRoute] long id,
        [FromServices] IGenerarArbolGenealogicoInputPort arbolInputPort,
        [FromServices] GenerarArbolGenealogicoPresenter arbolPresenter,
        [FromQuery] int niveles = 4)
    {
        var command = new GenerarArbolGenealogicoCommand
        {
            VacunoId = id,
            Niveles = niveles
        };

        await arbolInputPort.Handle(command);
        return StatusCode(arbolPresenter.StatusCode, arbolPresenter.Response);
    }

    [HttpGet("reportes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetReportesDisponibles()
    {
        var reportes = new[]
        {
            new
            {
                id = "listado",
                nombre = "Reporte listado de vacunos",
                descripcion = "Listado de vacunos registrados con filtros por fecha, estado, procedencia, etc.",
                ruta = "/reportes/listado"
            },
            new
            {
                id = "registro",
                nombre = "Reporte de registro por vacuno",
                descripcion = "Detalle y ficha de vida individual de un vacuno específico.",
                ruta = "/{vacunoId}/reporte"
            },
            new
            {
                id = "genealogia",
                nombre = "Gráfico genealógico por vacuno",
                descripcion = "Árbol genealógico (padres, abuelos) en formato gráfico de un vacuno.",
                ruta = "/{vacunoId}/genealogia"
            },
            new
            {
                id = "actividad",
                nombre = "Gráfico de vacunos en actividad",
                descripcion = "Evolución histórica de vacunos activos y dados de baja.",
                ruta = "/actividad"
            }
        };

        return Ok(reportes);
    }

    [HttpGet("reportes/excel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ReportesExcel(
        [FromQuery] ReportAnimalListRequest request,
        CancellationToken cancellationToken)
    {
        return await ReportesListado(request, "excel", cancellationToken);
    }

    [HttpGet("reportes/pdf")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ReportesPdf(
        [FromQuery] ReportAnimalListRequest request,
        CancellationToken cancellationToken)
    {
        return await ReportesListado(request, "pdf", cancellationToken);
    }

    [HttpGet("reportes/listado")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ReportesListado(
        [FromQuery] ReportAnimalListRequest request,
        [FromQuery] string? formato,
        CancellationToken cancellationToken)
    {
        bool exportExcel = formato?.Equals("excel", StringComparison.OrdinalIgnoreCase) ?? false;
        bool exportPdf = formato?.Equals("pdf", StringComparison.OrdinalIgnoreCase) ?? false;

        var presenter = new ReportAnimalListPresenter();
        var command = new ReportAnimalListCommand(
            request.FechaInicio,
            request.FechaFin,
            request.Keyword,
            request.RazaCode,
            request.ColorCode,
            request.SexoCode,
            request.TipoAdquisicionCode,
            request.GranjaId,
            request.EstadoCode,
            exportExcel,
            exportPdf);

        await _reportAnimalListInputPort.Handle(command, presenter, cancellationToken);
        return presenter.Result;
    }

    [HttpGet("{id:long}/genealogia/exportar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ExportarArbolGenealogico(
        [FromRoute] long id,
        [FromQuery] int niveles = 4,
        CancellationToken cancellationToken = default)
    {
        var command = new ExportarArbolGenealogicoCommand(niveles);
        var bytes = await _exportarArbolInputPort.HandleAsync(id, command, cancellationToken);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"genealogia-{id}.xlsx");
    }
}
