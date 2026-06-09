namespace ZooTech.Application.Common.Gateway.Tenant
{
    public interface ITenantStore
    {
        Task<TenantInfo?> GetBySubDomainAsync(string subDomain);
    }
}