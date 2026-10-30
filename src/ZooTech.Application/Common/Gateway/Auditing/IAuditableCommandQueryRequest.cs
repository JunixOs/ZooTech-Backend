using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Common.Gateway.Auditing
{
    public interface IAuditableCommandQueryRequest
    {
        AuditEventType EventType { get; }
        string Action { get; }
    }
}