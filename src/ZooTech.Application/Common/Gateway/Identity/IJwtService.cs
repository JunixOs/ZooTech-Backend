using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Common.Gateway.Identity
{
    public interface IJwtService
    {
        string GenerateToken(int userId, string userName, string email, UserRole userRole);
        bool IsTokenValid(string token);
    }
}