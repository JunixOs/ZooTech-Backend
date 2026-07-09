using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.DeleteTriaje;

public class DeleteTriajeCommand : IAuditableRequest
{
    public AuditEventType EventType => AuditEventType.Delete;
    public string Action => "Delete a triaje";

    public long Id { get; set; }
    public string? MotivoEliminacion { get; set; }
};

