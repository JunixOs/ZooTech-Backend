using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Modules.Module_Celo.UseCases.CreateCelo;
using ZooTech.Application.Modules.Module_Celo.UseCases.DeleteCelo;
using ZooTech.Application.Modules.Module_Celo.UseCases.GetCelos;
using ZooTech.Application.Modules.Module_Celo.UseCases.GetComparacionCelosRealVsEstandar;
using ZooTech.Application.Modules.Module_Celo.UseCases.GetReporteCelos;
using ZooTech.Application.Modules.Module_Celo.UseCases.UpdateCelo;
using ZooTech.InterfaceAdapters.DTOs;
using ZooTech.InterfaceAdapters.Modules.Module_Celo.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Celo.DTOs.Responses;
using ZooTech.InterfaceAdapters.Modules.Module_Celo.Mappers;

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

    public CeloController(
        IGetCelosInputPort getCelosInputPort,
        IGetReporteCelosInputPort getReporteCelosInputPort,
        ICreateCeloInputPort createCeloInputPort,
        IUpdateCeloInputPort updateCeloInputPort,
        IDeleteCeloInputPort deleteCeloInputPort,
        IGetComparacionCelosRealVsEstandarInputPort getComparacionInputPort)
    {
        _getCelosInputPort = getCelosInputPort;
        _getReporteCelosInputPort = getReporteCelosInputPort;
        _createCeloInputPort = createCeloInputPort;
        _updateCeloInputPort = updateCeloInputPort;
        _deleteCeloInputPort = deleteCeloInputPort;
        _getComparacionInputPort = getComparacionInputPort;
    }

    [HttpGet]
    [ProducesResponseType(typeof(GeneralResponseDTO<List<CeloItemResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCelos(CancellationToken cancellationToken)
    {
        var output = await _getCelosInputPort.HandleAsync(cancellationToken);
        var response = output.Items.Select(CeloMapper.ToResponse).ToList();

        return Ok(GeneralResponseDTO<List<CeloItemResponse>>.Ok(response));
    }

    [HttpGet("reportes/general")]
    [ProducesResponseType(typeof(GeneralResponseDTO<List<CeloReporteItemResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReporteCeloGeneral(CancellationToken cancellationToken)
    {
        var output = await _getReporteCelosInputPort.HandleAsync(cancellationToken);
        var response = output.Items.Select(item => CeloMapper.ToResponse(item)).ToList();

        return Ok(GeneralResponseDTO<List<CeloReporteItemResponse>>.Ok(response));
    }

    [HttpGet("reportes/por-vacuno")]
    [ProducesResponseType(typeof(GeneralResponseDTO<List<ReporteCeloPorVacunoResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReporteCeloPorVacuno(CancellationToken cancellationToken)
    {
        var output = await _getCelosInputPort.HandleAsync(cancellationToken);

        var response = output.Items
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
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetVacasEnCelo(CancellationToken cancellationToken)
    {
        var output = await _getCelosInputPort.HandleAsync(cancellationToken);

        var response = output.Items.Select((item, index) => new
        {
            id = index + 1,
            codigo = item.CodigoVacuno,
            nombre = item.NombreVacuno,
            diasRestante = 0,
            estado = "En celo",
            vecesEnCelo = item.VecesEnCelo,
            crias = 0
        }).ToList();

        return Ok(GeneralResponseDTO<object>.Ok(response));
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

    [HttpPost]
    [ProducesResponseType(typeof(GeneralResponseDTO<CreateCeloResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status409Conflict)]
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
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status404NotFound)]
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
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status404NotFound)]
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