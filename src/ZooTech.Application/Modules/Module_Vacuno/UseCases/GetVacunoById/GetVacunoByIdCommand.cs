using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.GetVacunoById
{
    public record GetVacunoByIdCommand(long Id) : IAuditableRequest
    {
        public AuditEventType EventType => AuditEventType.Read;

        public string Action => "Get vacuno by Id";
    }
}