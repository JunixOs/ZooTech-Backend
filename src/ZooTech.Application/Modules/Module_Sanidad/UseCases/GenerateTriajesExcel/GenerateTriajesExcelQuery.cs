using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GenerateTriajesExcel;

public sealed record GenerateTriajesExcelQuery(
    string? Fecha,
    string? FechaDesde,
    string? FechaHasta,
    string? Codigo,
    string? Nombre,
    string? TipoPeso,
    decimal? PesoKg,
    long? VacunoId) : IAuditableRequest
{
    public AuditEventType EventType => AuditEventType.DataExport;

    public string Action => "Generate triajes excel";
}