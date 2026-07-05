using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Common.Behaviors;
using ZooTech.Application.Common.Behaviors.Module_Auth.AdminLogin;
using ZooTech.Application.Common.Behaviors.Module_Auth.RegularLogin;
using ZooTech.Domain.Shared.Enums;
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
        private readonly IAdminLoginBehaviorPipelineFactory _adminLoginBehaviorPipelineFactory;
        private readonly IRegularLoginBehaviorPipelineFactory _regularLoginBehaviorPipelineFactory;

        public AuthController(
            IAdminLoginBehaviorPipelineFactory adminLoginBehaviorPipelineFactory,
            IRegularLoginBehaviorPipelineFactory regularLoginBehaviorPipelineFactory
        )
        {
            _adminLoginBehaviorPipelineFactory = adminLoginBehaviorPipelineFactory;
            _regularLoginBehaviorPipelineFactory = regularLoginBehaviorPipelineFactory;
        }

        [AllowAnonymous]
        [ServiceFilter(typeof(AnonymousOnlyFilter))]
        [ServiceFilter(typeof(TenantHeaderFilter))]
        [RestrictTenantType(Domain.Shared.Enums.TenantType.Tenant)]
        [HttpPost("user/login")]
        public async Task<IActionResult> LoginRegularUsers(
            [FromBody] RegularLoginRequestDTO request
        )
        {
            var behaviorPipeline = _regularLoginBehaviorPipelineFactory.Create();

            var result = await behaviorPipeline.Execute(
                RegularLoginMapper.ToCommand(request)
            );

            return Ok(result);
        }

        [AllowAnonymous]
        [ServiceFilter(typeof(AnonymousOnlyFilter))]
        [ServiceFilter(typeof(TenantHeaderFilter))]
        [RestrictTenantType(Domain.Shared.Enums.TenantType.Admin)]
        [HttpPost("admin/login")]
        public async Task<IActionResult> LoginAdminUsers(
            [FromBody] AdminLoginRequestDTO requestDto
        )
        {
            var behaviorPipeline = _adminLoginBehaviorPipelineFactory.Create();

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