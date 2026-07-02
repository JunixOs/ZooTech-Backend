using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.GetFecundacionForEdit;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.GetFecundacionOptions;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.SearchFecundacionVacunos;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.UpdateFecundacion;
using ZooTech.InterfaceAdapters.DTOs;
using ZooTech.InterfaceAdapters.Modules.Module_Fecundacion.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Fecundacion.DTOs.Responses;
using ZooTech.InterfaceAdapters.Modules.Module_Fecundacion.Mappers;

namespace ZooTech.InterfaceAdapters.Modules.Module_Fecundacion.Controllers;

[ApiController]
[Route("api/v1")]
[ApiExplorerSettings(GroupName = "public")]
public sealed class FecundacionController : ControllerBase
{
    private readonly IGetFecundacionForEditInputPort _getForEditInputPort;
    private readonly IGetFecundacionOptionsInputPort _getOptionsInputPort;
    private readonly ISearchFecundacionVacunosInputPort _searchVacunosInputPort;
    private readonly IUpdateFecundacionInputPort _updateInputPort;

    public FecundacionController(
        IGetFecundacionForEditInputPort getForEditInputPort,
        IGetFecundacionOptionsInputPort getOptionsInputPort,
        ISearchFecundacionVacunosInputPort searchVacunosInputPort,
        IUpdateFecundacionInputPort updateInputPort)
    {
        _getForEditInputPort = getForEditInputPort;
        _getOptionsInputPort = getOptionsInputPort;
        _searchVacunosInputPort = searchVacunosInputPort;
        _updateInputPort = updateInputPort;
    }

    [HttpGet("fecundaciones/{fecundacionId:long}")]
    [HttpGet("fecundacion/{fecundacionId:long}")]
    [ProducesResponseType(typeof(GeneralResponseDTO<FecundacionEditResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetForEdit(long fecundacionId, CancellationToken cancellationToken)
    {
        var output = await _getForEditInputPort.HandleAsync(fecundacionId, cancellationToken);
        return Ok(GeneralResponseDTO<FecundacionEditResponse>.Ok(FecundacionMapper.ToResponse(output)));
    }

    [HttpGet("fecundacion/opciones")]
    [ProducesResponseType(typeof(GeneralResponseDTO<FecundacionOptionsResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOptions(CancellationToken cancellationToken)
    {
        var output = await _getOptionsInputPort.HandleAsync(cancellationToken);
        return Ok(GeneralResponseDTO<FecundacionOptionsResponse>.Ok(FecundacionMapper.ToResponse(output)));
    }

    [HttpGet("fecundacion/vacunos")]
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

    [HttpPatch("fecundaciones/{fecundacionId:long}")]
    [HttpPut("fecundacion/{fecundacionId:long}")]
    [ProducesResponseType(typeof(GeneralResponseDTO<FecundacionUpdateResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status404NotFound)]
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
}
