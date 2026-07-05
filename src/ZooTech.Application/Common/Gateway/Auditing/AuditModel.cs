using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Common.Gateway.Auditing
{
    public class AuditModel
    {
        public AuditEventType EventType { get; set; } = default!;
        public string Action { get; set; } = default!;
        public object? RequestValues = default!;
        public object? ResponseValues = default!;
    }
}