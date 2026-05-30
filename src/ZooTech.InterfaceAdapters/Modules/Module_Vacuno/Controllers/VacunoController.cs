using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Common.Gateway.Repositories;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Responses;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Mappers;

namespace ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Controllers;

[ApiController]
[Route("v1/vacunos")]
public sealed class VacunoController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IVacunoRepository _vacunoRepository;

    public VacunoController(IMediator mediator, IVacunoRepository vacunoRepository)
    {
        _mediator = mediator;
        _vacunoRepository = vacunoRepository;
    }

    [HttpGet]
    public async Task<IActionResult> ListarVacunos(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10,
        [FromQuery] string? q = null,
        [FromQuery] string? estado = null,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        limit = Math.Clamp(limit, 1, 50);

        var total = await _vacunoRepository.ContarAsync(q, estado, cancellationToken);
        var vacunos = await _vacunoRepository.ListarAsync(page, limit, q, estado, cancellationToken);

        return Ok(new
        {
            data = vacunos.Select(v => new
            {
                id = v.Id,
                codigo = v.Codigo,
                fechaRegistro = v.CreadoEn,
                nombre = v.Nombre,
                raza = v.Raza,
                procedencia = $"{v.Granja} - {v.Distrito}",
                estado = v.IdEstado == 1 ? "vivo" : "muerto"
            }),
            pagination = new
            {
                page,
                limit,
                total,
                totalPages = total == 0 ? 1 : (int)Math.Ceiling(total / (double)limit)
            }
        });
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(RegistrarVacunoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RegistrarVacuno(
        [FromForm] RegistrarVacunoRequest request,
        CancellationToken cancellationToken)
    {
        var command = VacunoMapper.ToCommand(request);
        var result = await _mediator.Send(command, cancellationToken);
        var response = VacunoMapper.ToResponse(result, $"{Request.Scheme}://{Request.Host}");

        return CreatedAtAction(
            nameof(ObtenerVacuno),
            new { vacunoId = response.Id },
            response);
    }

    [HttpGet("{vacunoId:int}")]
    public async Task<IActionResult> ObtenerVacuno(int vacunoId, CancellationToken cancellationToken)
    {
        var vacuno = await _vacunoRepository.ObtenerPorIdAsync(vacunoId, cancellationToken);
        if (vacuno is null)
            return NotFound();

        var baseUrl = $"{Request.Scheme}://{Request.Host}";

        return Ok(new
        {
            id = vacuno.Id,
            codigo = vacuno.Codigo,
            nombre = vacuno.Nombre,
            fechaNacimiento = vacuno.FechaNacimiento,
            adquisicionPor = VacunoMapper.NombreTipoAdquisicion(vacuno.Adquisicion.IdTipoAdquisicion),
            precioCompra = vacuno.Adquisicion.PrecioCompra,
            raza = vacuno.Raza,
            color = vacuno.Color,
            sexo = vacuno.Sexo,
            codigoPadre = vacuno.CodigoPadre,
            codigoMadre = vacuno.CodigoMadre,
            granja = vacuno.Granja,
            distrito = vacuno.Distrito,
            departamento = vacuno.Departamento,
            provincia = vacuno.Provincia,
            aptoPara = vacuno.Utilizacion.AptoPara,
            fechaEspecificacion = vacuno.Utilizacion.FechaEspecificacion,
            observaciones = vacuno.Utilizacion.Observaciones,
            fotoUrl = vacuno.Foto is not null ? $"{baseUrl}/files/{vacuno.Foto.RutaArchivo}" : null,
            estado = vacuno.IdEstado == 1 ? "vivo" : "muerto",
            creadoEn = vacuno.CreadoEn,
            actualizadoEn = vacuno.ActualizadoEn
        });
    }
}
