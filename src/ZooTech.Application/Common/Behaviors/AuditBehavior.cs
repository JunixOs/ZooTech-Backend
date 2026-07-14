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
            AuditModel? auditModel = null;
            
            if(request is IAuditableRequest auditable)
            {
                auditModel = new AuditModel
                {
                    EventType = auditable.EventType,
                    Action = auditable.Action,
                    RequestValues = request
                };
            }

            var response = await next();

            if(auditModel is not null)
            {
                auditModel.ResponseValues = response;

                await _appAuditService.AuditEventAsync(auditModel);
            }

            return response;
        }
    }
}