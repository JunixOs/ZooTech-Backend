using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.CreateOrdenio;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.DeleteOrdenio;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GetOrdenioById;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.ListOrdenios;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.UpdateOrdenio;
using ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.Mappers;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Ports;

namespace ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.Controllers;

[ApiController]
[Route("v1/produccion-leche")]
public sealed class ProduccionLecheController : ControllerBase
{


    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { 
            status = "Api funcionando de manera correcta",
            model = "modulo produccion leche"

        });
    }

    [HttpGet("vacunos")]
    public async Task<IActionResult> GetVacunos(
        [FromServices] IOrdenioRepository repository,
        CancellationToken cancellationToken)
    {
        var list = await repository.ListVacunosAsync(cancellationToken);
        var mappedList = list.Select(x => new {
            id = x.Id,
            
            nombre = x.Nombre,
            raza = x.RazaCode,
            ultimoRegistro = "2023-10-01",
            promedio = "15L"
        }).ToList();
        return Ok(new { data = mappedList });
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateOrdenioRequest request,
        [FromServices] ICreateOrdenioInputPort inputPort,
        CancellationToken cancellationToken)
    {
        try
        {
            var output = await inputPort.HandleAsync(ProduccionLecheMapper.ToCommand(request), cancellationToken);
            var response = ProduccionLecheMapper.ToResponse(output);
            return Created($"/v1/produccion-leche/{response.Data.Id}", response);
        }
        catch (ConflictException ex)
        {
            return Conflict(ToError("CONFLICT_ERROR", ex.Message));
        }
    }


    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(
        [FromRoute] long id,
        [FromServices] IGetOrdenioByIdInputPort inputPort,
        CancellationToken cancellationToken)
    {
        try
        {
            var output = await inputPort.HandleAsync(id, cancellationToken);
            return Ok(ProduccionLecheMapper.ToResponse(output));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ToError("ORDENIO_NOT_FOUND", ex.Message));
        }
    }

    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] long? vacunoId,
        [FromQuery] string? estadoOrdenioCode,
        [FromQuery] DateTime? fechaDesde,
        [FromQuery] DateTime? fechaHasta,
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        [FromServices] IListOrdeniosInputPort inputPort,
        CancellationToken cancellationToken)
    {
        var currentPage = page ?? 1;
        var currentPageSize = pageSize ?? 20;

        var output = await inputPort.HandleAsync(
            new ListOrdeniosQuery(vacunoId, estadoOrdenioCode, fechaDesde, fechaHasta, currentPage, currentPageSize),
            cancellationToken);

        return Ok(ProduccionLecheMapper.ToResponse(output, currentPage, currentPageSize));
    }

    [HttpPatch("{id:long}")]
    public async Task<IActionResult> Update(
        [FromRoute] long id,
        [FromBody] UpdateOrdenioRequest request,
        [FromServices] IUpdateOrdenioInputPort inputPort,
        CancellationToken cancellationToken)
    {
        try
        {
            var output = await inputPort.HandleAsync(id, ProduccionLecheMapper.ToCommand(request), cancellationToken);
            return Ok(ProduccionLecheMapper.ToResponse(output));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ToError("ORDENIO_NOT_FOUND", ex.Message));
        }
        catch (ConflictException ex)
        {
            return Conflict(ToError("CONFLICT_ERROR", ex.Message));
        }
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(
        [FromRoute] long id,
        [FromBody] DeleteOrdenioRequest request,
        [FromServices] IDeleteOrdenioInputPort inputPort,
        CancellationToken cancellationToken)
    {
        try
        {
            await inputPort.HandleAsync(id, ProduccionLecheMapper.ToCommand(request), cancellationToken);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(ToError("ORDENIO_NOT_FOUND", ex.Message));
        }
        catch (ConflictException ex)
        {
            return Conflict(ToError("CONFLICT_ERROR", ex.Message));
        }
    }

    private static object ToError(string code, string message)
        => new
        {
            error = new
            {
                code,
                message,
                details = Array.Empty<object>()
            }
        };
}
