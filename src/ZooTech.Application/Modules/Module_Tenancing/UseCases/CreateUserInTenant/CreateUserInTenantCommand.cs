using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateUserInTenant
{
    public class CreateUserInTenantCommand : IAuditableRequest
    {
        public AuditEventType EventType => AuditEventType.Create;
        public string Action => "Creating a tenant user";

        public string? Code { get; set; }
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public bool IsActive { get; set; }

        public string? TenantDatabaseName { get; set; }
    }
}