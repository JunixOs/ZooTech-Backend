using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.GetFecundacionEstado;

public sealed record GetFecundacionEstadoCommand(long VacunoId) : IAuditableRequest
{
    public AuditEventType EventType => AuditEventType.Read;

    public string Action => "Get fecundacion estado";
}