namespace ZooTech.Application.Common.Gateway.Auditing
{
    public class AuditModel
    {
        public string EventType { get; set; } = default!;
        public string Action { get; set; } = default!;
        public long UserId { get; set; }
        public string UserName { get; set; } = default!;
        public Dictionary<string, object>? OldValues = default!;
        public Dictionary<string, object>? NewValues = default!;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}