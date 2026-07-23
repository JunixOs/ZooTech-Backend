using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text.Json;
// Removed obsolete Animals import
using ZooTech.Application.Modules.Module_Vacuno.UseCases.CreateVacuno;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.DeleteVacuno;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GetArbolGenealogico;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GetVacunoById;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunos;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ListarVacunosReporte;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.UpdateVacuno;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ExportarArbolGenealogico;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ExportarActividadVacunos;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Interfaces;
// Removed GanaderiaDbContext dependency
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
using ZooTech.Application.Common.Behaviors.Module_Vacuno.ExportarActividadVacunos;
using ZooTech.Application.Common.Behaviors.Module_Vacuno.ReporteVacuno.ListarVacunosReporte;
using ZooTech.Application.Common.Behaviors.Module_Vacuno.ReporteVacuno.ObtenerRegistroVacunoReporte;
using ZooTech.Application.Common.Behaviors.Module_Vacuno.GetArbolGenealogico;
using ZooTech.Application.Common.Behaviors.Module_Vacuno.GetActivityStats;
using ZooTech.Application.Common.Gateway.Reports;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GetActivityStats;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ObtenerRegistroVacunoReporte;
using ZooTech.Application.Modules.Module_Vacuno.Services;

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
    private readonly IObtenerRegistroVacunoReporteBehaviorPipelineFactory _obtenerRegistroVacunoReporteBehaviorPipelineFactory;
    private readonly IGetArbolGenealogicoBehaviorPipelineFactory _getArbolGenealogicoBehaviorPipelineFactory;
    private readonly IGetActivityStatsBehaviorPipelineFactory _getActivityStatsBehaviorPipelineFactory;
    private readonly IExportarActividadVacunosBehaviorPipelineFactory _exportarActividadVacunosBehaviorPipelineFactory;
    private readonly IReportFileStorage _reportFileStorage;
    private readonly IVacunoRepository _vacunoRepository;

    public VacunoController(
        IListarVacunosBehaviorPipelineFactory listarVacunosBehaviorPipelineFactory,
        ICreateVacunoBehaviorPipelineFactory createVacunoBehaviorPipelineFactory,
        IGetVacunoByIdBehaviorPipelineFactory getVacunoByIdBehaviorPipelineFactory,
        IUpdateVacunoBehaviorPipelineFactory updateVacunoBehaviorPipelineFactory,
        IDeleteVacunoBehaviorPipelineFactory deleteVacunoBehaviorPipelineFactory,
        IExportarArbolGenealogicoBehaviorPipelineFactory exportarArbolGenealogicoBehaviorPipelineFactory,
        IListarVacunosReporteBehaviorPipelineFactory listarVacunosReporteBehaviorPipelineFactory,
        IObtenerRegistroVacunoReporteBehaviorPipelineFactory obtenerRegistroVacunoReporteBehaviorPipelineFactory,
        IGetArbolGenealogicoBehaviorPipelineFactory getArbolGenealogicoBehaviorPipelineFactory,
        IGetActivityStatsBehaviorPipelineFactory getActivityStatsBehaviorPipelineFactory,
        IExportarActividadVacunosBehaviorPipelineFactory exportarActividadVacunosBehaviorPipelineFactory,
        IReportFileStorage reportFileStorage,
        IVacunoRepository vacunoRepository)
    {
        _listarVacunosBehaviorPipelineFactory = listarVacunosBehaviorPipelineFactory;
        _createVacunoBehaviorPipelineFactory = createVacunoBehaviorPipelineFactory;
        _getVacunoByIdBehaviorPipelineFactory = getVacunoByIdBehaviorPipelineFactory;
        _updateVacunoBehaviorPipelineFactory = updateVacunoBehaviorPipelineFactory;
        _deleteVacunoBehaviorPipelineFactory = deleteVacunoBehaviorPipelineFactory;
        _exportarArbolGenealogicoBehaviorPipelineFactory = exportarArbolGenealogicoBehaviorPipelineFactory;
        _listarVacunosReporteBehaviorPipelineFactory = listarVacunosReporteBehaviorPipelineFactory;
        _obtenerRegistroVacunoReporteBehaviorPipelineFactory = obtenerRegistroVacunoReporteBehaviorPipelineFactory;
        _getArbolGenealogicoBehaviorPipelineFactory = getArbolGenealogicoBehaviorPipelineFactory;
        _getActivityStatsBehaviorPipelineFactory = getActivityStatsBehaviorPipelineFactory;
        _exportarActividadVacunosBehaviorPipelineFactory = exportarActividadVacunosBehaviorPipelineFactory;
        _reportFileStorage = reportFileStorage;
        _vacunoRepository = vacunoRepository;
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
    public async Task<IActionResult> ExportarArbolGenealogico([FromRoute] long id, [FromQuery] int niveles = 4, [FromQuery] string formato = "excel", CancellationToken cancellationToken = default)
    {
        var behaviorPipeline = _exportarArbolGenealogicoBehaviorPipelineFactory.Create();

        var command = new ExportarArbolGenealogicoCommand(id, niveles, formato);

        var result = await behaviorPipeline.Execute(command, cancellationToken);
        return File(result.Bytes, result.ContentType, result.FileName);
    }

    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(GeneralResponseDTO<VacunoResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreateVacunoRequest request,
        [FromServices] IVacunoResponseReadRepository responseReadRepository,
        CancellationToken cancellationToken)
    {
        return await ExecuteCreateAsync(request, null, responseReadRepository, cancellationToken);
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [RequestFormLimits(MultipartBodyLengthLimit = VacunoPhotoUpload.MaxBytes + 1_048_576)]
    [ProducesResponseType(typeof(GeneralResponseDTO<VacunoResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateMultipart(
        [FromForm] VacunoMultipartRequest multipart,
        [FromServices] IVacunoResponseReadRepository responseReadRepository,
        CancellationToken cancellationToken)
    {
        var request = DeserializePayload<CreateVacunoRequest>(multipart.Payload);
        var photo = await ReadPhotoAsync(multipart.Foto, cancellationToken);
        return await ExecuteCreateAsync(request, photo, responseReadRepository, cancellationToken);
    }

    [HttpGet("{identifier}")]
    [ProducesResponseType(typeof(GeneralResponseDTO<VacunoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        [FromRoute] string identifier,
        [FromServices] IVacunoResponseReadRepository responseReadRepository,
        CancellationToken cancellationToken)
    {
        var behaviorPipeline = _getVacunoByIdBehaviorPipelineFactory.Create();

        long id = await ResolveIdAsync(identifier, cancellationToken);
        var output = await behaviorPipeline.Execute(
            new GetVacunoByIdCommand(id),
            cancellationToken
        );
        var response = await EnrichResponseAsync(output.Data, responseReadRepository, cancellationToken);
        return Ok(GeneralResponseDTO<VacunoResponse>.Ok(response));
    }

    [HttpPatch("{identifier}")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(GeneralResponseDTO<VacunoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        [FromRoute] string identifier,
        [FromBody] UpdateVacunoRequest request,
        [FromServices] IVacunoResponseReadRepository responseReadRepository,
        CancellationToken cancellationToken)
    {
        long id = await ResolveIdAsync(identifier, cancellationToken);
        if (id <= 0)
        {
            return NotFound(GeneralResponseDTO<object>.Fail("El vacuno no existe."));
        }

        return await ExecuteUpdateAsync(
            id,
            request,
            null,
            responseReadRepository,
            cancellationToken);
    }

    [HttpPatch("{identifier}")]
    [Consumes("multipart/form-data")]
    [RequestFormLimits(MultipartBodyLengthLimit = VacunoPhotoUpload.MaxBytes + 1_048_576)]
    [ProducesResponseType(typeof(GeneralResponseDTO<VacunoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMultipart(
        [FromRoute] string identifier,
        [FromForm] VacunoMultipartRequest multipart,
        [FromServices] IVacunoResponseReadRepository responseReadRepository,
        CancellationToken cancellationToken)
    {
        var id = await ResolveIdAsync(identifier, cancellationToken);
        if (id <= 0)
        {
            return NotFound(GeneralResponseDTO<object>.Fail("El vacuno no existe."));
        }

        var request = DeserializePayload<UpdateVacunoRequest>(multipart.Payload);
        var photo = await ReadPhotoAsync(multipart.Foto, cancellationToken);
        return await ExecuteUpdateAsync(
            id,
            request,
            photo,
            responseReadRepository,
            cancellationToken);
    }

    [HttpGet("{id:long}/foto")]
    [Produces("image/jpeg", "image/png")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPhoto(
        [FromRoute] long id,
        [FromServices] IVacunoResponseReadRepository responseReadRepository,
        [FromServices] IVacunoPhotoStorage photoStorage,
        CancellationToken cancellationToken)
    {
        var photo = await responseReadRepository.GetPhotoAsync(id, cancellationToken);
        if (photo is null)
        {
            return NotFound();
        }

        var content = await photoStorage.ReadAsync(photo.RelativePath, cancellationToken);
        if (content is null)
        {
            return NotFound();
        }

        Response.Headers.CacheControl = "private,max-age=300";
        return File(content.Content, content.ContentType);
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

        return Ok(RegistroVacunoReporteMapper.ToResponse(response));
    }

    [HttpGet("{vacunoId:long}/reporte")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReporteIndividual(
        [FromRoute] long vacunoId,
        [FromQuery] string? formato,
        CancellationToken cancellationToken)
    {
        var behaviorPipeline = _obtenerRegistroVacunoReporteBehaviorPipelineFactory.Create();
        var response = await behaviorPipeline.Execute(
            new ObtenerRegistroVacunoReporteQuery(vacunoId, formato),
            cancellationToken);

        return Ok(RegistroVacunoReporteMapper.ToResponse(response));
    }

    [HttpGet("reportes/descargas/{fileName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DescargarReporte(
        [FromRoute] string fileName,
        CancellationToken cancellationToken)
    {
        var report = await _reportFileStorage.ReadAsync(fileName, cancellationToken);
        if (report is null)
        {
            return NotFound();
        }

        return File(report.Content, report.ContentType, report.FileName);
    }

    [HttpGet("granjas")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarGranjas(
        [FromServices] IVacunoGranjaReadRepository granjaReadRepository,
        CancellationToken cancellationToken)
    {
        var granjas = await granjaReadRepository.ListarActivasAsync(cancellationToken);
        var response = granjas.Select(g => new
        {
            id = g.Id,
            nombre = g.Nombre,
            codigoDistrito = g.CodigoDistrito,
            distrito = g.Distrito,
            provincia = g.Provincia,
            departamento = g.Departamento
        });

        return Ok(response);
    }

    [HttpGet("estadisticas/actividad")]
    [ProducesResponseType(typeof(VacunoActivityStatsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActivityStats(
        [FromQuery] System.DateOnly? fechaInicio,
        [FromQuery] System.DateOnly? fechaFin,
        CancellationToken cancellationToken)
    {
        var query = new GetActivityStatsQuery(fechaInicio, fechaFin);
        var pipeline = _getActivityStatsBehaviorPipelineFactory.Create();
        var output = await pipeline.Execute(query, cancellationToken);

        return Ok(new VacunoActivityStatsResponse(
            output.FechaInicio,
            output.FechaFin,
            output.Points.Select(p => new VacunoActivityPointResponse(p.Fecha, p.Cantidad)).ToList(),
            output.Mayor,
            output.Menor
        ));
    }

    [HttpGet("estadisticas/actividad/exportar")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ExportActivityStats(
        [FromQuery] DateOnly? fechaInicio,
        [FromQuery] DateOnly? fechaFin,
        [FromQuery] string formato,
        CancellationToken cancellationToken)
    {
        var query = new ExportarActividadVacunosQuery(fechaInicio, fechaFin, formato);
        var pipeline = _exportarActividadVacunosBehaviorPipelineFactory.Create();
        var document = await pipeline.Execute(query, cancellationToken);

        return File(document.Content, document.ContentType, document.FileName);
    }

    private async Task<VacunoResponse> EnrichResponseAsync(
        ZooTech.Application.Modules.Module_Vacuno.Common.VacunoOutput dto,
        IVacunoResponseReadRepository responseReadRepository,
        CancellationToken cancellationToken)
    {
        var idsToResolve = new List<long>();
        if (dto.PadreId.HasValue) idsToResolve.Add(dto.PadreId.Value);
        if (dto.MadreId.HasValue) idsToResolve.Add(dto.MadreId.Value);

        var codes = await responseReadRepository.GetActiveCodesByIdsAsync(idsToResolve, cancellationToken);
        string? codigoPadre = dto.PadreId.HasValue ? codes.GetValueOrDefault(dto.PadreId.Value) : null;
        string? codigoMadre = dto.MadreId.HasValue ? codes.GetValueOrDefault(dto.MadreId.Value) : null;

        string? granjaNombre = null;
        string? distritoNombre = null;
        string? provinciaNombre = null;
        string? departamentoNombre = null;
        string? codigoDistrito = null;

        var granja = await responseReadRepository.GetGranjaDetailsAsync(dto.GranjaId, cancellationToken);
        if (granja != null)
        {
            granjaNombre = granja.Nombre;
            codigoDistrito = granja.CodigoDistrito;
            distritoNombre = granja.Distrito;
            provinciaNombre = granja.Provincia;
            departamentoNombre = granja.Departamento;
        }

        var utilizacion = await responseReadRepository.GetLatestUtilizacionAsync(dto.Id, cancellationToken);
        var precioCompra = await responseReadRepository.GetPrecioCompraAsync(dto.Id, cancellationToken);
        var photo = await responseReadRepository.GetPhotoAsync(dto.Id, cancellationToken);

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
            utilizacion?.TipoUtilizacionCode,
            utilizacion?.CreatedAt,
            precioCompra,
            photo is null ? null : $"/api/v1/vacunos/{dto.Id}/foto");
    }

    private async Task<IActionResult> ExecuteCreateAsync(
        CreateVacunoRequest request,
        VacunoPhotoUpload? photo,
        IVacunoResponseReadRepository responseReadRepository,
        CancellationToken cancellationToken)
    {
        var behaviorPipeline = _createVacunoBehaviorPipelineFactory.Create();
        var command = VacunoMapper.ToCommand(request, photo);
        var output = await behaviorPipeline.Execute(command, cancellationToken);
        var response = await EnrichResponseAsync(output.Data, responseReadRepository, cancellationToken);
        return Created(
            $"/api/v1/vacunos/{response.Id}",
            GeneralResponseDTO<VacunoResponse>.Ok(response));
    }

    private async Task<IActionResult> ExecuteUpdateAsync(
        long id,
        UpdateVacunoRequest request,
        VacunoPhotoUpload? photo,
        IVacunoResponseReadRepository responseReadRepository,
        CancellationToken cancellationToken)
    {
        var behaviorPipeline = _updateVacunoBehaviorPipelineFactory.Create();
        var command = VacunoMapper.ToCommand(id, request, photo);
        var output = await behaviorPipeline.Execute(command, cancellationToken);
        var response = await EnrichResponseAsync(output.Data, responseReadRepository, cancellationToken);
        return Ok(GeneralResponseDTO<VacunoResponse>.Ok(response));
    }

    private static T DeserializePayload<T>(string payload)
    {
        if (string.IsNullOrWhiteSpace(payload))
        {
            throw new ArgumentException("El contenido del formulario es obligatorio.", nameof(payload));
        }

        try
        {
            return JsonSerializer.Deserialize<T>(
                       payload,
                       new JsonSerializerOptions(JsonSerializerDefaults.Web))
                   ?? throw new ArgumentException(
                       "El contenido del formulario no es valido.",
                       nameof(payload));
        }
        catch (JsonException exception)
        {
            throw new ArgumentException(
                "El contenido del formulario no es un JSON valido.",
                nameof(payload),
                exception);
        }
    }

    private static async Task<VacunoPhotoUpload?> ReadPhotoAsync(
        IFormFile? photo,
        CancellationToken cancellationToken)
    {
        if (photo is null)
        {
            return null;
        }

        if (photo.Length <= 0 || photo.Length > VacunoPhotoUpload.MaxBytes)
        {
            throw new ArgumentException(
                $"La foto no puede superar {VacunoPhotoUpload.MaxBytes} bytes.",
                nameof(photo));
        }

        await using var stream = photo.OpenReadStream();
        using var buffer = new MemoryStream((int)photo.Length);
        await stream.CopyToAsync(buffer, cancellationToken);
        return new VacunoPhotoUpload(photo.FileName, photo.ContentType, buffer.ToArray());
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
