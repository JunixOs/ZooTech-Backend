using ZooTech.Domain.Admin.Entities;

namespace ZooTech.Application.Common.Gateway.Repositories.MainTenantsDb
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshTokenDomainEntity> GetById(string id);

        // El usuario debe enviar su refreshToken
        Task<RefreshTokenDomainEntity> GetByAdminUserIdAndToken(string adminUserId, string token);
        Task Update(RefreshTokenDomainEntity refreshTokenDomainEntity);
        Task DeleteByAdminUserIdAndToken(string adminUserId, string token);
    }
}