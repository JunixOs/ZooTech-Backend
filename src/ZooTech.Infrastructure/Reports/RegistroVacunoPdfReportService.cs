using System.Globalization;
using System.Net;
using System.Text;
using Microsoft.Extensions.Options;
using ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ObtenerRegistroVacunoReporte;
using ZooTech.Infrastructure.Storage;

namespace ZooTech.Infrastructure.Reports;

public sealed class RegistroVacunoPdfReportService : IRegistroVacunoPdfReportService
{
    private readonly ReportStorageOptions _storageOptions;

    public RegistroVacunoPdfReportService(IOptions<ReportStorageOptions> storageOptions)
    {
        _storageOptions = storageOptions.Value;
    }
    public async Task<RegistroVacunoPdfReportResult> GenerateAsync(
        RegistroVacunoDetalle vacuno,
        CancellationToken cancellationToken = default)
    {
        var codigo = SanitizeFileNamePart(vacuno.Codigo);
        var fecha = DateTime.UtcNow.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
        var fileName = $"reporte_{codigo}_{fecha}.pdf";

        var outputDirectory = Path.Combine(
            AppContext.BaseDirectory,
            _storageOptions.ReportesBasePath,
            _storageOptions.ReportesVacunosPath);

        Directory.CreateDirectory(outputDirectory);

        var filePath = Path.Combine(outputDirectory, fileName);
        var image = TryLoadJpegImage(vacuno);
        var pdfBytes = BuildPdf(vacuno, image);

        await File.WriteAllBytesAsync(filePath, pdfBytes, cancellationToken);

        var downloadUrl = $"{_storageOptions.ReportesUrlBase}{Uri.EscapeDataString(fileName)}";
        return new RegistroVacunoPdfReportResult(fileName, downloadUrl);
    }

    private static byte[] BuildPdf(RegistroVacunoDetalle v, JpegImageInfo? image)
    {
        var lines = BuildLines(v, image is not null);
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

    private static IReadOnlyList<PdfLine> BuildLines(RegistroVacunoDetalle v, bool hasImage)
    {
        var lines = new List<PdfLine>
        {
            new("ZooTech | Modulo Vacuno", 18, true),
            new("Reporte de registro por vacuno", 14, true),
            new($"Generado: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)}", 9, false),
            new("", 8, false),
            new("Datos de Identificacion", 12, true),
            Row("ID", v.Id.ToString(CultureInfo.InvariantCulture)),
            Row("Codigo", v.Codigo),
            Row("Nombre", v.Nombre),
            Row("Fecha de nacimiento", FormatDate(v.FechaNacimiento)),
            Row("Sexo", v.Sexo),
            Row("Raza", v.Raza),
            Row("Color", v.Color),
            Row("Estado", v.Estado),
            Row("Fecha de registro", FormatDate(v.FechaRegistro)),
            new("", 8, false),
            new("Trazabilidad", 12, true),
            Row("Codigo padre", v.CodigoPadre),
            Row("Codigo madre", v.CodigoMadre),
            Row("Codigo abuelo", v.CodigoAbuelo),
            Row("Codigo abuela", v.CodigoAbuela),
            Row("Granja", v.Granja),
            Row("Distrito", v.Distrito),
            Row("Provincia", v.Provincia),
            Row("Departamento", v.Departamento),
            Row("Procedencia", v.Procedencia),
            Row("Adquisicion por", v.AdquisicionPor),
            Row("Precio compra", FormatDecimal(v.PrecioCompra)),
            Row("Fecha adquisicion", FormatDate(v.FechaAdquisicion)),
            new("", 8, false),
            new("Especializacion", 12, true),
            Row("Apto para", v.AptoPara),
            Row("Fecha especificacion", FormatDate(v.FechaEspecificacion)),
            new("", 8, false),
            new("Observaciones", 12, true),
            Row("Observaciones", v.Observaciones),
            Row("Motivo estado", v.MotivoEstado),
            Row("Foto", hasImage ? "Incluida en el reporte" : "Sin foto disponible o formato no compatible"),
            new("", 8, false),
            new("Auditoria", 12, true),
            Row("Creado por", v.CreadoPor),
            Row("Creado en", FormatDateTime(v.CreadoEn)),
            Row("Actualizado por", v.ActualizadoPor),
            Row("Actualizado en", FormatDateTime(v.ActualizadoEn))
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

    private static string FormatDate(DateOnly? date) => date?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? string.Empty;

    private static string FormatDecimal(decimal? value) => value?.ToString("0.##", CultureInfo.InvariantCulture) ?? string.Empty;

    private static string FormatDateTime(DateTime? value) => value?.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) ?? string.Empty;

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
