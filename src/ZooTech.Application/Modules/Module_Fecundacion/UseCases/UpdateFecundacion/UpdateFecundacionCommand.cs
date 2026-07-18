using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.UpdateFecundacion;

public sealed record UpdateFecundacionCommand(
    long Id,
    string TipoFecundacionCode,
    long VacunoReceptorId,
    string TipoDonante,
    long? VacunoDonanteId,
    string? ExternoDonanteNombre,
    DateOnly FechaProcedimiento,
    string ResponsableNombre,
    string ResultadoCode,
    string EstadoFecundacionCode,
    string? ObservacionesVeterinarias,
    string? CodigoSemen,
    string? CodigoEmbrion) : IAuditableRequest
{
    public AuditEventType EventType => AuditEventType.Update;

    public string Action => "Update fecundacion";
}