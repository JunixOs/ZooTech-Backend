namespace ZooTech.Application.Common.Gateway.Auditing
{
    public interface IAppAuditService
    {
        Task AuditEventAsync(AuditModel auditModel);
        Task AuditErrorAsync(AuditModel auditModel);
    }
}