using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Modules.Module_Celo.UseCases.CreateCelo;
using ZooTech.Application.Modules.Module_Celo.UseCases.DeleteCelo;
using ZooTech.Application.Modules.Module_Celo.UseCases.GetCelos;
using ZooTech.Application.Modules.Module_Celo.UseCases.GetComparacionCelosRealVsEstandar;
using ZooTech.Application.Modules.Module_Celo.UseCases.GetComparacionCelosRealVsEstandarPorVacuno;
using ZooTech.Application.Modules.Module_Celo.UseCases.GetReporteCelos;
using ZooTech.Application.Modules.Module_Celo.UseCases.GetVacasEnCelo;
using ZooTech.Application.Modules.Module_Celo.UseCases.ListCelos;
using ZooTech.Application.Modules.Module_Celo.UseCases.ListReporteCeloGeneral;
using ZooTech.Application.Modules.Module_Celo.UseCases.UpdateCelo;
using ZooTech.InterfaceAdapters.DTOs;
using ZooTech.InterfaceAdapters.Modules.Module_Celo.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Celo.DTOs.Responses;
using ZooTech.InterfaceAdapters.Modules.Module_Celo.Mappers;
using ApiErrorResponse = ZooTech.InterfaceAdapters.DTOs.Responses.ErrorResponse;

namespace ZooTech.InterfaceAdapters.Modules.Module_Celo.Controllers;

[ApiController]
[Route("api/v1/celo")]
[ApiExplorerSettings(GroupName = "public")]
public sealed class CeloController : ControllerBase
{
    private readonly IGetCelosInputPort _getCelosInputPort;
    private readonly IGetReporteCelosInputPort _getReporteCelosInputPort;
    private readonly ICreateCeloInputPort _createCeloInputPort;
    private readonly IUpdateCeloInputPort _updateCeloInputPort;
    private readonly IDeleteCeloInputPort _deleteCeloInputPort;
    private readonly IGetComparacionCelosRealVsEstandarInputPort _getComparacionInputPort;
    private readonly IGetComparacionCelosRealVsEstandarPorVacunoInputPort _getComparacionPorVacunoInputPort;
    private readonly IGetVacasEnCeloInputPort _getVacasEnCeloInputPort;
    private readonly IListCelosInputPort _listCelosInputPort;
    private readonly IListReporteCeloGeneralInputPort _listReporteCeloGeneralInputPort;

    public CeloController(
        IGetCelosInputPort getCelosInputPort,
        IGetReporteCelosInputPort getReporteCelosInputPort,
        ICreateCeloInputPort createCeloInputPort,
        IUpdateCeloInputPort updateCeloInputPort,
        IDeleteCeloInputPort deleteCeloInputPort,
        IGetComparacionCelosRealVsEstandarInputPort getComparacionInputPort,
        IGetComparacionCelosRealVsEstandarPorVacunoInputPort getComparacionPorVacunoInputPort,
        IGetVacasEnCeloInputPort getVacasEnCeloInputPort,
        IListCelosInputPort listCelosInputPort,
        IListReporteCeloGeneralInputPort listReporteCeloGeneralInputPort)
    {
        _getCelosInputPort = getCelosInputPort;
        _getReporteCelosInputPort = getReporteCelosInputPort;
        _createCeloInputPort = createCeloInputPort;
        _updateCeloInputPort = updateCeloInputPort;
        _deleteCeloInputPort = deleteCeloInputPort;
        _getComparacionInputPort = getComparacionInputPort;
        _getComparacionPorVacunoInputPort = getComparacionPorVacunoInputPort;
        _getVacasEnCeloInputPort = getVacasEnCeloInputPort;
        _listCelosInputPort = listCelosInputPort;
        _listReporteCeloGeneralInputPort = listReporteCeloGeneralInputPort;
    }

    [HttpGet]
    [ProducesResponseType(typeof(GeneralResponseDTO<ListCelosResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCelos(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] DateTime? fechaInicio = null,
        [FromQuery] DateTime? fechaFin = null,
        [FromQuery] Dictionary<string, string>? columnFilters = null,
        CancellationToken cancellationToken = default)
    {
        var output = await _listCelosInputPort.HandleAsync(
            search, page, pageSize, fechaInicio, fechaFin, columnFilters, cancellationToken);
        var response = CeloMapper.ToResponse(output);

        return Ok(GeneralResponseDTO<ListCelosResponse>.Ok(response));
    }

    [HttpGet("reportes/general")]
    [ProducesResponseType(typeof(GeneralResponseDTO<ListReporteCeloGeneralResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReporteCeloGeneral(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] DateTime? fechaInicio = null,
        [FromQuery] DateTime? fechaFin = null,
        [FromQuery] Dictionary<string, string>? columnFilters = null,
        CancellationToken cancellationToken = default)
    {
        var output = await _listReporteCeloGeneralInputPort.HandleAsync(
            search, page, pageSize, fechaInicio, fechaFin, columnFilters, cancellationToken);
        var response = CeloMapper.ToResponse(output);

        return Ok(GeneralResponseDTO<ListReporteCeloGeneralResponse>.Ok(response));
    }

    [HttpGet("reportes/por-vacuno")]
    [ProducesResponseType(typeof(GeneralResponseDTO<List<ReporteCeloPorVacunoResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReporteCeloPorVacuno(
        [FromQuery] DateTime? fechaInicio,
        [FromQuery] DateTime? fechaFin,
        CancellationToken cancellationToken)
    {
        var output = await _getCelosInputPort.HandleAsync(cancellationToken);

        var inicio = fechaInicio.HasValue ? DateOnly.FromDateTime(fechaInicio.Value) : (DateOnly?)null;
        var fin = fechaFin.HasValue ? DateOnly.FromDateTime(fechaFin.Value) : (DateOnly?)null;

        var items = output.Items.Where(item =>
            (!inicio.HasValue || item.Fecha >= inicio.Value) &&
            (!fin.HasValue || item.Fecha <= fin.Value));

        var response = items
            .GroupBy(item => new
            {
                item.CodigoVacuno,
                item.NombreVacuno
            })
            .Select(group => new ReporteCeloPorVacunoResponse
            {
                CodigoVacuno = group.Key.CodigoVacuno,
                Nombre = group.Key.NombreVacuno,
                UltimoCelo = group.Max(item => item.Fecha),
                VecesEnCelo = group.Count()
            })
            .OrderByDescending(item => item.UltimoCelo)
            .ToList();

        return Ok(GeneralResponseDTO<List<ReporteCeloPorVacunoResponse>>.Ok(response));
    }

    [HttpGet("reportes/por-vacuno/{codigoVacuno}")]
    [ProducesResponseType(typeof(GeneralResponseDTO<List<CeloReporteItemResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDetalleCeloPorVacuno(
    string codigoVacuno,
    CancellationToken cancellationToken)
    {
        var output = await _getReporteCelosInputPort.HandleAsync(cancellationToken);

        var response = output.Items
            .Where(item => item.CodigoVacuno == codigoVacuno)
            .OrderByDescending(item => item.Fecha)
            .ThenByDescending(item => item.Hora)
            .Select(item => CeloMapper.ToResponse(item))
            .ToList();

        return Ok(GeneralResponseDTO<List<CeloReporteItemResponse>>.Ok(response));
    }

    [HttpGet("vacas-en-celo")]
    [ProducesResponseType(typeof(GeneralResponseDTO<List<VacaEnCeloResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetVacasEnCelo(
        [FromQuery] DateTime? fechaInicio,
        [FromQuery] DateTime? fechaFin,
        CancellationToken cancellationToken)
    {
        var output = await _getVacasEnCeloInputPort.HandleAsync(
            fechaInicio,
            fechaFin,
            cancellationToken);

        var response = output.Items.Select(item => CeloMapper.ToResponse(item)).ToList();

        return Ok(GeneralResponseDTO<List<VacaEnCeloResponse>>.Ok(response));
    }

    [HttpGet("reportes/comparacion-real-vs-estandar")]
    [ProducesResponseType(typeof(GeneralResponseDTO<List<ComparacionCelosResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetComparacionRealVsEstandar(
        [FromQuery] DateTime? fechaInicio,
        [FromQuery] DateTime? fechaFin,
        CancellationToken cancellationToken)
    {
        var output = await _getComparacionInputPort.HandleAsync(
            fechaInicio,
            fechaFin,
            cancellationToken);

        var response = output.Items
            .Select(CeloMapper.ToResponse)
            .ToList();

        return Ok(
            GeneralResponseDTO<List<ComparacionCelosResponse>>.Ok(response));
    }

    [HttpGet("reportes/comparacion-por-vacuno/{codigoVacuno}")]
    [ProducesResponseType(typeof(GeneralResponseDTO<List<ComparacionCelosPorVacunoResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetComparacionPorVacuno(
        string codigoVacuno,
        [FromQuery] DateTime? fechaInicio,
        [FromQuery] DateTime? fechaFin,
        CancellationToken cancellationToken)
    {
        var output = await _getComparacionPorVacunoInputPort.HandleAsync(
            codigoVacuno,
            fechaInicio,
            fechaFin,
            cancellationToken);

        var response = output.Items
            .Select(CeloMapper.ToResponse)
            .ToList();

        return Ok(
            GeneralResponseDTO<List<ComparacionCelosPorVacunoResponse>>.Ok(response));
    }

    [HttpPost]
    [ProducesResponseType(typeof(GeneralResponseDTO<CreateCeloResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateCelo(
        [FromBody] CreateCeloRequest request,
        CancellationToken cancellationToken)
    {
        var command = CeloMapper.ToCommand(request);
        var output = await _createCeloInputPort.HandleAsync(command, cancellationToken);
        var response = CeloMapper.ToResponse(output);

        return CreatedAtAction(
            nameof(GetCelos),
            null,
            GeneralResponseDTO<CreateCeloResponse>.Ok(response));
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(GeneralResponseDTO<UpdateCeloResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCelo(
        long id,
        [FromBody] UpdateCeloRequest request,
        CancellationToken cancellationToken)
    {
        var command = CeloMapper.ToCommand(request) with { Id = id };
        var output = await _updateCeloInputPort.HandleAsync(command, cancellationToken);
        var response = CeloMapper.ToResponse(output);

        return Ok(GeneralResponseDTO<UpdateCeloResponse>.Ok(response));
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCelo(
        long id,
        [FromBody] DeleteCeloRequest request,
        CancellationToken cancellationToken)
    {
        var command = CeloMapper.ToCommand(request, id);
        await _deleteCeloInputPort.HandleAsync(command, cancellationToken);

        return Ok(GeneralResponseDTO<object>.Ok(new
        {
            mensaje = "Celo eliminado con éxito"
        }));
    }
}