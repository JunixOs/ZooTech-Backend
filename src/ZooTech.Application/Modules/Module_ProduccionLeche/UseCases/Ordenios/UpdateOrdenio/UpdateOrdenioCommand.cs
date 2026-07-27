using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.UpdateOrdenio;

public class UpdateOrdenioCommand : IAuditableCommandQueryRequest
{
    public AuditEventType EventType => AuditEventType.Update;
    public string Action => "Update a ordeño record";

    public long Id { get; set; }
    public DateTime FechaHora { get; set; }
    public long EncargadoUsuarioId { get; set; }
    public decimal Litros { get; set; }
    public string? EstadoOrdenioCode { get; set; }
    public string? Observaciones { get; set; }

}