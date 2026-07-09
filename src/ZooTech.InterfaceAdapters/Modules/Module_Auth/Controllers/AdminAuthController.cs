using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Common.Behaviors.Module_Auth.AdminLogin;
using ZooTech.Domain.Shared.Enums;
using ZooTech.InterfaceAdapters.Filters;
using ZooTech.InterfaceAdapters.Modules.Module_Auth.DTOs;
using ZooTech.InterfaceAdapters.Modules.Module_Auth.Mappers;

namespace ZooTech.InterfaceAdapters.Modules.Module_Auth
{
    [ApiController]
    [Route("api/v1/auth")]
    [ApiExplorerSettings(GroupName = "auth")]
    public class AdminAuthController : ControllerBase
    {
        private readonly IAdminLoginBehaviorPipelineFactory _adminLoginBehaviorPipelineFactory;

        public AdminAuthController(
            IAdminLoginBehaviorPipelineFactory adminLoginBehaviorPipelineFactory
        )
        {
            _adminLoginBehaviorPipelineFactory = adminLoginBehaviorPipelineFactory;
        }

        [AllowAnonymous]
        [ServiceFilter(typeof(AnonymousOnlyFilter))]
        [ServiceFilter(typeof(TenantHeaderFilter))]
        [RestrictTenantType(TenantType.Admin)]
        [HttpPost("admin/login")]
        public async Task<IActionResult> LoginAdminUsers(
            [FromBody] AdminLoginRequestDTO requestDto,
            CancellationToken cancellationToken = default
        )
        {
            var behaviorPipeline = _adminLoginBehaviorPipelineFactory.Create();

            var result = await behaviorPipeline.Execute(
                AdminLoginMapper.ToCommand(requestDto),
                cancellationToken
            );

            return Ok(result);
        }
    }
}
