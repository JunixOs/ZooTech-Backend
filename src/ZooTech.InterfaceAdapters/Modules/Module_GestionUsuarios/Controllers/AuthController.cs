using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Common.Behaviors;
using ZooTech.Application.Modules.Module_GestionUsuarios.UseCases.AdminUser.Login;
using ZooTech.Application.Modules.Module_GestionUsuarios.UseCases.RegularUser.Login;
using ZooTech.Domain.Shared.Enums;
using ZooTech.InterfaceAdapters.Filters;
using ZooTech.InterfaceAdapters.Modules.Module_GestionUsuarios.DTOs;
using ZooTech.InterfaceAdapters.Modules.Module_GestionUsuarios.Mappers;

namespace ZooTech.InterfaceAdapters.Modules.Module_GestionUsuarios
{
    [ApiController]
    [Route("api/v1/gestionusuarios/auth")]
    [ApiExplorerSettings(GroupName = "gestionusuarios")]
    public class AuthController : ControllerBase
    {
        private readonly IBehaviorDispatcher _behaviorDispatcher;

        public AuthController(
            IBehaviorDispatcher behaviorDispatcher
        )
        {
            _behaviorDispatcher = behaviorDispatcher;
        }

        [ServiceFilter(typeof(TenantHeaderFilter))]
        [HttpPost("logout")]
        [Tags("Autenticación")]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken = default)
        {
            return Ok();
        }

        [AllowAnonymous]
        [ServiceFilter(typeof(AnonymousOnlyFilter))]
        [ServiceFilter(typeof(TenantHeaderFilter))]
        [RestrictTenantType(TenantType.Admin)]
        [HttpPost("login/admin-user")]
        [Tags("Autenticación")]
        public async Task<IActionResult> LoginAdminUsers(
            [FromBody] AuthAdminUserLoginRequestDTO requestDto,
            CancellationToken cancellationToken = default
        )
        {
            var result = await _behaviorDispatcher.Send<Application.Modules.Module_GestionUsuarios.UseCases.AdminUser.Login.LoginCommand, string>(
                AuthAdminUserLoginMapper.ToCommand(requestDto),
                cancellationToken
            );

            return Ok(result);
        }

        [AllowAnonymous]
        [ServiceFilter(typeof(AnonymousOnlyFilter))]
        [ServiceFilter(typeof(TenantHeaderFilter))]
        [RestrictTenantType(TenantType.Tenant)]
        [HttpPost("login/reg-user")]
        [Tags("Autenticación")]
        public async Task<IActionResult> LoginRegularUsers(
            [FromBody] AuthRegularUserLoginRequestDTO request,
            CancellationToken cancellationToken = default
        )
        {
            var result = await _behaviorDispatcher.Send<Application.Modules.Module_GestionUsuarios.UseCases.RegularUser.Login.LoginCommand, string>(
                AuthRegularUserLoginMapper.ToCommand(request),
                cancellationToken
            );

            return Ok(result);
        }
    }
}