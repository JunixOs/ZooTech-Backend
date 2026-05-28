using MediatR;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateTenant;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateTenant.Ports;
using ZooTech.InterfaceAdapters.Modules.Module_Tenancing.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Tenancing.Mappers;
using ZooTech.InterfaceAdapters.Modules.Module_Tenancing.Presenters;

namespace ZooTech.InterfaceAdapters.Modules.Module_Tenancing.Controllers
{
    [ApiController]
    [Route("tenancing")]
    public class TenancingController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICreateTenantInputPort _createTenantInputPort;
        private readonly CreateTenantPresenter _tenancingPresenter;

        public TenancingController(
            IMediator mediator,
            ICreateTenantInputPort createTenantInputPort,
            CreateTenantPresenter tenancingPresenter
        )
        {
            _mediator = mediator;
            _createTenantInputPort = createTenantInputPort;
            _tenancingPresenter = tenancingPresenter;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTenant(
            [FromBody] CreateTenantRequestDto requestDto
        )
        {   
            var command = CreateTenantMapper.ToCommand(requestDto);

            await _mediator.Send(command);

            return Ok(_tenancingPresenter.Response);
        }
    }
}