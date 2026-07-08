using Microsoft.AspNetCore.Mvc.Filters;
using ZooTech.Application.Common.Gateway.Identity;
using ZooTech.InterfaceAdapters.Exceptions;

namespace ZooTech.InterfaceAdapters.Filters
{

    public class AnonymousOnlyFilter : IAuthorizationFilter
    {
        private readonly ICurrentUserService _currentUserService;

        public AnonymousOnlyFilter(
            ICurrentUserService currentUserService
        )
        {
            _currentUserService = currentUserService;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (_currentUserService.IsAuthenticated)
            {
                throw new AlreadyAuthenticatedException();
            }
        }
    }
}