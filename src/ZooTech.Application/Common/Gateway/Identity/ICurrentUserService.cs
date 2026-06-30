namespace ZooTech.Application.Common.Gateway.Identity
{
    public class ICurrentUserService
    {
        long? UserId { get; }
        string? UserName { get; }
        string? Role { get; }
        bool IsAuthenticated { get; }
    }
}