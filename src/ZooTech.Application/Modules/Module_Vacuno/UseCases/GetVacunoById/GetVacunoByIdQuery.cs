using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.GetVacunoById
{
    public record GetVacunoByIdQuery(long Id) : IAuditableCommandQueryRequest
    {
        public AuditEventType EventType => AuditEventType.Read;

        public string Action => "Get vacuno by Id";
    }
}