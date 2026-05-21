using ZooTech.API.Services;

namespace ZooTech.API.Security;

public sealed class DemoAuthEndpointFilter(IDemoAuthService authService) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var httpContext = context.HttpContext;
        var authorization = httpContext.Request.Headers.Authorization.ToString();
        var token = authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
            ? authorization["Bearer ".Length..].Trim()
            : null;

        if (!authService.IsValidToken(token))
        {
            return Results.Unauthorized();
        }

        return await next(context);
    }
}
