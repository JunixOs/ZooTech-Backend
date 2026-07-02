using MediatR;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Application.Common.Gateway.Tenant;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateTenant
{
    public class CreateTenantInteractor : ICreateTenantInputPort, IAuditableRequest
    {
        private readonly ITenantProvisioningService _provisioningService;

        public AuditEventType EventType => AuditEventType.Create;
        public string Action => "Create a new tenant";

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
