using ZooTech.API.Models;
using ZooTech.API.Services;

namespace ZooTech.API.Endpoints;

public static class AuthEndpoints
{
    public static RouteGroupBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Autenticacion");

        group.MapPost("/login", (LoginRequest request, IDemoAuthService authService) =>
            {
                var response = authService.Authenticate(request);
                return response is null
                    ? Results.Unauthorized()
                    : Results.Ok(response);
            })
            .WithName("Login")
            .WithSummary("Autentica al usuario demo y devuelve token Bearer.");

        return group;
    }
}
