using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Common.Behaviors.Module_Tenancing.CreateTenant;
using ZooTech.Application.Modules.Module_Tenancing.UseCases;
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
        private readonly ICreateTenantBehaviorPipelineFactory _createTenantBehaviorPipelineFactory;

        public TenancingController(
            ICreateTenantBehaviorPipelineFactory createTenantBehaviorPipelineFactory
        )
        {
            _createTenantBehaviorPipelineFactory = createTenantBehaviorPipelineFactory;
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
        public async Task<IActionResult> CreateTenant([FromBody] CreateTenantRequestDto requestDto)
        {
            var behaviorPipeline = _createTenantBehaviorPipelineFactory.Create();

            var command = CreateTenantMapper.ToCommand(requestDto);
            var result = await behaviorPipeline.Execute(command);

            return Ok(CreateTenantMapper.ToResponseDto(result));
        }

        [ServiceFilter(typeof(TenantHeaderFilter))]
        [HttpGet("info")]
        public async Task GetTenantInfo()
        {
            return;
        }
    }
}
