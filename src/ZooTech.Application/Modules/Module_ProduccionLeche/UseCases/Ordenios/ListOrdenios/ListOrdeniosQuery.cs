using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.ListOrdenios;

public class ListOrdeniosQuery : IAuditableCommandQueryRequest
{
    public AuditEventType EventType => AuditEventType.Read;
    public string Action => "List ordeños";

    public long? VacunoId { get; set; }
    public string? EstadoOrdenioCode { get; set; }
    public DateTime? FechaDesde { get; set; }
    public DateTime? FechaHasta { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
