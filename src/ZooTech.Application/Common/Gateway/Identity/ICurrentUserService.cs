namespace ZooTech.Application.Common.Gateway.Identity
{
    public class ICurrentUserService
    {
        public int? UserId { get; }
        public string? UserName { get; }
        public string? Email { get; }
        public string? Role { get; }
        public bool IsAuthenticated { get; }
    }
}