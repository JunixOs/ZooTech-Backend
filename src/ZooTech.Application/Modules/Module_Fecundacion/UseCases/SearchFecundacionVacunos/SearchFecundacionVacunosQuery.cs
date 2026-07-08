using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.SearchFecundacionVacunos;

public sealed record SearchFecundacionVacunosQuery(
    string? Sexo,
    string? Query,
    bool SoloDisponibles = false,
    long? ExcluirFecundacionId = null) : IAuditableRequest
{
    public AuditEventType EventType => AuditEventType.Search;

    public string Action => "Search fecundacion vacunos";
}