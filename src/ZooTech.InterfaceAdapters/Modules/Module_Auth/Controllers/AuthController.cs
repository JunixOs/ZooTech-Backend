using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ZooTech.InterfaceAdapters.Modules.Module_Auth.Controllers;

[ApiController]
[Route("v1/auth")]
public sealed class AuthController : ControllerBase
{
    private const string DemoEmail = "admin@zootech.com";
    private const string DemoPassword = "Zootech2026!";

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (!string.Equals(request.Email?.Trim(), DemoEmail, StringComparison.OrdinalIgnoreCase) ||
            request.Password != DemoPassword)
        {
            return Unauthorized(new
            {
                message = "Credenciales invalidas."
            });
        }

        var expiresAt = DateTime.UtcNow.AddHours(8);

        return Ok(new LoginResponse
        {
            Token = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
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
