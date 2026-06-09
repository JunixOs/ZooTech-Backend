namespace ZooTech.Application.Common.Gateway.Context
{
    public interface ITenantContext
    {
        int TenantId { get; }
        string Code { get; }
        string SubDomain { get; }
        string DatabaseName { get; }

        void SetTenant(int id, string code, string subDomain, string databaseName);
    }
}
