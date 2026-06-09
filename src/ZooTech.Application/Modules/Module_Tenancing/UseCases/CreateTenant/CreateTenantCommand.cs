using MediatR;
using ZooTech.Domain.Enums;

namespace ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateTenant
{
    public class CreateTenantCommand : IRequest<CreateTenantResult>
    {
        public string Code { get; init; } = default!;
        public string SubDomain { get; init; } = default!;
        public string DisplayName { get; init; } = default!;
        public string LegalName { get; init; } = default!;
        public string Email { get; init; } = default!;
        public string Phone { get; init; } = default!;
        public TenantAddress TenantAddress { get; init; } = default!;
        public TenantBranding TenantBranding { get; init; } = default!;
        public TenantDatabaseConnection TenantDatabaseConnection { get; init; } = default!;
        public string TimeZone { get; init; } = default!;
        public TenantStatus Status { get; init; } = TenantStatus.TRIAL;
        public string Metadata { get; init; } = default!;
    }

    public class TenantAddress
    {
        public string Country { get; init; } = default!;
        public string State { get; init; } = default!;
        public string Province { get; init; } = default!;
        public string City { get; init; } = default!;
        public string AddressLine_1 { get; init; } = default!;
        public string AddressLine_2 { get; init; } = default!;
        public string Metadata { get; init; } = default!;
    }

    public class TenantBranding
    {
        public string PrimaryColor { get; init; } = default!;
        public string SecondaryColor { get; init; } = default!;
        public string LogoUrl { get; init; } = default!;
        public string Metadata { get; init; } = default!;
    }

    public class TenantDatabaseConnection
    {
        public bool IsActive { get; init; } = true;
    }
}
