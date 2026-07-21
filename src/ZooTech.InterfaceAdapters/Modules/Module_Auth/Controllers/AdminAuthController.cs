using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Common.Behaviors;
using ZooTech.Application.Modules.Module_Auth.UseCases.AdminLogin;
using ZooTech.Domain.Shared.Enums;
using ZooTech.InterfaceAdapters.Filters;
using ZooTech.InterfaceAdapters.Modules.Module_Auth.DTOs;
using ZooTech.InterfaceAdapters.Modules.Module_Auth.Mappers;

namespace ZooTech.InterfaceAdapters.Modules.Module_Auth
{
    [ApiController]
    [Route("api/v1/auth/admin")]
    [ApiExplorerSettings(GroupName = "auth")]
    public class AdminAuthController : ControllerBase
    {
        private readonly IBehaviorDispatcher _behaviorDispatcher;

        public AdminAuthController(
            IBehaviorDispatcher behaviorDispatcher
        )
        {
            _behaviorDispatcher = behaviorDispatcher;
        }

        [AllowAnonymous]
        [ServiceFilter(typeof(AnonymousOnlyFilter))]
        [ServiceFilter(typeof(TenantHeaderFilter))]
        [RestrictTenantType(TenantType.Admin)]
        [HttpPost("login")]
        public async Task<IActionResult> LoginAdminUsers(
            [FromBody] AdminLoginRequestDTO requestDto,
            CancellationToken cancellationToken = default
        )
        {
            var result = await _behaviorDispatcher.Send<AdminLoginCommand , string>(
                AdminLoginMapper.ToCommand(requestDto),
                cancellationToken
            );

            return Ok(result);
        }
    }
}
