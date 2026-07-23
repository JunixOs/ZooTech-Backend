using ZooTech.Application.Common.Caching;
using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Application.Common.Gateway.Parametrization;
using ZooTech.Application.Modules.Module_Vacuno.Common;
using ZooTech.Domain.Configuration;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunos;

public record ListarVacunosCommand(
    string? Query = null,
    DateTime? FechaDesde = null,
    DateTime? FechaHasta = null,
    string? Estado = null,
    int Page = 1,
    int Limit = 20) : IAuditableRequest, ICacheableRequest
{
    public AuditEventType EventType => AuditEventType.Read;

    public string Action => "Listar vacunos";

    public async Task<string> GetCacheKeyAsync(ITenantConfigurationProvider tenantConfigurationProvider)
    {
        var fechaDesde = FechaDesde;
        if (!fechaDesde.HasValue && !FechaHasta.HasValue)
        {
            var defaultFilterDays = await tenantConfigurationProvider.GetSettingAsync(
                Settings.Vacunos.VacunosDefaultFilterDays
            );
            fechaDesde = DateTime.UtcNow.AddDays(-defaultFilterDays);
        }
        var page = Page <= 0 ? 1 : Page;
        var limit = Limit <= 0 ? 20 : Math.Min(Limit, 100);

        return VacunoCacheKeys.Listar(Query, fechaDesde, FechaHasta, Estado, page, limit);
    }
}