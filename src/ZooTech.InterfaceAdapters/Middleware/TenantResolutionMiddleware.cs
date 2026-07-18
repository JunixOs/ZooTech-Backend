using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Gateway.Context;
using ZooTech.Application.Common.Gateway.Tenant;
using ZooTech.Domain.Shared.Enums;
using ZooTech.InterfaceAdapters.Exceptions;

namespace ZooTech.InterfaceAdapters.Middleware
{
    public class TenantResolutionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _baseDomain;
        private readonly string? _adminSubDomain;

        public TenantResolutionMiddleware(
            RequestDelegate next,
            IConfiguration config
        )
        {
            _next = next;
            _baseDomain = config["MultiTenant:BaseDomain"]
                ?? throw new UndefinedConfigurationValue(
                    message: "Missing configuration: MultiTenant:BaseDomain"
                );
            _adminSubDomain = config["MultiTenant:AdminSubDomain"]
                ?? throw new UndefinedConfigurationValue(
                    message: "Missing configuration: MultiTenant:AdminSubDomain"
                );
        }

        public async Task InvokeAsync(
            HttpContext context,
            ITenantStore tenantStore,
            ITenantContext tenantContext
        )
        {
            var path = context.Request.Path.Value ?? "";
            if (path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase) ||
                path.Equals("/favicon.ico", StringComparison.OrdinalIgnoreCase) ||
                context.Request.Method == "OPTIONS")
            {
                await _next(context);
                return;
            }

            var domain = context.Request.Headers["X-Tenant-Url"].FirstOrDefault();

            if (domain == null)
            {
                domain = "localhost";
            }

            var subDomain = ExtractSubDomain(domain);

            if (subDomain == null && (domain == "localhost" || domain == "127.0.0.1" || domain.StartsWith("localhost:")))
            {
                // En desarrollo local, las rutas admin usan subdomain 'admin', el resto 'zootecniaunas'
                subDomain = path.StartsWith("/api/v1/auth/admin", StringComparison.OrdinalIgnoreCase)
                    || path.StartsWith("/api/v1/tenancing", StringComparison.OrdinalIgnoreCase)
                    ? _adminSubDomain
                    : "zootecniaunas";
            }

            if (subDomain == null)
            {
                throw new NotFoundException(ScopeName.Interface_Adapters);
            }

            var tenant = await tenantStore.GetBySubDomainAsync(subDomain);

            if (tenant == null)
            {
                throw new NotFoundException(ScopeName.Interface_Adapters);
            }

            tenantContext.SetTenant(
                tenant.Id,
                tenant.Code,
                tenant.LegalName,
                tenant.DisplayName,
                subDomain == _adminSubDomain ? TenantType.Admin : TenantType.Tenant,
                tenant.SubDomain,
                tenant.DatabaseName
            );

            await _next(context);
        }

        private string? ExtractSubDomain(string host)
        {
            var cleanHost = host.Contains(":") ? host.Split(':')[0] : host;

            if (cleanHost.EndsWith(".localhost"))
            {
                return cleanHost[..^(".localhost".Length)];
            }
            if (cleanHost.EndsWith(".zentrycorp.dev"))
            {
                return cleanHost[..^(".zentrycorp.dev".Length)];
            }

            if (!cleanHost.EndsWith("." + _baseDomain))
            {
                return null;
            }

            return cleanHost[..^(_baseDomain.Length + 1)];
        }
    }
}
