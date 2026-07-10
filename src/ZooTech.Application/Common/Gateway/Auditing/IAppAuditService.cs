namespace ZooTech.Application.Common.Gateway.Auditing
{
    public interface IAppAuditService
    {
        Task SaveLogAsync(AuditModel auditModel);
    }
}