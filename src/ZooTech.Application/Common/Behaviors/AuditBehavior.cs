using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Application.Common.Gateway.Identity;

namespace ZooTech.Application.Common.Behaviors
{
    public class AuditBehavior<TRequest, TResponse> : IBehavior<TRequest, TResponse>
    {
        private readonly IAppAuditService _appAuditService;
        private readonly ICurrentUserService _currentUserService;

        public AuditBehavior(
            IAppAuditService appAuditService,
            ICurrentUserService currentUserService
        )
        {
            _appAuditService = appAuditService;
            _currentUserService = currentUserService;
        }
        
        public async Task<TResponse> Handle(TRequest request, Func<Task<TResponse>> next)
        {
            var response = await next();

            if(request is IAuditableRequest auditable)
            {
                await _appAuditService.SaveLogAsync(
                    new AuditModel
                    {
                        EventType = auditable.EventType,
                        Action = auditable.Action,
                        UserId = _currentUserService.UserId,
                        UserName = _currentUserService.UserName
                    }
                );
            }

            return response;
        }
    }
}