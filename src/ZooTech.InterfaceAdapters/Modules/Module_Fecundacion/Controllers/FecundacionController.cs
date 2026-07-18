using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Common.Behaviors.Module_Fecundacion.CreateFecundacion;
using ZooTech.Application.Common.Behaviors.Module_Fecundacion.DeleteFecundacion;
using ZooTech.Application.Common.Behaviors.Module_Fecundacion.GetFecundacionForEdit;
using ZooTech.Application.Common.Behaviors.Module_Fecundacion.GetFecundacionOptions;
using ZooTech.Application.Common.Behaviors.Module_Fecundacion.ListarFecundacion;
using ZooTech.Application.Common.Behaviors.Module_Fecundacion.SearchFecundacionVacunos;
using ZooTech.Application.Common.Behaviors.Module_Fecundacion.UpdateFecundacion;
using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.CreateFecundacion;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.DeleteFecundacion;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.GetFecundacionForEdit;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.GetFecundacionOptions;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.ListarFecundacion;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.SearchFecundacionVacunos;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.UpdateFecundacion;
using ZooTech.Domain.Shared.Enums;
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
    private readonly ICreateFecundacionBehaviorPipelineFactory _createFecundacionBehaviorPipelineFactory;
    private readonly IListarFecundacionBehaviorPipelineFactory _listarFecundacionBehaviorPipelineFactory;
    private readonly IGetFecundacionForEditBehaviorPipelineFactory _getFecundacionForEditBehaviorPipelineFactory;
    private readonly IGetFecundacionOptionsBehaviorPipelineFactory _getFecundacionOptionsBehaviorPipelineFactory;
    private readonly ISearchFecundacionVacunosBehaviorPipelineFactory _searchFecundacionVacunosBehaviorPipelineFactory;
    private readonly IUpdateFecundacionBehaviorPipelineFactory _updateFecundacionBehaviorPipelineFactory;
    private readonly IDeleteFecundacionBehaviorPipelineFactory _deleteFecundacionBehaviorPipelineFactory;

    public FecundacionController(
        ICreateFecundacionBehaviorPipelineFactory createFecundacionBehaviorPipelineFactory,
        IListarFecundacionBehaviorPipelineFactory listarFecundacionBehaviorPipelineFactory,
        IGetFecundacionForEditBehaviorPipelineFactory getFecundacionForEditBehaviorPipelineFactory,
        IGetFecundacionOptionsBehaviorPipelineFactory getFecundacionOptionsBehaviorPipelineFactory,
        ISearchFecundacionVacunosBehaviorPipelineFactory searchFecundacionVacunosBehaviorPipelineFactory,
        IUpdateFecundacionBehaviorPipelineFactory updateFecundacionBehaviorPipelineFactory,
        IDeleteFecundacionBehaviorPipelineFactory deleteFecundacionBehaviorPipelineFactory
    )
    {
        _createFecundacionBehaviorPipelineFactory = createFecundacionBehaviorPipelineFactory;
        _listarFecundacionBehaviorPipelineFactory = listarFecundacionBehaviorPipelineFactory;
        _getFecundacionForEditBehaviorPipelineFactory = getFecundacionForEditBehaviorPipelineFactory;
        _getFecundacionOptionsBehaviorPipelineFactory = getFecundacionOptionsBehaviorPipelineFactory;
        _searchFecundacionVacunosBehaviorPipelineFactory = searchFecundacionVacunosBehaviorPipelineFactory;
        _updateFecundacionBehaviorPipelineFactory = updateFecundacionBehaviorPipelineFactory;
        _deleteFecundacionBehaviorPipelineFactory = deleteFecundacionBehaviorPipelineFactory;
    }

    // ===== TUYO — sin cambios =====
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<List<FecundacionItemResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarFecundacion(
        [FromQuery] string? search,
        [FromQuery] string? query,
        [FromQuery(Name = "q")] string? q,
        [FromQuery] DateTime? fechaDesde,
        [FromQuery] DateTime? fechaHasta,
        [FromQuery] string? resultado,
        [FromQuery] int page = 1,
        [FromQuery] int? pageSize = null,
        [FromQuery] int? limit = null,
        CancellationToken cancellationToken = default)
    {
        var currentPage = NormalizePage(page);
        var currentPageSize = NormalizePageSize(pageSize ?? limit);
        var searchTerm = FirstNonBlank(search, query, q);

        var command = new ListarFecundacionCommand(
            Query: searchTerm,
            FechaDesde: fechaDesde,
            FechaHasta: fechaHasta,
            Resultado: FecundacionMapper.ToInternalResultado(resultado),
            Page: currentPage,
            Limit: currentPageSize);

        var behaviorPipeline = _listarFecundacionBehaviorPipelineFactory.Create();

        var output = await behaviorPipeline.Execute(command, cancellationToken);
        var response = output.Items.Select(FecundacionMapper.ToListItemResponse).ToList();
        return Ok(PagedResponse<List<FecundacionItemResponse>>.OkPaged(response, currentPage, currentPageSize, output.TotalCount));
    }

    // ===== DE ÉL — Create =====
    [HttpPost]
    [ProducesResponseType(typeof(GeneralResponseDTO<CreateFecundacionResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateFecundacion(
        [FromBody] CreateFecundacionRequest request,
        CancellationToken cancellationToken)
    {
        var behaviorPipeline = _createFecundacionBehaviorPipelineFactory.Create();

        var command = FecundacionMapper.ToCommand(request, GetUserIdFromHeader());
        var output = await behaviorPipeline.Execute(command, cancellationToken);
        var response = FecundacionMapper.ToResponse(output);

        return StatusCode(StatusCodes.Status201Created, GeneralResponseDTO<CreateFecundacionResponse>.Ok(response));
    }

    [HttpGet("{fecundacionId:long}")]
    [ProducesResponseType(typeof(GeneralResponseDTO<FecundacionEditResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(long fecundacionId, CancellationToken cancellationToken)
    {
        var behaviorPipeline = _getFecundacionForEditBehaviorPipelineFactory.Create();

        var output = await behaviorPipeline.Execute(
            new GetFecundacionForEditCommand(fecundacionId), 
            cancellationToken
        );
        return Ok(GeneralResponseDTO<FecundacionEditResponse>.Ok(FecundacionMapper.ToResponse(output)));
    }

    [HttpGet("opciones")]
    [ProducesResponseType(typeof(GeneralResponseDTO<FecundacionOptionsResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOptions(CancellationToken cancellationToken)
    {
        var behaviorPipeline = _getFecundacionOptionsBehaviorPipelineFactory.Create();

        var output = await behaviorPipeline.Execute(
            EmptyCommand.Value(
                AuditEventType.Read,
                "Get fecundacion options"
            ),
            cancellationToken
        );
        return Ok(GeneralResponseDTO<FecundacionOptionsResponse>.Ok(FecundacionMapper.ToResponse(output)));
    }

    [HttpGet("vacunos")]
    [ProducesResponseType(typeof(GeneralResponseDTO<IReadOnlyList<FecundacionVacunoOptionResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchVacunos(
        [FromQuery] string? sexo,
        [FromQuery(Name = "q")] string? query,
        [FromQuery] bool soloDisponibles = false,
        [FromQuery] long? excluirFecundacionId = null,
        CancellationToken cancellationToken = default)
    {
        var behaviorPipeline = _searchFecundacionVacunosBehaviorPipelineFactory.Create();

        var output = await behaviorPipeline.Execute(
            new SearchFecundacionVacunosQuery(sexo, query, soloDisponibles, excluirFecundacionId),
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
        var behaviorPipelineGetForEdit = _getFecundacionForEditBehaviorPipelineFactory.Create();
        var behaviorPipelineUpdate = _updateFecundacionBehaviorPipelineFactory.Create();


        var current = await behaviorPipelineGetForEdit.Execute(
            new GetFecundacionForEditCommand(fecundacionId), 
            cancellationToken
        );
        var output = await behaviorPipelineUpdate.Execute(
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
        var behaviorPipeline = _deleteFecundacionBehaviorPipelineFactory.Create();

        var command = new DeleteFecundacionCommand(id, request.Razon);
        await behaviorPipeline.Execute(command, cancellationToken);
        return NoContent();
    }

    private static int NormalizePage(int page)
        => page <= 0 ? 1 : page;

    private static int NormalizePageSize(int? pageSize)
        => !pageSize.HasValue || pageSize.Value <= 0
            ? 20
            : Math.Min(pageSize.Value, 100);

    private static string? FirstNonBlank(params string?[] values)
        => values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value))?.Trim();

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
