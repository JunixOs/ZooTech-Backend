using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ZooTech.Application.Common.Gateway.Context;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.InterfaceAdapters.Filters
{
    public class RestrictTenantTypeFilter : IActionFilter
    {
        private readonly ITenantContext _tenantContext;
        private readonly TenantType _allowedType;

        public RestrictTenantTypeFilter(
            ITenantContext tenantContext,
            TenantType allowedType)
        {
            _tenantContext = tenantContext;
            _allowedType = allowedType;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (_tenantContext.Type != _allowedType)
            {
                throw new Exceptions.UnauthorizedAccessException();
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }

    public class RestrictTenantTypeAttribute : TypeFilterAttribute
    {
        public RestrictTenantTypeAttribute(TenantType allowedType)
            : base(typeof(RestrictTenantTypeFilter))
        {
            Arguments = new object[]
            {
                allowedType
            };
        }
    }
}