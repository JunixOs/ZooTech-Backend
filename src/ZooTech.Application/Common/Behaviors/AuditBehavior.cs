using ZooTech.Application.Common.Gateway.Auditing;

namespace ZooTech.Application.Common.Behaviors
{
    public class AuditBehavior<TRequest, TResponse> : IBehavior<TRequest, TResponse>
    {
        private readonly IAppAuditService _appAuditService;

        public AuditBehavior(
            IAppAuditService appAuditService
        )
        {
            _appAuditService = appAuditService;
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
                    }
                );
            }

            return response;
        }
    }
}