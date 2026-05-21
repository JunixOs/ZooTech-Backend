using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Modules.Module_Celo.UseCases.ListarCelos;
using ZooTech.InterfaceAdapters.DTOs;
using ZooTech.InterfaceAdapters.Modules.Module_Celo.DTOs.Responses;

namespace ZooTech.InterfaceAdapters.Modules.Module_Celo.Controllers;

[ApiController]
[Route("api/v1/celo")]
[ApiExplorerSettings(GroupName = "public")]
public class CeloController : ControllerBase
{
    private readonly IListarCelosUseCase _listarCelosUseCase;

    public CeloController(IListarCelosUseCase listarCelosUseCase)
    {
        _listarCelosUseCase = listarCelosUseCase;
    }

    [HttpGet]
    public async Task<IActionResult> ListarCelos(CancellationToken cancellationToken)
    {
        var items = await _listarCelosUseCase.ExecuteAsync(cancellationToken);

        var response = items.Select(item => new CeloItemResponse
        {
            CodigoRegistro = item.CodigoRegistro,
            Fecha = item.Fecha,
            Hora = item.Hora,
            CodigoVacuno = item.CodigoVacuno,
            NombreVacuno = item.NombreVacuno,
            VecesEnCelo = item.VecesEnCelo
        }).ToList();

        return Ok(GeneralResponseDTO<List<CeloItemResponse>>.Ok(response));
    }
}
