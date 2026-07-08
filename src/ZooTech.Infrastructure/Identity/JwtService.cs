using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ZooTech.Application.Common.Gateway.Identity;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Infrastructure.Identity
{
    public class JwtService : IJwtService
    {
        private readonly JwtSettings _settings;

        public JwtService(IOptions<JwtSettings> settings)
        {
            _settings = settings.Value;
        }

        public (string token, string jti) GenerateToken(
            int userId, 
            string userName,
            string email, 
            UserRole userRole
        )
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_settings.SecretKey)
            );

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var jti = Guid.NewGuid().ToString();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Role, userRole.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, jti)
            };

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    int.TryParse(_settings.ExpirationMinutes, out var minutes) ? minutes : 30
                ),
                signingCredentials: credentials
            );

            return (new JwtSecurityTokenHandler().WriteToken(token), jti);
        }

        public bool IsTokenValid(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            try
            {
                handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = _settings.Issuer,
                    ValidAudience = _settings.Audience,

                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(_settings.SecretKey!)
                    )
                }, out _);

                return true;
            }
            catch 
            {
                return false;
            }
        }

        public bool IsJwt(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            return handler.CanReadToken(token);
        }
    }
}