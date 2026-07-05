using ZooTech.Domain.Admin.Entities;

namespace ZooTech.Application.Common.Gateway.Repositories.MainTenantsDb
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshTokenDomainEntity> GetById(string id);

        // El usuario debe enviar su refreshToken
        Task<RefreshTokenDomainEntity> GetByAdminUserIdAndToken(int adminUserId, string unhashedToken);
        Task Update(RefreshTokenDomainEntity refreshTokenDomainEntity);
        Task DeleteByAdminUserIdAndToken(int adminUserId, string unhashedToken);
    }
}