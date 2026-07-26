using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Tenancing.UseCases.ListTenants
{
    public sealed record ListTenantsQuery() : IAuditableCommandQueryRequest
    {
        public AuditEventType EventType => AuditEventType.Read;
        public string Action => "List tenants";
    }
}