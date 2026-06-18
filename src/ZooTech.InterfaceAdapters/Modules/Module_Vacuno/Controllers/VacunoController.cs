using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
}
