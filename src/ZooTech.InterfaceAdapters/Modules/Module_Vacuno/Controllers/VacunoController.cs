using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Common.Gateway.Features;
using ZooTech.Application.Common.Gateway.Repositories;
using ZooTech.Domain.Module_Vacuno.Entities;
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
    private readonly IArchivoService _archivoService;

    public VacunoController(
        IMediator mediator,
        IVacunoRepository vacunoRepository,
        IArchivoService archivoService)
    {
        _mediator = mediator;
        _vacunoRepository = vacunoRepository;
        _archivoService = archivoService;
    }

    [HttpGet]
    public async Task<IActionResult> ListarVacunos(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10,
        [FromQuery] int? pageSize = null,
        [FromQuery] string? q = null,
        [FromQuery] string? search = null,
        [FromQuery] string? estado = null,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        limit = pageSize ?? limit;
        limit = Math.Clamp(limit, 1, 50);
        q = string.IsNullOrWhiteSpace(q) ? search : q;

        var baseUrl = $"{Request.Scheme}://{Request.Host}";
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
                fechaNacimiento = v.FechaNacimiento,
                raza = v.Raza,
                color = v.Color,
                sexo = v.Sexo,
                aptoPara = v.Utilizacion != null ? v.Utilizacion.AptoPara : string.Empty,
                procedencia = $"{v.Granja} - {v.Distrito}",
                fotoUrl = v.Foto is not null ? $"{baseUrl}/files/{v.Foto.RutaArchivo}" : null,
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

    [HttpGet("options")]
    public async Task<IActionResult> ObtenerOpciones(CancellationToken cancellationToken)
    {
        var ubigeo = await _vacunoRepository.ListarUbigeoAsync(cancellationToken);

        return Ok(new
        {
            adquisicionOptions = new[] { "monta", "compra" },
            razaOptions = new[] { "Jersey", "Holstein", "Angus", "Hereford", "Simmental", "Brown Swiss", "Brahman", "Charolais" },
            sexoOptions = new[] { "hembra", "macho" },
            aptoParaOptions = new[] { "produccion_leche", "carne", "reproduccion" },
            ubigeoOptions = ubigeo
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

        return Ok(ToDetalleResponse(vacuno));
    }

    [HttpGet("codigo/{codigo}")]
    public async Task<IActionResult> ObtenerVacunoPorCodigo(string codigo, CancellationToken cancellationToken)
    {
        var vacuno = await _vacunoRepository.ObtenerPorCodigoAsync(codigo, cancellationToken);
        if (vacuno is null)
            return NotFound();

        return Ok(ToDetalleResponse(vacuno));
    }

    [HttpPut("{codigo}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> EditarVacuno(
        string codigo,
        [FromForm] EditarVacunoRequest request,
        CancellationToken cancellationToken)
    {
        var validationMessage = ValidarEdicion(request);
        if (validationMessage is not null)
            return BadRequest(new { message = validationMessage });

        var rutaFoto = request.Foto is null
            ? null
            : await _archivoService.GuardarAsync(
                request.Foto.OpenReadStream(),
                request.Foto.FileName,
                "vacunos/fotos",
                cancellationToken);

        var data = new EditarVacunoData(
            Nombre: request.Nombre.Trim(),
            IdRaza: VacunoMapper.IdRaza(request.Raza),
            Raza: request.Raza.Trim(),
            IdColor: VacunoMapper.IdColor(request.Color),
            Color: request.Color.Trim(),
            IdSexo: VacunoMapper.IdSexo(request.Sexo),
            Sexo: request.Sexo.Trim().ToLowerInvariant(),
            IdGranja: VacunoMapper.IdUbigeo(request.Granja),
            Granja: request.Granja.Trim(),
            IdDistrito: VacunoMapper.IdUbigeo(request.CodigoDistrito, request.Distrito),
            Distrito: request.Distrito.Trim(),
            IdDepartamento: VacunoMapper.IdUbigeo(request.CodigoDepartamento, request.Departamento),
            Departamento: request.Departamento.Trim(),
            IdProvincia: VacunoMapper.IdUbigeo(request.CodigoProvincia, request.Provincia),
            Provincia: request.Provincia.Trim(),
            IdTipoUtilizacion: VacunoMapper.IdTipoUtilizacion(request.AptoPara),
            AptoPara: request.AptoPara.Trim(),
            FechaEspecificacion: request.FechaEspecificacion,
            Observaciones: string.IsNullOrWhiteSpace(request.Observaciones) ? null : request.Observaciones.Trim(),
            IdTipoAdquisicion: VacunoMapper.IdTipoAdquisicion(request.AdquisicionPor),
            PrecioCompra: string.Equals(request.AdquisicionPor, "compra", StringComparison.OrdinalIgnoreCase)
                ? request.PrecioCompra
                : null,
            RutaFoto: rutaFoto,
            ActualizadoEn: DateTime.UtcNow);

        if (await _vacunoRepository.ExisteRegistroDuplicadoAsync(codigo, data, cancellationToken))
            return Conflict(new { message = "Ya existe otro vacuno con los mismos datos principales." });

        var vacuno = await _vacunoRepository.ActualizarAsync(codigo, data, cancellationToken);
        if (vacuno is null)
            return NotFound();

        return Ok(ToDetalleResponse(vacuno));
    }

    private object ToDetalleResponse(Vacuno vacuno)
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";

        return new
        {
            id = vacuno.Id,
            codigo = vacuno.Codigo,
            nombre = vacuno.Nombre,
            fechaNacimiento = vacuno.FechaNacimiento,
            adquisicionPor = vacuno.Adquisicion != null
                ? VacunoMapper.NombreTipoAdquisicion(vacuno.Adquisicion.IdTipoAdquisicion)
                : "monta",
            precioCompra = vacuno.Adquisicion?.PrecioCompra,
            raza = vacuno.Raza,
            color = vacuno.Color,
            sexo = vacuno.Sexo,
            codigoPadre = vacuno.CodigoPadre,
            codigoMadre = vacuno.CodigoMadre,
            granja = vacuno.Granja,
            distrito = vacuno.Distrito,
            departamento = vacuno.Departamento,
            provincia = vacuno.Provincia,
            aptoPara = vacuno.Utilizacion != null ? vacuno.Utilizacion.AptoPara : string.Empty,
            fechaEspecificacion = vacuno.Utilizacion?.FechaEspecificacion,
            fechaRegistroEspecializacion = vacuno.Utilizacion?.FechaEspecificacion,
            fechaRegistroFuncion = vacuno.Utilizacion?.FechaEspecificacion,
            observaciones = vacuno.Utilizacion?.Observaciones,
            fotoUrl = vacuno.Foto is not null ? $"{baseUrl}/files/{vacuno.Foto.RutaArchivo}" : null,
            estado = vacuno.IdEstado == 1 ? "vivo" : "muerto",
            creadoEn = vacuno.CreadoEn,
            actualizadoEn = vacuno.ActualizadoEn
        };
    }

    private static string? ValidarEdicion(EditarVacunoRequest request)
    {
        var camposObligatorios = new (string Valor, string Nombre)[]
        {
            (request.Nombre, "nombre"),
            (request.Raza, "raza"),
            (request.Color, "color"),
            (request.Sexo, "sexo"),
            (request.Granja, "granja"),
            (request.Distrito, "distrito"),
            (request.Departamento, "departamento"),
            (request.Provincia, "provincia"),
            (request.AptoPara, "apto para"),
            (request.AdquisicionPor, "adquisicion")
        };

        var campoVacio = camposObligatorios.FirstOrDefault(campo => string.IsNullOrWhiteSpace(campo.Valor));
        if (campoVacio.Nombre is not null)
            return $"El campo {campoVacio.Nombre} es obligatorio.";

        var inputsLimitados = new (string Valor, string Nombre)[]
        {
            (request.Nombre, "nombre"),
            (request.Color, "color"),
            (request.Granja, "granja")
        };

        var campoLargo = inputsLimitados.FirstOrDefault(campo => campo.Valor.Trim().Length > 15);
        if (campoLargo.Nombre is not null)
            return $"El campo {campoLargo.Nombre} debe tener como maximo 15 caracteres.";

        if (string.Equals(request.AdquisicionPor, "compra", StringComparison.OrdinalIgnoreCase)
            && (!request.PrecioCompra.HasValue || request.PrecioCompra <= 0))
            return "El precio de compra es obligatorio y debe ser mayor a 0 cuando la adquisicion es compra.";

        var observaciones = request.Observaciones?.Trim() ?? string.Empty;
        var palabras = string.IsNullOrWhiteSpace(observaciones)
            ? 0
            : observaciones.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;

        if (observaciones.Length > 150 || palabras > 30)
            return "Las observaciones no pueden exceder 30 palabras o 150 caracteres.";

        if (request.Foto is not null && !EsImagenPermitida(request.Foto))
            return "La foto debe estar en formato JPG, JPEG o PNG.";

        return null;
    }

    private static bool EsImagenPermitida(IFormFile foto)
    {
        var extension = Path.GetExtension(foto.FileName);
        return string.Equals(extension, ".jpg", StringComparison.OrdinalIgnoreCase)
            || string.Equals(extension, ".jpeg", StringComparison.OrdinalIgnoreCase)
            || string.Equals(extension, ".png", StringComparison.OrdinalIgnoreCase);
    }
}
