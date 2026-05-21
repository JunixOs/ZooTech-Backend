using MediatR;

namespace ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateTenant
{
    public class CreateTenantCommand : IRequest<Unit>
    {
        public string Code { get; set; } = default!;
        public string SubDomain { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
        public string LegalName { get; set; } = default!;

        public string Email { get; set; } = default!;
        public string Phone { get; set; } = default!;

        public TenantAddress TenantAddress { get; set; } = default!;
        public TenantBranding TenantBranding  { get; set; } = default!;
        public TenantDatabaseConnection TenantDatabaseConnection { get; set; } = default!;
        public string TimeZone { get; set; } = default!;
        public string Status { get; set; } = "TRIAL";
        public string Metadata { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public class TenantAddress
    {
        public string Country { get; set; } = default!;
        public string State { get; set; } = default!;
        public string Province { get; set; } = default!;
        public string City { get; set; } = default!;
        public string AddressLine_1 { get; set; } = default!;
        public string AddressLine_2 { get; set; } = default!;
        public string Metadata { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public class TenantBranding
    {
        public string PrimaryColor {  get; set; } = default!;
        public string SecondaryColor { get; set; } = default!;
        public string LogoUrl { get; set; } = default!;
        public string Metadata { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public class TenantDatabaseConnection
    {
        public bool IsActive { get; set; } = true;
    }
}
