using Microsoft.Extensions.Logging;
using System.Text.Json;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Application.Common.Gateway.Context;
using ZooTech.Application.Common.Gateway.Configuration;
using ZooTech.Application.Modules.Module_ReporteVacuno.Common;

namespace ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ListarReportesDisponibles;

public sealed class ListarReportesDisponiblesUseCase : IListarReportesDisponiblesUseCase
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ISettingProvider _settingProvider;
    private readonly ITenantContext _tenantContext;
    private readonly ILogger<ListarReportesDisponiblesUseCase> _logger;

    public ListarReportesDisponiblesUseCase(
        IDateTimeProvider dateTimeProvider,
        ISettingProvider settingProvider,
        ITenantContext tenantContext,
        ILogger<ListarReportesDisponiblesUseCase> logger)
    {
        _dateTimeProvider = dateTimeProvider;
        _settingProvider = settingProvider;
        _tenantContext = tenantContext;
        _logger = logger;
    }

    public async Task<ReportesDisponiblesResponse> HandleAsync(ListarReportesDisponiblesQuery query, CancellationToken cancellationToken = default)
    {
        var defaultDays = await _settingProvider.GetSettingAsync<int>("REPORTS_DEFAULT_DAYS", _tenantContext.TenantId);
        var dateFormat = await _settingProvider.GetSettingAsync<string>("REPORTS_DATE_FORMAT", _tenantContext.TenantId);

        var rango = ReporteVacunoDateRangeResolver.Resolve(
            query.FechaDesde,
            query.FechaHasta,
            _dateTimeProvider.Today,
            defaultDays,
            dateFormat);

        _logger.LogInformation(
            "[ReporteVacuno] rango aplicado: [{FechaDesde} - {FechaHasta}]",
            rango.FechaDesde,
            rango.FechaHasta);

        var catalogJson = await _settingProvider.GetSettingAsync<string>("REPORTS_AVAILABLE_CATALOG", _tenantContext.TenantId);
        var catalog = string.IsNullOrWhiteSpace(catalogJson)
            ? []
            : JsonSerializer.Deserialize<ReporteDisponibleItem[]>(catalogJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? [];

        return new ReportesDisponiblesResponse(
            catalog,
            new ReportesDisponiblesFiltros(
                rango.FechaDesde,
                rango.FechaHasta,
                Normalize(query.Q)));
    }

    private static string? Normalize(string? value)
    {
        var normalized = value?.Trim();
        return string.IsNullOrWhiteSpace(normalized) ? null : normalized;
    }
}
