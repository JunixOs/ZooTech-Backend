using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.GetArbolGenealogico;

public record GetArbolGenealogicoQuery(long Id, int Niveles) : IAuditableCommandQueryRequest
{
    public AuditEventType EventType => AuditEventType.Read;

    public string Action => "Get arbol genealogico";
}