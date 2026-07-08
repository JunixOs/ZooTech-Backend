using ZooTech.Application.Modules.Module_Tenancing.UseCases.ListAdminUsers;
using ZooTech.Domain.Admin.Entities;

namespace ZooTech.Application.Common.Gateway.Repositories.MainTenantsDb
{
    public interface IAdminUserRepository
    {  
        Task Create(AdminUserDomainEntity adminUserDomainEntity);
        Task Update(AdminUserDomainEntity adminUserDomainEntity);
        Task<AdminUserDomainEntity?> GetById(int id);
        Task DeleteById(int id);
        
        Task<List<ListAdminUsersOutput>> FindAll();

        Task<string?> GetPasswordHashByEmail(string email);
        Task<AdminUserDomainEntity?> GetByEmail(string email);
    }
}