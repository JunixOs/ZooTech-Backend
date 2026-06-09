using System.Globalization;
using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ObtenerRegistroVacunoReporte;
using ZooTech.Application.Common.Gateway.Context;
using ZooTech.Application.Common.Gateway.Configuration;
using ZooTech.Infrastructure.Storage;

namespace ZooTech.Infrastructure.Reports;

public sealed class RegistroVacunoPdfReportService : IRegistroVacunoPdfReportService
{
    private readonly ReportStorageOptions _storageOptions;
    private readonly ISettingProvider _settingProvider;
    private readonly ITenantContext _tenantContext;

    public RegistroVacunoPdfReportService(
        IOptions<ReportStorageOptions> storageOptions,
        ISettingProvider settingProvider,
        ITenantContext tenantContext)
    {
        _storageOptions = storageOptions.Value;
        _settingProvider = settingProvider;
        _tenantContext = tenantContext;
    }
    public async Task<RegistroVacunoPdfReportResult> GenerateAsync(
        RegistroVacunoDetalle vacuno,
        CancellationToken cancellationToken = default)
    {
        var cultureStr = await _settingProvider.GetSettingAsync<string>("REPORTS_CULTURE_INFO", _tenantContext.TenantId);
        var culture = string.IsNullOrWhiteSpace(cultureStr) ? CultureInfo.InvariantCulture : new CultureInfo(cultureStr);

        var namePattern = await _settingProvider.GetSettingAsync<string>("REPORTS_NAME_PATTERN", _tenantContext.TenantId);
        if (string.IsNullOrWhiteSpace(namePattern)) namePattern = "reporte_{0}_{1}.pdf";

        var headersJson = await _settingProvider.GetSettingAsync<string>("REPORTS_EXCEL_HEADERS", _tenantContext.TenantId);
        var headers = string.IsNullOrWhiteSpace(headersJson)
            ? new Dictionary<string, string>()
            : JsonSerializer.Deserialize<Dictionary<string, string>>(headersJson) ?? new Dictionary<string, string>();

        var codigo = SanitizeFileNamePart(vacuno.Codigo);
        var fecha = DateTime.UtcNow.ToString("yyyyMMdd", culture);
        var fileName = string.Format(culture, namePattern, codigo, fecha);

        var basePath = await _settingProvider.GetSettingAsync<string>("REPORTS_BASE_PATH", _tenantContext.TenantId);
        if (string.IsNullOrWhiteSpace(basePath)) basePath = _storageOptions.ReportesBasePath;

        var vacunosPath = await _settingProvider.GetSettingAsync<string>("REPORTS_VACUNOS_PATH", _tenantContext.TenantId);
        if (string.IsNullOrWhiteSpace(vacunosPath)) vacunosPath = _storageOptions.ReportesVacunosPath;

        var urlBase = await _settingProvider.GetSettingAsync<string>("REPORTS_URL_BASE", _tenantContext.TenantId);
        if (string.IsNullOrWhiteSpace(urlBase)) urlBase = _storageOptions.ReportesUrlBase;

        var outputDirectory = Path.Combine(
            AppContext.BaseDirectory,
            basePath,
            vacunosPath);

        Directory.CreateDirectory(outputDirectory);

        var filePath = Path.Combine(outputDirectory, fileName);
        var image = TryLoadJpegImage(vacuno);
        var pdfBytes = BuildPdf(vacuno, image, culture, headers);

        await File.WriteAllBytesAsync(filePath, pdfBytes, cancellationToken);

        var downloadUrl = $"{urlBase}{Uri.EscapeDataString(fileName)}";
        return new RegistroVacunoPdfReportResult(fileName, downloadUrl);
    }

    private static byte[] BuildPdf(RegistroVacunoDetalle v, JpegImageInfo? image, CultureInfo culture, Dictionary<string, string> headers)
    {
        var lines = BuildLines(v, image is not null, culture, headers);
        var content = BuildPageContent(lines, image);
        var objects = new List<byte[]>();

        objects.Add(Ascii("<< /Type /Catalog /Pages 2 0 R >>"));
        objects.Add(Ascii("<< /Type /Pages /Kids [3 0 R] /Count 1 >>"));

        var pageResources = image is null
            ? "<< /Font << /F1 4 0 R >> >>"
            : "<< /Font << /F1 4 0 R >> /XObject << /Im1 6 0 R >> >>";

        objects.Add(Ascii($"<< /Type /Page /Parent 2 0 R /MediaBox [0 0 595 842] /Resources {pageResources} /Contents 5 0 R >>"));
        objects.Add(Ascii("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>"));
        objects.Add(StreamObject(Ascii(content)));

        if (image is not null)
        {
            var imageHeader = Ascii($"<< /Type /XObject /Subtype /Image /Width {image.Width} /Height {image.Height} /ColorSpace /DeviceRGB /BitsPerComponent 8 /Filter /DCTDecode /Length {image.Bytes.Length} >>\nstream\n");
            var imageFooter = Ascii("\nendstream");
            objects.Add(Concat(imageHeader, image.Bytes, imageFooter));
        }

        return BuildPdfFile(objects);
    }

    private static IReadOnlyList<PdfLine> BuildLines(RegistroVacunoDetalle v, bool hasImage, CultureInfo culture, Dictionary<string, string> headers)
    {
        string Header(string key, string defaultVal) => headers.TryGetValue(key, out var val) ? val : defaultVal;

        var lines = new List<PdfLine>
        {
            new("ZooTech | Modulo Vacuno", 18, true),
            new(Header("title", "Reporte de registro por vacuno"), 14, true),
            new($"{Header("gen_date", "Generado")}: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", culture)}", 9, false),
            new("", 8, false),
            new(Header("id_data", "Datos de Identificacion"), 12, true),
            Row(Header("id", "ID"), v.Id.ToString(culture)),
            Row(Header("code", "Codigo"), v.Codigo),
            Row(Header("name", "Nombre"), v.Nombre),
            Row(Header("birth_date", "Fecha de nacimiento"), FormatDate(v.FechaNacimiento, culture)),
            Row(Header("sex", "Sexo"), v.Sexo),
            Row(Header("breed", "Raza"), v.Raza),
            Row(Header("color", "Color"), v.Color),
            Row(Header("status", "Estado"), v.Estado),
            Row(Header("reg_date", "Fecha de registro"), FormatDate(v.FechaRegistro, culture)),
            new("", 8, false),
            new(Header("traceability", "Trazabilidad"), 12, true),
            Row(Header("sire_code", "Codigo padre"), v.CodigoPadre),
            Row(Header("dam_code", "Codigo madre"), v.CodigoMadre),
            Row(Header("gsire_code", "Codigo abuelo"), v.CodigoAbuelo),
            Row(Header("gdam_code", "Codigo abuela"), v.CodigoAbuela),
            Row(Header("farm", "Granja"), v.Granja),
            Row(Header("district", "Distrito"), v.Distrito),
            Row(Header("province", "Provincia"), v.Provincia),
            Row(Header("department", "Departamento"), v.Departamento),
            Row(Header("origin", "Procedencia"), v.Procedencia),
            Row(Header("acquired_by", "Adquisicion por"), v.AdquisicionPor),
            Row(Header("purchase_price", "Precio compra"), FormatDecimal(v.PrecioCompra, culture)),
            Row(Header("purchase_date", "Fecha adquisicion"), FormatDate(v.FechaAdquisicion, culture)),
            new("", 8, false),
            new(Header("specialization", "Especializacion"), 12, true),
            Row(Header("apt_for", "Apto para"), v.AptoPara),
            Row(Header("spec_date", "Fecha especificacion"), FormatDate(v.FechaEspecificacion, culture)),
            new("", 8, false),
            new(Header("observations_title", "Observaciones"), 12, true),
            Row(Header("observations", "Observaciones"), v.Observaciones),
            Row(Header("status_reason", "Motivo estado"), v.MotivoEstado),
            Row("Foto", hasImage ? "Incluida en el reporte" : "Sin foto disponible o formato no compatible"),
            new("", 8, false),
            new(Header("audit", "Auditoria"), 12, true),
            Row(Header("created_by", "Creado por"), v.CreadoPor),
            Row(Header("created_at", "Creado en"), FormatDateTime(v.CreadoEn, culture)),
            Row(Header("updated_by", "Actualizado por"), v.ActualizadoPor),
            Row(Header("updated_at", "Actualizado en"), FormatDateTime(v.ActualizadoEn, culture))
        };

        return lines;
    }

    private static PdfLine Row(string label, string? value) => new($"{label}: {value ?? string.Empty}", 9, false);

    private static string BuildPageContent(IReadOnlyList<PdfLine> lines, JpegImageInfo? image)
    {
        var sb = new StringBuilder();

        if (image is not null)
        {
            var box = FitImage(image.Width, image.Height, 130, 110);
            sb.Append(CultureInfo.InvariantCulture, $"q {box.Width:0.##} 0 0 {box.Height:0.##} 420 690 cm /Im1 Do Q\n");
        }

        var y = 800;
        foreach (var line in lines)
        {
            if (y < 42)
            {
                break;
            }

            if (string.IsNullOrEmpty(line.Text))
            {
                y -= line.FontSize + 4;
                continue;
            }

            var x = line.IsSection ? 50 : 60;
            var safeText = EscapePdfText(TrimToLength(line.Text, line.IsSection ? 80 : 105));
            sb.Append(CultureInfo.InvariantCulture, $"BT /F1 {line.FontSize} Tf {x} {y} Td ({safeText}) Tj ET\n");
            y -= line.IsSection ? 18 : 14;
        }

        sb.Append("BT /F1 8 Tf 50 28 Td (Documento generado automaticamente por ZooTech) Tj ET\n");
        return sb.ToString();
    }

    private static byte[] BuildPdfFile(IReadOnlyList<byte[]> objectBodies)
    {
        using var ms = new MemoryStream();
        WriteAscii(ms, "%PDF-1.4\n%\u00E2\u00E3\u00CF\u00D3\n");

        var offsets = new List<long> { 0 };

        for (var i = 0; i < objectBodies.Count; i++)
        {
            offsets.Add(ms.Position);
            WriteAscii(ms, $"{i + 1} 0 obj\n");
            ms.Write(objectBodies[i]);
            WriteAscii(ms, "\nendobj\n");
        }

        var xrefPosition = ms.Position;
        WriteAscii(ms, $"xref\n0 {objectBodies.Count + 1}\n");
        WriteAscii(ms, "0000000000 65535 f \n");
        foreach (var offset in offsets.Skip(1))
        {
            WriteAscii(ms, $"{offset:0000000000} 00000 n \n");
        }

        WriteAscii(ms, $"trailer\n<< /Size {objectBodies.Count + 1} /Root 1 0 R >>\nstartxref\n{xrefPosition}\n%%EOF");
        return ms.ToArray();
    }

    private static byte[] StreamObject(byte[] streamBytes)
    {
        var header = Ascii($"<< /Length {streamBytes.Length} >>\nstream\n");
        var footer = Ascii("\nendstream");
        return Concat(header, streamBytes, footer);
    }

    private static JpegImageInfo? TryLoadJpegImage(RegistroVacunoDetalle vacuno)
    {
        foreach (var candidate in GetImagePathCandidates(vacuno))
        {
            if (!File.Exists(candidate))
            {
                continue;
            }

            var extension = Path.GetExtension(candidate).ToLowerInvariant();
            if (extension is not ".jpg" and not ".jpeg")
            {
                continue;
            }

            try
            {
                var bytes = File.ReadAllBytes(candidate);
                var dimensions = TryGetJpegDimensions(bytes);
                if (dimensions is null)
                {
                    continue;
                }

                return new JpegImageInfo(bytes, dimensions.Value.Width, dimensions.Value.Height);
            }
            catch
            {
                // La foto nunca debe interrumpir la generacion del reporte.
            }
        }

        return null;
    }

    private static IEnumerable<string> GetImagePathCandidates(RegistroVacunoDetalle vacuno)
    {
        var values = new[]
        {
            vacuno.FotoUrl,
            vacuno.FotoRuta,
            vacuno.FotoNombreAlmacenado,
            vacuno.FotoNombreOriginal
        };

        var baseDirectory = AppContext.BaseDirectory;
        var wwwroot = Path.Combine(baseDirectory, "wwwroot");

        foreach (var value in values)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                continue;
            }

            if (Uri.TryCreate(value, UriKind.Absolute, out var uri) && uri.IsFile)
            {
                yield return uri.LocalPath;
                continue;
            }

            if (Uri.TryCreate(value, UriKind.Absolute, out uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            {
                continue;
            }

            if (Path.IsPathRooted(value))
            {
                yield return value;
                continue;
            }

            var relative = value.TrimStart('/', '\\');
            yield return Path.Combine(wwwroot, relative);
            yield return Path.Combine(baseDirectory, relative);
        }
    }

    private static (int Width, int Height)? TryGetJpegDimensions(byte[] bytes)
    {
        if (bytes.Length < 4 || bytes[0] != 0xFF || bytes[1] != 0xD8)
        {
            return null;
        }

        var index = 2;
        while (index + 9 < bytes.Length)
        {
            if (bytes[index] != 0xFF)
            {
                index++;
                continue;
            }

            var marker = bytes[index + 1];
            index += 2;

            if (marker == 0xD9 || marker == 0xDA)
            {
                break;
            }

            if (index + 2 > bytes.Length)
            {
                break;
            }

            var length = ReadBigEndianUInt16(bytes, index);
            if (length < 2 || index + length > bytes.Length)
            {
                break;
            }

            if (marker is 0xC0 or 0xC1 or 0xC2 or 0xC3 or 0xC5 or 0xC6 or 0xC7 or 0xC9 or 0xCA or 0xCB or 0xCD or 0xCE or 0xCF)
            {
                var height = ReadBigEndianUInt16(bytes, index + 3);
                var width = ReadBigEndianUInt16(bytes, index + 5);
                return (width, height);
            }

            index += length;
        }

        return null;
    }

    private static int ReadBigEndianUInt16(byte[] bytes, int index) => (bytes[index] << 8) + bytes[index + 1];

    private static (double Width, double Height) FitImage(int width, int height, double maxWidth, double maxHeight)
    {
        var ratio = Math.Min(maxWidth / width, maxHeight / height);
        return (width * ratio, height * ratio);
    }

    private static string SanitizeFileNamePart(string value)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var safe = new string(value
            .Where(ch => !invalidChars.Contains(ch) && !char.IsWhiteSpace(ch))
            .ToArray());

        return string.IsNullOrWhiteSpace(safe) ? "vacuno" : safe;
    }

    private static string TrimToLength(string value, int maxLength) =>
        value.Length <= maxLength ? value : value[..Math.Max(0, maxLength - 3)] + "...";

    private static string EscapePdfText(string value) =>
        value.Replace("\\", "\\\\", StringComparison.Ordinal)
             .Replace("(", "\\(", StringComparison.Ordinal)
             .Replace(")", "\\)", StringComparison.Ordinal)
             .Replace("\r", " ", StringComparison.Ordinal)
             .Replace("\n", " ", StringComparison.Ordinal);

    private static string FormatDate(DateOnly? date, CultureInfo culture) => date?.ToString("yyyy-MM-dd", culture) ?? string.Empty;

    private static string FormatDecimal(decimal? value, CultureInfo culture) => value?.ToString("0.##", culture) ?? string.Empty;

    private static string FormatDateTime(DateTime? value, CultureInfo culture) => value?.ToString("yyyy-MM-dd HH:mm:ss", culture) ?? string.Empty;

    private static byte[] Ascii(string value) => Encoding.ASCII.GetBytes(WebUtility.HtmlDecode(value));

    private static byte[] Concat(params byte[][] arrays)
    {
        var length = arrays.Sum(a => a.Length);
        var result = new byte[length];
        var offset = 0;

        foreach (var array in arrays)
        {
            Buffer.BlockCopy(array, 0, result, offset, array.Length);
            offset += array.Length;
        }

        return result;
    }

    private static void WriteAscii(Stream stream, string value)
    {
        var bytes = Encoding.ASCII.GetBytes(value);
        stream.Write(bytes, 0, bytes.Length);
    }

    private sealed record PdfLine(string Text, int FontSize, bool IsSection);

    private sealed record JpegImageInfo(byte[] Bytes, int Width, int Height);
}
