namespace ZooTech.Application.Common.Gateway.Tenant
{
    public class TenantInfo
    {
        public long Id { get; set; }
        public string SubDomain { get; set; } = default!;
        public string LegalName { get; set; } = default!;
        public string Code { get; set; } = default!;
        public string DatabaseName { get; set; } = default!;
        public string Status { get; set; } = "TRIAL";
        public string Email { get; set; } = default!;
    }
}