using ZooTech.Domain.Admin.Entities;

namespace ZooTech.Application.Common.Gateway.Repositories.MainTenantsDb
{
    public interface IAdminUserRepository
    {  
        Task Create(AdminUserDomainEntity adminUserDomainEntity);
        Task Update(AdminUserDomainEntity adminUserDomainEntity);
        Task<AdminUserDomainEntity> GetById(string id);

        Task<string> GetPasswordHashByEmail(string email);
    }
}