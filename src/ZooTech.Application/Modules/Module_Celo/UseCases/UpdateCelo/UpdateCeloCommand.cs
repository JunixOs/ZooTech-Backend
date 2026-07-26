using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.UpdateCelo;

public class UpdateCeloCommand : IAuditableCommandQueryRequest
{
    public AuditEventType EventType => AuditEventType.Update;

    public string Action => "Updating a celo record";

    public long Id { get; set; }
    public string? Observaciones { get; set; }
    public List<string>? CaracteristicaCodes { get; set; }

}
