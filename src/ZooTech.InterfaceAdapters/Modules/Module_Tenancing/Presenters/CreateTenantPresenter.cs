using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateTenant;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateTenant.Ports;
using ZooTech.InterfaceAdapters.Modules.Module_Tenancing.DTOs.Responses;
using ZooTech.InterfaceAdapters.Modules.Module_Tenancing.Mappers;

namespace ZooTech.InterfaceAdapters.Modules.Module_Tenancing.Presenters
{
    public class CreateTenantPresenter : ICreateTenantOutputPort
    {
        public CreateTenantResponseDto? Response { get; private set; }
        public Task Ok(CreateTenantOutput output)
        {
            Response = CreateTenantMapper.ToResponseDto(output);

            return Task.CompletedTask;
        }
    }
}