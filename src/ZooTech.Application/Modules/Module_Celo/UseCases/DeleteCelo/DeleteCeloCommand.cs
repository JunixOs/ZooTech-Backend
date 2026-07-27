using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.DeleteCelo;

public class DeleteCeloCommand : IAuditableCommandQueryRequest
{
    public AuditEventType EventType => AuditEventType.Delete;
    public string Action => "Deleting a celo record";

    public long Id { get; set; }
    public string? MotivoEliminacion { get; set; }

}