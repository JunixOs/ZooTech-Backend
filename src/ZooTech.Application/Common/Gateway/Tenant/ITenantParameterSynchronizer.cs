namespace ZooTech.Application.Common.Gateway.Tenant
{
    public interface ITenantParameterSynchronizer
    {
        Task SynchronizeAsync(int tenantId);
    }
}