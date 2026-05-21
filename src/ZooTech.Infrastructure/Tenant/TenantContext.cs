using ZooTech.Application.Common.Gateway.Context;

namespace ZooTech.Infrastructure.Tenant
{
    public class TenantContext : ITenantContext
    {
        public long TenantId {  get; private set; }
        public string Code { get; private set; } = "";
        public string SubDomain { get; private set; } = "";
        public string DatabaseName { get; private set; } = "";

        public void SetTenant(long id, string code , string subDomain , string databaseName)
        {
            TenantId = id;
            Code = code;
            SubDomain = subDomain;
            DatabaseName = databaseName;
        }
    }
}