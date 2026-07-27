using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ListarVacunosReporte;

public sealed record ListarVacunosReporteQuery(
    string? FechaDesde,
    string? FechaHasta,
    string? Search,
    string? Q,
    string? Codigo,
    string? FechaRegistro,
    string? Nombre,
    string? Raza,
    string? Procedencia,
    string? Estado,
    string? EstadoRegistro,
    string? AptoPara,
    string? Formato,
    string? Page,
    string? PageSize,
    string? Limit) : IAuditableCommandQueryRequest
{
    public AuditEventType EventType => AuditEventType.Read;

    public string Action => "Listar vacunos reporte";
}