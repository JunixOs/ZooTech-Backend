using ZooTech.Application.Common.Exceptions;
using ZooTech.Domain.Module_Vacuno.Interfaces;

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

        var domainEntity = await _repository.ObtenerRegistroAsync(query.VacunoId, cancellationToken);

        if (domainEntity is null)
        {
            throw new NotFoundException("No existe un vacuno con el ID enviado.");
        }

        var v = domainEntity;

        var razaCatalog = await _repository.ObtenerNombresCatalogoBatchAsync("raza", [v.RazaCode], cancellationToken);
        var sexoCatalog = await _repository.ObtenerNombresCatalogoBatchAsync("sexo", [v.SexoCode], cancellationToken);
        var colorCatalog = await _repository.ObtenerNombresCatalogoBatchAsync("color", [v.ColorCode], cancellationToken);
        var adquisicionCatalog = await _repository.ObtenerNombresCatalogoBatchAsync("tipoadquisicion", [v.TipoAdquisicionCode], cancellationToken);
        var codigosParentesco = await _repository.ObtenerCodigosVacunoBatchAsync(
            new[] { v.PadreId, v.MadreId }.Where(id => id.HasValue).Select(id => id!.Value),
            cancellationToken);
        var raza = GetCatalogValue(razaCatalog, v.RazaCode);
        var sexo = GetCatalogValue(sexoCatalog, v.SexoCode);
        var color = GetCatalogValue(colorCatalog, v.ColorCode);
        var adquisicionTipo = GetCatalogValue(adquisicionCatalog, v.TipoAdquisicionCode);
        var padre = v.PadreId.HasValue && codigosParentesco.TryGetValue(v.PadreId.Value, out var padreCodigo) ? padreCodigo : null;
        var madre = v.MadreId.HasValue && codigosParentesco.TryGetValue(v.MadreId.Value, out var madreCodigo) ? madreCodigo : null;
        var granja = await _repository.ObtenerDetallesGranjaAsync(v.GranjaId, cancellationToken);
        var adq = await _repository.ObtenerDetallesAdquisicionAsync(v.Id, cancellationToken);
        var est = await _repository.ObtenerEstadoActualAsync(v.Id, cancellationToken);
        var uti = await _repository.ObtenerUtilizacionActualAsync(v.Id, cancellationToken);
        var foto = await _repository.ObtenerFotoPrincipalAsync(v.Id, cancellationToken);
        var creadoPor = v.CreatedBy.HasValue ? await _repository.ObtenerNombreUsuarioAsync(v.CreatedBy, cancellationToken) : null;
        var actualizadoPor = v.UpdatedBy.HasValue ? await _repository.ObtenerNombreUsuarioAsync(v.UpdatedBy, cancellationToken) : null;
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var diasRegistrado = CalculateDaysRegistered(v.FechaRegistro, today);


        string? NormalizeCatalogValue(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToLowerInvariant().Replace(' ', '_');

        string DeterminarEstado(string? estadoCode, string? estadoNombre)
        {
            var normalizedCode = estadoCode?.Trim().ToUpperInvariant();
            if (normalizedCode is "SANO" or "ENFERMO" or "CUARENTENA" or "MUERTO")
            {
                return normalizedCode;
            }

            var normalizedName = estadoNombre?.Trim().ToUpperInvariant();
            return normalizedName switch
            {
                "SANO" => "SANO",
                "ENFERMO" => "ENFERMO",
                "CUARENTENA" => "CUARENTENA",
                "MUERTO" => "MUERTO",
                _ => "SANO"
            };
        }

        var detalle = new RegistroVacunoDetalle(
            v.Id,
            v.Codigo,
            v.Nombre,
            v.FechaNacimiento,
            NormalizeCatalogValue(adquisicionTipo),
            adq?.PrecioCompra,
            raza,
            color,
            NormalizeCatalogValue(sexo),
            padre,
            madre,
            null, // CodigoAbuelo not implemented recursively here to keep simple
            null, // CodigoAbuela
            granja?.Nombre,
            granja?.Distrito,
            granja?.Departamento,
            granja?.Provincia,
            adq?.Proveedor,
            NormalizeCatalogValue(uti),
            adq?.FechaAdquisicion,
            v.Observaciones,
            foto?.Id,
            foto?.NombreOriginal,
            foto?.NombreAlmacenado,
            foto?.RutaArchivo,
            foto?.RutaArchivo, // url same as ruta
            foto?.Extension,
            foto?.TamanoBytes,
            est?.EstadoCode,
            est?.EstadoNombre,
            DeterminarEstado(est?.EstadoCode, est?.EstadoNombre),
            est?.FechaEstado,
            est?.Motivo,
            v.FechaRegistro,
            diasRegistrado,
            adq?.FechaAdquisicion,
            creadoPor,
            v.CreatedAt,
            actualizadoPor,
            v.UpdatedAt);

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

    private static string? GetCatalogValue(Dictionary<string, string> catalog, string code)
        => catalog.TryGetValue(code, out var value) ? value : null;

    private static int CalculateDaysRegistered(DateOnly fechaRegistro, DateOnly referenceDate)
    {
        return Math.Max(referenceDate.DayNumber - fechaRegistro.DayNumber, 0);
    }
}
