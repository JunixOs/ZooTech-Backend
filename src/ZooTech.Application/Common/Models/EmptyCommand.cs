using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Common.Models
{
    public sealed class EmptyCommand : IAuditableRequest
    {
        public AuditEventType EventType { get; init; }
        public string Action { get; init; }

        public static EmptyCommand Value(AuditEventType auditEventType , string action)
        {
            return new EmptyCommand
            {
                EventType = auditEventType,
                Action = action,            
            };
        }

        private EmptyCommand()
        {
        }
    }
}