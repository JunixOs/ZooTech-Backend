using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GenerateTriajesPdf;

public sealed record GenerateTriajesPdfQuery(
    string? Fecha,
    string? FechaDesde,
    string? FechaHasta,
    string? Codigo,
    string? Nombre,
    string? TipoPeso,
    string? PesoKg,
    long? VacunoId) : IAuditableRequest
{
    public AuditEventType EventType => AuditEventType.DataExport;

    public string Action => "Generate triaje pdf";
}
