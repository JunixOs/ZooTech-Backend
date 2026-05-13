using Microsoft.AspNetCore.Mvc;
using ZooTech.Domain.Module_Sanidad.Interfaces;
using ZooTech.Infrastructure.Persistence.Modules.Module_Sanidad.Entities;
using ZooTech.InterfaceAdapters.Modules.Module_Sanidad.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Sanidad.DTOs.Responses;
namespace ZooTech.InterfaceAdapters.Module_Sanidad.Controllers;

[ApiController]
[Route("api/v1/triaje")]
public class TriajeController : ControllerBase
{
    private readonly ITriajeRepository _triajeRepository;

    public TriajeController(ITriajeRepository triajeRepository)
    {
        _triajeRepository = triajeRepository;
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<TriajeResponse>>> GetAll()
    {
        var triajes = await _triajeRepository.GetAllAsync();
        return Ok(triajes.OfType<Triaje>().Select(ToResponse));
    }


    [HttpGet("{id:long}")]
    public async Task<ActionResult<TriajeResponse>> GetById(long id)
    {
        var triaje = await _triajeRepository.GetByIdAsync(id);

        if (triaje is not Triaje entity)
        {
            return NotFound();
        }

        return Ok(ToResponse(entity));
    }

    
    [HttpPost]
    public async Task<ActionResult<TriajeResponse>> Create([FromBody] TriajeRequest request)
    {
        var codigo = await _triajeRepository.GenerateCodigoAsync();
        var triaje = new Triaje
        {
            Codigo = codigo,
            FechaHora = request.FechaHora,
            VacunoId = request.VacunoId,
            TipoPesoCode = request.TipoPesoCode,
            PesoKg = request.PesoKg,
            Observaciones = request.Observaciones,
            EstadoRegistroCode = request.EstadoRegistroCode,
            EncargadoUsuarioId = request.EncargadoUsuarioId,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        await _triajeRepository.AddAsync(triaje);

        var response = ToResponse(triaje);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    
    [HttpPut("{id:long}")]
    public async Task<ActionResult<TriajeResponse>> Update(long id, [FromBody] TriajeRequest request)
    {
        var triaje = await _triajeRepository.GetByIdAsync(id);
        if (triaje is not Triaje entity)
        {
            return NotFound();
        }

        entity.FechaHora = request.FechaHora;
        entity.VacunoId = request.VacunoId;
        entity.TipoPesoCode = request.TipoPesoCode;
        entity.PesoKg = request.PesoKg;
        entity.Observaciones = request.Observaciones;
        entity.EstadoRegistroCode = request.EstadoRegistroCode;
        entity.EncargadoUsuarioId = request.EncargadoUsuarioId;
        entity.UpdatedAt = DateTime.Now;

        await _triajeRepository.UpdateAsync(entity);

        return Ok(ToResponse(entity));
    }

    
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var triaje = await _triajeRepository.GetByIdAsync(id);

        if (triaje is not Triaje)
        {
            return NotFound();
        }

        await _triajeRepository.DeleteAsync(id);
        return NoContent();
    }

    private static TriajeResponse ToResponse(Triaje triaje)
    {
        return new TriajeResponse
        {
            Id = triaje.Id,
            Codigo = triaje.Codigo,
            FechaHora = triaje.FechaHora,
            VacunoId = triaje.VacunoId,
            TipoPesoCode = triaje.TipoPesoCode,
            PesoKg = triaje.PesoKg,
            Observaciones = triaje.Observaciones,
            EstadoRegistroCode = triaje.EstadoRegistroCode,
            EncargadoUsuarioId = triaje.EncargadoUsuarioId
        };
    }
}
