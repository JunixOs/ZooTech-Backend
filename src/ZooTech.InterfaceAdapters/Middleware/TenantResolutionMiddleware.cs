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
                    message: "Missing configuration: MultiTenant:BaseDomain"
                );
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
                throw new NotFoundException(ScopeName.Interface_Adapters);
            }

            var subDomain = ExtractSubDomain(domain);

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
            if (!host.EndsWith("." + _baseDomain))
            {
                return null;
            }

            return host[..^(_baseDomain.Length + 1)];
        }
    }
}
