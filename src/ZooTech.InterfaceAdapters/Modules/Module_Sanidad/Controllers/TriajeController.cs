using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Modules.Module_Sanidad.DTOs.Requests;
using ZooTech.Application.Modules.Module_Sanidad.DTOs.Responses;
using ZooTech.Application.Modules.Module_Sanidad.UseCases;

namespace ZooTech.InterfaceAdapters.Module_Sanidad.Controllers;

[ApiController]
[Route("api/v1/triaje")]
public class TriajeController : ControllerBase
{
    private readonly GetAllTriajesUseCase _getAllUseCase;
    private readonly GetTriajeByIdUseCase _getByIdUseCase;
    private readonly CreateTriajeUseCase _createUseCase;
    private readonly UpdateTriajeUseCase _updateUseCase;
    private readonly DeleteTriajeUseCase _deleteUseCase;
    private readonly GetAllTipoPesosUseCase _getTipoPesosUseCase;
    private readonly GetAllVacunosUseCase _getVacunosUseCase;
    private readonly GetHistorialByVacunoIdUseCase _getHistorialUseCase;
    private readonly GetHistorialGeneralUseCase _getHistorialGeneralUseCase;

    public TriajeController(
        GetAllTriajesUseCase getAllUseCase,
        GetTriajeByIdUseCase getByIdUseCase,
        CreateTriajeUseCase createUseCase,
        UpdateTriajeUseCase updateUseCase,
        DeleteTriajeUseCase deleteUseCase,
            GetAllTipoPesosUseCase getTipoPesosUseCase,
            GetAllVacunosUseCase getVacunosUseCase,
            GetHistorialByVacunoIdUseCase getHistorialUseCase,
            GetHistorialGeneralUseCase getHistorialGeneralUseCase)
    {
        _getAllUseCase = getAllUseCase;
        _getByIdUseCase = getByIdUseCase;
        _createUseCase = createUseCase;
        _updateUseCase = updateUseCase;
        _deleteUseCase = deleteUseCase;
        _getTipoPesosUseCase = getTipoPesosUseCase;
        _getVacunosUseCase = getVacunosUseCase;
        _getHistorialUseCase = getHistorialUseCase;
        _getHistorialGeneralUseCase = getHistorialGeneralUseCase;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<TriajeResponse>>> GetAll(
        [FromQuery] int pagina = 1,
        [FromQuery] int tamano = 10,
        [FromQuery] string? desde = null,
        [FromQuery] string? hasta = null,
        [FromQuery] string? codigo = null,
        [FromQuery] string? nombre = null,
        [FromQuery] string? tipoPeso = null,
        [FromQuery] decimal? pesoKg = null)
    {
        var result = await _getAllUseCase.ExecuteAsync(pagina, tamano, desde, hasta, codigo, nombre, tipoPeso, pesoKg);
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<TriajeResponse>> GetById(long id)
    {
        var result = await _getByIdUseCase.ExecuteAsync(id);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<TriajeResponse>> Create([FromBody] TriajeRequest request)
    {
        try
        {
            var result = await _createUseCase.ExecuteAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<TriajeResponse>> Update(long id, [FromBody] TriajeRequest request)
    {
        var result = await _updateUseCase.ExecuteAsync(id, request);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var deleted = await _deleteUseCase.ExecuteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }

    [HttpGet("tipos-peso")]
    public async Task<ActionResult<IEnumerable<TipoPesoResponse>>> GetTiposPeso()
    {
        var result = await _getTipoPesosUseCase.ExecuteAsync();
        return Ok(result);
    }

    [HttpGet("vacunos")]
    public async Task<ActionResult<IEnumerable<VacunoOptionResponse>>> GetVacunos()
    {
        var result = await _getVacunosUseCase.ExecuteAsync();
        return Ok(result);
    }

    [HttpGet("historial/{vacunoId:long}")]
    public async Task<ActionResult<IEnumerable<TriajeHistorialResponse>>> GetHistorial(
        long vacunoId,
        [FromQuery] string? desde = null,
        [FromQuery] string? hasta = null)
    {
        var result = await _getHistorialUseCase.ExecuteAsync(vacunoId, desde, hasta);
        return Ok(result);
    }

    [HttpGet("historial-general")]
    public async Task<ActionResult<IEnumerable<TriajeHistorialResponse>>> GetHistorialGeneral(
        [FromQuery] string? desde = null,
        [FromQuery] string? hasta = null)
    {
        var result = await _getHistorialGeneralUseCase.ExecuteAsync(desde, hasta);
        return Ok(result);
    }
}