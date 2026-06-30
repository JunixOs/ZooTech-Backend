using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using ZooTech.Application.Common.Gateway.Context;

namespace ZooTech.InterfaceAdapters.Middleware
{
    public class TenantHeaderFilter : IActionFilter
    {
        private readonly ITenantContext tenantContext;
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var headers = context.HttpContext.Response.Headers;

            headers.Append("X-Tenant-Id", tenantContext.TenantId.ToString());
            headers.Append("X-Tenant-Name", tenantContext.Code);
            headers.Append("X-Tenant-Legal-Name", tenantContext.LegalName);
            headers.Append("X-App-Variant", tenantContext.Type);
        }
    }
}