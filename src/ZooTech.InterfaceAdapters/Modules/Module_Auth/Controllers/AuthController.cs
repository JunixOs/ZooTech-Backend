using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ZooTech.InterfaceAdapters.DTOs.Responses;

namespace ZooTech.InterfaceAdapters.Modules.Module_Auth.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/v1/auth")]
public sealed class AuthController : ControllerBase
{
    private const string DemoEmail = "admin@zootech.com";
    private const string DemoPassword = "Zootech2026!";

    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (!string.Equals(request.Email?.Trim(), DemoEmail, StringComparison.OrdinalIgnoreCase) ||
            request.Password != DemoPassword)
        {
            return Unauthorized(ErrorResponse.Create(
                "INVALID_CREDENTIALS",
                "Credenciales invalidas."));
        }

        var expiresAt = DateTime.UtcNow.AddHours(GetExpirationHours());

        return Ok(new LoginResponse
        {
            Token = CreateJwt(expiresAt),
            TokenType = "Bearer",
            ExpiresAt = expiresAt,
            User = new AuthUserResponse
            {
                Id = 1,
                Nombre = "Administrador Zootech",
                Email = DemoEmail,
                Rol = "Administrador"
            }
        });
    }

    private string CreateJwt(DateTime expiresAt)
    {
        var issuer = GetRequiredConfiguration("Jwt:Issuer");
        var audience = GetRequiredConfiguration("Jwt:Audience");
        var signingKey = GetRequiredConfiguration("Jwt:SigningKey");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, "1"),
            new Claim(JwtRegisteredClaimNames.Email, DemoEmail),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Name, "Administrador Zootech"),
            new Claim(ClaimTypes.Email, DemoEmail),
            new Claim(ClaimTypes.Role, "Administrador")
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private double GetExpirationHours()
        => double.TryParse(_configuration["Jwt:ExpirationHours"], out var hours) && hours > 0
            ? hours
            : 8;

    private string GetRequiredConfiguration(string key)
        => _configuration[key]
            ?? throw new InvalidOperationException($"{key} no esta configurado.");
}

public sealed class LoginRequest
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}

public sealed class LoginResponse
{
    public string Token { get; set; } = default!;
    public string TokenType { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }
    public AuthUserResponse User { get; set; } = default!;
}

public sealed class AuthUserResponse
{
    public int Id { get; set; }
    public string Nombre { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Rol { get; set; } = default!;
}
