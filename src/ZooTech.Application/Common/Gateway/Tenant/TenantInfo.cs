using ZooTech.Domain.Admin.Enums;

namespace ZooTech.Application.Common.Gateway.Tenant
{
    public class TenantInfo
    {
        public int Id { get; set; }
        public string SubDomain { get; set; } = default!;
        public string LegalName { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
        public string Code { get; set; } = default!;
        public string DatabaseName { get; set; } = default!;
        public bool IsDatabaseActive { get; set; }
        public TenantStatus Status { get; set; } = TenantStatus.TRIAL;
        public string Email { get; set; } = default!;
    }
}
