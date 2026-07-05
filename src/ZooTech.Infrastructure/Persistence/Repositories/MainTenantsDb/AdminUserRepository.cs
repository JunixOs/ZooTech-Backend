using Microsoft.EntityFrameworkCore;
using ZooTech.Application.Common.Gateway.Repositories.MainTenantsDb;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.ListAdminUsers;
using ZooTech.Domain.Admin.Entities;
using ZooTech.Infrastructure.Persistence.Mappers.MainTenantsDb;
using ZooTech.Infrastructure.Tenant;

namespace ZooTech.Infrastructure.Persistence.Repositories.MainTenantsDb
{
    public class AdminUserRepository : IAdminUserRepository
    {
        private readonly ITenantDbContextFactory _tenantDbContextFactory;

        public AdminUserRepository(
            ITenantDbContextFactory tenantDbContextFactory
        )
        {
            _tenantDbContextFactory = tenantDbContextFactory;
        }

        public async Task Create(AdminUserDomainEntity adminUserDomainEntity)
        {
            var tenantDbContext = await _tenantDbContextFactory.CreateDbContextByTenantContext();

            await tenantDbContext.admin_users.AddAsync(
                AdminUserMapper.ToOrm(adminUserDomainEntity)
            );

            await tenantDbContext.SaveChangesAsync();
        }

        public async Task<AdminUserDomainEntity?> GetById(int id)
        {
            var tenantDbContext = await _tenantDbContextFactory.CreateDbContextByTenantContext();

            var adminUserOrm = await tenantDbContext.admin_users.FindAsync(id);
            
            if(adminUserOrm is null)
            {
                return null;
            }

            return AdminUserMapper.ToDomain(adminUserOrm);
        }

        public async Task<string?> GetPasswordHashByEmail(string email)
        {
            var tenantDbContext = await _tenantDbContextFactory.CreateDbContextByTenantContext();

            return await tenantDbContext.admin_users
                .Where(au => au.email == email)
                .Select(au => au.password_hash)
                .FirstOrDefaultAsync();
        }

        public async Task Update(AdminUserDomainEntity adminUserDomainEntity)
        {
            var tenantDbContext = await _tenantDbContextFactory.CreateDbContextByTenantContext();

            var adminUserOrm = AdminUserMapper.ToOrm(adminUserDomainEntity);
            tenantDbContext.admin_users.Update(adminUserOrm);

            await tenantDbContext.SaveChangesAsync();
        }

        public async Task<AdminUserDomainEntity?> GetByEmail(string email)
        {
            var tenantDbContext = await _tenantDbContextFactory.CreateDbContextByTenantContext();

            var adminUserOrm = await tenantDbContext.admin_users
                .Where(au => au.email == email)
                .FirstOrDefaultAsync();

            if(adminUserOrm is null)
            {
                return null;
            }

            return AdminUserMapper.ToDomain(adminUserOrm);
        }

        public async Task<List<ListAdminUsersOutput>> FindAll()
        {
            var tenantDbContext = await _tenantDbContextFactory.CreateDbContextByTenantContext();

            return await tenantDbContext.admin_users
                .Select(au => new ListAdminUsersOutput
                {
                    Id = au.id,
                    Email = au.email,
                    Username = au.username,
                    FirstName = au.first_name,
                    IsActive = au.is_active,
                    LastLoginAt = au.last_login_at,
                    CreatedAt = au.created_at
                })
                .ToListAsync();
        }

        public async Task DeleteById(int id)
        {
            var tenantDbContext = await _tenantDbContextFactory.CreateDbContextByTenantContext();

            await tenantDbContext.admin_users
                .Where(au => au.id == id)
                .ExecuteDeleteAsync();

            await tenantDbContext.SaveChangesAsync();
        }
    }
}