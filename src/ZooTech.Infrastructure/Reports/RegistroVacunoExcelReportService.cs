using System.Globalization;
using System.IO.Compression;
using System.Net;
using System.Text;
using ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ObtenerRegistroVacunoReporte;

namespace ZooTech.Infrastructure.Reports;

public sealed class RegistroVacunoExcelReportService : IRegistroVacunoExcelReportService
{
    public async Task<RegistroVacunoExcelReportResult> GenerateAsync(
        RegistroVacunoDetalle vacuno,
        CancellationToken cancellationToken = default)
    {
        var codigo = SanitizeFileNamePart(vacuno.Codigo);
        var fecha = DateTime.UtcNow.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
        var fileName = $"reporte_{codigo}_{fecha}.xlsx";

        var outputDirectory = Path.Combine(
            AppContext.BaseDirectory,
            "wwwroot",
            "reportes",
            "vacunos");

        Directory.CreateDirectory(outputDirectory);

        var filePath = Path.Combine(outputDirectory, fileName);
        var sheetXml = BuildWorksheetXml(vacuno);

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

        var downloadUrl = $"/reportes/vacunos/{Uri.EscapeDataString(fileName)}";
        return new RegistroVacunoExcelReportResult(fileName, downloadUrl);
    }

    private static IReadOnlyList<string?> Row(params string?[] values) => values;

    private static string BuildWorksheetXml(RegistroVacunoDetalle v)
    {
        var rows = new List<IReadOnlyList<string?>>
        {
            Row("Reporte de registro por vacuno", null),
            Row("Fecha de generación", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)),
            Row(null, null),
            Row("Datos de Identificación", null),
            Row("ID", v.Id.ToString(CultureInfo.InvariantCulture)),
            Row("Código", v.Codigo),
            Row("Nombre", v.Nombre),
            Row("Fecha de nacimiento", FormatDate(v.FechaNacimiento)),
            Row("Sexo", v.Sexo),
            Row("Raza", v.Raza),
            Row("Color", v.Color),
            Row("Estado", v.Estado),
            Row("Fecha de registro", FormatDate(v.FechaRegistro)),
            Row(null, null),
            Row("Trazabilidad", null),
            Row("Código padre", v.CodigoPadre),
            Row("Código madre", v.CodigoMadre),
            Row("Código abuelo", v.CodigoAbuelo),
            Row("Código abuela", v.CodigoAbuela),
            Row("Granja", v.Granja),
            Row("Distrito", v.Distrito),
            Row("Provincia", v.Provincia),
            Row("Departamento", v.Departamento),
            Row("Procedencia", v.Procedencia),
            Row("Adquisición por", v.AdquisicionPor),
            Row("Precio compra", FormatDecimal(v.PrecioCompra)),
            Row("Fecha adquisición", FormatDate(v.FechaAdquisicion)),
            Row(null, null),
            Row("Especialización", null),
            Row("Apto para", v.AptoPara),
            Row("Fecha especificación", FormatDate(v.FechaEspecificacion)),
            Row(null, null),
            Row("Observaciones", null),
            Row("Observaciones", v.Observaciones),
            Row("Motivo estado", v.MotivoEstado),
            Row(null, null),
            Row("Auditoría", null),
            Row("Creado por", v.CreadoPor),
            Row("Creado en", FormatDateTime(v.CreadoEn)),
            Row("Actualizado por", v.ActualizadoPor),
            Row("Actualizado en", FormatDateTime(v.ActualizadoEn))
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

    private static string FormatDate(DateOnly? date) => date?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? string.Empty;

    private static string FormatDecimal(decimal? value) => value?.ToString("0.##", CultureInfo.InvariantCulture) ?? string.Empty;

    private static string FormatDateTime(DateTime? value) => value?.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) ?? string.Empty;

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
