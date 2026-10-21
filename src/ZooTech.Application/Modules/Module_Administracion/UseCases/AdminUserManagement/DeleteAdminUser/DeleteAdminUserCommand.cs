using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Administracion.UseCases.AdminUserManagement.DeleteAdminUser
{
    public class DeleteAdminUserCommand : IAuditableCommandQueryRequest
    {
        public AuditEventType EventType => AuditEventType.Delete;
        public string Action => "Deleting a admin user";

        public int? Id { get; init; }
    }
}