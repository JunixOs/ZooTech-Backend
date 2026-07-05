using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.CreateFecundacion;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.DeleteFecundacion;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.GetFecundacionForEdit;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.GetFecundacionOptions;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.ListarFecundacionesPaginado;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.SearchFecundacionVacunos;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.UpdateFecundacion;
using ZooTech.InterfaceAdapters.DTOs;
using ZooTech.InterfaceAdapters.Modules.Module_Fecundacion.DTOs;
using ZooTech.InterfaceAdapters.Modules.Module_Fecundacion.DTOs.Responses;
using ZooTech.InterfaceAdapters.Modules.Module_Fecundacion.Mappers;
using ApiErrorResponse = ZooTech.InterfaceAdapters.DTOs.Responses.ErrorResponse;

namespace ZooTech.InterfaceAdapters.Modules.Module_Fecundacion.Controllers;

[ApiController]
[Route("api/v1/fecundaciones")]
[ApiExplorerSettings(GroupName = "public")]
public sealed class FecundacionController : ControllerBase
{
    private readonly ICreateFecundacionInputPort _createInputPort;
    private readonly IGetFecundacionForEditInputPort _getForEditInputPort;
    private readonly IGetFecundacionOptionsInputPort _getOptionsInputPort;
    private readonly ISearchFecundacionVacunosInputPort _searchVacunosInputPort;
    private readonly IUpdateFecundacionInputPort _updateInputPort;
    private readonly IListarFecundacionesPaginadoInputPort _listarPaginadoInputPort;
    private readonly IDeleteFecundacionInputPort _deleteInputPort;

    public FecundacionController(
        ICreateFecundacionInputPort createInputPort,
        IGetFecundacionForEditInputPort getForEditInputPort,
        IGetFecundacionOptionsInputPort getOptionsInputPort,
        ISearchFecundacionVacunosInputPort searchVacunosInputPort,
        IUpdateFecundacionInputPort updateInputPort,
        IListarFecundacionesPaginadoInputPort listarPaginadoInputPort,
        IDeleteFecundacionInputPort deleteInputPort)
    {
        _createInputPort = createInputPort;
        _getForEditInputPort = getForEditInputPort;
        _getOptionsInputPort = getOptionsInputPort;
        _searchVacunosInputPort = searchVacunosInputPort;
        _updateInputPort = updateInputPort;
        _listarPaginadoInputPort = listarPaginadoInputPort;
        _deleteInputPort = deleteInputPort;
    }

    [HttpGet]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Listar(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 20,
        [FromQuery] DateOnly? fechaDesde = null,
        [FromQuery] DateOnly? fechaHasta = null,
        [FromQuery] string? q = null,
        [FromQuery] string? tipoFecundacion = null,
        [FromQuery] string? estado = null,
        [FromQuery] string? responsable = null,
        CancellationToken cancellationToken = default)
    {
        var command = new ListarFecundacionesPaginadoCommand(
            page,
            limit,
            fechaDesde,
            fechaHasta,
            q,
            tipoFecundacion,
            estado,
            responsable);

        var output = await _listarPaginadoInputPort.HandleAsync(command, cancellationToken);

        var responseData = output.Data.Select(x => new FecundacionResponse(
            x.Id,
            x.Codigo,
            x.Fecha,
            x.Vacuno,
            FecundacionMapper.ToContractTipo(x.TipoFecundacion),
            x.ToroODonante,
            x.Responsable,
            FecundacionMapper.ToContractResultado(x.Estado)
        )).ToList();

        return Ok(GeneralResponseDTO<object>.Ok(new
        {
            data = responseData,
            pagination = new
            {
                page = output.Page,
                limit = output.Limit,
                total = output.Total,
                totalPages = output.TotalPages
            }
        }));
    }

    [HttpPost]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Registrar(
        [FromBody] CreateFecundacionRequest request,
        CancellationToken cancellationToken)
    {
        var command = FecundacionMapper.ToCommand(request, GetUserIdFromHeader());
        var output = await _createInputPort.HandleAsync(command, cancellationToken);

        var detail = await _getForEditInputPort.HandleAsync(output.Id, cancellationToken);

        return Created(
            $"/api/v1/fecundaciones/{output.Id}",
            GeneralResponseDTO<object>.Ok(new
            {
                id = detail.Id,
                codigo = detail.Codigo,
                tipoFecundacion = FecundacionMapper.ToContractTipo(detail.TipoFecundacionCode),
                vacunoReceptorId = detail.VacunoReceptorId,
                fechaProcedimiento = detail.FechaProcedimiento,
                resultado = FecundacionMapper.ToContractResultado(detail.ResultadoCode),
                creadoEn = detail.CreadoEn
            }));
    }

    [HttpGet("{fecundacionId:long}")]
    [ProducesResponseType(typeof(GeneralResponseDTO<FecundacionEditResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(long fecundacionId, CancellationToken cancellationToken)
    {
        var output = await _getForEditInputPort.HandleAsync(fecundacionId, cancellationToken);
        return Ok(GeneralResponseDTO<FecundacionEditResponse>.Ok(FecundacionMapper.ToResponse(output)));
    }

    [HttpGet("opciones")]
    [ProducesResponseType(typeof(GeneralResponseDTO<FecundacionOptionsResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOptions(CancellationToken cancellationToken)
    {
        var output = await _getOptionsInputPort.HandleAsync(cancellationToken);
        return Ok(GeneralResponseDTO<FecundacionOptionsResponse>.Ok(FecundacionMapper.ToResponse(output)));
    }

    [HttpGet("vacunos")]
    [ProducesResponseType(typeof(GeneralResponseDTO<IReadOnlyList<FecundacionVacunoOptionResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchVacunos(
        [FromQuery] string? sexo,
        [FromQuery(Name = "q")] string? query,
        CancellationToken cancellationToken)
    {
        var output = await _searchVacunosInputPort.HandleAsync(
            new SearchFecundacionVacunosQuery(sexo, query),
            cancellationToken);

        var response = output.Select(FecundacionMapper.ToResponse).ToList();
        return Ok(GeneralResponseDTO<IReadOnlyList<FecundacionVacunoOptionResponse>>.Ok(response));
    }

    [HttpPatch("{fecundacionId:long}")]
    [ProducesResponseType(typeof(GeneralResponseDTO<FecundacionUpdateResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(
        long fecundacionId,
        [FromBody] UpdateFecundacionRequest request,
        CancellationToken cancellationToken)
    {
        var current = await _getForEditInputPort.HandleAsync(fecundacionId, cancellationToken);
        var output = await _updateInputPort.HandleAsync(
            FecundacionMapper.ToCommand(request, current, fecundacionId),
            cancellationToken);

        return Ok(GeneralResponseDTO<FecundacionUpdateResponse>.Ok(FecundacionMapper.ToResponse(output)));
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Eliminar(
        long id,
        [FromBody] DeleteFecundacionRequest request,
        CancellationToken cancellationToken)
    {
        var command = new DeleteFecundacionCommand(request.Razon);
        await _deleteInputPort.HandleAsync(id, command, cancellationToken);
        return NoContent();
    }

    private long GetUserIdFromHeader()
    {
        if (Request.Headers.TryGetValue("X-User-Id", out var values) &&
            long.TryParse(values.FirstOrDefault(), out var userId))
        {
            return userId;
        }
        return 1;
    }
}
