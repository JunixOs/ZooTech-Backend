using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateAdminUser
{
    public class CreateAdminUserCommand : IAuditableCommandQueryRequest
    {
        public AuditEventType EventType => AuditEventType.Create;
        public string Action => "Create admin user";

        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool IsActive { get; set; }
        public string? Metadata { get; set; }
    }
}