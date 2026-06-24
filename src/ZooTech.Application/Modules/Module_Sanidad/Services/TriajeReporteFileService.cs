    using System.Globalization;
    using System.IO.Compression;
    using System.Text;
    using System.Xml;
    using ZooTech.Application.Modules.Module_Sanidad.DTOs.Responses;

    namespace ZooTech.Application.Modules.Module_Sanidad.Services;

    public class TriajeReporteFileService
    {
        private const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        private const string PdfContentType = "application/pdf";

        public ReporteTriajesArchivoResponse Generate(IEnumerable<TriajeReporteResponse> triajes, string formato)
        {
            var rows = triajes.ToList();
        var normalizedFormat = formato.Trim().ToLowerInvariant();
        var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);

        return normalizedFormat switch
        {
            "xlsx" or "excel" => new ReporteTriajesArchivoResponse
            {
                Content = BuildExcel(rows),
                ContentType = ExcelContentType,
                FileName = $"reporte-triajes-{timestamp}.xlsx",
                Message = "Descarga de reporte de triajes en Excel generada correctamente."
            },
            "pdf" => new ReporteTriajesArchivoResponse
            {
                Content = BuildPdf(rows),
                ContentType = PdfContentType,
                FileName = $"reporte-triajes-{timestamp}.pdf",
                Message = "Descarga de reporte de triajes en PDF generada correctamente."
            },
            _ => throw new ArgumentException("Formato no soportado. Use 'xlsx' o 'pdf'.", nameof(formato))
        };
    }

    private static byte[] BuildExcel(IReadOnlyCollection<TriajeReporteResponse> rows)
    {
        using var stream = new MemoryStream();
        using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true))
        {
            AddEntry(archive, "[Content_Types].xml", """
                <?xml version="1.0" encoding="UTF-8" standalone="yes"?>
                <Types xmlns="http://schemas.openxmlformats.org/package/2006/content-types">
                  <Default Extension="rels" ContentType="application/vnd.openxmlformats-package.relationships+xml"/>
                  <Default Extension="xml" ContentType="application/xml"/>
                  <Override PartName="/xl/workbook.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml"/>
                  <Override PartName="/xl/worksheets/sheet1.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml"/>
                </Types>
                """);
            AddEntry(archive, "_rels/.rels", """
                <?xml version="1.0" encoding="UTF-8" standalone="yes"?>
                <Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
                  <Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument" Target="xl/workbook.xml"/>
                </Relationships>
                """);
            AddEntry(archive, "xl/workbook.xml", """
                <?xml version="1.0" encoding="UTF-8" standalone="yes"?>
                <workbook xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main" xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships">
                  <sheets>
                    <sheet name="Triajes" sheetId="1" r:id="rId1"/>
                  </sheets>
                </workbook>
                """);
            AddEntry(archive, "xl/_rels/workbook.xml.rels", """
                <?xml version="1.0" encoding="UTF-8" standalone="yes"?>
                <Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
                  <Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet" Target="worksheets/sheet1.xml"/>
                </Relationships>
                """);
            AddEntry(archive, "xl/worksheets/sheet1.xml", BuildSheet(rows));
        }

        return stream.ToArray();
    }

    private static string BuildSheet(IEnumerable<TriajeReporteResponse> rows)
    {
        var builder = new StringBuilder();
        builder.Append("""
            <?xml version="1.0" encoding="UTF-8" standalone="yes"?>
            <worksheet xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main">
              <sheetData>
            """);

        AppendRow(builder, 1, new[]
        {
            "C.Registro", "Fecha", "Hora", "Vacuno", "Tipo de peso medido", "Peso (Kg)", "Observaciones"
        });

        var rowNumber = 2;
        foreach (var row in rows)
        {
            AppendRow(builder, rowNumber++, new[]
            {
                row.Codigo,
                row.FechaHora.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                row.FechaHora.ToString("HH:mm:ss", CultureInfo.InvariantCulture),
                row.VacunoNombre ?? string.Empty,
                row.TipoPesoCode,
                row.PesoKg.ToString(CultureInfo.InvariantCulture),
                row.Observaciones ?? string.Empty
            });
        }

        builder.Append("""
              </sheetData>
            </worksheet>
            """);
        return builder.ToString();
    }

    private static void AppendRow(StringBuilder builder, int rowNumber, IReadOnlyList<string> values)
    {
        builder.Append(CultureInfo.InvariantCulture, $"<row r=\"{rowNumber}\">");

        for (var index = 0; index < values.Count; index++)
        {
            var cellReference = $"{GetColumnName(index + 1)}{rowNumber}";
            builder.Append(CultureInfo.InvariantCulture, $"<c r=\"{cellReference}\" t=\"inlineStr\"><is><t>{XmlEscape(values[index])}</t></is></c>");
        }

        builder.Append("</row>");
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

    private static void AddEntry(ZipArchive archive, string path, string content)
    {
        var entry = archive.CreateEntry(path);
        using var writer = new StreamWriter(entry.Open(), new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
        writer.Write(content);
    }

    private static byte[] BuildPdf(IReadOnlyCollection<TriajeReporteResponse> rows)
    {
        var lines = new List<string>
        {
            "Reporte de triajes",
            $"Generado: {DateTime.Now:yyyy-MM-dd HH:mm:ss}",
            string.Empty,
            "C.Registro | Fecha | Hora | Vacuno | Tipo peso | Peso (Kg) | Observaciones"
        };

        lines.AddRange(rows.Select(row =>
            $"{row.Codigo} | {row.FechaHora:yyyy-MM-dd} | {row.FechaHora:HH:mm:ss} | {Trim(row.VacunoNombre ?? string.Empty, 18)} | {Trim(row.TipoPesoCode, 18)} | {row.PesoKg.ToString(CultureInfo.InvariantCulture)} | {Trim(row.Observaciones ?? string.Empty, 30)}"));

        var pageContents = lines
            .Chunk(38)
            .Select(BuildPdfPageContent)
            .ToList();

        return BuildPdfDocument(pageContents);
    }

    private static string BuildPdfPageContent(IEnumerable<string> lines)
    {
        var builder = new StringBuilder();
        builder.AppendLine("BT");
        builder.AppendLine("/F1 9 Tf");
        builder.AppendLine("40 800 Td");

        foreach (var line in lines)
        {
            builder.Append('(').Append(PdfEscape(line)).AppendLine(") Tj");
            builder.AppendLine("0 -18 Td");
        }

        builder.AppendLine("ET");
        return builder.ToString();
    }

    private static byte[] BuildPdfDocument(IReadOnlyList<string> pageContents)
    {
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream, Encoding.ASCII, leaveOpen: true);
        var offsets = new List<long> { 0 };
        var pageCount = pageContents.Count == 0 ? 1 : pageContents.Count;

        writer.WriteLine("%PDF-1.4");
        WriteObject(writer, offsets, 1, "<< /Type /Catalog /Pages 2 0 R >>");

        var kids = string.Join(' ', Enumerable.Range(0, pageCount).Select(index => $"{3 + (index * 2)} 0 R"));
        WriteObject(writer, offsets, 2, $"<< /Type /Pages /Kids [{kids}] /Count {pageCount} >>");

        for (var index = 0; index < pageCount; index++)
        {
            var pageObject = 3 + (index * 2);
            var contentObject = pageObject + 1;
            WriteObject(writer, offsets, pageObject, $"<< /Type /Page /Parent 2 0 R /MediaBox [0 0 842 595] /Resources << /Font << /F1 << /Type /Font /Subtype /Type1 /BaseFont /Courier >> >> >> /Contents {contentObject} 0 R >>");
            WriteStreamObject(writer, offsets, contentObject, pageContents.ElementAtOrDefault(index) ?? string.Empty);
        }

        var xrefOffset = stream.Position;
        writer.WriteLine("xref");
        writer.WriteLine($"0 {offsets.Count}");
        writer.WriteLine("0000000000 65535 f ");

        foreach (var offset in offsets.Skip(1))
        {
            writer.WriteLine($"{offset:0000000000} 00000 n ");
        }

        writer.WriteLine("trailer");
        writer.WriteLine($"<< /Size {offsets.Count} /Root 1 0 R >>");
        writer.WriteLine("startxref");
        writer.WriteLine(xrefOffset);
        writer.WriteLine("%%EOF");
        writer.Flush();

        return stream.ToArray();
    }

    private static void WriteObject(StreamWriter writer, ICollection<long> offsets, int number, string content)
    {
        writer.Flush();
        offsets.Add(writer.BaseStream.Position);
        writer.WriteLine($"{number} 0 obj");
        writer.WriteLine(content);
        writer.WriteLine("endobj");
    }

    private static void WriteStreamObject(StreamWriter writer, ICollection<long> offsets, int number, string content)
    {
        var bytes = Encoding.ASCII.GetBytes(content);
        writer.Flush();
        offsets.Add(writer.BaseStream.Position);
        writer.WriteLine($"{number} 0 obj");
        writer.WriteLine($"<< /Length {bytes.Length} >>");
        writer.WriteLine("stream");
        writer.Flush();
        writer.BaseStream.Write(bytes, 0, bytes.Length);
        writer.WriteLine();
        writer.WriteLine("endstream");
        writer.WriteLine("endobj");
    }

    private static string XmlEscape(string value)
    {
        return SecurityElementEscape(value);
    }

    private static string SecurityElementEscape(string value)
    {
        return value
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&apos;");
    }

    private static string PdfEscape(string value)
    {
        return value
            .Replace("\\", "\\\\")
            .Replace("(", "\\(")
            .Replace(")", "\\)");
    }

    private static string Trim(string value, int maxLength)
    {
        return value.Length <= maxLength ? value : value[..maxLength];
    }
}
