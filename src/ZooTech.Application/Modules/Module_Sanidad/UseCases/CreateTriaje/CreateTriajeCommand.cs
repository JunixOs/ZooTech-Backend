using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.CreateTriaje;

public class CreateTriajeCommand : IAuditableRequest
{
    public AuditEventType EventType => AuditEventType.Create;
    public string Action => "Create a triaje";

    public long VacunoId { get; set; }
    public string? TipoPesoCode { get; set; }
    public decimal PesoKg { get; set; }
    public string? Observaciones { get; set; }
    public string? EstadoRegistroCode { get; set; }
    public long? EncargadoUsuarioId { get; set; }
}