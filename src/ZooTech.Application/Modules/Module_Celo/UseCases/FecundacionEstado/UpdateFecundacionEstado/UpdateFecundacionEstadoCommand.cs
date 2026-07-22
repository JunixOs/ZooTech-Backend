using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.UpdateFecundacionEstado;

public sealed record UpdateFecundacionEstadoCommand(
    long FecundacionId,
    string? EstadoFecundacion,
    long? UpdatedBy) : IAuditableRequest
{
    public AuditEventType EventType => AuditEventType.Update;

    public string Action => "Update a fecundacion estado";
}