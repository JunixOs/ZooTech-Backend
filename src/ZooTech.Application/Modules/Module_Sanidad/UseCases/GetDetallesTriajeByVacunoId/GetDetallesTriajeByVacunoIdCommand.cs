using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetDetalleTriajeByVacunoId
{
    public class GetDetallesTriajeByVacunoIdCommand : IAuditableCommandQueryRequest
    {
        public AuditEventType EventType => AuditEventType.Read;
        public string Action => "Get detalles triaje by vacuno Id";
    
        public long VacunoId { get; set; }
    }
}