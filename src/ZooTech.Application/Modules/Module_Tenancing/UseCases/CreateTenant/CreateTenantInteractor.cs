
using ZooTech.Application.Common.Gateway.Tenant;

namespace ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateTenant
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

        public async Task<CreateTenantOutput> Handle(CreateTenantCommand request)
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
