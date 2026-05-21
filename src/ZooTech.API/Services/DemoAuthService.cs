using ZooTech.API.Models;

namespace ZooTech.API.Services;

public interface IDemoAuthService
{
    LoginResponse? Authenticate(LoginRequest request);
    bool IsValidToken(string? token);
}

public sealed class DemoAuthService : IDemoAuthService
{
    public const string DemoEmail = "admin@zootech.com";
    public const string DemoPassword = "Zootech2026!";
    public const string DemoToken = "zootech-demo-token";

    private static readonly UserResponse DemoUser = new(
        Id: 1,
        Nombre: "Administrador Zootech",
        Email: DemoEmail,
        Rol: "Administrador");

    public LoginResponse? Authenticate(LoginRequest request)
    {
        if (!string.Equals(request.Email, DemoEmail, StringComparison.OrdinalIgnoreCase) ||
            request.Password != DemoPassword)
        {
            return null;
        }

        return new LoginResponse(
            Token: DemoToken,
            TokenType: "Bearer",
            ExpiresAt: DateTime.UtcNow.AddHours(8),
            User: DemoUser);
    }

    public bool IsValidToken(string? token)
    {
        return string.Equals(token, DemoToken, StringComparison.Ordinal);
    }
}
