namespace ZooTech.Application.Common.Gateway.Auditing
{
    public interface IAppAuditService
    {
        Task SavingChangesAsync(AuditModel auditModel);
    }
}