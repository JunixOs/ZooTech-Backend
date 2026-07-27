using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosPdf;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosExcel;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GetOrdenioById;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.ListOrdenios;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunos;
using ZooTech.InterfaceAdapters.DTOs;
using ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.DTOs.Responses;
using ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.Mappers;
using ZooTech.Application.Common.Behaviors;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.CreateOrdenio;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.UpdateOrdenio;
using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.DeleteOrdenio;

namespace ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.Controllers;

[ApiController]
[Route("api/v1/produccion-leche")]
[ApiExplorerSettings(GroupName = "produccion_leche")]
public sealed class ProduccionLecheController : ControllerBase
{
    private readonly IBehaviorDispatcher _behaviorDispatcher;

    public ProduccionLecheController(
        IBehaviorDispatcher behaviorDispatcher
    )
    {
        _behaviorDispatcher = behaviorDispatcher;
    }

    [HttpGet("health")]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status200OK)]
    public IActionResult Health()
    {
        return Ok(new { status = "Api funcionando de manera correcta", model = "modulo produccion leche" });
    }

    [HttpGet("vacunos")]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetVacunos(CancellationToken cancellationToken)
    {
        // TODO: este endpoint necesita su propio caso de uso sin paginar para el selector de Leche, en vez de forzar Limit al máximo de ListarVacunos
        var output = await _behaviorDispatcher.Send<ListarVacunosQuery , ListarVacunosOutput>(
            new ListarVacunosQuery(Limit: 100, FechaDesde: DateTime.MinValue, FechaHasta: DateTime.MaxValue), 
            cancellationToken);
        var data = output.Items.Select(x => new { id = x.Id, codigo = x.Codigo, nombre = x.Nombre, raza = x.RazaCode });
        return Ok(GeneralResponseDTO<object>.Ok(data));
    }

    [HttpPost]
    [ProducesResponseType(typeof(GeneralResponseDTO<OrdenioResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreateOrdenioRequest request,
        CancellationToken cancellationToken)
    {
        var data = ProduccionLecheMapper.ToResponse(
            await _behaviorDispatcher.Send<CreateOrdenioCommand , CreateOrdenioOutput>(ProduccionLecheMapper.ToCommand(request), cancellationToken));
        return Created($"/api/v1/produccion-leche/{data.Id}", GeneralResponseDTO<OrdenioResponse>.Ok(data));
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(GeneralResponseDTO<OrdenioResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] long id, CancellationToken cancellationToken)
    {
        var data = ProduccionLecheMapper.ToResponse(await _behaviorDispatcher.Send<GetOrdenioByIdQuery , GetOrdenioByIdOutput>(
                new GetOrdenioByIdQuery
                {
                    Id = id
                }, 
                cancellationToken
            ));
        return Ok(GeneralResponseDTO<OrdenioResponse>.Ok(data));
    }

    [HttpGet]
    [ProducesResponseType(typeof(GeneralResponseDTO<ListOrdeniosResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> List(
        [FromQuery] long? vacunoId,
        [FromQuery] string? estadoOrdenioCode,
        [FromQuery] DateTime? fechaDesde,
        [FromQuery] DateTime? fechaHasta,
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        CancellationToken cancellationToken)
    {
        var currentPage = page ?? 1;
        var currentPageSize = pageSize ?? 20;
        var data = ProduccionLecheMapper.ToResponse(
            await _behaviorDispatcher.Send<ListOrdeniosQuery , ListOrdeniosOutput>(
                new ListOrdeniosQuery
                {
                    VacunoId = vacunoId, 
                    EstadoOrdenioCode = estadoOrdenioCode, 
                    FechaDesde = fechaDesde, 
                    FechaHasta = fechaHasta, 
                    Page = currentPage, 
                    PageSize = currentPageSize
                },
                cancellationToken
            ),
            currentPage, currentPageSize);
        return Ok(GeneralResponseDTO<ListOrdeniosResponse>.Ok(data));
    }

    [HttpGet("reporte/pdf")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GeneratePdf(
        [FromQuery] long? vacunoId,
        [FromQuery] string? estadoOrdenioCode,
        [FromQuery] DateTime? fechaDesde,
        [FromQuery] DateTime? fechaHasta,
        [FromQuery] bool comparativo,
        CancellationToken cancellationToken)
    {
        var report = await _behaviorDispatcher.Send<GenerateOrdeniosComparationPdfQuery , GenerateOrdeniosPdfOutput>(
            new GenerateOrdeniosComparationPdfQuery
            {
                VacunoId = vacunoId, 
                EstadoOrdenioCode = estadoOrdenioCode, 
                FechaDesde = fechaDesde, 
                FechaHasta = fechaHasta, 
                Comparativo = comparativo
            },
            cancellationToken
        );

        return File(report.Content, report.ContentType, report.FileName);
    }

    [HttpGet("reporte/excel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GenerateExcel(
        [FromQuery] long? vacunoId,
        [FromQuery] string? estadoOrdenioCode,
        [FromQuery] DateTime? fechaDesde,
        [FromQuery] DateTime? fechaHasta,
        [FromQuery] bool comparativo,
        CancellationToken cancellationToken)
    {
        var report = await _behaviorDispatcher.Send<GenerateOrdeniosComparationExcelQuery , GenerateOrdeniosExcelOutput>(
            new GenerateOrdeniosComparationExcelQuery
            {
                VacunoId = vacunoId, 
                EstadoOrdenioCode = estadoOrdenioCode, 
                FechaDesde = fechaDesde, 
                FechaHasta = fechaHasta,
                Comparativo = comparativo
            },
            cancellationToken
        );

        return File(report.Content, report.ContentType, report.FileName);
    }

    [HttpPatch("{id:long}")]
    [ProducesResponseType(typeof(GeneralResponseDTO<OrdenioResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        [FromRoute] long id,
        [FromBody] UpdateOrdenioRequest request,
        CancellationToken cancellationToken)
    {
        var command = ProduccionLecheMapper.ToCommand(request);
        command.Id = id;

        var data = ProduccionLecheMapper.ToResponse(
            await _behaviorDispatcher.Send<UpdateOrdenioCommand , UpdateOrdenioOutput>(command, cancellationToken));
        return Ok(GeneralResponseDTO<OrdenioResponse>.Ok(data));
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        [FromRoute] long id,
        [FromBody] DeleteOrdenioRequest request,
        CancellationToken cancellationToken)
    {
        var command = ProduccionLecheMapper.ToCommand(request);
        command.Id = id;

        await _behaviorDispatcher.Send<DeleteOrdenioCommand , EmptyOutput>(command, cancellationToken);
        return NoContent();
    }
}
