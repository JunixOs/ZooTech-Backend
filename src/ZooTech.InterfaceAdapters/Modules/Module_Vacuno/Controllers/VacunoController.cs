using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Net;
using System.Text;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.CreateVacuno;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.DeleteVacuno;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GetVacunoById;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunos;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.UpdateVacuno;
using ZooTech.Domain.Module_Vacuno.Interfaces;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.InterfaceAdapters.DTOs;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Responses;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Mappers;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GenerarArbolGenealogico;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Presenters;

namespace ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Controllers;

[ApiController]
[Route("api/v1/vacuno")]
[ApiExplorerSettings(GroupName = "public")]
public sealed class VacunoController : ControllerBase
{
    private readonly IListarVacunosInputPort _listarInputPort;
    private readonly ICreateVacunoInputPort _createInputPort;
    private readonly IGetVacunoByIdInputPort _getByIdInputPort;
    private readonly IUpdateVacunoInputPort _updateInputPort;
    private readonly IDeleteVacunoInputPort _deleteInputPort;
    private readonly IVacunoRepository _vacunoRepository;

    public VacunoController(
        IListarVacunosInputPort listarInputPort,
        ICreateVacunoInputPort createInputPort,
        IGetVacunoByIdInputPort getByIdInputPort,
        IUpdateVacunoInputPort updateInputPort,
        IDeleteVacunoInputPort deleteInputPort,
        IVacunoRepository vacunoRepository)
    {
        _listarInputPort = listarInputPort;
        _createInputPort = createInputPort;
        _getByIdInputPort = getByIdInputPort;
        _updateInputPort = updateInputPort;
        _deleteInputPort = deleteInputPort;
        _vacunoRepository = vacunoRepository;
    }

    [HttpGet]
    [ProducesResponseType(typeof(GeneralResponseDTO<List<VacunoItemResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarVacunos(
        [FromQuery] string? q,
        [FromQuery] string? estado,
        [FromQuery] System.DateOnly? fechaDesde,
        [FromQuery] System.DateOnly? fechaHasta,
        CancellationToken cancellationToken)
    {
        var output = await _listarInputPort.HandleAsync(cancellationToken);
        var response = output.Items.Select(VacunoMapper.ToResponse).ToList();

        if (!string.IsNullOrWhiteSpace(q))
        {
            var query = q.Trim().ToLower();
            response = response.Where(x => 
                x.Codigo.ToLower().Contains(query) || 
                x.Nombre.ToLower().Contains(query)
            ).ToList();
        }

        if (!string.IsNullOrWhiteSpace(estado))
        {
            var filterEstado = estado.Trim().ToLower();
            response = response.Where(x => x.Estado == filterEstado).ToList();
        }

        if (fechaDesde.HasValue)
        {
            response = response.Where(x => x.FechaNacimiento >= fechaDesde.Value).ToList();
        }

        if (fechaHasta.HasValue)
        {
            response = response.Where(x => x.FechaNacimiento <= fechaHasta.Value).ToList();
        }

        return Ok(GeneralResponseDTO<List<VacunoItemResponse>>.Ok(response));
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
        if (!string.IsNullOrWhiteSpace(request.CodigoPadre) && request.CodigoPadre.Trim() == request.Codigo.Trim())
        {
            return BadRequest(new
            {
                error = new
                {
                    code = "VALIDATION_ERROR",
                    message = "Los datos enviados no son válidos.",
                    details = new[]
                    {
                        new { field = "codigoPadre", message = "Un vacuno no puede ser su propio padre." }
                    }
                }
            });
        }

        if (!string.IsNullOrWhiteSpace(request.CodigoMadre) && request.CodigoMadre.Trim() == request.Codigo.Trim())
        {
            return BadRequest(new
            {
                error = new
                {
                    code = "VALIDATION_ERROR",
                    message = "Los datos enviados no son válidos.",
                    details = new[]
                    {
                        new { field = "codigoMadre", message = "Un vacuno no puede ser su propia madre." }
                    }
                }
            });
        }

        long? padreId = null;
        if (!string.IsNullOrWhiteSpace(request.CodigoPadre))
        {
            var padre = await db.vacunos.FirstOrDefaultAsync(v => v.codigo == request.CodigoPadre.Trim() && v.deleted_at == null, cancellationToken);
            if (padre == null)
            {
                return BadRequest(new
                {
                    error = new
                    {
                        code = "VALIDATION_ERROR",
                        message = "Los datos enviados no son válidos.",
                        details = new[]
                        {
                            new { field = "codigoPadre", message = "El vacuno padre especificado no existe." }
                        }
                    }
                });
            }
            padreId = padre.id;
        }

        long? madreId = null;
        if (!string.IsNullOrWhiteSpace(request.CodigoMadre))
        {
            var madre = await db.vacunos.FirstOrDefaultAsync(v => v.codigo == request.CodigoMadre.Trim() && v.deleted_at == null, cancellationToken);
            if (madre == null)
            {
                return BadRequest(new
                {
                    error = new
                    {
                        code = "VALIDATION_ERROR",
                        message = "Los datos enviados no son válidos.",
                        details = new[]
                        {
                            new { field = "codigoMadre", message = "El vacuno madre especificado no existe." }
                        }
                    }
                });
            }
            madreId = madre.id;
        }

        long granjaId = 0;
        if (request.GranjaId.HasValue && request.GranjaId.Value > 0)
        {
            var granjaExiste = await db.granjas.AnyAsync(g => g.id == request.GranjaId.Value && g.activo, cancellationToken);
            if (!granjaExiste)
            {
                return BadRequest(new
                {
                    error = new
                    {
                        code = "VALIDATION_ERROR",
                        message = "Los datos enviados no son válidos.",
                        details = new[]
                        {
                            new { field = "granjaId", message = "La granja seleccionada no existe o no está activa." }
                        }
                    }
                });
            }
            granjaId = request.GranjaId.Value;
        }
        else
        {
            var granjaNombre = request.Granja?.Trim();
            var distritoCodigo = request.CodigoDistrito?.Trim();
            if (!string.IsNullOrWhiteSpace(granjaNombre) && !string.IsNullOrWhiteSpace(distritoCodigo))
            {
                var distritoExists = await db.geo_distritos.AnyAsync(d => d.codigo == distritoCodigo, cancellationToken);
                if (!distritoExists)
                {
                    return BadRequest(new
                    {
                        error = new
                        {
                            code = "VALIDATION_ERROR",
                            message = "Los datos enviados no son válidos.",
                            details = new[]
                            {
                                new { field = "codigoDistrito", message = "El distrito especificado no es válido o no está registrado." }
                            }
                        }
                    });
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
            return BadRequest(new
            {
                error = new
                {
                    code = "VALIDATION_ERROR",
                    message = "Los datos enviados no son válidos.",
                    details = new[]
                    {
                        new { field = "granjaId", message = "La granja seleccionada no es válida o no ha sido especificada." }
                    }
                }
            });
        }

        var command = VacunoMapper.ToCommand(request, padreId, madreId, granjaId);
        var output = await _createInputPort.HandleAsync(command, cancellationToken);
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
        long id = await ResolveIdAsync(identifier, cancellationToken);
        var output = await _getByIdInputPort.HandleAsync(id, cancellationToken);
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

        if (!string.IsNullOrWhiteSpace(request.CodigoPadre) && request.CodigoPadre.Trim() == ownCodigo)
        {
            return BadRequest(new
            {
                error = new
                {
                    code = "VALIDATION_ERROR",
                    message = "Los datos enviados no son válidos.",
                    details = new[]
                    {
                        new { field = "codigoPadre", message = "Un vacuno no puede ser su propio padre." }
                    }
                }
            });
        }

        if (!string.IsNullOrWhiteSpace(request.CodigoMadre) && request.CodigoMadre.Trim() == ownCodigo)
        {
            return BadRequest(new
            {
                error = new
                {
                    code = "VALIDATION_ERROR",
                    message = "Los datos enviados no son válidos.",
                    details = new[]
                    {
                        new { field = "codigoMadre", message = "Un vacuno no puede ser su propia madre." }
                    }
                }
            });
        }

        long? padreId = null;
        if (!string.IsNullOrWhiteSpace(request.CodigoPadre))
        {
            var padre = await db.vacunos.FirstOrDefaultAsync(v => v.codigo == request.CodigoPadre.Trim() && v.deleted_at == null, cancellationToken);
            if (padre == null)
            {
                return BadRequest(new
                {
                    error = new
                    {
                        code = "VALIDATION_ERROR",
                        message = "Los datos enviados no son válidos.",
                        details = new[]
                        {
                            new { field = "codigoPadre", message = "El vacuno padre especificado no existe." }
                        }
                    }
                });
            }
            padreId = padre.id;
        }

        long? madreId = null;
        if (!string.IsNullOrWhiteSpace(request.CodigoMadre))
        {
            var madre = await db.vacunos.FirstOrDefaultAsync(v => v.codigo == request.CodigoMadre.Trim() && v.deleted_at == null, cancellationToken);
            if (madre == null)
            {
                return BadRequest(new
                {
                    error = new
                    {
                        code = "VALIDATION_ERROR",
                        message = "Los datos enviados no son válidos.",
                        details = new[]
                        {
                            new { field = "codigoMadre", message = "El vacuno madre especificado no existe." }
                        }
                    }
                });
            }
            madreId = madre.id;
        }

        long granjaId = 0;
        if (request.GranjaId.HasValue && request.GranjaId.Value > 0)
        {
            var granjaExiste = await db.granjas.AnyAsync(g => g.id == request.GranjaId.Value && g.activo, cancellationToken);
            if (!granjaExiste)
            {
                return BadRequest(new
                {
                    error = new
                    {
                        code = "VALIDATION_ERROR",
                        message = "Los datos enviados no son válidos.",
                        details = new[]
                        {
                            new { field = "granjaId", message = "La granja seleccionada no existe o no está activa." }
                        }
                    }
                });
            }
            granjaId = request.GranjaId.Value;
        }
        else
        {
            var granjaNombre = request.Granja?.Trim();
            var distritoCodigo = request.CodigoDistrito?.Trim();
            if (!string.IsNullOrWhiteSpace(granjaNombre) && !string.IsNullOrWhiteSpace(distritoCodigo))
            {
                var distritoExists = await db.geo_distritos.AnyAsync(d => d.codigo == distritoCodigo, cancellationToken);
                if (!distritoExists)
                {
                    return BadRequest(new
                    {
                        error = new
                        {
                            code = "VALIDATION_ERROR",
                            message = "Los datos enviados no son válidos.",
                            details = new[]
                            {
                                new { field = "codigoDistrito", message = "El distrito especificado no es válido o no está registrado." }
                            }
                        }
                    });
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
            return BadRequest(new
            {
                error = new
                {
                    code = "VALIDATION_ERROR",
                    message = "Los datos enviados no son válidos.",
                    details = new[]
                    {
                        new { field = "granjaId", message = "La granja seleccionada no es válida o no ha sido especificada." }
                    }
                }
            });
        }

        var command = VacunoMapper.ToCommand(request, padreId, madreId, granjaId);
        var output = await _updateInputPort.HandleAsync(id, command, cancellationToken);
        var response = await EnrichResponseAsync(output.Data, db, cancellationToken);
        return Ok(GeneralResponseDTO<VacunoResponse>.Ok(response));
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

    [HttpGet("{identifier}/genealogia")]
    [ProducesResponseType(typeof(GeneralResponseDTO<GenerarArbolGenealogicoOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GenerarArbolGenealogico(
        [FromRoute] string identifier,
        [FromServices] IGenerarArbolGenealogicoInputPort arbolInputPort,
        [FromServices] GenerarArbolGenealogicoPresenter arbolPresenter,
        [FromQuery] int niveles = 4,
        CancellationToken cancellationToken = default)
    {
        long id = await ResolveIdAsync(identifier, cancellationToken);
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
        [FromQuery] string? fechaDesde,
        [FromQuery] string? fechaHasta,
        [FromQuery] string? q,
        [FromQuery] string? codigo,
        [FromQuery] string? fechaRegistro,
        [FromQuery] string? nombre,
        [FromQuery] string? raza,
        [FromQuery] string? procedencia,
        [FromQuery] string? estado,
        [FromQuery] string? aptoPara,
        [FromQuery] string? formato,
        [FromQuery] int? page,
        [FromQuery] int? limit,
        CancellationToken cancellationToken)
    {
        var desde = ParseDateOrNull(fechaDesde);
        var hasta = ParseDateOrNull(fechaHasta);
        if (desde.HasValue && hasta.HasValue && desde.Value > hasta.Value)
        {
            return BadRequest(new { message = "fechaDesde no puede ser mayor que fechaHasta." });
        }

        var rows = await BuildReporteListadoRowsAsync(cancellationToken);
        rows = ApplyReporteListadoFilters(rows, desde, hasta, q, codigo, fechaRegistro, nombre, raza, procedencia, estado);

        var normalizedFormato = NormalizeFormat(formato);
        var total = rows.Count;
        var pageNumber = Math.Max(1, page ?? 1);
        var pageSize = Math.Clamp(limit ?? 10, 1, 100);
        var pagedRows = rows
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        string? downloadUrl = null;
        if (normalizedFormato is "excel" or "pdf")
        {
            downloadUrl = await GenerateListadoReportFileAsync(rows, normalizedFormato, cancellationToken);
        }

        return Ok(new
        {
            data = pagedRows,
            resumen = new { totalVacunos = total },
            filtros = new
            {
                fechaDesde = desde,
                fechaHasta = hasta,
                q,
                codigo,
                fechaRegistro,
                nombre,
                raza,
                procedencia,
                estado,
                aptoPara,
                formato = normalizedFormato
            },
            downloadUrl
        });
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
        var output = await _getByIdInputPort.HandleAsync(vacunoId, cancellationToken);
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
            codigoDistrito);
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

    private async Task<List<object>> BuildReporteListadoRowsAsync(CancellationToken cancellationToken)
    {
        var vacunos = await _vacunoRepository.ListAllForDisplayAsync(cancellationToken);

        return vacunos
            .Select(x => new
            {
                id = x.Vacuno.Id,
                codigo = x.Vacuno.Codigo,
                fechaRegistro = x.Vacuno.FechaRegistro.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                nombre = x.Vacuno.Nombre,
                raza = x.Vacuno.RazaCode,
                procedencia = x.Procedencia,
                estado = x.Vacuno.IsDeleted ? "muerto" : "vivo"
            })
            .Cast<object>()
            .ToList();
    }

    private static List<object> ApplyReporteListadoFilters(
        IEnumerable<object> rows,
        DateOnly? fechaDesde,
        DateOnly? fechaHasta,
        string? q,
        string? codigo,
        string? fechaRegistro,
        string? nombre,
        string? raza,
        string? procedencia,
        string? estado)
    {
        return rows
            .Where(row =>
            {
                dynamic item = row;
                var itemFecha = ParseDateOrNull((string)item.fechaRegistro);
                if (fechaDesde.HasValue && itemFecha.HasValue && itemFecha.Value < fechaDesde.Value) return false;
                if (fechaHasta.HasValue && itemFecha.HasValue && itemFecha.Value > fechaHasta.Value) return false;
                if (!Contains(item.codigo, q) && !Contains(item.nombre, q)) return false;
                if (!Contains(item.codigo, codigo)) return false;
                if (!Contains(item.fechaRegistro, fechaRegistro)) return false;
                if (!Contains(item.nombre, nombre)) return false;
                if (!Contains(item.raza, raza)) return false;
                if (!Contains(item.procedencia, procedencia)) return false;
                if (!Contains(item.estado, estado)) return false;
                return true;
            })
            .ToList();
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
            sexo = response.SexoCode,
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

    private async Task<string> GenerateListadoReportFileAsync(
        IReadOnlyCollection<object> rows,
        string formato,
        CancellationToken cancellationToken)
    {
        var fileName = $"reporte_listado_vacunos_{DateTime.UtcNow:yyyyMMddHHmmss}.{(formato == "pdf" ? "pdf" : "csv")}";
        var path = Path.Combine(GetReportOutputDirectory(), fileName);
        var content = BuildListadoReportContent(rows);

        if (formato == "pdf")
        {
            await System.IO.File.WriteAllBytesAsync(path, BuildSimplePdf("Reporte listado de vacunos", content), cancellationToken);
        }
        else
        {
            await System.IO.File.WriteAllTextAsync(path, content, Encoding.UTF8, cancellationToken);
        }

        return $"/api/v1/vacuno/reportes/descargas/{Uri.EscapeDataString(fileName)}";
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

        return $"/api/v1/vacuno/reportes/descargas/{Uri.EscapeDataString(fileName)}";
    }

    private static string BuildListadoReportContent(IEnumerable<object> rows)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Codigo,Registro,Nombre,Raza,Procedencia,Estado");

        foreach (dynamic item in rows)
        {
            sb.AppendLine(string.Join(",",
                Csv(item.codigo),
                Csv(item.fechaRegistro),
                Csv(item.nombre),
                Csv(item.raza),
                Csv(item.procedencia),
                Csv(item.estado)));
        }

        return sb.ToString();
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

    private static string NormalizeFormat(string? formato)
    {
        var normalized = formato?.Trim().ToLowerInvariant();
        return normalized is "excel" or "pdf" ? normalized : "json";
    }

    private static bool Contains(string? value, string? filter)
    {
        return string.IsNullOrWhiteSpace(filter)
            || (!string.IsNullOrWhiteSpace(value)
                && value.Contains(filter.Trim(), StringComparison.OrdinalIgnoreCase));
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
}
