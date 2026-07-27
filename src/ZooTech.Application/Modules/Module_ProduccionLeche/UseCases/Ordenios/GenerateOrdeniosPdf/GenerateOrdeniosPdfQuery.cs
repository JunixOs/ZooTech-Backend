using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosPdf;

public class GenerateOrdeniosComparationPdfQuery : IAuditableCommandQueryRequest
{
    public AuditEventType EventType => AuditEventType.DataExport;
    public string Action => "Generate ordeño pdf";
    
    public long? VacunoId { get; set; }
    public string? EstadoOrdenioCode { get; set; }
    public DateTime? FechaDesde { get; set; }
    public DateTime? FechaHasta { get; set; }
    public bool Comparativo { get; set; } = false;

}
