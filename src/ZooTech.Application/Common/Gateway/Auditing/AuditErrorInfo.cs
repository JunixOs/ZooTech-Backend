using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Common.Gateway.Auditing
{
    public class AuditErrorInfo
    {
        public AuditEventType EventType { get; set; }
        public string? CustomMessage { get; set; }
        
        public string? Type { get; set; }
        public string? Message { get; set; }
        public string? Source { get; set; }
        public string? Method { get; set; }
        public string? Inner { get; set; }

        public string? StackTrace { get; set; }

        public ErrorAppInformation? AppInformation { get; set; }
    }

    public class ErrorAppInformation
    {
        public string? ErrorCode { get; set; }
        public ScopeName? ScopeName { get; set; }
        public ModuleName? ModuleName { get; set; }
        public List<string>? Details { get; set; }
        public string? CompleteErrorCode { get; set; }
    }
}