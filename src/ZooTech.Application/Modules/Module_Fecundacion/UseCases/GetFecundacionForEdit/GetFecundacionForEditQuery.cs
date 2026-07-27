using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.GetFecundacionForEdit
{
    public record GetFecundacionForEditQuery(
        long Id
    ) : IAuditableCommandQueryRequest
    {
        public AuditEventType EventType => AuditEventType.Read;

        public string Action => "Get fecundacion for edit";
    }
}