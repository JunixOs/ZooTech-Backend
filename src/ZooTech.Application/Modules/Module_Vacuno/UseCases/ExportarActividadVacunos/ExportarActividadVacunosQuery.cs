using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ExportarActividadVacunos;

public sealed record ExportarActividadVacunosQuery(
    DateOnly? FechaInicio,
    DateOnly? FechaFin,
    string Formato) : IAuditableCommandQueryRequest
{
    public AuditEventType EventType => AuditEventType.DataExport;

    public string Action => "Exportar actividad de vacunos";
}
