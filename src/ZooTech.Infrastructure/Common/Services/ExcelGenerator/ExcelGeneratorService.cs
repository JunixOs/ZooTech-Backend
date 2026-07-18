using ClosedXML.Excel;
using System.Globalization;
using ZooTech.Application.Common.Gateway.Services;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosExcel;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GenerateTriajesExcel;

namespace ZooTech.Infrastructure.Common.Services.ExcelGenerator;

public sealed class ExcelGeneratorService : IExcelGeneratorService
{
    // Misma paleta de colores que el PDF
    private static readonly XLColor Primary = XLColor.FromHtml("#802b4d");
    private static readonly XLColor PrimaryLight = XLColor.FromHtml("#ecdfe4");
    private static readonly XLColor TextDark = XLColor.FromHtml("#2d2d2d");
    private static readonly XLColor BorderSoft = XLColor.FromHtml("#d9bfca");

    public byte[] GenerateOrdeniosReport(GenerateOrdeniosExcelDocument document)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Reporte de Ordenios");

        RenderHeader(worksheet, "Reporte de Ordeños", document.GeneratedAtUtc, BuildFilters(document));

        RenderTable(
            worksheet,
            startRow: 5,
            headers: new[] { "Código", "Fecha", "Vacuno", "Litros", "Estado" },
            items: document.Items,
            cellSelector: item => new object[]
            {
                item.Codigo,
                FormatDateTime(item.FechaHora),
                item.NombreVacuno,
                item.Litros,
                item.EstadoOrdenioCode
            });

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private static string BuildFilters(GenerateOrdeniosExcelDocument document)
    {
        return BuildFilterLine(
            ("VacunoId", document.VacunoId?.ToString()),
            ("Estado", document.EstadoOrdenioCode),
            ("Desde", document.FechaDesde?.ToString("yyyy-MM-dd HH:mm")),
            ("Hasta", document.FechaHasta?.ToString("yyyy-MM-dd HH:mm")));
    }

    public byte[] GenerateTriajesReport(GenerateTriajesExcelDocument document)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Reporte de Triajes");

        RenderHeader(worksheet, "Reporte de Triajes", document.GeneratedAtUtc, BuildTriajeFilters(document));

        RenderTable(
            worksheet,
            startRow: 5,
            headers: new[] { "C.Registro", "Fecha", "Hora", "Vacuno", "Tipo Peso", "Peso (Kg)", "Observaciones" },
            items: document.Items,
            cellSelector: item => new object[]
            {
                item.Codigo,
                FormatDate(item.FechaHora),
                FormatTime(item.FechaHora),
                item.VacunoNombre,
                item.TipoPesoCode,
                item.PesoKg,
                item.Observaciones ?? string.Empty
            });

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private static string BuildTriajeFilters(GenerateTriajesExcelDocument document)
    {
        return BuildFilterLine(
            ("Fecha", document.Fecha),
            ("Código", document.Codigo),
            ("Nombre", document.Nombre),
            ("Tipo peso", document.TipoPeso),
            ("Peso", document.PesoKg.HasValue ? $"{document.PesoKg.Value} Kg" : null));
    }


    private static void RenderHeader(IXLWorksheet worksheet, string title, DateTime generatedAtUtc, string filterLine)
    {
        worksheet.Cell(1, 1).Value = title;
        worksheet.Cell(1, 1).Style.Font.Bold = true;
        worksheet.Cell(1, 1).Style.Font.FontSize = 16;
        worksheet.Cell(1, 1).Style.Font.FontColor = Primary;

        worksheet.Cell(2, 1).Value = $"Generado: {FormatDateTime(generatedAtUtc)} UTC";
        worksheet.Cell(3, 1).Value = filterLine;
    }

    private static void RenderTable<T>(
        IXLWorksheet worksheet,
        int startRow,
        string[] headers,
        IReadOnlyList<T> items,
        Func<T, object[]> cellSelector)
    {
        var currentRow = startRow;

        for (var i = 0; i < headers.Length; i++)
        {
            var cell = worksheet.Cell(currentRow, i + 1);
            cell.Value = headers[i];
            cell.Style.Fill.BackgroundColor = Primary;
            cell.Style.Font.FontColor = XLColor.White;
            cell.Style.Font.Bold = true;
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }

        currentRow++;
        for (var i = 0; i < items.Count; i++)
        {
            var values = cellSelector(items[i]);
            var isAlternate = i % 2 == 1;

            for (var col = 0; col < values.Length; col++)
            {
                SetCellValue(worksheet.Cell(currentRow, col + 1), values[col]);
            }

            // Aplicar estilo de fila cebra
            var rowRange = worksheet.Range(currentRow, 1, currentRow, values.Length);
            rowRange.Style.Font.FontColor = TextDark;

            if (isAlternate)
            {
                rowRange.Style.Fill.BackgroundColor = PrimaryLight;
            }

            // Borde suave
            rowRange.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            rowRange.Style.Border.BottomBorderColor = BorderSoft;

            currentRow++;
        }

        // Auto-ajustar el ancho de las columnas
        worksheet.Columns().AdjustToContents();
    }

    private static void SetCellValue(IXLCell cell, object? value)
    {
        cell.Value = value switch
        {
            string s => s,
            decimal d => d,
            double d => d,
            int i => i,
            long l => l,
            DateTime dt => dt,
            bool b => b,
            null => string.Empty,
            _ => value.ToString() ?? string.Empty
        };
    }

    private static string BuildFilterLine(params (string Label, string? Value)[] filters)
    {
        var parts = filters
            .Where(f => !string.IsNullOrWhiteSpace(f.Value))
            .Select(f => $"{f.Label}: {f.Value}")
            .ToList();

        return parts.Count == 0 ? "Filtros: sin filtros" : $"Filtros: {string.Join(" | ", parts)}";
    }

    private static string FormatDate(DateTime value)
        => value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

    private static string FormatTime(DateTime value)
        => value.ToString("HH:mm:ss", CultureInfo.InvariantCulture);

    private static string FormatDateTime(DateTime value)
        => value.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
}
