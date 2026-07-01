using ZooTech.Application.Common.Gateway.Context;

namespace ZooTech.Infrastructure.Context
{
    public class TenantContext : ITenantContext
    {
        public int TenantId { get; private set; }
        public string Code { get; private set; } = "";
        public string LegalName { get; private set; } = "";
        public string DisplayName { get; private set; } = ""; 
        public string Type { get; private set; } = "";
        public string SubDomain { get; private set; } = "";
        public string DatabaseName { get; private set; } = "";

        public void SetTenant(
            int id, 
            string code, 
            string legalName,
            string displayName, 
            string type, 
            string subDomain, 
            string databaseName
        )
        {
            TenantId = id;
            Code = code;
            LegalName = legalName;
            DisplayName = displayName;
            Type = type;
            SubDomain = subDomain;
            DatabaseName = databaseName;
        }
    }
}
