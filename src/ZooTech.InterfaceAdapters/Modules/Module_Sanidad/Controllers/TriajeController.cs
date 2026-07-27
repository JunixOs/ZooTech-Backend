using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Common.Behaviors;
using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.CreateTriaje;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.DeleteTriaje;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GenerateTriajesExcel;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GenerateTriajesPdf;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllTipoPesos;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllTriajes;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllVacunosSanidad;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetHistorialByVacunoId;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetHistorialGeneral;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetTriajeById;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.UpdateTriaje;
using ZooTech.Domain.Shared.Enums;
using ZooTech.InterfaceAdapters.DTOs;
using ZooTech.InterfaceAdapters.Filters;
using ZooTech.InterfaceAdapters.Modules.Module_Sanidad.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Sanidad.DTOs.Responses;
using ZooTech.InterfaceAdapters.Modules.Module_Sanidad.Mappers;

namespace ZooTech.InterfaceAdapters.Module_Sanidad.Controllers;

[ApiController]
[Route("api/v1/triaje")]
[ApiExplorerSettings(GroupName = "public")]
public sealed class TriajeController : ControllerBase
{
    private readonly IBehaviorDispatcher _behaviorDispatcher;

    public TriajeController(
        IBehaviorDispatcher behaviorDispatcher
    )
    {
        _behaviorDispatcher = behaviorDispatcher;
    }

    [HttpGet]
    [ServiceFilter(typeof(TenantHeaderFilter))]
    [RestrictTenantType(TenantType.Tenant)]
    [Authorize(Roles = AuthorizationRoles.Regular)]
    [ProducesResponseType(typeof(GeneralResponseDTO<PagedTriajeResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pagina = 1,
        [FromQuery] int tamano = 10,
        [FromQuery] string? fecha = null,
        [FromQuery] string? fechaDesde = null,
        [FromQuery] string? fechaHasta = null,
        [FromQuery] string? codigo = null,
        [FromQuery] string? nombre = null,
        [FromQuery] string? tipoPeso = null,
        [FromQuery] string? pesoKg = null,
        [FromQuery] long? vacunoId = null,
        [FromQuery] bool? uniqueVacuno = null,
        CancellationToken cancellationToken = default)
    {
        var currentPage = pagina <= 0 ? 1 : pagina;
        var currentTamano = tamano <= 0 ? 10 : Math.Min(tamano, 100);

        var query = new GetAllTriajesQuery
        {
            Pagina = pagina,
            Tamano = tamano,
            Fecha = fecha,
            FechaDesde = fechaDesde,
            FechaHasta = fechaHasta,
            Codigo = codigo,
            Nombre = nombre,
            TipoPeso = tipoPeso,
            PesoKg = pesoKg,
            VacunoId = vacunoId,
            UniqueVacuno = uniqueVacuno
        };

        var output = await _behaviorDispatcher.Send<GetAllTriajesQuery , GetAllTriajesOutput>(query, cancellationToken);
        return Ok(GeneralResponseDTO<PagedTriajeResponse>.Ok(TriajeMapper.ToPagedResponse(output, currentPage, currentTamano)));
    }

    [HttpGet("{id:long}")]
    [ServiceFilter(typeof(TenantHeaderFilter))]
    [RestrictTenantType(TenantType.Tenant)]
    [Authorize(Roles = AuthorizationRoles.Regular)]
    [ProducesResponseType(typeof(GeneralResponseDTO<TriajeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken = default)
    {
        var output = await _behaviorDispatcher.Send<GetTriajeByIdQuery , GetTriajeByIdOutput>(
            new GetTriajeByIdQuery
            {
                Id = id
            },
            cancellationToken
        );

        return Ok(GeneralResponseDTO<TriajeResponse>.Ok(TriajeMapper.ToResponse(output)));
    }

    [HttpPost]
    [ServiceFilter(typeof(TenantHeaderFilter))]
    [RestrictTenantType(TenantType.Tenant)]
    [Authorize(Roles = AuthorizationRoles.Regular)]
    [ProducesResponseType(typeof(GeneralResponseDTO<TriajeResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] TriajeRequest request, CancellationToken cancellationToken = default)
    {
        var command = TriajeMapper.ToCreateCommand(request);
        var output = await _behaviorDispatcher.Send<CreateTriajeCommand , CreateTriajeOutput>(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = output.Id },
            GeneralResponseDTO<TriajeResponse>.Ok(TriajeMapper.ToResponse(output)));
    }

    [HttpPatch("{id:long}")]
    [ServiceFilter(typeof(TenantHeaderFilter))]
    [RestrictTenantType(TenantType.Tenant)]
    [Authorize(Roles = AuthorizationRoles.Regular)]
    [ProducesResponseType(typeof(GeneralResponseDTO<TriajeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateTriajeRequest request, CancellationToken cancellationToken = default)
    {
        var command = TriajeMapper.ToUpdateCommand(request);
        command.Id = id;

        var output = await _behaviorDispatcher.Send<UpdateTriajeCommand , UpdateTriajeOutput>(command, cancellationToken);

        return Ok(GeneralResponseDTO<TriajeResponse>.Ok(TriajeMapper.ToResponse(output)));
    }

    [HttpDelete("{id:long}")]
    [ServiceFilter(typeof(TenantHeaderFilter))]
    [RestrictTenantType(TenantType.Tenant)]
    [Authorize(Roles = AuthorizationRoles.Regular)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id, [FromBody] DeleteTriajeRequest request, CancellationToken cancellationToken = default)
    {
        await _behaviorDispatcher.Send<DeleteTriajeCommand , EmptyOutput>(
            new DeleteTriajeCommand
            {
                Id = id,
                MotivoEliminacion = request.MotivoEliminacion
            },
            cancellationToken
        );

        return NoContent();
    }

    [HttpGet("reporte/pdf")]
    [ServiceFilter(typeof(TenantHeaderFilter))]
    [RestrictTenantType(TenantType.Tenant)]
    [Authorize(Roles = AuthorizationRoles.Regular)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GeneratePdf(
        [FromQuery] string? fecha,
        [FromQuery] string? fechaDesde,
        [FromQuery] string? fechaHasta,
        [FromQuery] string? codigo,
        [FromQuery] string? nombre,
        [FromQuery] string? tipoPeso,
        [FromQuery] string? pesoKg,
        [FromQuery] long? vacunoId,
        CancellationToken cancellationToken)
    {
        var report = await _behaviorDispatcher.Send<GenerateTriajesPdfQuery , GenerateTriajesPdfOutput>(
            new GenerateTriajesPdfQuery(fecha, fechaDesde, fechaHasta, codigo, nombre, tipoPeso, pesoKg, vacunoId),
            cancellationToken);

        return File(report.Content, report.ContentType, report.FileName);
    }

    [HttpGet("reporte/excel")]
    [ServiceFilter(typeof(TenantHeaderFilter))]
    [RestrictTenantType(TenantType.Tenant)]
    [Authorize(Roles = AuthorizationRoles.Regular)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GenerateExcel(
        [FromQuery] string? fecha,
        [FromQuery] string? fechaDesde,
        [FromQuery] string? fechaHasta,
        [FromQuery] string? codigo,
        [FromQuery] string? nombre,
        [FromQuery] string? tipoPeso,
        [FromQuery] string? pesoKg,
        [FromQuery] long? vacunoId,
        CancellationToken cancellationToken)
    {
        var report = await _behaviorDispatcher.Send<GenerateTriajesExcelQuery , GenerateTriajesExcelOutput>(
            new GenerateTriajesExcelQuery(fecha, fechaDesde, fechaHasta, codigo, nombre, tipoPeso, pesoKg, vacunoId),
            cancellationToken);

        return File(report.Content, report.ContentType, report.FileName);
    }

    [HttpGet("tipos-peso")]
    [ServiceFilter(typeof(TenantHeaderFilter))]
    [RestrictTenantType(TenantType.Tenant)]
    [Authorize(Roles = AuthorizationRoles.Regular)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTiposPeso(CancellationToken cancellationToken = default)
    {
        var output = await _behaviorDispatcher.Send<EmptyCommandQuery , GetAllTipoPesosOutput>(
            EmptyCommandQuery.Value(
                AuditEventType.Read,
                "Get all tipos peso"
            ),
            cancellationToken
        );
        return Ok(GeneralResponseDTO<object>.Ok(output.Items.Select(t => new { code = t.Code, nombre = t.Nombre })));
    }

    [HttpGet("vacunos")]
    [ServiceFilter(typeof(TenantHeaderFilter))]
    [RestrictTenantType(TenantType.Tenant)]
    [Authorize(Roles = AuthorizationRoles.Regular)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetVacunos(CancellationToken cancellationToken = default)
    {
        var output = await _behaviorDispatcher.Send<EmptyCommandQuery , GetAllVacunosSanidadOutput>(
            EmptyCommandQuery.Value(
                AuditEventType.Read,
                "Get all vacunos sanidad"
            ),
            cancellationToken
        );

        return Ok(GeneralResponseDTO<object>.Ok(output.Items.Select(v => new { id = v.Id, codigo = v.Codigo, nombre = v.Nombre })));
    }

    [HttpGet("historial/{vacunoId:long}")]
    [ServiceFilter(typeof(TenantHeaderFilter))]
    [RestrictTenantType(TenantType.Tenant)]
    [Authorize(Roles = AuthorizationRoles.Regular)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHistorial(
        long vacunoId,
        [FromQuery] string? desde = null,
        [FromQuery] string? hasta = null,
        CancellationToken cancellationToken = default)
    {
        var output = await _behaviorDispatcher.Send<GetHistorialByVacunoIdQuery , GetHistorialByVacunoIdOutput>(
            new GetHistorialByVacunoIdQuery
            {
                Vacunoid = vacunoId,
                FechaDesde = desde,
                FechaHasta = hasta
            },
            cancellationToken
        );
        return Ok(GeneralResponseDTO<object>.Ok(output.Items.Select(t => new { id = t.Id, fechaHora = t.FechaHora, tipoPesoCode = t.TipoPesoCode, pesoKg = t.PesoKg })));
    }

    [HttpGet("historial-general")]
    [ServiceFilter(typeof(TenantHeaderFilter))]
    [RestrictTenantType(TenantType.Tenant)]
    [Authorize(Roles = AuthorizationRoles.Regular)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHistorialGeneral(
        [FromQuery] string? desde = null,
        [FromQuery] string? hasta = null,
        CancellationToken cancellationToken = default)
    {
        var output = await _behaviorDispatcher.Send<GetHistorialGeneralQuery , GetHistorialGeneralOutput>(
            new GetHistorialGeneralQuery
            {
                FechaDesde = desde,
                FechaHasta = hasta
            },
            cancellationToken
        );
        return Ok(GeneralResponseDTO<object>.Ok(output.Items.Select(t => new { id = t.Id, fechaHora = t.FechaHora, tipoPesoCode = t.TipoPesoCode, pesoKg = t.PesoKg })));
    }
}
