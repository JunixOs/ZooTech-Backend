using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunos;
using ZooTech.InterfaceAdapters.DTOs;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Responses;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Mappers;

namespace ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Controllers;

[ApiController]
[Route("api/v1/vacuno")]
[ApiExplorerSettings(GroupName = "public")]
public sealed class VacunoController : ControllerBase
{
    private readonly IListarVacunosInputPort _listarVacunosInputPort;

    public VacunoController(IListarVacunosInputPort listarVacunosInputPort)
    {
        _listarVacunosInputPort = listarVacunosInputPort;
    }

    [HttpGet]
    [ProducesResponseType(typeof(GeneralResponseDTO<List<VacunoItemResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarVacunos(CancellationToken cancellationToken)
    {
        var output = await _listarVacunosInputPort.HandleAsync(cancellationToken);

        var response = output.Items
            .Select(VacunoMapper.ToResponse)
            .ToList();

        return Ok(GeneralResponseDTO<List<VacunoItemResponse>>.Ok(response));
    }
}
