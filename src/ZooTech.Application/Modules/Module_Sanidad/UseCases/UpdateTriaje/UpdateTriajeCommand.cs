using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.UpdateTriaje;

public class UpdateTriajeCommand : IAuditableCommandQueryRequest
{
    public AuditEventType EventType => AuditEventType.Update;
    public string Action => "Update a triaje";

    public long Id { get; set; }
    public string? TipoPesoCode { get; set; }
    public decimal PesoKg { get; set; }
    public string? Observaciones { get; set; }
    public long? EncargadoUsuarioId { get; set; }

}
