using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Common.Behaviors;
using ZooTech.Application.Modules.Module_Auth.UseCases.RegularLogin;
using ZooTech.Domain.Shared.Enums;
using ZooTech.InterfaceAdapters.Filters;
using ZooTech.InterfaceAdapters.Modules.Module_Auth.DTOs;
using ZooTech.InterfaceAdapters.Modules.Module_Auth.Mappers;

namespace ZooTech.InterfaceAdapters.Modules.Module_Auth
{
    [ApiController]
    [Route("api/v1/auth/user")]
    [ApiExplorerSettings(GroupName = "auth")]
    public class UserAuthController : ControllerBase
    {
        private readonly IBehaviorDispatcher _behaviorDispatcher;

        public UserAuthController(
            IBehaviorDispatcher behaviorDispatcher
        )
        {
            _behaviorDispatcher = behaviorDispatcher;
        }

        [AllowAnonymous]
        [ServiceFilter(typeof(AnonymousOnlyFilter))]
        [ServiceFilter(typeof(TenantHeaderFilter))]
        [RestrictTenantType(TenantType.Tenant)]
        [HttpPost("login")]
        public async Task<IActionResult> LoginRegularUsers(
            [FromBody] RegularLoginRequestDTO request,
            CancellationToken cancellationToken = default
        )
        {
            var result = await _behaviorDispatcher.Send<RegularLoginCommand , string>(
                RegularLoginMapper.ToCommand(request),
                cancellationToken
            );

            return Ok(result);
        }
    }
}
