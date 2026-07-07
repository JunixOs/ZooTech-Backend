using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetHistorialByVacunoId
{
    public class GetHistorialByVacunoIdCommand : IAuditableRequest
    {
        public AuditEventType EventType => AuditEventType.Read;
        public string Action => "Get historial by vacuno Id";

        public long Vacunoid { get; set; }
    }
}