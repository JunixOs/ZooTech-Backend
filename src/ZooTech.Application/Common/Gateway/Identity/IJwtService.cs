namespace ZooTech.Application.Common.Gateway.Identity
{
    public interface IJwtService
    {
        string GenerateToken(int userId, string userName, string email, string userRole);
        bool IsTokenValid(string token);
    }
}