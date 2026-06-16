using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using ZooTech.Application.Common.Gateway.Context;

namespace ZooTech.Infrastructure.Tenant;

/// <summary>
/// Implementación de ITenantContext para desarrollo y producción.
/// Lee el TenantId desde el header HTTP "X-Tenant-Id".
/// Si no viene el header, usa el DefaultTenantId del appsettings.json.
/// </summary>
public class TenantContext : ITenantContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly Guid _defaultTenantId;

    public TenantContext(
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration)
    {
        _httpContextAccessor = httpContextAccessor;

        var defaultTenantIdStr = configuration["Tenancy:DefaultTenantId"];
        _defaultTenantId = Guid.TryParse(defaultTenantIdStr, out var parsed)
            ? parsed
            : Guid.Parse("00000000-0000-0000-0000-000000000001");
    }

    public Guid TenantId
    {
        get
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext != null
                && httpContext.Request.Headers.TryGetValue("X-Tenant-Id", out var headerValue)
                && Guid.TryParse(headerValue, out var tenantId))
            {
                return tenantId;
            }

            // Fallback al tenant por defecto configurado en appsettings
            return _defaultTenantId;
        }
    }

    public string DatabaseName => $"GanaderiaDb_{TenantId}";
}
