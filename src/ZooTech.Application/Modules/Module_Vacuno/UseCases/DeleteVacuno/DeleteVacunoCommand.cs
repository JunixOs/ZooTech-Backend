using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.DeleteVacuno;

public sealed record DeleteVacunoCommand(
    long Id,
    string MotivoEliminacion
) : IAuditableCommandQueryRequest
{
    public AuditEventType EventType => AuditEventType.Delete;

    public string Action => "Delete a vacuno";
}