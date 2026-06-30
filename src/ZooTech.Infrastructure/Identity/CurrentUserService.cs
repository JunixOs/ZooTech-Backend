using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ZooTech.Application.Common.Gateway.Identity;

namespace ZooTech.Infrastructure.Identity
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public CurrentUserService(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        private ClaimsPrincipal? User => _contextAccessor.HttpContext?.User;

        public Guid? UserId
        {
            get
            {
                var value = User?.FindFirstValue(JwtRegisteredClaimNames.Sub);

                return Guid.TryParse(value, out var id)
                    ? id
                    : null;
            }
        }

        public string? Email =>
            User?.FindFirstValue(ClaimTypes.Name);

        public string? Role =>
            User?.FindFirstValue(ClaimTypes.Role);

        public bool IsAuthenticated =>
            User?.Identity?.IsAuthenticated ?? false;
    }
}