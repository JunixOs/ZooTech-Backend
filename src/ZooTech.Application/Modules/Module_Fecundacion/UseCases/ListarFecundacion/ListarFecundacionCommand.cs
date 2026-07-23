using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.ListarFecundacion;

public record ListarFecundacionCommand(
    string? Query = null,
    DateTime? FechaDesde = null,
    DateTime? FechaHasta = null,
    string? Resultado = null,
    int Page = 1,
    int Limit = 20) : IAuditableRequest
{
    public AuditEventType EventType => AuditEventType.Read;

    public string Action => "Listar fecundacion";
}