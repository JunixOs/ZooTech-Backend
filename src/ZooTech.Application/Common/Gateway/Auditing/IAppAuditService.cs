namespace ZooTech.Application.Common.Gateway.Auditing
{
    public interface IAppAuditService
    {
        Task AuditEventAsync(AuditEventInfo auditEventInfo);
        Task AuditErrorAsync(AuditErrorInfo auditErrorInfo);
    }
}