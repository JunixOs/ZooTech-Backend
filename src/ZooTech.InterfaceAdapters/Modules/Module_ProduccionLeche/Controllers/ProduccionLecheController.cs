using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.CreateOrdenio;
using ZooTech.InterfaceAdapters.DTOs;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.DeleteOrdenio;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GetOrdenioById;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.ListOrdenios;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.UpdateOrdenio;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunos;
using ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.Mappers;

namespace ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.Controllers;

[ApiController]
[Route("api/v1/produccion-leche")]
public sealed class ProduccionLecheController : ControllerBase
{
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new
        {
            status = "Api funcionando de manera correcta",
            model = "modulo produccion leche"
        });
    }

    [HttpGet("vacunos")]
    public async Task<IActionResult> GetVacunos(
        [FromServices] IListarVacunosInputPort listarVacunosInputPort,
        CancellationToken cancellationToken)
    {
        var output = await listarVacunosInputPort.HandleAsync(cancellationToken);
        var mappedList = output.Items.Select(x => new
        {
            id = x.Id,
            codigo = x.Codigo,
            nombre = x.Nombre,
            raza = x.RazaCode
        }).ToList();

        return Ok(new { data = mappedList });
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateOrdenioRequest request,
        [FromServices] ICreateOrdenioInputPort inputPort,
        CancellationToken cancellationToken)
    {
        var data = ProduccionLecheMapper.ToResponse(
            await inputPort.HandleAsync(ProduccionLecheMapper.ToCommand(request), cancellationToken));
        return Created($"/api/v1/produccion-leche/{data.Id}",
            GeneralResponseDTO<object>.Ok(data));
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(
        [FromRoute] long id,
        [FromServices] IGetOrdenioByIdInputPort inputPort,
        CancellationToken cancellationToken)
    {
        var data = ProduccionLecheMapper.ToResponse(await inputPort.HandleAsync(id, cancellationToken));
        return Ok(GeneralResponseDTO<object>.Ok(data));
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
        var data = ProduccionLecheMapper.ToResponse(
            await inputPort.HandleAsync(
                new ListOrdeniosQuery(vacunoId, estadoOrdenioCode, fechaDesde, fechaHasta, currentPage, currentPageSize),
                cancellationToken),
            currentPage, currentPageSize);
        return Ok(GeneralResponseDTO<object>.Ok(data));
    }

    [HttpPatch("{id:long}")]
    public async Task<IActionResult> Update(
        [FromRoute] long id,
        [FromBody] UpdateOrdenioRequest request,
        [FromServices] IUpdateOrdenioInputPort inputPort,
        CancellationToken cancellationToken)
    {
        var data = ProduccionLecheMapper.ToResponse(
            await inputPort.HandleAsync(id, ProduccionLecheMapper.ToCommand(request), cancellationToken));
        return Ok(GeneralResponseDTO<object>.Ok(data));
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(
        [FromRoute] long id,
        [FromBody] DeleteOrdenioRequest request,
        [FromServices] IDeleteOrdenioInputPort inputPort,
        CancellationToken cancellationToken)
    {
        await inputPort.HandleAsync(id, ProduccionLecheMapper.ToCommand(request), cancellationToken);
        return NoContent();
    }
}
