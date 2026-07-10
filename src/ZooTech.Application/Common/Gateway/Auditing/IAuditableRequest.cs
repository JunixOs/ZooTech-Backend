using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Common.Gateway.Auditing
{
    public interface IAuditableRequest
    {
        AuditEventType EventType { get; }
        string Action { get; }
    }
}