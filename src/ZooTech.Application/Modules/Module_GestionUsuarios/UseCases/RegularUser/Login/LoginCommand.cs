using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_GestionUsuarios.UseCases.RegularUser.Login
{
    public class LoginCommand : IAuditableCommandQueryRequest
    {
        public AuditEventType EventType => AuditEventType.Login;
        public string Action => "A user is login into the application.";

        public string? Email { get; set; }
    }
}