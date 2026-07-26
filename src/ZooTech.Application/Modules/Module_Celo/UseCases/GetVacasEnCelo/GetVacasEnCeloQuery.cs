using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.GetVacasEnCelo
{
    public class GetVacasEnCeloQuery : IAuditableRequest
    {
        public AuditEventType EventType => AuditEventType.Read;
        public string Action => "Get vacas en celo";

        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
    }
}