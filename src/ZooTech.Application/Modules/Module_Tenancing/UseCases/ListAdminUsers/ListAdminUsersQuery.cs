using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Tenancing
{
    public sealed record ListAdminUsersQuery() : IAuditableRequest
    {
        public AuditEventType EventType => AuditEventType.Read;

        public string Action => "List admin users";
    }
}