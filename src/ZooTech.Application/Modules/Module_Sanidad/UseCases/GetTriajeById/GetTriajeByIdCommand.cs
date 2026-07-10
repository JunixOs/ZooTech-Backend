using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetTriajeById
{
    public class GetTriajeByIdCommand : IAuditableRequest
    {
        public AuditEventType EventType => AuditEventType.Read;
        public string Action => "Get triaje by Id";

        public long Id { get; set; }
    }
}