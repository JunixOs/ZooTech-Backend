namespace ZooTech.API.Models;

public sealed record LoginRequest(
    string Email,
    string Password);

public sealed record UserResponse(
    long Id,
    string Nombre,
    string Email,
    string Rol);

public sealed record LoginResponse(
    string Token,
    string TokenType,
    DateTime ExpiresAt,
    UserResponse User);
