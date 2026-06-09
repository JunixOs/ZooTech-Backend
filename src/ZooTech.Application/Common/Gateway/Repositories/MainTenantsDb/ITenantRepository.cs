using ZooTech.Domain.Entities;

namespace ZooTech.Application.Common.Gateway.Repositories.MainTenantsDb;

public interface ITenantRepository
{
    Task<TenantDomainEntity?> GetByIdAsync(int id);
    Task<List<TenantDomainEntity>> ListAllAsync();
}
