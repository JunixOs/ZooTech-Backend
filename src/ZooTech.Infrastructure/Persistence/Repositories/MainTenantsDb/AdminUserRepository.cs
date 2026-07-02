using ZooTech.Application.Common.Gateway.Repositories.MainTenantsDb;
using ZooTech.Domain.Admin.Entities;
using ZooTech.Infrastructure.Persistence.Context;

namespace ZooTech.Infrastructure.Persistence.Repositories.MainTenantsDb
{
    public class AdminUserRepository : IAdminUserRepository
    {
        private readonly IGanaderiaDbContextFactory _factory;

        public AdminUserRepository(IGanaderiaDbContextFactory factory)
        {
            _factory = factory;
        }

        public Task Create(AdminUserDomainEntity adminUserDomainEntity)
        {
            
            throw new NotImplementedException();
        }

        public Task<AdminUserDomainEntity> GetById(string id)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetPasswordHashByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public Task Update(AdminUserDomainEntity adminUserDomainEntity)
        {
            throw new NotImplementedException();
        }
    }
}