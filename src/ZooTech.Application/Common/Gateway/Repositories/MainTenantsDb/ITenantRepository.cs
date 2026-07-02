using ZooTech.Application.Modules.Module_Tenancing.ListTenants;
using ZooTech.Domain.Admin.Entities;

namespace ZooTech.Application.Common.Gateway.Repositories.MainTenantsDb;

public interface ITenantRepository
{
    Task<TenantDomainEntity?> GetByIdAsync(int id);
    Task<List<TenantDomainEntity>> ListAllAsync();

    Task<List<ListTenantsOutput?>> ListAllTenants();
}
