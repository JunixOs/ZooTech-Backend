namespace ZooTech.Application.Common.Gateway.Identity
{
    public interface IJwtService
    {
        string GenerateToken(long userId, string email, string userRole);
        bool IsTokenValid(string token);
    }
}