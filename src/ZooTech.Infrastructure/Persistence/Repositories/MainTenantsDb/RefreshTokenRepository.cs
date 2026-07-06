using Microsoft.EntityFrameworkCore;
using ZooTech.Application.Common.Gateway.Repositories.MainTenantsDb;
using ZooTech.Domain.Admin.Entities;
using ZooTech.Infrastructure.Tenant;

namespace ZooTech.Infrastructure.Persistence.Repositories.MainTenantsDb
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        public readonly ITenantDbContextFactory _tenantDbContextFactory;

        public RefreshTokenRepository(
            ITenantDbContextFactory tenantDbContextFactory
        )
        {
            _tenantDbContextFactory = tenantDbContextFactory;
        }

        public async Task DeleteByAdminUserIdAndToken(int adminUserId, string unhashedToken)
        {
            var tenantDbContext = await _tenantDbContextFactory.CreateDbContextByTenantContext();

            await tenantDbContext.refresh_tokens
                .Where(rt => 
                    rt.admin_user_id == adminUserId && 
                    rt.token == unhashedToken
                )
                .ExecuteDeleteAsync();
        }

        public Task<RefreshTokenDomainEntity> GetByAdminUserIdAndToken(int adminUserId, string token)
        {
            throw new NotImplementedException();
        }

        public Task<RefreshTokenDomainEntity> GetById(string id)
        {
            throw new NotImplementedException();
        }

        public Task Update(RefreshTokenDomainEntity refreshTokenDomainEntity)
        {
            throw new NotImplementedException();
        }
    }
}