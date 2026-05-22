using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using ZooTech.Application.Common.Gateway.Context;
using ZooTech.Application.Common.Gateway.Tenant;

namespace ZooTech.InterfaceAdapters.Middleware
{
    public class TenantResolutionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _baseDomain;
        private readonly string _adminSubDomain;

        public TenantResolutionMiddleware(
            RequestDelegate next,
            IConfiguration config
        )
        {
            _next = next;
            _baseDomain = config["MultiTenant:BaseDomain"];
            _adminSubDomain = config["MultiTenant:AdminSubDomain"];
        }

        public async Task InvokeAsync(
            HttpContext context,
            ITenantStore tenantStore,
            ITenantContext tenantContext
        )
        {
            var host = context.Request.Host.Host;

            var subDomain = ExtractSubDomain(host);

            if(subDomain == null)
            {
                context.Response.StatusCode = 404;
                return;
            }

            var tenant = await tenantStore.GetBySubDomainAsync(subDomain);

            if(tenant == null)
            {
                // TODO: Modificar para que coincida con api-contract.md
                context.Response.StatusCode = 404;
                return;
            }

            tenantContext.SetTenant(
                tenant.Id,
                tenant.Code,
                tenant.SubDomain,
                tenant.DatabaseName
            );

            await _next(context);
        }

        private string? ExtractSubDomain(string host)
        {
            if(!host.EndsWith("." + _baseDomain))
            {
                return null;
            }

            return host[..^(_baseDomain.Length + 1)];
        }
    }
}