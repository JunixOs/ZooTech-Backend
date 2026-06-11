using ZooTech.Application.Common.Exceptions;



namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ObtenerRegistroVacunoReporte;

public sealed class ObtenerRegistroVacunoReporteUseCase : IObtenerRegistroVacunoReporteUseCase
{
    private readonly IRegistroVacunoReadRepository _repository;
    private readonly IRegistroVacunoExcelReportService _excelReportService;
    private readonly IRegistroVacunoPdfReportService _pdfReportService;
    public ObtenerRegistroVacunoReporteUseCase(
        IRegistroVacunoReadRepository repository,
        IRegistroVacunoExcelReportService excelReportService,
        IRegistroVacunoPdfReportService pdfReportService)
    {
        _repository = repository;
        _excelReportService = excelReportService;
        _pdfReportService = pdfReportService;
    }

    public async Task<RegistroVacunoReporteResponse> HandleAsync(
        ObtenerRegistroVacunoReporteQuery query,
        CancellationToken cancellationToken = default)
    {
        if (query.VacunoId <= 0)
        {
            throw new ArgumentException("El ID del vacuno debe ser mayor que cero.");
        }

        string[] allowedFormats = ["json", "pdf", "excel"];

        var formato = Normalize(query.Formato) ?? "json";
        EnsureFormatoValido(formato, allowedFormats);

        var detalle = await _repository.ObtenerRegistroAsync(query.VacunoId, cancellationToken);

        if (detalle is null)
        {
            throw new ZooTech.Application.Common.Exceptions.NotFoundException("No existe un vacuno con el ID enviado.");
        }

        if (formato is "excel")
        {
            var excel = await _excelReportService.GenerateAsync(detalle, cancellationToken);

            return new RegistroVacunoReporteResponse(
                detalle,
                Array.Empty<object>(),
                excel.DownloadUrl);
        }

        if (formato is "pdf")
        {
            var pdf = await _pdfReportService.GenerateAsync(detalle, cancellationToken);

            return new RegistroVacunoReporteResponse(
                detalle,
                Array.Empty<object>(),
                pdf.DownloadUrl);
        }

        return new RegistroVacunoReporteResponse(
            detalle,
            Array.Empty<object>(),
            null);
    }

    private static string? Normalize(string? value)
    {
        var normalized = value?.Trim();
        return string.IsNullOrWhiteSpace(normalized) ? null : normalized.ToLowerInvariant();
    }

    private static void EnsureFormatoValido(string formato, string[] allowedFormats)
    {
        if (!allowedFormats.Contains(formato, StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException($"El formato debe ser uno de los permitidos: {string.Join(", ", allowedFormats)}.");
        }
    }
}

