using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Common.Gateway.Auditing
{
    public class AuditModel
    {
        public AuditEventType EventType { get; set; } = default!;
        public string Action { get; set; } = default!;
        public Dictionary<string, object>? OldValues = default!;
        public Dictionary<string, object>? NewValues = default!;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}