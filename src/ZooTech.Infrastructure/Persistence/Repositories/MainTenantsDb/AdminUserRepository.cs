using Microsoft.EntityFrameworkCore;
using ZooTech.Application.Common.Gateway.Repositories.MainTenantsDb;
using ZooTech.Domain.Admin.Entities;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Mappers.MainTenantsDb;

namespace ZooTech.Infrastructure.Persistence.Repositories.MainTenantsDb
{
    public class AdminUserRepository : IAdminUserRepository
    {
        private readonly IDbContextFactory _dbContextFactory;

        public AdminUserRepository(IDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task Create(AdminUserDomainEntity adminUserDomainEntity)
        {
            var tenantDbContext = await _dbContextFactory.GetTenantDbContext();

            await tenantDbContext.admin_users.AddAsync(
                AdminUserMapper.ToOrm(adminUserDomainEntity)
            );

            await tenantDbContext.SaveChangesAsync();
        }

        public async Task<AdminUserDomainEntity?> GetById(int id)
        {
            var tenantDbContext = await _dbContextFactory.GetTenantDbContext();

            var adminUserOrm = await tenantDbContext.admin_users.FindAsync(id);
            
            if(adminUserOrm is null)
            {
                return null;
            }

            return AdminUserMapper.ToDomain(adminUserOrm);
        }

        public async Task<string?> GetPasswordHashByEmail(string email)
        {
            var tenantDbContext = await _dbContextFactory.GetTenantDbContext();

            return await tenantDbContext.admin_users
                .Where(au => au.email == email)
                .Select(au => au.password_hash)
                .FirstOrDefaultAsync();
        }

        public async Task Update(AdminUserDomainEntity adminUserDomainEntity)
        {
            var tenantDbContext = await _dbContextFactory.GetTenantDbContext();

            var adminUserOrm = AdminUserMapper.ToOrm(adminUserDomainEntity);
            tenantDbContext.admin_users.Update(adminUserOrm);

            await tenantDbContext.SaveChangesAsync();
        }
    }
}