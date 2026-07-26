using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.GetComparacionCelosRealVsEstandar
{
    public class GetComparacionCelosRealVsEstandarQuery : IAuditableRequest
    {
        public AuditEventType EventType => AuditEventType.Read;
        public string Action => "Get comparacion celos real vs estandar";

        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
    }
}