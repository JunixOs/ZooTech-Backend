using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Tenancing.UseCases.DeleteAdminUser
{
    public class DeleteAdminUserCommand : IAuditableRequest
    {
        public AuditEventType EventType => AuditEventType.Delete;
        public string Action => "Deleting a admin user";

        public int? Id { get; init; }
    }
}