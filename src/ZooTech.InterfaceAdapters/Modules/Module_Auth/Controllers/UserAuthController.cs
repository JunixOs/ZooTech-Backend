using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public class UserAuthController : ControllerBase
    {
        private readonly IRegularLoginBehaviorPipelineFactory _regularLoginBehaviorPipelineFactory;

        public UserAuthController(
            IRegularLoginBehaviorPipelineFactory regularLoginBehaviorPipelineFactory
        )
        {
            _regularLoginBehaviorPipelineFactory = regularLoginBehaviorPipelineFactory;
        }

        [AllowAnonymous]
        [ServiceFilter(typeof(AnonymousOnlyFilter))]
        [ServiceFilter(typeof(TenantHeaderFilter))]
        [RestrictTenantType(TenantType.Tenant)]
        [HttpPost("user/login")]
        public async Task<IActionResult> LoginRegularUsers(
            [FromBody] RegularLoginRequestDTO request,
            CancellationToken cancellationToken = default
        )
        {
            var behaviorPipeline = _regularLoginBehaviorPipelineFactory.Create();

            var result = await behaviorPipeline.Execute(
                RegularLoginMapper.ToCommand(request),
                cancellationToken
            );

            return Ok(result);
        }
    }
}
