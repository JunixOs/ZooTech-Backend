using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Common.Behaviors.Module_Auth.AdminLogin;
using ZooTech.Application.Common.Behaviors.Module_Auth.RegularLogin;
using ZooTech.InterfaceAdapters.Filters;
using ZooTech.InterfaceAdapters.Modules.Module_Auth.DTOs;
using ZooTech.InterfaceAdapters.Modules.Module_Auth.Mappers;

namespace ZooTech.InterfaceAdapters.Modules.Module_Auth
{
    [ApiController]
        [Route("api/v1/auth")]
    [ApiExplorerSettings(GroupName = "auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAdminLoginBehaviorPipeline _adminLoginBehaviorPipeline;
        private readonly IRegularLoginBehaviorPipeline _regularLoginBehaviorPipeline;

        public AuthController(
            IAdminLoginBehaviorPipeline adminLoginBehaviorPipeline,
            IRegularLoginBehaviorPipeline regularLoginBehaviorPipeline
        )
        {
            _adminLoginBehaviorPipeline = adminLoginBehaviorPipeline;
            _regularLoginBehaviorPipeline = regularLoginBehaviorPipeline;
        }

        [ServiceFilter(typeof(TenantHeaderFilter))]
        [RestrictTenantType(Domain.Shared.Enums.TenantType.Tenant)]
        [HttpPost("user/login")]
        public async Task<IActionResult> LoginRegularUsers(
            [FromBody] RegularLoginRequest request
        )
        {
            var behaviorPipeline = _regularLoginBehaviorPipeline.Create();

            var result = await behaviorPipeline.Execute(
                RegularLoginMapper.ToCommand(request)
            );

            return Ok(result);
        }

        [ServiceFilter(typeof(TenantHeaderFilter))]
        [RestrictTenantType(Domain.Shared.Enums.TenantType.Admin)]
        [HttpPost("admin/login")]
        public async Task<IActionResult> LoginAdminUsers(
            [FromBody] AdminLoginRequest requestDto
        )
        {
            var behaviorPipeline = _adminLoginBehaviorPipeline.Create();

            var result = await behaviorPipeline.Execute(
                AdminLoginMapper.ToCommand(requestDto)
            );

            return Ok(result);
        }

        [ServiceFilter(typeof(TenantHeaderFilter))]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            return Ok();
        }
    }
}