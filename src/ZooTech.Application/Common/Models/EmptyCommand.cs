using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Common.Models
{
    public sealed class EmptyCommandQuery : IAuditableCommandQueryRequest
    {
        public AuditEventType EventType { get; init; }
        public string Action { get; init; }

        public static EmptyCommandQuery Value(AuditEventType auditEventType , string action)
        {
            return new EmptyCommandQuery
            {
                EventType = auditEventType,
                Action = action,            
            };
        }

        private EmptyCommandQuery()
        {
        }
    }
}