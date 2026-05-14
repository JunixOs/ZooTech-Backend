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

    public TriajeController(
        GetAllTriajesUseCase getAllUseCase,
        GetTriajeByIdUseCase getByIdUseCase,
        CreateTriajeUseCase createUseCase,
        UpdateTriajeUseCase updateUseCase,
        DeleteTriajeUseCase deleteUseCase)
    {
        _getAllUseCase = getAllUseCase;
        _getByIdUseCase = getByIdUseCase;
        _createUseCase = createUseCase;
        _updateUseCase = updateUseCase;
        _deleteUseCase = deleteUseCase;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TriajeResponse>>> GetAll()
    {
        var result = await _getAllUseCase.ExecuteAsync();
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
}