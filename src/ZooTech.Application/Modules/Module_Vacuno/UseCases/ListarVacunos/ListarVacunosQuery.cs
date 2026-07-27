using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunos;

public record ListarVacunosQuery(
    string? Query = null,
    DateTime? FechaDesde = null,
    DateTime? FechaHasta = null,
    string? Estado = null,
    int Page = 1,
    int Limit = 20) : IAuditableCommandQueryRequest
{
    public AuditEventType EventType => AuditEventType.Read;

    public string Action => "Listar vacunos";
}