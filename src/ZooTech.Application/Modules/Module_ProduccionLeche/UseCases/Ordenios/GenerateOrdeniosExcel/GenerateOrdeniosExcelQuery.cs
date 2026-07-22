using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosExcel;

public class GenerateOrdeniosComparationExcelQuery : IAuditableRequest
{
    public AuditEventType EventType => AuditEventType.DataExport;
    public string Action => "Generate a ordeño excel";

    public long? VacunoId { get; set; }
    public string? EstadoOrdenioCode { get; set; }
    public DateTime? FechaDesde { get; set; }
    public DateTime? FechaHasta { get; set; }
    public bool Comparativo { get; set; } = false;

}
