using System.Globalization;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ObtenerRegistroVacunoReporte;
using ZooTech.Infrastructure.Storage;

namespace ZooTech.Infrastructure.Reports;

public sealed class RegistroVacunoExcelReportService : IRegistroVacunoExcelReportService
{
    private readonly ReportStorageOptions _storageOptions;

    public RegistroVacunoExcelReportService(
        IOptions<ReportStorageOptions> storageOptions)
    {
        _storageOptions = storageOptions.Value;
    }

    public async Task<RegistroVacunoExcelReportResult> GenerateAsync(
        RegistroVacunoDetalle vacuno,
        CancellationToken cancellationToken = default)
    {
        var culture = CultureInfo.InvariantCulture;

        var namePattern = "reporte_{0}_{1}.xlsx";

        var headers = new Dictionary<string, string>();

        var codigo = SanitizeFileNamePart(vacuno.Codigo);
        var fecha = DateTime.UtcNow.ToString("yyyyMMdd", culture);
        var fileName = string.Format(culture, namePattern, codigo, fecha);

        var basePath = _storageOptions.ReportesBasePath;

        var vacunosPath = _storageOptions.ReportesVacunosPath;

        var urlBase = _storageOptions.ReportesUrlBase;

        var outputDirectory = Path.Combine(
            AppContext.BaseDirectory,
            basePath,
            vacunosPath);

        Directory.CreateDirectory(outputDirectory);

        var filePath = Path.Combine(outputDirectory, fileName);
        var sheetXml = BuildWorksheetXml(vacuno, culture, headers);

        await using var fileStream = new FileStream(
            filePath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 81920,
            useAsync: true);

        using (var archive = new ZipArchive(fileStream, ZipArchiveMode.Create, leaveOpen: true))
        {
            await AddEntryAsync(archive, "[Content_Types].xml", ContentTypesXml, cancellationToken);
            await AddEntryAsync(archive, "_rels/.rels", RootRelationshipsXml, cancellationToken);
            await AddEntryAsync(archive, "xl/workbook.xml", WorkbookXml, cancellationToken);
            await AddEntryAsync(archive, "xl/_rels/workbook.xml.rels", WorkbookRelationshipsXml, cancellationToken);
            await AddEntryAsync(archive, "xl/worksheets/sheet1.xml", sheetXml, cancellationToken);
        }

        var downloadUrl = $"{urlBase}{Uri.EscapeDataString(fileName)}";
        return new RegistroVacunoExcelReportResult(fileName, downloadUrl);
    }

    private static IReadOnlyList<string?> Row(params string?[] values) => values;

    private static string BuildWorksheetXml(RegistroVacunoDetalle v, CultureInfo culture, Dictionary<string, string> headers)
    {
        string Header(string key, string defaultVal) => headers.TryGetValue(key, out var val) ? val : defaultVal;

        var rows = new List<IReadOnlyList<string?>>
        {
            Row(Header("title", "Reporte de registro por vacuno"), null),
            Row(Header("gen_date", "Fecha de generación"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", culture)),
            Row(null, null),
            Row(Header("id_data", "Datos de Identificación"), null),
            Row(Header("id", "ID"), v.Id.ToString(culture)),
            Row(Header("code", "Código"), v.Codigo),
            Row(Header("name", "Nombre"), v.Nombre),
            Row(Header("birth_date", "Fecha de nacimiento"), FormatDate(v.FechaNacimiento, culture)),
            Row(Header("sex", "Sexo"), v.Sexo),
            Row(Header("breed", "Raza"), v.Raza),
            Row(Header("color", "Color"), v.Color),
            Row(Header("status", "Estado"), v.Estado),
            Row(Header("reg_date", "Fecha de registro"), FormatDate(v.FechaRegistro, culture)),
            Row(null, null),
            Row(Header("traceability", "Trazabilidad"), null),
            Row(Header("sire_code", "Código padre"), v.CodigoPadre),
            Row(Header("dam_code", "Código madre"), v.CodigoMadre),
            Row(Header("gsire_code", "Código abuelo"), v.CodigoAbuelo),
            Row(Header("gdam_code", "Código abuela"), v.CodigoAbuela),
            Row(Header("farm", "Granja"), v.Granja),
            Row(Header("district", "Distrito"), v.Distrito),
            Row(Header("province", "Provincia"), v.Provincia),
            Row(Header("department", "Departamento"), v.Departamento),
            Row(Header("origin", "Procedencia"), v.Procedencia),
            Row(Header("acquired_by", "Adquisición por"), v.AdquisicionPor),
            Row(Header("purchase_price", "Precio compra"), FormatDecimal(v.PrecioCompra, culture)),
            Row(Header("purchase_date", "Fecha adquisición"), FormatDate(v.FechaAdquisicion, culture)),
            Row(null, null),
            Row(Header("specialization", "Especialización"), null),
            Row(Header("apt_for", "Apto para"), v.AptoPara),
            Row(Header("spec_date", "Fecha especificación"), FormatDate(v.FechaEspecificacion, culture)),
            Row(null, null),
            Row(Header("observations_title", "Observaciones"), null),
            Row(Header("observations", "Observaciones"), v.Observaciones),
            Row(Header("status_reason", "Motivo estado"), v.MotivoEstado),
            Row(null, null),
            Row(Header("audit", "Auditoría"), null),
            Row(Header("created_by", "Creado por"), v.CreadoPor),
            Row(Header("created_at", "Creado en"), FormatDateTime(v.CreadoEn, culture)),
            Row(Header("updated_by", "Actualizado por"), v.ActualizadoPor),
            Row(Header("updated_at", "Actualizado en"), FormatDateTime(v.ActualizadoEn, culture))
        };

        var sb = new StringBuilder();
        sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
        sb.Append("<worksheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\">");
        sb.Append("<cols><col min=\"1\" max=\"1\" width=\"28\" customWidth=\"1\"/><col min=\"2\" max=\"2\" width=\"45\" customWidth=\"1\"/></cols>");
        sb.Append("<sheetData>");

        for (var i = 0; i < rows.Count; i++)
        {
            var rowNumber = i + 1;
            sb.Append(CultureInfo.InvariantCulture, $"<row r=\"{rowNumber}\">");

            var row = rows[i];
            for (var col = 0; col < row.Count; col++)
            {
                var value = row[col];
                if (value is null)
                {
                    continue;
                }

                var cellReference = $"{GetColumnName(col + 1)}{rowNumber}";
                sb.Append(CultureInfo.InvariantCulture, $"<c r=\"{cellReference}\" t=\"inlineStr\"><is><t>{Xml(value)}</t></is></c>");
            }

            sb.Append("</row>");
        }

        sb.Append("</sheetData>");
        sb.Append("</worksheet>");
        return sb.ToString();
    }

    private static async Task AddEntryAsync(
        ZipArchive archive,
        string entryName,
        string content,
        CancellationToken cancellationToken)
    {
        var entry = archive.CreateEntry(entryName, CompressionLevel.Optimal);
        await using var stream = entry.Open();
        await using var writer = new StreamWriter(stream, new UTF8Encoding(false));
        await writer.WriteAsync(content.AsMemory(), cancellationToken);
    }

    private static string SanitizeFileNamePart(string value)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var safe = new string(value
            .Where(ch => !invalidChars.Contains(ch) && !char.IsWhiteSpace(ch))
            .ToArray());

        return string.IsNullOrWhiteSpace(safe) ? "vacuno" : safe;
    }

    private static string GetColumnName(int columnNumber)
    {
        var dividend = columnNumber;
        var columnName = string.Empty;

        while (dividend > 0)
        {
            var modulo = (dividend - 1) % 26;
            columnName = Convert.ToChar('A' + modulo) + columnName;
            dividend = (dividend - modulo) / 26;
        }

        return columnName;
    }

    private static string Xml(string value) => WebUtility.HtmlEncode(value) ?? string.Empty;

    private static string FormatDate(DateOnly? date, CultureInfo culture) => date?.ToString("yyyy-MM-dd", culture) ?? string.Empty;

    private static string FormatDecimal(decimal? value, CultureInfo culture) => value?.ToString("0.##", culture) ?? string.Empty;

    private static string FormatDateTime(DateTime? value, CultureInfo culture) => value?.ToString("yyyy-MM-dd HH:mm:ss", culture) ?? string.Empty;

    private const string ContentTypesXml = """
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<Types xmlns="http://schemas.openxmlformats.org/package/2006/content-types">
  <Default Extension="rels" ContentType="application/vnd.openxmlformats-package.relationships+xml"/>
  <Default Extension="xml" ContentType="application/xml"/>
  <Override PartName="/xl/workbook.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml"/>
  <Override PartName="/xl/worksheets/sheet1.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml"/>
</Types>
""";

    private const string RootRelationshipsXml = """
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
  <Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument" Target="xl/workbook.xml"/>
</Relationships>
""";

    private const string WorkbookXml = """
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<workbook xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main" xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships">
  <sheets>
    <sheet name="Registro Vacuno" sheetId="1" r:id="rId1"/>
  </sheets>
</workbook>
""";

    private const string WorkbookRelationshipsXml = """
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
  <Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet" Target="worksheets/sheet1.xml"/>
</Relationships>
""";
}
