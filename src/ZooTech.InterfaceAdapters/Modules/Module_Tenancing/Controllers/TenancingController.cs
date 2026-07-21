using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Common.Behaviors;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateTenant;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateUserInTenant;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.ListTenants;
using ZooTech.Domain.Shared.Enums;
using ZooTech.InterfaceAdapters.Filters;
using ZooTech.InterfaceAdapters.Modules.Module_Tenancing.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Tenancing.Mappers;

namespace ZooTech.InterfaceAdapters.Modules.Module_Tenancing.Controllers
{
    [ApiController]
    [Route("api/v1/tenancing")]
    [ApiExplorerSettings(GroupName = "tenancing")]
    public class TenancingController : ControllerBase
    {
        private readonly IBehaviorDispatcher _behaviorDispatcher;
        public TenancingController(
            IBehaviorDispatcher behaviorDispatcher
        )
        {
            _behaviorDispatcher = behaviorDispatcher;
        }

        [ServiceFilter(typeof(TenantHeaderFilter))]
        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok();
        }

        [ServiceFilter(typeof(TenantHeaderFilter))]
        [RestrictTenantType(TenantType.Admin)]
        [Authorize(Roles = AuthorizationRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> CreateTenant(
            [FromBody] CreateTenantRequestDto requestDto,
            CancellationToken cancellationToken = default
        )
        {
            var command = CreateTenantMapper.ToCommand(requestDto);
            var result = await _behaviorDispatcher.Send<CreateTenantCommand , CreateTenantOutput>(
                command , cancellationToken
            );

            return Ok(CreateTenantMapper.ToResponseDto(result));
        }

        [ServiceFilter(typeof(TenantHeaderFilter))]
        [HttpGet("info")]
        public async Task GetTenantInfo()
        {
            return;
        }

        [ServiceFilter(typeof(TenantHeaderFilter))]
        [RestrictTenantType(TenantType.Admin)]
        [Authorize(Roles = AuthorizationRoles.Admin)]
        [HttpPost("create-tenant-user")]
        public async Task<IActionResult> CreateUserInTenant(
            [FromBody] CreateUserInTenantRequestDTO requestDto,
            CancellationToken cancellationToken = default
        )
        {
            var result = await _behaviorDispatcher.Send<CreateUserInTenantCommand , CreateUserInTenantOutput>(
                CreateUserInTenantMapper.ToCommand(requestDto),
                cancellationToken
            );

            return Ok(CreateUserInTenantMapper.ToResponse(result));
        }

        [ServiceFilter(typeof(TenantHeaderFilter))]
        [RestrictTenantType(TenantType.Admin)]
        [Authorize(Roles = AuthorizationRoles.Admin)]
        [HttpGet("list")]
        public async Task<IActionResult> ListTenants(
            CancellationToken cancellationToken = default
        )
        {
            var result = await _behaviorDispatcher.Send<ListTenantsQuery , List<ListTenantsOutput>>(
                new(),
                cancellationToken
            );

            return Ok(ListTenantsMapper.ToResponse(result));
        }
    }
}
