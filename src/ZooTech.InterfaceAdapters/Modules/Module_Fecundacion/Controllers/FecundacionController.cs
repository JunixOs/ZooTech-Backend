using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Common.Behaviors;
using ZooTech.Application.Common.Gateway.Identity;
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
using ZooTech.InterfaceAdapters.Filters;
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
    private readonly ICurrentUserService _currentUserService;

    private readonly IBehaviorDispatcher _behaviorDispatcher;

    public FecundacionController(
        ICurrentUserService currentUserService,

        IBehaviorDispatcher behaviorDispatcher
    )
    {
        _currentUserService = currentUserService;

        _behaviorDispatcher = behaviorDispatcher;
    }

    [HttpGet]
    [ServiceFilter(typeof(TenantHeaderFilter))]
    [RestrictTenantType(TenantType.Tenant)]
    [Authorize(Roles = AuthorizationRoles.Regular)]
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

        var command = new ListarFecundacionQuery(
            Query: searchTerm,
            FechaDesde: fechaDesde,
            FechaHasta: fechaHasta,
            Resultado: FecundacionMapper.ToInternalResultado(resultado),
            Page: currentPage,
            Limit: currentPageSize);

        var output = await _behaviorDispatcher.Send<ListarFecundacionQuery , ListarFecundacionOutput>(command, cancellationToken);
        var response = output.Items.Select(FecundacionMapper.ToListItemResponse).ToList();
        return Ok(PagedResponse<List<FecundacionItemResponse>>.OkPaged(response, currentPage, currentPageSize, output.TotalCount));
    }

    // ===== DE ÉL — Create =====
    [HttpPost]
    [ServiceFilter(typeof(TenantHeaderFilter))]
    [RestrictTenantType(TenantType.Tenant)]
    [Authorize(Roles = AuthorizationRoles.Regular)]
    [ProducesResponseType(typeof(GeneralResponseDTO<CreateFecundacionResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateFecundacion(
        [FromBody] CreateFecundacionRequest request,
        CancellationToken cancellationToken)
    {
        var command = FecundacionMapper.ToCommand(request, _currentUserService.UserId);
        var output = await _behaviorDispatcher.Send<CreateFecundacionCommand , CreateFecundacionOutput>(command, cancellationToken);
        var response = FecundacionMapper.ToResponse(output);

        return StatusCode(StatusCodes.Status201Created, GeneralResponseDTO<CreateFecundacionResponse>.Ok(response));
    }

    [HttpGet("{fecundacionId:long}")]
    [ServiceFilter(typeof(TenantHeaderFilter))]
    [RestrictTenantType(TenantType.Tenant)]
    [Authorize(Roles = AuthorizationRoles.Regular)]
    [ProducesResponseType(typeof(GeneralResponseDTO<FecundacionEditResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(long fecundacionId, CancellationToken cancellationToken)
    {
        var output = await _behaviorDispatcher.Send<GetFecundacionForEditQuery , GetFecundacionForEditOutput>(
            new GetFecundacionForEditQuery(fecundacionId), 
            cancellationToken
        );
        return Ok(GeneralResponseDTO<FecundacionEditResponse>.Ok(FecundacionMapper.ToResponse(output)));
    }

    [HttpGet("opciones")]
    [ServiceFilter(typeof(TenantHeaderFilter))]
    [RestrictTenantType(TenantType.Tenant)]
    [Authorize(Roles = AuthorizationRoles.Regular)]
    [ProducesResponseType(typeof(GeneralResponseDTO<FecundacionOptionsResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOptions(CancellationToken cancellationToken)
    {
        var output = await _behaviorDispatcher.Send<EmptyCommandQuery , GetFecundacionOptionsOutput>(
            EmptyCommandQuery.Value(
                AuditEventType.Read,
                "Get fecundacion options"
            ),
            cancellationToken
        );
        return Ok(GeneralResponseDTO<FecundacionOptionsResponse>.Ok(FecundacionMapper.ToResponse(output)));
    }

    [HttpGet("vacunos")]
    [ServiceFilter(typeof(TenantHeaderFilter))]
    [RestrictTenantType(TenantType.Tenant)]
    [Authorize(Roles = AuthorizationRoles.Regular)]
    [ProducesResponseType(typeof(GeneralResponseDTO<IReadOnlyList<FecundacionVacunoOptionResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchVacunos(
        [FromQuery] string? sexo,
        [FromQuery(Name = "q")] string? query,
        [FromQuery] bool soloDisponibles = false,
        [FromQuery] long? excluirFecundacionId = null,
        CancellationToken cancellationToken = default)
    {
        var output = await _behaviorDispatcher.Send<SearchFecundacionVacunosQuery , IReadOnlyList<SearchFecundacionVacunoOutput>>(
            new SearchFecundacionVacunosQuery(sexo, query, soloDisponibles, excluirFecundacionId),
            cancellationToken);

        var response = output.Select(FecundacionMapper.ToResponse).ToList();
        return Ok(GeneralResponseDTO<IReadOnlyList<FecundacionVacunoOptionResponse>>.Ok(response));
    }

    [HttpPatch("{fecundacionId:long}")]
    [ServiceFilter(typeof(TenantHeaderFilter))]
    [RestrictTenantType(TenantType.Tenant)]
    [Authorize(Roles = AuthorizationRoles.Regular)]
    [ProducesResponseType(typeof(GeneralResponseDTO<FecundacionUpdateResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(
        long fecundacionId,
        [FromBody] UpdateFecundacionRequest request,
        CancellationToken cancellationToken)
    {
        var current = await _behaviorDispatcher.Send<GetFecundacionForEditQuery , GetFecundacionForEditOutput>(
            new GetFecundacionForEditQuery(fecundacionId), 
            cancellationToken
        );
        var output = await _behaviorDispatcher.Send<UpdateFecundacionCommand , UpdateFecundacionOutput>(
            FecundacionMapper.ToCommand(request, current, fecundacionId),
            cancellationToken);

        return Ok(GeneralResponseDTO<FecundacionUpdateResponse>.Ok(FecundacionMapper.ToResponse(output)));
    }

    [HttpDelete("{id:long}")]
    [ServiceFilter(typeof(TenantHeaderFilter))]
    [RestrictTenantType(TenantType.Tenant)]
    [Authorize(Roles = AuthorizationRoles.Regular)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Eliminar(
        long id,
        [FromBody] DeleteFecundacionRequest request,
        CancellationToken cancellationToken)
    {
        var command = new DeleteFecundacionCommand(id, request.Razon);
        await _behaviorDispatcher.Send<DeleteFecundacionCommand , EmptyOutput>(command, cancellationToken);
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
}
