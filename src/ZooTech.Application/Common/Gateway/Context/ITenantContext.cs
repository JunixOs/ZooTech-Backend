namespace ZooTech.Application.Common.Gateway.Context
{
    public interface ITenantContext
    {
        int TenantId { get; }
        string Code { get; }
        string SubDomain { get; }
        string LegalName { get; }
        string Type { get; }
        string DatabaseName { get; }

        void SetTenant(int id, string code, string legalName, string type, string subDomain, string databaseName);
    }
}
