using System.Globalization;
using System.IO.Compression;
using System.Net;
using System.Text;
using ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ListarReporteVacunos;

namespace ZooTech.Infrastructure.Reports;

public sealed class ListadoVacunosReportFileService : IListadoVacunosReportFileService
{
    public async Task<ListadoVacunosReportFileResult> GenerateExcelAsync(
        IReadOnlyCollection<VacunoListadoItem> data,
        ReporteVacunoResumen resumen,
        ReporteVacunoFiltros filtros,
        CancellationToken cancellationToken = default)
    {
        var fecha = DateTime.UtcNow.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
        var fileName = $"reporte_listado_vacunos_{fecha}.xlsx";
        var filePath = GetOutputPath(fileName);
        var sheetXml = BuildWorksheetXml(data, resumen, filtros);

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

        return new ListadoVacunosReportFileResult(fileName, BuildDownloadUrl(fileName));
    }

    public async Task<ListadoVacunosReportFileResult> GeneratePdfAsync(
        IReadOnlyCollection<VacunoListadoItem> data,
        ReporteVacunoResumen resumen,
        ReporteVacunoFiltros filtros,
        CancellationToken cancellationToken = default)
    {
        var fecha = DateTime.UtcNow.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
        var fileName = $"reporte_listado_vacunos_{fecha}.pdf";
        var filePath = GetOutputPath(fileName);
        var lines = BuildPdfLines(data, resumen, filtros);
        var bytes = BuildSimplePdf(lines);

        await File.WriteAllBytesAsync(filePath, bytes, cancellationToken);

        return new ListadoVacunosReportFileResult(fileName, BuildDownloadUrl(fileName));
    }

    private static string GetOutputPath(string fileName)
    {
        var outputDirectory = Path.Combine(
            AppContext.BaseDirectory,
            "wwwroot",
            "reportes",
            "vacunos");

        Directory.CreateDirectory(outputDirectory);
        return Path.Combine(outputDirectory, fileName);
    }

    private static string BuildDownloadUrl(string fileName)
    {
        return $"/reportes/vacunos/{Uri.EscapeDataString(fileName)}";
    }

    private static IReadOnlyList<string?> Row(params string?[] values) => values;

    private static string BuildWorksheetXml(
        IReadOnlyCollection<VacunoListadoItem> data,
        ReporteVacunoResumen resumen,
        ReporteVacunoFiltros filtros)
    {
        var rows = new List<IReadOnlyList<string?>>
        {
            Row("Reporte listado de vacunos", null, null, null, null, null, null),
            Row("Fecha de generación", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture), null, null, null, null, null),
            Row("Fecha desde", FormatDate(filtros.FechaDesde), "Fecha hasta", FormatDate(filtros.FechaHasta), "Formato", filtros.Formato, null),
            Row("Busqueda", filtros.Q, "Codigo", filtros.Codigo, "Fecha registro", filtros.FechaRegistro, null),
            Row("Nombre", filtros.Nombre, "Raza", filtros.Raza, "Procedencia", filtros.Procedencia, null),
            Row("Estado", filtros.Estado, "Apto para", filtros.AptoPara, "Total vacunos", resumen.TotalVacunos.ToString(CultureInfo.InvariantCulture), null),
            Row(null, null, null, null, null, null, null),
            Row("Código", "Registro", "Nombre", "Raza", "Procedencia", "Estado", "ID")
        };

        foreach (var item in data)
        {
            rows.Add(Row(
                item.Codigo,
                FormatDate(item.FechaRegistro),
                item.Nombre,
                item.Raza,
                item.Procedencia,
                item.Estado,
                item.Id.ToString(CultureInfo.InvariantCulture)));
        }

        var sb = new StringBuilder();
        sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
        sb.Append("<worksheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\">");
        sb.Append("<cols>");
        sb.Append("<col min=\"1\" max=\"1\" width=\"18\" customWidth=\"1\"/>");
        sb.Append("<col min=\"2\" max=\"2\" width=\"16\" customWidth=\"1\"/>");
        sb.Append("<col min=\"3\" max=\"3\" width=\"24\" customWidth=\"1\"/>");
        sb.Append("<col min=\"4\" max=\"4\" width=\"22\" customWidth=\"1\"/>");
        sb.Append("<col min=\"5\" max=\"5\" width=\"36\" customWidth=\"1\"/>");
        sb.Append("<col min=\"6\" max=\"6\" width=\"14\" customWidth=\"1\"/>");
        sb.Append("<col min=\"7\" max=\"7\" width=\"10\" customWidth=\"1\"/>");
        sb.Append("</cols><sheetData>");

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

        sb.Append("</sheetData></worksheet>");
        return sb.ToString();
    }

    private static IReadOnlyList<string> BuildPdfLines(
        IReadOnlyCollection<VacunoListadoItem> data,
        ReporteVacunoResumen resumen,
        ReporteVacunoFiltros filtros)
    {
        var lines = new List<string>
        {
            "ZooTech | Modulo Vacuno",
            "Reporte listado de vacunos",
            $"Generado: {DateTime.Now:yyyy-MM-dd HH:mm:ss}",
            $"Rango: {FormatDate(filtros.FechaDesde)} - {FormatDate(filtros.FechaHasta)}",
            $"Filtros: q={Empty(filtros.Q)} | codigo={Empty(filtros.Codigo)} | fechaRegistro={Empty(filtros.FechaRegistro)} | nombre={Empty(filtros.Nombre)} | raza={Empty(filtros.Raza)} | procedencia={Empty(filtros.Procedencia)} | estado={Empty(filtros.Estado)} | aptoPara={Empty(filtros.AptoPara)}",
            $"Total vacunos: {resumen.TotalVacunos}",
            "",
            "Codigo | Registro | Nombre | Raza | Procedencia | Estado"
        };

        foreach (var item in data)
        {
            lines.Add($"{item.Codigo} | {FormatDate(item.FechaRegistro)} | {item.Nombre} | {Empty(item.Raza)} | {Empty(item.Procedencia)} | {Empty(item.Estado)}");
        }

        if (data.Count == 0)
        {
            lines.Add("No se encontraron resultados.");
        }

        return lines;
    }

    private static byte[] BuildSimplePdf(IReadOnlyList<string> lines)
    {
        var escapedLines = lines
            .Take(42)
            .Select((line, index) => $"BT /F1 9 Tf 50 {760 - (index * 16)} Td ({PdfText(line)}) Tj ET");

        var content = string.Join("\n", escapedLines);
        var contentBytes = Encoding.ASCII.GetBytes(content);
        var objects = new List<string>
        {
            "1 0 obj << /Type /Catalog /Pages 2 0 R >> endobj\n",
            "2 0 obj << /Type /Pages /Kids [3 0 R] /Count 1 >> endobj\n",
            "3 0 obj << /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Resources << /Font << /F1 4 0 R >> >> /Contents 5 0 R >> endobj\n",
            "4 0 obj << /Type /Font /Subtype /Type1 /BaseFont /Helvetica >> endobj\n",
            $"5 0 obj << /Length {contentBytes.Length} >> stream\n{content}\nendstream endobj\n"
        };

        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream, Encoding.ASCII, leaveOpen: true);
        writer.Write("%PDF-1.4\n");

        var offsets = new List<long> { 0 };
        foreach (var obj in objects)
        {
            writer.Flush();
            offsets.Add(stream.Position);
            writer.Write(obj);
        }

        writer.Flush();
        var xrefPosition = stream.Position;
        writer.WriteLine("xref");
        writer.WriteLine($"0 {objects.Count + 1}");
        writer.WriteLine("0000000000 65535 f ");

        foreach (var offset in offsets.Skip(1))
        {
            writer.WriteLine($"{offset:0000000000} 00000 n ");
        }

        writer.WriteLine("trailer");
        writer.WriteLine($"<< /Size {objects.Count + 1} /Root 1 0 R >>");
        writer.WriteLine("startxref");
        writer.WriteLine(xrefPosition);
        writer.WriteLine("%%EOF");
        writer.Flush();

        return stream.ToArray();
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

    private static string Empty(string? value) => value ?? string.Empty;

    private static string Xml(string value) => WebUtility.HtmlEncode(value) ?? string.Empty;

    private static string FormatDate(DateOnly date) => date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

    private static string PdfText(string value)
    {
        return value
            .Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("(", "\\(", StringComparison.Ordinal)
            .Replace(")", "\\)", StringComparison.Ordinal);
    }

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
    <sheet name="Listado Vacunos" sheetId="1" r:id="rId1"/>
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
