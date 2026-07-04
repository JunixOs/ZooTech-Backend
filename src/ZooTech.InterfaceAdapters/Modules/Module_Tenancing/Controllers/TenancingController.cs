using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Common.Behaviors.Module_Tenancing;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateTenant;
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
        private readonly ICreateTenantPipelineFactory _pipeline;

        public TenancingController(
            ICreateTenantPipelineFactory pipeline
        )
        {
            _pipeline = pipeline;
        }

        [ServiceFilter(typeof(TenantHeaderFilter))]
        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok();
        }

        [ServiceFilter(typeof(TenantHeaderFilter))]
        [Authorize(Roles = AuthorizationRoles.Admin)]
        [HttpPost("create")]
        public async Task<IActionResult> CreateTenant([FromBody] CreateTenantRequestDto requestDto)
        {
            var behaviorPipeline = _pipeline.Create();

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
