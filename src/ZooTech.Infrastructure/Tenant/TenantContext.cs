using ZooTech.Application.Common.Gateway.Context;

namespace ZooTech.Infrastructure.Tenant
{
    public class TenantContext : ITenantContext
    {
        public int TenantId { get; private set; }
        public string Code { get; private set; } = "";
        public string LegalName { get; private set; } = "";
        public string Type { get; private set; } = "";
        public string SubDomain { get; private set; } = "";
        public string DatabaseName { get; private set; } = "";

        public void SetTenant(int id, string code, string legalName, string type, string subDomain, string databaseName)
        {
            TenantId = id;
            Code = code;
            LegalName = legalName;
            Type = type;
            SubDomain = subDomain;
            DatabaseName = databaseName;
        }
    }
}
