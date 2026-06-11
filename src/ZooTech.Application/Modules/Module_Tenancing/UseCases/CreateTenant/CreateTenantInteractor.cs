using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Gateway.Context;
using ZooTech.Application.Common.Gateway.Tenant;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateTenant.Ports;

namespace ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateTenant
{
    public class CreateTenantInteractor : ICreateTenantInputPort
    {
        private readonly ITenantContext _tenantContext;
        private readonly ITenantProvisioningService _tenantProvisioningService;
        private readonly ICreateTenantOutputPort _createTenantOutputPort;

        public CreateTenantInteractor(
            ITenantContext tenantContext,
            ITenantProvisioningService tenantProvisioningService,
            ICreateTenantOutputPort createTenantOutputPort
        )
        {
            _tenantContext = tenantContext;
            _tenantProvisioningService = tenantProvisioningService;
            _createTenantOutputPort = createTenantOutputPort;
        }

        public async Task Handle(CreateTenantCommand cmd)
        {
            bool success = await _tenantProvisioningService.ProvisionAsync(cmd);

            if (!success)
            {
                throw new TenantProvisioningException();
            }

            await _createTenantOutputPort.Ok(new CreateTenantOutput
            {
                Code = cmd.Code,
                SubDomain = cmd.SubDomain,
                DisplayName = cmd.DisplayName,
                LegalName = cmd.LegalName
            });
        }
    }
}