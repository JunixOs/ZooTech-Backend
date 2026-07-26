using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.CreateCelo;

public class CreateCeloCommand : IAuditableCommandQueryRequest
{
    public AuditEventType EventType => AuditEventType.Create;
    public string Action => "Creating a celo record";

    public long VacunoId { get; set; }
    public long EncargadoUsuarioId { get; set; }
    public DateTime? FechaHora { get; set; }
    public string? Observaciones { get; set; }
    public List<string> CaracteristicaCodes { get; set; } = new();

}
