namespace ZooTech.Application.Common.Gateway.Identity
{
    public interface IJwtService
    {
        string GenerateToken(long userId, string username, string userRole);
        bool IsTokenValid(string token);
    }
}