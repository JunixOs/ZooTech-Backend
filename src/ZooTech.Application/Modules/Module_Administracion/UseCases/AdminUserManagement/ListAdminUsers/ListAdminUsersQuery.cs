using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Administracion.UseCases.AdminUserManagement.ListAdminUsers
{
    public sealed record ListAdminUsersQuery() : IAuditableCommandQueryRequest
    {
        public AuditEventType EventType => AuditEventType.Read;

        public string Action => "List admin users";
    }
}