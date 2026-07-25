using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.GetHistorialCeloPorVacuno;

public sealed record GetHistorialCeloPorVacunoCommand(string CodigoVacuno, long RegistroId) : IAuditableRequest
{
    public AuditEventType EventType => AuditEventType.Read;
    public string Action => "Reading heat history by cow";
}
