using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_GestionUsuarios.UseCases.AdminUser.Login
{
    public class LoginCommand : IAuditableCommandQueryRequest
    {
        public AuditEventType EventType => AuditEventType.Login;
        public string Action => "A user is login into the application.";

        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}