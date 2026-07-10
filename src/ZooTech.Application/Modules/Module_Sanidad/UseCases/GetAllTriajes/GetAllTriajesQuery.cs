using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllTriajes;

public class GetAllTriajesQuery : IAuditableRequest
{
    public AuditEventType EventType => AuditEventType.Read;
    public string Action => "Get all triajes";

    public int Pagina { get; set; }
    public int Tamano { get; set; }
    public string? Fecha { get; set; }
    public string? FechaDesde { get; set; }
    public string? FechaHasta { get; set; }
    public string? Codigo { get; set; }
    public string? Nombre { get; set; }
    public string? TipoPeso { get; set; }
    public decimal? PesoKg { get; set; }
    public long? VacunoId { get; set; }
    public bool? UniqueVacuno { get; set; }
}
