using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Common.Behaviors.Module_Tenancing;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateTenant;
using ZooTech.InterfaceAdapters.Filters;
using ZooTech.InterfaceAdapters.Modules.Module_Tenancing.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Tenancing.Mappers;

namespace ZooTech.InterfaceAdapters.Modules.Module_Tenancing.Controllers
{
    [ApiController]
    [Route("tenancing")]
    [ApiExplorerSettings(GroupName = "admin")]
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
        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateTenant([FromBody] CreateTenantRequestDto requestDto)
        {
            var behaviorPipeline = _pipeline.Create();

            var command = CreateTenantMapper.ToCommand(requestDto);
            var result = await behaviorPipeline.Execute(command);

            return Ok(CreateTenantMapper.ToResponseDto(result));
        }

        [ServiceFilter(typeof(TenantHeaderFilter))]
        [Authorize(Roles = "Admin")]
        [HttpGet("info")]
        public async Task GetTenantInfo()
        {
            return;
        }
    }
}
