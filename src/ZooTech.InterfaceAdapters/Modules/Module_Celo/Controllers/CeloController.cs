using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Common.Behaviors.Module_Celo.CreateCelo;
using ZooTech.Application.Common.Behaviors.Module_Celo.DeleteCelo;
using ZooTech.Application.Common.Behaviors.Module_Celo.GetCelos;
using ZooTech.Application.Common.Behaviors.Module_Celo.UpdateCelo;
using ZooTech.Application.Common.Models;
using ZooTech.Domain.Shared.Enums;
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
    private readonly IGetCelosBehaviorPipelineFactory _getCelosBehaviorPipelineFactory;
    private readonly ICreateCeloBehaviorPipelineFactory _createCeloBehaviorPipelineFactory;
    private readonly IUpdateCeloBehaviorPipelineFactory _updateCeloBehaviorPipelineFactory;
    private readonly IDeleteCeloBehaviorPipelineFactory _deleteCeloBehaviorPipelineFactory;

    public CeloController(
        IGetCelosBehaviorPipelineFactory getCelosBehaviorPipelineFactory,
        ICreateCeloBehaviorPipelineFactory createCeloBehaviorPipelineFactory,
        IUpdateCeloBehaviorPipelineFactory updateCeloBehaviorPipelineFactory,
        IDeleteCeloBehaviorPipelineFactory deleteCeloBehaviorPipelineFactory
    )
    {
        _getCelosBehaviorPipelineFactory = getCelosBehaviorPipelineFactory;
        _createCeloBehaviorPipelineFactory = createCeloBehaviorPipelineFactory;;
        _updateCeloBehaviorPipelineFactory = updateCeloBehaviorPipelineFactory;
        _deleteCeloBehaviorPipelineFactory = deleteCeloBehaviorPipelineFactory;
    }

    [HttpGet]
    [ProducesResponseType(typeof(GeneralResponseDTO<List<CeloItemResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCelos(CancellationToken cancellationToken)
    {
        var behaviorPipeline = _getCelosBehaviorPipelineFactory.Create();

        var result = await behaviorPipeline.Execute(
            EmptyCommand.Value(AuditEventType.Read , "Get celos"),
            cancellationToken
        );

        var response = result.Items.Select(CeloMapper.ToResponse).ToList();
        return Ok(GeneralResponseDTO<List<CeloItemResponse>>.Ok(response));
    }

    [HttpGet("vacas-en-celo")]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetVacasEnCelo(CancellationToken cancellationToken)
    {
        var behaviorPipeline = _getCelosBehaviorPipelineFactory.Create();

        var output = await behaviorPipeline.Execute(
            EmptyCommand.Value(
                AuditEventType.Read,
                "Get celos"
            ),
            cancellationToken
        );

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

    [HttpPost]
    [ProducesResponseType(typeof(GeneralResponseDTO<CreateCeloResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GeneralResponseDTO<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateCelo(
        [FromBody] CreateCeloRequest request,
        CancellationToken cancellationToken)
    {
        var behaviorPipeline = _createCeloBehaviorPipelineFactory.Create();

        var command = CeloMapper.ToCommand(request);
        var output = await behaviorPipeline.Execute(command, cancellationToken);
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
        var behaviorPipeline = _updateCeloBehaviorPipelineFactory.Create();

        var command = CeloMapper.ToCommand(request);
        var output = await behaviorPipeline.Execute(command, cancellationToken);
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
        var behaviorPipeline = _deleteCeloBehaviorPipelineFactory.Create();

        var command = CeloMapper.ToCommand(request, id);
        await behaviorPipeline.Execute(command, cancellationToken);

        return Ok(GeneralResponseDTO<object>.Ok(new { mensaje = "Celo eliminado con éxito" }));
    }
}
