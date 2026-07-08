using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Net;
using System.Text;
using ZooTech.Application.Modules.Animals.UseCases.ReportAnimalList;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.CreateVacuno;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.DeleteVacuno;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GetArbolGenealogico;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GetVacunoById;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunos;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ListarVacunosReporte;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.UpdateVacuno;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ExportarArbolGenealogico;
using ZooTech.Domain.Module_Vacuno.Interfaces;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.InterfaceAdapters.DTOs;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Responses;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Mappers;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Mappers.ReporteVacuno;
using ZooTech.Application.Common.Behaviors.Module_Vacuno.ListarVacunos;
using ZooTech.Application.Common.Behaviors.Module_Vacuno.CreateVacuno;
using ZooTech.Application.Common.Behaviors.Module_Vacuno.GetVacunoById;
using ZooTech.Application.Common.Behaviors.Module_Vacuno.UpdateVacuno;
using ZooTech.Application.Common.Behaviors.Module_Vacuno.DeleteVacuno;
using ZooTech.Application.Common.Behaviors.Module_Vacuno.ExportarArbolGenealogico;
using ZooTech.Application.Common.Behaviors.Module_Vacuno.ReporteVacuno.ListarVacunosReporte;
using ZooTech.Application.Common.Behaviors.Module_Vacuno.GetArbolGenealogico;

namespace ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Controllers;

[ApiController]
[Route("api/v1/vacunos")]
[ApiExplorerSettings(GroupName = "public")]
public sealed class VacunoController : ControllerBase
{
    private readonly IListarVacunosBehaviorPipelineFactory _listarVacunosBehaviorPipelineFactory;
    private readonly ICreateVacunoBehaviorPipelineFactory _createVacunoBehaviorPipelineFactory;
    private readonly IGetVacunoByIdBehaviorPipelineFactory _getVacunoByIdBehaviorPipelineFactory;
    private readonly IUpdateVacunoBehaviorPipelineFactory _updateVacunoBehaviorPipelineFactory;
    private readonly IDeleteVacunoBehaviorPipelineFactory _deleteVacunoBehaviorPipelineFactory;
    private readonly IExportarArbolGenealogicoBehaviorPipelineFactory _exportarArbolGenealogicoBehaviorPipelineFactory;
    private readonly IListarVacunosReporteBehaviorPipelineFactory _listarVacunosReporteBehaviorPipelineFactory;
    private readonly IGetArbolGenealogicoBehaviorPipelineFactory _getArbolGenealogicoBehaviorPipelineFactory;
    private readonly IVacunoRepository _vacunoRepository;
    private readonly IAnimalReportExcelService _animalReportExcelService;
    private readonly IAnimalReportPdfService _animalReportPdfService;

    public VacunoController(
        IListarVacunosBehaviorPipelineFactory listarVacunosBehaviorPipelineFactory,
        ICreateVacunoBehaviorPipelineFactory createVacunoBehaviorPipelineFactory,
        IGetVacunoByIdBehaviorPipelineFactory getVacunoByIdBehaviorPipelineFactory,
        IUpdateVacunoBehaviorPipelineFactory updateVacunoBehaviorPipelineFactory,
        IDeleteVacunoBehaviorPipelineFactory deleteVacunoBehaviorPipelineFactory,
        IExportarArbolGenealogicoBehaviorPipelineFactory exportarArbolGenealogicoBehaviorPipelineFactory,
        IListarVacunosReporteBehaviorPipelineFactory listarVacunosReporteBehaviorPipelineFactory,
        IGetArbolGenealogicoBehaviorPipelineFactory getArbolGenealogicoBehaviorPipelineFactory,
        IVacunoRepository vacunoRepository,
        IAnimalReportExcelService animalReportExcelService,
        IAnimalReportPdfService animalReportPdfService)
    {
        _listarVacunosBehaviorPipelineFactory = listarVacunosBehaviorPipelineFactory;
        _createVacunoBehaviorPipelineFactory = createVacunoBehaviorPipelineFactory;
        _getVacunoByIdBehaviorPipelineFactory = getVacunoByIdBehaviorPipelineFactory;
        _updateVacunoBehaviorPipelineFactory = updateVacunoBehaviorPipelineFactory;
        _deleteVacunoBehaviorPipelineFactory = deleteVacunoBehaviorPipelineFactory;
        _exportarArbolGenealogicoBehaviorPipelineFactory = exportarArbolGenealogicoBehaviorPipelineFactory;
        _listarVacunosReporteBehaviorPipelineFactory = listarVacunosReporteBehaviorPipelineFactory;
        _getArbolGenealogicoBehaviorPipelineFactory = getArbolGenealogicoBehaviorPipelineFactory;

        _vacunoRepository = vacunoRepository;
        _animalReportExcelService = animalReportExcelService;
        _animalReportPdfService = animalReportPdfService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<List<VacunoItemResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarVacunos(
    [FromQuery] string? search,
    [FromQuery] string? query,
    [FromQuery(Name = "q")] string? q,
    [FromQuery] DateTime? fechaDesde,
    [FromQuery] DateTime? fechaHasta,
    [FromQuery] string? estado,
    [FromQuery] int page = 1,
    [FromQuery] int? pageSize = null,
    [FromQuery] int? limit = null,
    CancellationToken cancellationToken = default)
    {
        var currentPage = NormalizePage(page);
        var currentPageSize = NormalizePageSize(pageSize ?? limit);
        var searchTerm = FirstNonBlank(search, query, q);

        var command = new ListarVacunosCommand(
            Query: searchTerm,
            FechaDesde: fechaDesde,
            FechaHasta: fechaHasta,
            Estado: estado,
            Page: currentPage,
            Limit: currentPageSize);

        var behaviorPipeline = _listarVacunosBehaviorPipelineFactory.Create();

        var output = await behaviorPipeline.Execute(command, cancellationToken);
        var response = output.Items.Select(VacunoMapper.ToResponse).ToList();
        return Ok(PagedResponse<List<VacunoItemResponse>>.OkPaged(response, currentPage, currentPageSize, output.TotalCount));
    }

    [HttpGet("{id:long}/genealogia")]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetArbolGenealogico(
    [FromRoute] long id, [FromQuery] int niveles = 4, CancellationToken cancellationToken = default)
    {
        var behaviorPipeline = _getArbolGenealogicoBehaviorPipelineFactory.Create();

        var command = new GetArbolGenealogicoCommand(id, niveles);
        var output = await behaviorPipeline.Execute(command, cancellationToken);
        return Ok(GeneralResponseDTO<object>.Ok(output.Arbol));
    }

    [HttpGet("{id:long}/genealogia/exportar")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ExportarArbolGenealogico([FromRoute] long id, [FromQuery] int niveles = 4, CancellationToken cancellationToken = default)
    {
        var behaviorPipeline = _exportarArbolGenealogicoBehaviorPipelineFactory.Create();

        var command = new ExportarArbolGenealogicoCommand(id, niveles);

        var result = await behaviorPipeline.Execute(command, cancellationToken);
        return File(result.excel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Genealogia_{id}.xlsx");
    }

    [HttpPost]
    [ProducesResponseType(typeof(GeneralResponseDTO<VacunoResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreateVacunoRequest request,
        [FromServices] GanaderiaDbContext db,
        CancellationToken cancellationToken)
    {
        var (padreId, madreId, parentError) = await ResolveParentsAsync(
            request.CodigoPadre, request.CodigoMadre, request.Codigo.Trim(), db, cancellationToken);
        if (parentError != null)
        {
            return parentError;
        }

        var (granjaId, granjaError) = await ResolveGranjaAsync(
            request.GranjaId, request.Granja, request.CodigoDistrito, db, cancellationToken);
        if (granjaError != null)
        {
            return granjaError;
        }

        var behaviorPipeline = _createVacunoBehaviorPipelineFactory.Create();

        var command = VacunoMapper.ToCommand(request, padreId, madreId, granjaId);
        var output = await behaviorPipeline.Execute(command, cancellationToken);
        var response = await EnrichResponseAsync(output.Data, db, cancellationToken);
        return Created($"/api/v1/vacuno/{response.Id}", GeneralResponseDTO<VacunoResponse>.Ok(response));
    }

    [HttpGet("{identifier}")]
    [ProducesResponseType(typeof(GeneralResponseDTO<VacunoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        [FromRoute] string identifier,
        [FromServices] GanaderiaDbContext db,
        CancellationToken cancellationToken)
    {
        var behaviorPipeline = _getVacunoByIdBehaviorPipelineFactory.Create();

        long id = await ResolveIdAsync(identifier, cancellationToken);
        var output = await behaviorPipeline.Execute(
            new GetVacunoByIdCommand(id),
            cancellationToken
        );
        var response = await EnrichResponseAsync(output.Data, db, cancellationToken);
        return Ok(GeneralResponseDTO<VacunoResponse>.Ok(response));
    }

    [HttpPatch("{identifier}")]
    [ProducesResponseType(typeof(GeneralResponseDTO<VacunoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        [FromRoute] string identifier,
        [FromBody] UpdateVacunoRequest request,
        [FromServices] GanaderiaDbContext db,
        CancellationToken cancellationToken)
    {
        long id = await ResolveIdAsync(identifier, cancellationToken);
        var existingVacuno = await db.vacunos.FirstOrDefaultAsync(v => v.id == id && v.deleted_at == null, cancellationToken);
        if (existingVacuno == null)
        {
            return NotFound(GeneralResponseDTO<object>.Fail("El vacuno no existe."));
        }

        var ownCodigo = existingVacuno.codigo;

        var (padreId, madreId, parentError) = await ResolveParentsAsync(
            request.CodigoPadre, request.CodigoMadre, ownCodigo, db, cancellationToken);
        if (parentError != null)
        {
            return parentError;
        }

        var (granjaId, granjaError) = await ResolveGranjaAsync(
            request.GranjaId, request.Granja, request.CodigoDistrito, db, cancellationToken);
        if (granjaError != null)
        {
            return granjaError;
        }

        var behaviorPipeline = _updateVacunoBehaviorPipelineFactory.Create();

        var command = VacunoMapper.ToCommand(id, request, padreId, madreId, granjaId);
        var output = await behaviorPipeline.Execute(command, cancellationToken);
        var response = await EnrichResponseAsync(output.Data, db, cancellationToken);
        return Ok(GeneralResponseDTO<VacunoResponse>.Ok(response));
    }

    [HttpDelete("{identifier}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] string identifier, [FromBody] DeleteVacunoRequest request, CancellationToken cancellationToken)
    {
        var behaviorPipeline = _deleteVacunoBehaviorPipelineFactory.Create();

        long id = await ResolveIdAsync(identifier, cancellationToken);
        await behaviorPipeline.Execute(VacunoMapper.ToCommand(id, request), cancellationToken);
        return NoContent();
    }


    [HttpGet("reportes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult ReportesDisponibles(
        [FromQuery] string? fechaDesde,
        [FromQuery] string? fechaHasta)
    {
        return Ok(new
        {
            reportes = new[]
            {
                new
                {
                    id = "listado",
                    titulo = "Reporte de listado de vacunos",
                    descripcion = "Lista los vacunos registrados, sus datos principales y el estado actual del registro.",
                    disponible = true
                },
                new
                {
                    id = "registro",
                    titulo = "Reporte de registro por vacuno",
                    descripcion = "Presenta el historial registrado para un vacuno dentro del rango de fechas.",
                    disponible = true
                }
            },
            filtros = new
            {
                fechaDesde = ParseDateOrNull(fechaDesde),
                fechaHasta = ParseDateOrNull(fechaHasta)
            }
        });
    }

    [HttpGet("reportes/listado")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ReportesListado(
        [FromQuery] ListadoVacunosRequest request,
        CancellationToken cancellationToken)
    {
        var behaviorPipeline = _listarVacunosReporteBehaviorPipelineFactory.Create();

        var response = await behaviorPipeline.Execute(
            RegistroVacunoReporteMapper.ToApplicationQuery(request),
            cancellationToken);

        var normalizedFormato = NormalizeFormat(request.Formato);
        if (normalizedFormato is "excel" or "pdf")
        {
            var rows = await LoadAllListadoReporteItemsAsync(request, response.TotalCount, cancellationToken);
            var downloadUrl = await GenerateListadoReportFileAsync(
                rows,
                normalizedFormato,
                response.Filtros.FechaDesde,
                response.Filtros.FechaHasta,
                response.Filtros.Q,
                cancellationToken);

            response = response with { DownloadUrl = downloadUrl };
        }

        return Ok(RegistroVacunoReporteMapper.ToResponse(response));
    }

    [HttpGet("{vacunoId:long}/reporte")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReporteIndividual(
        [FromRoute] long vacunoId,
        [FromQuery] string? formato,
        [FromServices] GanaderiaDbContext db,
        CancellationToken cancellationToken)
    {
        var behaviorPipeline = _getVacunoByIdBehaviorPipelineFactory.Create();

        var output = await behaviorPipeline.Execute(
            new GetVacunoByIdCommand(vacunoId), 
            cancellationToken
        );
        var response = await EnrichResponseAsync(output.Data, db, cancellationToken);
        var normalizedFormato = NormalizeFormat(formato);
        var detalle = await BuildRegistroVacunoDetalleAsync(response, db, cancellationToken);

        string? downloadUrl = null;
        if (normalizedFormato is "excel" or "pdf")
        {
            downloadUrl = await GenerateRegistroReportFileAsync(detalle, normalizedFormato, cancellationToken);
        }

        return Ok(new
        {
            vacuno = detalle,
            historial = Array.Empty<object>(),
            downloadUrl
        });
    }

    [HttpGet("reportes/descargas/{fileName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DescargarReporte([FromRoute] string fileName)
    {
        var safeFileName = Path.GetFileName(fileName);
        var path = Path.Combine(GetReportOutputDirectory(), safeFileName);

        if (!System.IO.File.Exists(path))
        {
            return NotFound();
        }

        var contentType = safeFileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)
            ? "application/pdf"
            : safeFileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase)
                ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                : "text/csv";

        return PhysicalFile(path, contentType, safeFileName);
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

        var vacunos = await _vacunoRepository.ListAllWithDeletedAsync(cancellationToken);
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

    private async Task<VacunoResponse> EnrichResponseAsync(
        ZooTech.Application.Modules.Module_Vacuno.Common.VacunoOutput dto,
        GanaderiaDbContext db,
        CancellationToken cancellationToken)
    {
        string? codigoPadre = null;
        if (dto.PadreId.HasValue)
        {
            codigoPadre = await db.vacunos
                .Where(v => v.id == dto.PadreId.Value && v.deleted_at == null)
                .Select(v => v.codigo)
                .FirstOrDefaultAsync(cancellationToken);
        }

        string? codigoMadre = null;
        if (dto.MadreId.HasValue)
        {
            codigoMadre = await db.vacunos
                .Where(v => v.id == dto.MadreId.Value && v.deleted_at == null)
                .Select(v => v.codigo)
                .FirstOrDefaultAsync(cancellationToken);
        }

        string? granjaNombre = null;
        string? distritoNombre = null;
        string? provinciaNombre = null;
        string? departamentoNombre = null;
        string? codigoDistrito = null;

        var granja = await db.granjas
            .Include(g => g.distrito_codigoNavigation)
                .ThenInclude(d => d.provincia_codigoNavigation)
                    .ThenInclude(p => p.departamento_codigoNavigation)
            .FirstOrDefaultAsync(g => g.id == dto.GranjaId, cancellationToken);

        if (granja != null)
        {
            granjaNombre = granja.nombre;
            codigoDistrito = granja.distrito_codigo;
            distritoNombre = granja.distrito_codigoNavigation?.nombre;
            provinciaNombre = granja.distrito_codigoNavigation?.provincia_codigoNavigation?.nombre;
            departamentoNombre = granja.distrito_codigoNavigation?.provincia_codigoNavigation?.departamento_codigoNavigation?.nombre;
        }

        var utilizacion = await db.vacuno_utilizacion_historials
            .AsNoTracking()
            .Where(u => u.vacuno_id == dto.Id)
            .OrderByDescending(u => u.created_at)
            .Select(u => new { u.tipo_utilizacion_code, u.created_at })
            .FirstOrDefaultAsync(cancellationToken);

        return new VacunoResponse(
            dto.Id,
            dto.Codigo,
            dto.Nombre,
            dto.FechaNacimiento,
            dto.TipoAdquisicionCode,
            dto.RazaCode,
            dto.ColorCode,
            dto.SexoCode,
            dto.PadreId,
            dto.MadreId,
            dto.GranjaId,
            dto.Observaciones,
            dto.FechaRegistro,
            dto.CreatedAt,
            dto.UpdatedAt,
            codigoPadre,
            codigoMadre,
            granjaNombre,
            distritoNombre,
            provinciaNombre,
            departamentoNombre,
            codigoDistrito,
            utilizacion?.tipo_utilizacion_code,
            utilizacion?.created_at);
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

    private static bool IsMaleSexCode(string? sexoCode)
        => string.Equals(sexoCode, "macho", StringComparison.OrdinalIgnoreCase)
            || string.Equals(sexoCode, "M", StringComparison.OrdinalIgnoreCase);

    private static bool IsFemaleSexCode(string? sexoCode)
        => string.Equals(sexoCode, "hembra", StringComparison.OrdinalIgnoreCase)
            || string.Equals(sexoCode, "H", StringComparison.OrdinalIgnoreCase)
            || string.Equals(sexoCode, "F", StringComparison.OrdinalIgnoreCase);

    private IActionResult ValidationErrorResponse(
        string field,
        string message,
        string topMessage = "Los datos enviados no son válidos.")
    {
        return BadRequest(new
        {
            error = new
            {
                code = "VALIDATION_ERROR",
                message = topMessage,
                details = new[]
                {
                    new { field, message }
                }
            }
        });
    }

    private async Task<(long? PadreId, long? MadreId, IActionResult? Error)> ResolveParentsAsync(
        string? codigoPadre,
        string? codigoMadre,
        string ownCodigo,
        GanaderiaDbContext db,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(codigoPadre) && codigoPadre.Trim() == ownCodigo)
        {
            return (null, null, ValidationErrorResponse("codigoPadre", "Un vacuno no puede ser su propio padre."));
        }

        if (!string.IsNullOrWhiteSpace(codigoMadre) && codigoMadre.Trim() == ownCodigo)
        {
            return (null, null, ValidationErrorResponse("codigoMadre", "Un vacuno no puede ser su propia madre."));
        }

        long? padreId = null;
        if (!string.IsNullOrWhiteSpace(codigoPadre))
        {
            var padre = await db.vacunos.FirstOrDefaultAsync(v => v.codigo == codigoPadre.Trim()
                && v.deleted_at == null
                && !db.v_vacuno_estado_vigentes.Any(e => e.vacuno_id == v.id && e.estado_code == "MUERTO"), cancellationToken);
            if (padre == null)
            {
                return (null, null, ValidationErrorResponse("codigoPadre", "El vacuno padre especificado no existe."));
            }
            if (!IsMaleSexCode(padre.sexo_code))
            {
                return (null, null, ValidationErrorResponse(
                    "codigoPadre",
                    "El padre debe ser un vacuno macho activo.",
                    "Los datos enviados no son validos."));
            }
            padreId = padre.id;
        }

        long? madreId = null;
        if (!string.IsNullOrWhiteSpace(codigoMadre))
        {
            var madre = await db.vacunos.FirstOrDefaultAsync(v => v.codigo == codigoMadre.Trim()
                && v.deleted_at == null
                && !db.v_vacuno_estado_vigentes.Any(e => e.vacuno_id == v.id && e.estado_code == "MUERTO"), cancellationToken);
            if (madre == null)
            {
                return (null, null, ValidationErrorResponse("codigoMadre", "El vacuno madre especificado no existe."));
            }
            if (!IsFemaleSexCode(madre.sexo_code))
            {
                return (null, null, ValidationErrorResponse(
                    "codigoMadre",
                    "La madre debe ser un vacuno hembra activo.",
                    "Los datos enviados no son validos."));
            }
            madreId = madre.id;
        }

        return (padreId, madreId, null);
    }

    private async Task<(long GranjaId, IActionResult? Error)> ResolveGranjaAsync(
        long? granjaIdRequest,
        string? granjaNombreRequest,
        string? codigoDistritoRequest,
        GanaderiaDbContext db,
        CancellationToken cancellationToken)
    {
        long granjaId = 0;
        if (granjaIdRequest.HasValue && granjaIdRequest.Value > 0)
        {
            var granjaExiste = await db.granjas.AnyAsync(g => g.id == granjaIdRequest.Value && g.activo, cancellationToken);
            if (!granjaExiste)
            {
                return (0, ValidationErrorResponse("granjaId", "La granja seleccionada no existe o no está activa."));
            }
            granjaId = granjaIdRequest.Value;
        }
        else
        {
            var granjaNombre = granjaNombreRequest?.Trim();
            var distritoCodigo = codigoDistritoRequest?.Trim();
            if (!string.IsNullOrWhiteSpace(granjaNombre) && !string.IsNullOrWhiteSpace(distritoCodigo))
            {
                var distritoExists = await db.geo_distritos.AnyAsync(d => d.codigo == distritoCodigo, cancellationToken);
                if (!distritoExists)
                {
                    return (0, ValidationErrorResponse("codigoDistrito", "El distrito especificado no es válido o no está registrado."));
                }

                var granja = await db.granjas.FirstOrDefaultAsync(g => g.nombre == granjaNombre && g.distrito_codigo == distritoCodigo, cancellationToken);
                if (granja == null)
                {
                    granja = new ZooTech.Infrastructure.Persistence.Entities.granja
                    {
                        nombre = granjaNombre,
                        distrito_codigo = distritoCodigo,
                        activo = true,
                        created_at = DateTime.UtcNow,
                        updated_at = DateTime.UtcNow
                    };
                    db.granjas.Add(granja);
                    await db.SaveChangesAsync(cancellationToken);
                }
                granjaId = granja.id;
            }
        }

        if (granjaId <= 0)
        {
            return (0, ValidationErrorResponse("granjaId", "La granja seleccionada no es válida o no ha sido especificada."));
        }

        return (granjaId, null);
    }

    private async Task<object> BuildRegistroVacunoDetalleAsync(
        VacunoResponse response,
        GanaderiaDbContext db,
        CancellationToken cancellationToken)
    {
        string? codigoAbuelo = null;
        string? codigoAbuela = null;

        if (response.PadreId.HasValue)
        {
            var padre = await db.vacunos
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.id == response.PadreId.Value, cancellationToken);

            if (padre?.padre_id is not null)
            {
                codigoAbuelo = await db.vacunos
                    .Where(v => v.id == padre.padre_id.Value)
                    .Select(v => v.codigo)
                    .FirstOrDefaultAsync(cancellationToken);
            }

            if (padre?.madre_id is not null)
            {
                codigoAbuela = await db.vacunos
                    .Where(v => v.id == padre.madre_id.Value)
                    .Select(v => v.codigo)
                    .FirstOrDefaultAsync(cancellationToken);
            }
        }

        return new
        {
            id = response.Id,
            codigo = response.Codigo,
            nombre = response.Nombre,
            fechaNacimiento = response.FechaNacimiento.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            adquisicionPor = response.TipoAdquisicionCode,
            precioCompra = (decimal?)null,
            raza = response.RazaCode,
            color = response.ColorCode,
            sexo = (string?)null,
            codigoPadre = response.CodigoPadre,
            codigoMadre = response.CodigoMadre,
            codigoAbuelo,
            codigoAbuela,
            granja = response.Granja,
            distrito = response.Distrito,
            departamento = response.Departamento,
            provincia = response.Provincia,
            procedencia = response.Granja,
            aptoPara = (string?)null,
            fechaEspecificacion = response.FechaRegistro.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            observaciones = response.Observaciones,
            fotoUrl = (string?)null,
            estado = "vivo",
            fechaRegistro = response.FechaRegistro.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            creadoEn = response.CreatedAt.ToString("O", CultureInfo.InvariantCulture),
            actualizadoEn = response.UpdatedAt.ToString("O", CultureInfo.InvariantCulture)
        };
    }

    private async Task<IReadOnlyCollection<VacunoListadoReporteItem>> LoadAllListadoReporteItemsAsync(
        ListadoVacunosRequest request,
        int totalCount,
        CancellationToken cancellationToken)
    {
        const int exportPageSize = 100;

        if (totalCount <= 0)
        {
            return [];
        }

        var rows = new List<VacunoListadoReporteItem>(totalCount);
        var totalPages = (int)Math.Ceiling((double)totalCount / exportPageSize);

        for (var page = 1; page <= totalPages; page++)
        {
            var behaviorPipeline = _listarVacunosReporteBehaviorPipelineFactory.Create();

            var pageRequest = CloneListadoRequest(request, page, exportPageSize);
            var pageResponse = await behaviorPipeline.Execute(
                RegistroVacunoReporteMapper.ToApplicationQuery(pageRequest),
                cancellationToken);

            rows.AddRange(pageResponse.Data);
        }

        return rows;
    }

    private static ListadoVacunosRequest CloneListadoRequest(
        ListadoVacunosRequest request,
        int page,
        int pageSize)
    {
        return new ListadoVacunosRequest
        {
            FechaDesde = request.FechaDesde,
            FechaHasta = request.FechaHasta,
            Search = request.Search,
            Q = request.Q,
            Codigo = request.Codigo,
            FechaRegistro = request.FechaRegistro,
            Nombre = request.Nombre,
            Raza = request.Raza,
            Procedencia = request.Procedencia,
            Estado = request.Estado,
            EstadoRegistro = request.EstadoRegistro,
            AptoPara = request.AptoPara,
            Formato = "json",
            Page = page.ToString(CultureInfo.InvariantCulture),
            PageSize = pageSize.ToString(CultureInfo.InvariantCulture)
        };
    }

    private async Task<string> GenerateListadoReportFileAsync(
        IReadOnlyCollection<VacunoListadoReporteItem> rows,
        string formato,
        DateOnly? fechaDesde,
        DateOnly? fechaHasta,
        string? keyword,
        CancellationToken cancellationToken)
    {
        var fileName = $"reporte_listado_vacunos_{DateTime.UtcNow:yyyyMMddHHmmss}.{(formato == "pdf" ? "pdf" : "xlsx")}";
        var path = Path.Combine(GetReportOutputDirectory(), fileName);
        var output = BuildAnimalListReportOutput(rows, fechaDesde, fechaHasta, keyword);

        if (formato == "pdf")
        {
            await System.IO.File.WriteAllBytesAsync(
                path,
                _animalReportPdfService.GenerateAnimalListPdf(output),
                cancellationToken);
        }
        else
        {
            await System.IO.File.WriteAllBytesAsync(
                path,
                _animalReportExcelService.GenerateAnimalListExcel(output),
                cancellationToken);
        }

        return $"/api/v1/vacunos/reportes/descargas/{Uri.EscapeDataString(fileName)}";
    }

    private async Task<string> GenerateRegistroReportFileAsync(
        object detalle,
        string formato,
        CancellationToken cancellationToken)
    {
        dynamic item = detalle;
        var fileName = $"reporte_registro_vacuno_{item.codigo}_{DateTime.UtcNow:yyyyMMddHHmmss}.{(formato == "pdf" ? "pdf" : "csv")}";
        var path = Path.Combine(GetReportOutputDirectory(), fileName);
        var content = BuildRegistroReportContent(detalle);

        if (formato == "pdf")
        {
            await System.IO.File.WriteAllBytesAsync(path, BuildSimplePdf("Reporte registro por vacuno", content), cancellationToken);
        }
        else
        {
            await System.IO.File.WriteAllTextAsync(path, content, Encoding.UTF8, cancellationToken);
        }

        return $"/api/v1/vacunos/reportes/descargas/{Uri.EscapeDataString(fileName)}";
    }

    private static ReportAnimalListOutput BuildAnimalListReportOutput(
        IReadOnlyCollection<VacunoListadoReporteItem> rows,
        DateOnly? fechaDesde,
        DateOnly? fechaHasta,
        string? keyword)
    {
        var items = rows
            .Select(item => new ReportAnimalListItem(
                item.Codigo,
                item.Nombre,
                item.FechaNacimiento,
                item.TipoAdquisicion ?? string.Empty,
                item.Raza ?? string.Empty,
                item.Color ?? string.Empty,
                item.Sexo ?? string.Empty,
                item.Granja ?? item.Procedencia ?? string.Empty,
                item.Estado,
                item.FechaRegistro))
            .ToList();

        var dates = items.Select(item => item.FechaRegistro).ToList();
        var fallbackDate = DateOnly.FromDateTime(DateTime.UtcNow);

        return new ReportAnimalListOutput(
            fechaDesde ?? (dates.Count > 0 ? dates.Min() : fallbackDate),
            fechaHasta ?? (dates.Count > 0 ? dates.Max() : fallbackDate),
            string.IsNullOrWhiteSpace(keyword) ? null : keyword.Trim(),
            null,
            null,
            null,
            null,
            null,
            null,
            items);
    }

    private static string BuildRegistroReportContent(object detalle)
    {
        dynamic item = detalle;
        var sb = new StringBuilder();
        sb.AppendLine("Campo,Valor");
        sb.AppendLine($"Codigo,{Csv(item.codigo)}");
        sb.AppendLine($"Nombre,{Csv(item.nombre)}");
        sb.AppendLine($"Fecha nacimiento,{Csv(item.fechaNacimiento)}");
        sb.AppendLine($"Raza,{Csv(item.raza)}");
        sb.AppendLine($"Color,{Csv(item.color)}");
        sb.AppendLine($"Sexo,{Csv(item.sexo)}");
        sb.AppendLine($"Padre,{Csv(item.codigoPadre)}");
        sb.AppendLine($"Madre,{Csv(item.codigoMadre)}");
        sb.AppendLine($"Procedencia,{Csv(item.procedencia)}");
        sb.AppendLine($"Observaciones,{Csv(item.observaciones)}");
        return sb.ToString();
    }

    private static byte[] BuildSimplePdf(string title, string content)
    {
        var lines = new[] { title, $"Generado: {DateTime.Now:yyyy-MM-dd HH:mm:ss}" }
            .Concat(content.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries))
            .Take(44)
            .Select((line, index) => $"BT /F1 9 Tf 50 {760 - (index * 16)} Td ({PdfText(line)}) Tj ET");
        var streamContent = string.Join("\n", lines);
        var streamBytes = Encoding.ASCII.GetBytes(streamContent);
        var objects = new List<string>
        {
            "1 0 obj << /Type /Catalog /Pages 2 0 R >> endobj\n",
            "2 0 obj << /Type /Pages /Kids [3 0 R] /Count 1 >> endobj\n",
            "3 0 obj << /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Resources << /Font << /F1 4 0 R >> >> /Contents 5 0 R >> endobj\n",
            "4 0 obj << /Type /Font /Subtype /Type1 /BaseFont /Helvetica >> endobj\n",
            $"5 0 obj << /Length {streamBytes.Length} >> stream\n{streamContent}\nendstream endobj\n"
        };

        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream, Encoding.ASCII, leaveOpen: true);
        writer.Write("%PDF-1.4\n");

        var offsets = new List<long> { 0 };
        foreach (var obj in objects)
        {
            writer.Flush();
            offsets.Add(stream.Position);
            writer.Write(obj);
        }

        writer.Flush();
        var xrefPosition = stream.Position;
        writer.WriteLine("xref");
        writer.WriteLine($"0 {objects.Count + 1}");
        writer.WriteLine("0000000000 65535 f ");

        foreach (var offset in offsets.Skip(1))
        {
            writer.WriteLine($"{offset:0000000000} 00000 n ");
        }

        writer.WriteLine("trailer");
        writer.WriteLine($"<< /Size {objects.Count + 1} /Root 1 0 R >>");
        writer.WriteLine("startxref");
        writer.WriteLine(xrefPosition);
        writer.WriteLine("%%EOF");
        writer.Flush();

        return stream.ToArray();
    }

    private static string GetReportOutputDirectory()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "reportes", "vacunos");
        Directory.CreateDirectory(path);
        return path;
    }

    private static DateOnly? ParseDateOrNull(string? value)
    {
        return DateOnly.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)
            ? date
            : null;
    }

    private static int NormalizePage(int page)
        => page <= 0 ? 1 : page;

    private static int NormalizePageSize(int? pageSize)
        => !pageSize.HasValue || pageSize.Value <= 0
            ? 20
            : Math.Min(pageSize.Value, 100);

    private static string? FirstNonBlank(params string?[] values)
        => values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value))?.Trim();

    private static string NormalizeFormat(string? formato)
    {
        var normalized = formato?.Trim().ToLowerInvariant();
        return normalized is "excel" or "pdf" ? normalized : "json";
    }

    private static string Csv(object? value)
    {
        var text = Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
        return $"\"{text.Replace("\"", "\"\"", StringComparison.Ordinal)}\"";
    }

    private static string PdfText(string value)
    {
        return WebUtility.HtmlDecode(value)
            .Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("(", "\\(", StringComparison.Ordinal)
            .Replace(")", "\\)", StringComparison.Ordinal);
    }

    [HttpGet("catalogos")]
    [ProducesResponseType(typeof(GeneralResponseDTO<VacunoCatalogsResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCatalogos(CancellationToken cancellationToken)
    {
        var catalogs = await _vacunoRepository.GetCatalogsAsync(cancellationToken);
        return Ok(GeneralResponseDTO<VacunoCatalogsResponse>.Ok(VacunoMapper.ToResponse(catalogs)));
    }

    [HttpGet("referencias")]
    [ProducesResponseType(typeof(GeneralResponseDTO<List<VacunoReferenceResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarReferencias(CancellationToken cancellationToken)
    {
        var items = await _vacunoRepository.ListReferencesAsync(cancellationToken);
        return Ok(GeneralResponseDTO<List<VacunoReferenceResponse>>.Ok(items.Select(VacunoMapper.ToResponse).ToList()));
    }
}
