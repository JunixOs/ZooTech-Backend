using ZooTech.Application.Common.Caching;
using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Application.Common.Gateway.Parametrization;
using ZooTech.Application.Modules.Module_Fecundacion.Common;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.ListarFecundacion;

public record ListarFecundacionCommand(
    string? Query = null,
    DateTime? FechaDesde = null,
    DateTime? FechaHasta = null,
    string? Resultado = null,
    int Page = 1,
    int Limit = 20) : IAuditableRequest, ICacheableRequest
{
    public AuditEventType EventType => AuditEventType.Read;

    public string Action => "Listar fecundacion";

    public Task<string> GetCacheKeyAsync(ITenantConfigurationProvider tenantConfigurationProvider)
    {
        var page = Page <= 0 ? 1 : Page;
        var limit = Limit <= 0 ? 20 : Math.Min(Limit, 100);

        return Task.FromResult(FecundacionCacheKeys.Listar(Query, FechaDesde, FechaHasta, Resultado, page, limit));
    }
}