using ZooTech.Application.Common.Gateway.Tenant;
using ZooTech.Application.Modules.Module_Administracion.UseCases.TenantManagement.CreateTenant.Ports;

namespace ZooTech.Application.Modules.Module_Administracion.UseCases.TenantManagement.CreateTenant
{
    public class CreateTenantInteractor : ICreateTenantInputPort
    {
        private readonly ITenantProvisioningService _provisioningService;

        public CreateTenantInteractor(
            ITenantProvisioningService provisioningService
        )
        {
            _provisioningService = provisioningService;
        }

        public async Task<CreateTenantOutput> HandleAsync(CreateTenantCommand request, CancellationToken cancellationToken = default)
        {
            await _provisioningService.ProvisionAsync(request);

            return new CreateTenantOutput
            {
                Code = request.Code,
                SubDomain = request.SubDomain,
                DisplayName = request.DisplayName,
                LegalName = request.LegalName
            };
        }
    }


}
