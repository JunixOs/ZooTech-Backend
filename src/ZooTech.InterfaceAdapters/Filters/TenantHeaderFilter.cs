using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using ZooTech.Application.Common.Gateway.Context;

namespace ZooTech.InterfaceAdapters.Filters
{
    public class TenantHeaderFilter : IActionFilter
    {
        private readonly ITenantContext _tenantContext;

        public TenantHeaderFilter(ITenantContext tenantContext)
        {
            _tenantContext = tenantContext;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            var headers = context.HttpContext.Response.Headers;

            headers.Append("X-Tenant-Id", _tenantContext.TenantId.ToString());
            headers.Append("X-Tenant-Name", _tenantContext.Code);
            headers.Append("X-Tenant-Legal-Name", WebUtility.UrlEncode(_tenantContext.LegalName));
            headers.Append("X-Tenant-Type", _tenantContext.Type.ToString());
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
        }
    }
}