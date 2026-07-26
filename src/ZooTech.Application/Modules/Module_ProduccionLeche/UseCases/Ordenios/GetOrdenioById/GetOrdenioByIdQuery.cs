using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GetOrdenioById
{
    public class GetOrdenioByIdQuery : IAuditableCommandQueryRequest
    {
        public AuditEventType EventType => AuditEventType.Read;
        public string Action => "Get a ordeño by id";

        public long Id { get; set; }
    }
}