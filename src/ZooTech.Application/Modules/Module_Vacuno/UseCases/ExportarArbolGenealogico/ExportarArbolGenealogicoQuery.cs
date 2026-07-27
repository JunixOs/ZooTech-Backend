using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ExportarArbolGenealogico;

public sealed record ExportarArbolGenealogicoQuery(
    long VacunoId,
    int Niveles = 4,
    string Formato = "excel"
) : IAuditableCommandQueryRequest
{
    public AuditEventType EventType => AuditEventType.DataExport;

    public string Action => "Exportar arbol genealogico";
}