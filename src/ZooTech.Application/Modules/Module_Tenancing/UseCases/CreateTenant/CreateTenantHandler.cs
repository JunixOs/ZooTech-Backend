using MediatR;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Gateway.Tenant;

namespace ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateTenant
{
    public class CreateTenantHandler
        : IRequestHandler<CreateTenantCommand, CreateTenantResult>
    {
        private readonly ITenantProvisioningService _provisioningService;

        public CreateTenantHandler(ITenantProvisioningService provisioningService)
        {
            _provisioningService = provisioningService;
        }

        public async Task<CreateTenantResult> Handle(
            CreateTenantCommand request,
            CancellationToken cancellationToken)
        {
            var success = await _provisioningService.ProvisionAsync(request);

            if (!success)
            {
                throw new TenantProvisioningException();
            }

            return new CreateTenantResult
            {
                Code = request.Code,
                SubDomain = request.SubDomain,
                DisplayName = request.DisplayName,
                LegalName = request.LegalName
            };
        }
    }

    public class CreateTenantResult
    {
        public string Code { get; init; } = default!;
        public string SubDomain { get; init; } = default!;
        public string DisplayName { get; init; } = default!;
        public string LegalName { get; init; } = default!;
    }
}
