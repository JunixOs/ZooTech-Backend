using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Common.Behaviors.Module_Auth.AdminLogin;
using ZooTech.Application.Common.Behaviors.Module_Auth.RegularLogin;
using ZooTech.Application.Modules.Module_Auth.UseCases.AdminLogin;
using ZooTech.InterfaceAdapters.Filters;

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
        public async Task<IActionResult> LoginRegularUsers(string email)
        {
            var behaviorPipeline = _regularLoginBehaviorPipeline.Create();

            var result = await behaviorPipeline.Execute(email);

            return Ok(result);
        }

        [ServiceFilter(typeof(TenantHeaderFilter))]
        [RestrictTenantType(Domain.Shared.Enums.TenantType.Admin)]
        [HttpPost("admin/login")]
        public async Task<IActionResult> LoginAdminUsers(string email, string password)
        {
            var behaviorPipeline = _adminLoginBehaviorPipeline.Create();

            var result = await behaviorPipeline.Execute(
                new AdminLoginCommand
                {
                    Email = email,
                    Password = password
                }
            );

            return Ok(result);
        }
    }
}