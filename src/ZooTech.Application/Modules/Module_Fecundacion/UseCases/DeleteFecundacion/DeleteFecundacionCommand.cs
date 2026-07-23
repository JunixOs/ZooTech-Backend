using ZooTech.Application.Common.Caching;
using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.DeleteFecundacion;

public sealed record DeleteFecundacionCommand(long Id, string Razon) : IAuditableRequest, IEvictCacheRequest
{
    public AuditEventType EventType => AuditEventType.Delete;

    public string Action => "Delete fecundacion";

    public string[] GetCachePrefixesToEvict() => ["fecundacion:listar"];
}