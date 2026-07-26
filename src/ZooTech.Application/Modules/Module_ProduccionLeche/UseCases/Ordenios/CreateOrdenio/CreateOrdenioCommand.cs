using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.CreateOrdenio;

public class CreateOrdenioCommand : IAuditableCommandQueryRequest
{
    public AuditEventType EventType => AuditEventType.Create;
    public string Action => "Create a ordeño record";

    public string? Codigo { get; set; }
    public DateTime FechaHora { get; set; }
    public long VacunoId { get; set; }
    public long EncargadoUsuarioId { get; set; }
    public decimal Litros { get; set; }
    public string? EstadoOrdenioCode { get; set; }
    public string? Observaciones { get; set; }
}
