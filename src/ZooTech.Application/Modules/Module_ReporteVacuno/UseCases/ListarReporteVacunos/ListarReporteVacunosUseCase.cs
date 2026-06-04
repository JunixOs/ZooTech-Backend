using System.Globalization;
using Microsoft.Extensions.Logging;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Application.Modules.Module_ReporteVacuno.Common;

namespace ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ListarReporteVacunos;

public sealed class ListarReporteVacunosUseCase : IListarReporteVacunosUseCase
{
    private static readonly string[] FormatosPermitidos = ["json", "pdf", "excel"];
    private static readonly string[] EstadosPermitidos = ["vivo", "muerto"];
    private static readonly string[] AptosPermitidos = ["produccion_leche", "carne", "reproduccion"];

    private readonly IReporteVacunoReadRepository _repository;
    private readonly IListadoVacunosReportFileService _reportFileService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<ListarReporteVacunosUseCase> _logger;

    public ListarReporteVacunosUseCase(
        IReporteVacunoReadRepository repository,
        IListadoVacunosReportFileService reportFileService,
        IDateTimeProvider dateTimeProvider,
        ILogger<ListarReporteVacunosUseCase> logger)
    {
        _repository = repository;
        _reportFileService = reportFileService;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    public async Task<ListadoVacunosReporteResponse> HandleAsync(
        ListarReporteVacunosQuery query,
        CancellationToken cancellationToken = default)
    {
        var formato = Normalize(query.Formato) ?? "json";
        EnsureAllowed("formato", formato, FormatosPermitidos, "INVALID_REPORT_FORMAT", "El formato debe ser json, pdf o excel.", "Formato no permitido.");

        var rango = ReporteVacunoDateRangeResolver.Resolve(
            query.FechaDesde,
            query.FechaHasta,
            _dateTimeProvider.Today);

        var page = ParsePositiveIntOrDefault(query.Page, "page", 1);
        var limit = ParsePositiveIntOrDefault(query.Limit, "limit", 20);
        limit = Math.Min(limit, 100);

        var estado = Normalize(query.Estado);
        if (estado is not null)
        {
            EnsureAllowed("estado", estado, EstadosPermitidos, "VALIDATION_ERROR", "Los datos enviados no son validos.", "El estado debe ser vivo o muerto.");
        }

        var aptoPara = Normalize(query.AptoPara);
        if (aptoPara is not null)
        {
            EnsureAllowed("aptoPara", aptoPara, AptosPermitidos, "VALIDATION_ERROR", "Los datos enviados no son validos.", "Valor de aptoPara no permitido.");
        }

        _logger.LogInformation(
            "[ReporteVacuno] rango aplicado: [{FechaDesde} - {FechaHasta}]",
            rango.FechaDesde,
            rango.FechaHasta);

        var criteria = new ReporteVacunoListadoCriteria(
            rango.FechaDesde,
            rango.FechaHasta,
            Normalize(query.Q),
            Normalize(query.Codigo),
            Normalize(query.FechaRegistro),
            Normalize(query.Nombre),
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
            criteria.Codigo,
            criteria.FechaRegistro,
            criteria.Nombre,
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
