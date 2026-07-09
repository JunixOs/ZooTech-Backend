using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Common.Gateway.Context
{
    public interface ITenantContext
    {
        int TenantId { get; }
        string Code { get; }
        string SubDomain { get; }
        string LegalName { get; }
        string DisplayName { get; }
        TenantType Type { get; }
        string DatabaseName { get; }

        void SetTenant(int id, string code, string legalName, string displayName, TenantType type, string subDomain, string databaseName);
    }
}
