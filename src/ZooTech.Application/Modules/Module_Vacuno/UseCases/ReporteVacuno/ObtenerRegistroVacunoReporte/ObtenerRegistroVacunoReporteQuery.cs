using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ObtenerRegistroVacunoReporte;

public sealed record ObtenerRegistroVacunoReporteQuery(
    long VacunoId,
    string? Formato) : IAuditableCommandQueryRequest
{
    public AuditEventType EventType => AuditEventType.DataExport;

    public string Action => "Obtener registro vacuno reporte";
}