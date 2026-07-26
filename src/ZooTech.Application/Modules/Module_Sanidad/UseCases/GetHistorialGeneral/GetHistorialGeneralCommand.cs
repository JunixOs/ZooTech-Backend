using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetHistorialGeneral
{
    public class GetHistorialGeneralCommand : IAuditableCommandQueryRequest
    {
        public AuditEventType EventType => AuditEventType.Read;
        public string Action => "Get historial general";

        public string? FechaDesde { get; set; } 
        public string? FechaHasta { get; set; }
    }
}