namespace ZooTech.InterfaceAdapters.Modules.Module_Tenancing.DTOs.Requests
{
    public class CreateTenantRequestDto
    {
        public string Code { get; set; } = default!;

        public string SubDomain { get; set; }
            = default!;

        public string DisplayName { get; set; }
            = default!;

        public string LegalName { get; set; }
            = default!;

        public string Email { get; set; }
            = default!;

        public string Phone { get; set; }
            = default!;

        public TenantAddressRequestDto
            TenantAddress { get; set; }
            = default!;

        public TenantBrandingRequestDto
            TenantBranding { get; set; }
            = default!;

        public TenantDatabaseConnectionRequestDto
            TenantDatabaseConnection { get; set; }
            = default!;

        public string TimeZone { get; set; }
            = default!;
    }

    public class TenantAddressRequestDto
    {
        public string Country { get; set; }
            = default!;

        public string State { get; set; }
            = default!;

        public string Province { get; set; }
            = default!;

        public string City { get; set; }
            = default!;

        public string AddressLine1 { get; set; }
            = default!;

        public string? AddressLine2 { get; set; }
    }

    public class TenantBrandingRequestDto
    {
        public string PrimaryColor { get; set; }
            = default!;

        public string SecondaryColor { get; set; }
            = default!;

        public string LogoUrl { get; set; }
            = default!;
    }

    public class TenantDatabaseConnectionRequestDto
    {
        public bool IsActive { get; set; }
    }
}