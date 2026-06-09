using System.Globalization;
using Microsoft.Extensions.Logging;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Application.Common.Gateway.Context;
using ZooTech.Application.Common.Gateway.Configuration;
using ZooTech.Application.Modules.Module_ReporteVacuno.Common;

namespace ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ListarReporteVacunos;

public sealed class ListarReporteVacunosUseCase : IListarReporteVacunosUseCase
{

    private readonly IReporteVacunoReadRepository _repository;
    private readonly IListadoVacunosReportFileService _reportFileService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ISettingProvider _settingProvider;
    private readonly ITenantContext _tenantContext;
    private readonly ILogger<ListarReporteVacunosUseCase> _logger;

    public ListarReporteVacunosUseCase(
        IReporteVacunoReadRepository repository,
        IListadoVacunosReportFileService reportFileService,
        IDateTimeProvider dateTimeProvider,
        ISettingProvider settingProvider,
        ITenantContext tenantContext,
        ILogger<ListarReporteVacunosUseCase> logger)
    {
        _repository = repository;
        _reportFileService = reportFileService;
        _dateTimeProvider = dateTimeProvider;
        _settingProvider = settingProvider;
        _tenantContext = tenantContext;
        _logger = logger;
    }

    public async Task<ListadoVacunosReporteResponse> HandleAsync(
        ListarReporteVacunosQuery query,
        CancellationToken cancellationToken = default)
    {
        var defaultDays = await _settingProvider.GetSettingAsync<int>("REPORTS_DEFAULT_DAYS", _tenantContext.TenantId);
        var dateFormat = await _settingProvider.GetSettingAsync<string>("REPORTS_DATE_FORMAT", _tenantContext.TenantId);
        var allowedFormatsRaw = await _settingProvider.GetSettingAsync<string[]>("REPORTS_ALLOWED_FORMATS", _tenantContext.TenantId);
        var allowedFormats = allowedFormatsRaw is { Length: > 0 } ? allowedFormatsRaw : ["json", "pdf", "excel"];

        var formato = Normalize(query.Formato) ?? "json";
        EnsureAllowed("formato", formato, allowedFormats, "INVALID_REPORT_FORMAT", "El formato debe ser json, pdf o excel.", "Formato no permitido.");

        var rango = ReporteVacunoDateRangeResolver.Resolve(
            query.FechaDesde,
            query.FechaHasta,
            _dateTimeProvider.Today,
            defaultDays,
            dateFormat);

        var page = ParsePositiveIntOrDefault(query.Page, "page", 1);
        var limit = ParsePositiveIntOrDefault(query.Limit, "limit", 20);
        limit = Math.Min(limit, 100);

        var estado = Normalize(query.Estado);
        if (estado is not null)
        {
            var allowedEstados = new[] { "vivo", "muerto" };
            EnsureAllowed("estado", estado, allowedEstados, "VALIDATION_ERROR", "Los datos enviados no son validos.", "El estado debe ser vivo o muerto.");
        }

        var aptoPara = Normalize(query.AptoPara);
        if (aptoPara is not null)
        {
            var allowedAptos = new[] { "reproduccion", "produccion_leche", "produccion_carne", "venta", "descarte" };
            EnsureAllowed("aptoPara", aptoPara, allowedAptos, "VALIDATION_ERROR", "Los datos enviados no son validos.", "Valor de aptoPara no permitido.");
        }

        _logger.LogInformation(
            "[ReporteVacuno] rango aplicado: [{FechaDesde} - {FechaHasta}]",
            rango.FechaDesde,
            rango.FechaHasta);

        var criteria = new ReporteVacunoListadoCriteria(
            rango.FechaDesde,
            rango.FechaHasta,
            Normalize(query.Q),
            Normalize(query.Raza),
            Normalize(query.Procedencia),
            estado,
            aptoPara,
            page,
            limit);

        var pageResult = await _repository.ListarAsync(criteria, cancellationToken);

        var resumen = new ReporteVacunoResumen(pageResult.TotalRegistros);
        var filtros = new ReporteVacunoFiltros(
            rango.FechaDesde,
            rango.FechaHasta,
            criteria.Q,
            criteria.Raza,
            criteria.Procedencia,
            criteria.Estado,
            criteria.AptoPara,
            formato);

        var downloadUrl = formato switch
        {
            "excel" => (await _reportFileService.GenerateExcelAsync(pageResult.Items, resumen, filtros, cancellationToken)).DownloadUrl,
            "pdf" => (await _reportFileService.GeneratePdfAsync(pageResult.Items, resumen, filtros, cancellationToken)).DownloadUrl,
            _ => null
        };

        return new ListadoVacunosReporteResponse(
            pageResult.Items,
            resumen,
            filtros,
            downloadUrl);
    }

    private static string? Normalize(string? value)
    {
        var normalized = value?.Trim();
        return string.IsNullOrWhiteSpace(normalized) ? null : normalized.ToLowerInvariant();
    }

    private static int ParsePositiveIntOrDefault(string? value, string field, int defaultValue)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return defaultValue;
        }

        if (!int.TryParse(value.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out var parsed))
        {
            throw new ApplicationRuleException(
                "VALIDATION_ERROR",
                "Los datos enviados no son validos.",
                [new ApplicationErrorDetail(field, $"El parametro {field} debe ser numerico.")]);
        }

        return parsed < 1 ? defaultValue : parsed;
    }

    private static void EnsureAllowed(
        string field,
        string value,
        IReadOnlyCollection<string> allowed,
        string code,
        string message,
        string detail)
    {
        if (allowed.Contains(value))
        {
            return;
        }

        throw new ApplicationRuleException(
            code,
            message,
            [new ApplicationErrorDetail(field, detail)]);
    }
}
