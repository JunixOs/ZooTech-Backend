using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.DeleteOrdenio;

public class DeleteOrdenioCommand : IAuditableRequest
{
    public AuditEventType EventType => AuditEventType.Delete;
    public string Action => "Delete a ordeño record";
    
    public long Id { get; set; }
    public string? MotivoEliminacion { get; set; }

}
