using MediatR;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateTenant;
using ZooTech.InterfaceAdapters.Middleware;
using ZooTech.InterfaceAdapters.Modules.Module_Tenancing.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Tenancing.Mappers;

namespace ZooTech.InterfaceAdapters.Modules.Module_Tenancing.Controllers
{
    [ApiController]
    [Route("tenancing")]
    [ApiExplorerSettings(GroupName = "admin")]
    public class TenancingController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TenancingController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [ServiceFilter(typeof(TenantHeaderFilter))]
        [HttpPost("create")]
        public async Task<IActionResult> CreateTenant([FromBody] CreateTenantRequestDto requestDto)
        {
            var command = CreateTenantMapper.ToCommand(requestDto);
            var result = await _mediator.Send(command);
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
