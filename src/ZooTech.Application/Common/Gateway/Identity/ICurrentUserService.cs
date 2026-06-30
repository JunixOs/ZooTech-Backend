namespace ZooTech.Application.Common.Gateway.Identity
{
    public class ICurrentUserService
    {
        long? UserId { get; }
        string? Email { get; }
        string? Role { get; }
        bool IsAuthenticated { get; }
    }
}