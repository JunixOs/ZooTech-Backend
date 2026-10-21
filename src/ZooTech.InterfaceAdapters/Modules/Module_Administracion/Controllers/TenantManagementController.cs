using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Common.Behaviors;
using ZooTech.Application.Modules.Module_Administracion.UseCases.TenantManagement.CreateTenant;
using ZooTech.Application.Modules.Module_Administracion.UseCases.TenantManagement.CreateUserInTenant;
using ZooTech.Application.Modules.Module_Administracion.UseCases.TenantManagement.ListTenants;
using ZooTech.Domain.Shared.Enums;
using ZooTech.InterfaceAdapters.Filters;
using ZooTech.InterfaceAdapters.Modules.Module_Administracion.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Administracion.Mappers;

namespace ZooTech.InterfaceAdapters.Modules.Module_Administracion.Controllers
{
    [ApiController]
    [Route("api/v1/administracion/tenant")]
    [ApiExplorerSettings(GroupName = "administracion")]
    public class TenantManagementController : ControllerBase
    {
        private readonly IBehaviorDispatcher _behaviorDispatcher;
        public TenantManagementController(
            IBehaviorDispatcher behaviorDispatcher
        )
        {
            _behaviorDispatcher = behaviorDispatcher;
        }

        [ServiceFilter(typeof(TenantHeaderFilter))]
        [HttpGet("health")]
        [Tags("Public")]
        public IActionResult Health()
        {
            return Ok();
        }

        [ServiceFilter(typeof(TenantHeaderFilter))]
        [RestrictTenantType(TenantType.Admin)]
        [Authorize(Roles = AuthorizationRoles.Admin)]
        [HttpPost]
        [Tags("Tenant Management")]
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
        [Tags("Tenant Management")]
        public async Task GetTenantInfo()
        {
            return;
        }

        [ServiceFilter(typeof(TenantHeaderFilter))]
        [RestrictTenantType(TenantType.Admin)]
        [Authorize(Roles = AuthorizationRoles.Admin)]
        [HttpPost("create-tenant-user")]
        [Tags("Tenant Management")]
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
        [Tags("Tenant Management")]
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
