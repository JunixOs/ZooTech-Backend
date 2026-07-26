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
            AuditEventInfo? auditEventInfo = null;
            
            if(request is IAuditableCommandQueryRequest auditable)
            {
                auditEventInfo = new AuditEventInfo
                {
                    EventType = auditable.EventType,
                    Action = auditable.Action,
                    RequestValues = request
                };
            }

            var response = await next();

            if(auditEventInfo is not null)
            {
                auditEventInfo.ResponseValues = response is IAuditResponseMetadataProvider metadataProvider
                    ? metadataProvider.GetAuditMetadata()
                    : response;

                await _appAuditService.AuditEventAsync(auditEventInfo);
            }

            return response;
        }
    }
}
