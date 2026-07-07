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

    public byte[] GenerateOrdeniosReport(GenerateOrdeniosExcelDocument document)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Reporte de Ordenios");

        // 1. Configurar Encabezado y Filtros
        worksheet.Cell(1, 1).Value = "Reporte de Ordeños";
        worksheet.Cell(1, 1).Style.Font.Bold = true;
        worksheet.Cell(1, 1).Style.Font.FontSize = 16;
        worksheet.Cell(1, 1).Style.Font.FontColor = Primary;

        worksheet.Cell(2, 1).Value = $"Generado: {FormatDateTime(document.GeneratedAtUtc)} UTC";
        worksheet.Cell(3, 1).Value = BuildFilters(document);

        // 2. Configurar Cabeceras de la Tabla (Fila 5)
        var currentRow = 5;
        string[] headers = { "Código", "Fecha", "Vacuno", "Litros", "Estado" };

        for (int i = 0; i < headers.Length; i++)
        {
            var cell = worksheet.Cell(currentRow, i + 1);
            cell.Value = headers[i];
            cell.Style.Fill.BackgroundColor = Primary;
            cell.Style.Font.FontColor = XLColor.White;
            cell.Style.Font.Bold = true;
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }

        // 3. Llenar los Datos
        currentRow++;
        for (var i = 0; i < document.Items.Count; i++)
        {
            var item = document.Items[i];
            var isAlternate = i % 2 == 1;

            worksheet.Cell(currentRow, 1).Value = item.Codigo;
            worksheet.Cell(currentRow, 2).Value = FormatDateTime(item.FechaHora);
            worksheet.Cell(currentRow, 3).Value = item.NombreVacuno;
            worksheet.Cell(currentRow, 4).Value = item.Litros;
            worksheet.Cell(currentRow, 5).Value = item.EstadoOrdenioCode;

            // Aplicar estilo de fila cebra
            var rowRange = worksheet.Range(currentRow, 1, currentRow, 5);
            rowRange.Style.Font.FontColor = TextDark;

            if (isAlternate)
            {
                rowRange.Style.Fill.BackgroundColor = PrimaryLight;
            }

            // Borde suave
            rowRange.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            rowRange.Style.Border.BottomBorderColor = XLColor.FromHtml("#d9bfca");

            currentRow++;
        }

        // Auto-ajustar el ancho de las columnas
        worksheet.Columns().AdjustToContents();

        // 4. Guardar en memoria y retornar los bytes
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private static string BuildFilters(GenerateOrdeniosExcelDocument document)
    {
        var filters = new List<string>();

        if (document.VacunoId.HasValue)
            filters.Add($"VacunoId: {document.VacunoId.Value}");

        if (!string.IsNullOrWhiteSpace(document.EstadoOrdenioCode))
            filters.Add($"Estado: {document.EstadoOrdenioCode}");

        if (document.FechaDesde.HasValue)
            filters.Add($"Desde: {document.FechaDesde.Value:yyyy-MM-dd HH:mm}");

        if (document.FechaHasta.HasValue)
            filters.Add($"Hasta: {document.FechaHasta.Value:yyyy-MM-dd HH:mm}");

        return filters.Count == 0 ? "Filtros: sin filtros" : $"Filtros: {string.Join(" | ", filters)}";
    }

    public byte[] GenerateTriajesReport(GenerateTriajesExcelDocument document)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Reporte de Triajes");

        worksheet.Cell(1, 1).Value = "Reporte de Triajes";
        worksheet.Cell(1, 1).Style.Font.Bold = true;
        worksheet.Cell(1, 1).Style.Font.FontSize = 16;
        worksheet.Cell(1, 1).Style.Font.FontColor = Primary;

        worksheet.Cell(2, 1).Value = $"Generado: {FormatDateTime(document.GeneratedAtUtc)} UTC";
        worksheet.Cell(3, 1).Value = BuildTriajeFilters(document);

        var currentRow = 5;
        string[] headers = { "C.Registro", "Fecha", "Hora", "Vacuno", "Tipo Peso", "Peso (Kg)", "Observaciones" };

        for (int i = 0; i < headers.Length; i++)
        {
            var cell = worksheet.Cell(currentRow, i + 1);
            cell.Value = headers[i];
            cell.Style.Fill.BackgroundColor = Primary;
            cell.Style.Font.FontColor = XLColor.White;
            cell.Style.Font.Bold = true;
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }

        currentRow++;
        for (var i = 0; i < document.Items.Count; i++)
        {
            var item = document.Items[i];
            var isAlternate = i % 2 == 1;

            worksheet.Cell(currentRow, 1).Value = item.Codigo;
            worksheet.Cell(currentRow, 2).Value = FormatDate(item.FechaHora);
            worksheet.Cell(currentRow, 3).Value = FormatTime(item.FechaHora);
            worksheet.Cell(currentRow, 4).Value = item.VacunoNombre;
            worksheet.Cell(currentRow, 5).Value = item.TipoPesoCode;
            worksheet.Cell(currentRow, 6).Value = item.PesoKg;
            worksheet.Cell(currentRow, 7).Value = item.Observaciones ?? string.Empty;

            var rowRange = worksheet.Range(currentRow, 1, currentRow, 7);
            rowRange.Style.Font.FontColor = TextDark;

            if (isAlternate)
            {
                rowRange.Style.Fill.BackgroundColor = PrimaryLight;
            }

            rowRange.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            rowRange.Style.Border.BottomBorderColor = XLColor.FromHtml("#d9bfca");

            currentRow++;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private static string BuildTriajeFilters(GenerateTriajesExcelDocument document)
    {
        var filters = new List<string>();

        if (!string.IsNullOrWhiteSpace(document.FechaDesde))
            filters.Add($"Desde: {document.FechaDesde}");
        if (!string.IsNullOrWhiteSpace(document.FechaHasta))
            filters.Add($"Hasta: {document.FechaHasta}");
        if (document.VacunoId.HasValue)
            filters.Add($"Vacuno ID: {document.VacunoId.Value}");
        if (document.VacunoNombre.HasValue)
            filters.Add($"Nombre Vacuno: {document.VacunoNombre.Value}");

        if (!string.IsNullOrWhiteSpace(document.Fecha))
            filters.Add($"Fecha: {document.Fecha}");

        if (!string.IsNullOrWhiteSpace(document.Codigo))
            filters.Add($"Código: {document.Codigo}");

        if (!string.IsNullOrWhiteSpace(document.Nombre))
            filters.Add($"Nombre: {document.Nombre}");

        if (!string.IsNullOrWhiteSpace(document.TipoPeso))
            filters.Add($"Tipo peso: {document.TipoPeso}");

        if (document.PesoKg.HasValue)
            filters.Add($"Peso: {document.PesoKg.Value} Kg");

        return filters.Count == 0 ? "Filtros: sin filtros" : $"Filtros: {string.Join(" | ", filters)}";
    }

    private static string FormatDate(DateTime value)
        => value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

    private static string FormatTime(DateTime value)
        => value.ToString("HH:mm:ss", CultureInfo.InvariantCulture);

    private static string FormatDateTime(DateTime value)
        => value.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
}