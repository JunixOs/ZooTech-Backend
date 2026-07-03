using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using ZooTech.Application.Common.Gateway.Context;
using ZooTech.Application.Common.Gateway.Tenant;
using ZooTech.Domain.Shared.Enums;

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
                ?? throw new InvalidOperationException("Missing configuration: MultiTenant:BaseDomain");
            _adminSubDomain = config["MultiTenant:AdminSubDomain"];
        }

        public async Task InvokeAsync(
            HttpContext context,
            ITenantStore tenantStore,
            ITenantContext tenantContext
        )
        {
            var domain = context.Request.Headers["X-Tenant-Url"].FirstOrDefault();

            if(domain == null)
            {
                context.Response.StatusCode = 404;
                return;
            }

            var subDomain = ExtractSubDomain(domain);

            if (subDomain == null)
            {
                context.Response.StatusCode = 404;
                return;
            }

            var tenant = await tenantStore.GetBySubDomainAsync(subDomain);

            if (tenant == null)
            {
                context.Response.StatusCode = 404;
                return;
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
            if (!host.EndsWith("." + _baseDomain))
            {
                return null;
            }

            return host[..^(_baseDomain.Length + 1)];
        }
    }
}
