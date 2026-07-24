using System.Globalization;
using ClosedXML.Excel;
using ZooTech.Application.Common.Gateway.Reports;
using ZooTech.Application.Common.Gateway.Time;

namespace ZooTech.Infrastructure.Common.Export;

public sealed class StyledExcelReportRenderer : IStyledExcelReportRenderer
{
    private const string ContentType =
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    private readonly ITenantReportBrandingProvider _brandingProvider;
    private readonly IDateTimeProvider _dateTimeProvider;

    public StyledExcelReportRenderer(
        ITenantReportBrandingProvider brandingProvider,
        IDateTimeProvider dateTimeProvider)
    {
        _brandingProvider = brandingProvider;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<GeneratedReportDocument> RenderAsync<T>(
        StyledTabularReport<T> report,
        CancellationToken cancellationToken = default)
    {
        var branding = await _brandingProvider.GetAsync(cancellationToken);
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add(report.Metadata.SheetName);
        var columnCount = Math.Max(report.Columns.Count, 2);

        RenderHeader(
            worksheet,
            report.Metadata,
            report.Theme,
            branding,
            columnCount);

        var currentRow = 5;
        foreach (var item in report.Summary)
        {
            worksheet.Cell(currentRow, 1).Value = item.Label;
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 1).Style.Fill.BackgroundColor =
                XLColor.FromHtml(report.Theme.PrimaryLight);
            worksheet.Cell(currentRow, 2).Value = item.Value;
            ApplyBorders(worksheet.Range(currentRow, 1, currentRow, columnCount), report.Theme);
            currentRow++;
        }

        if (report.Summary.Count > 0)
        {
            currentRow++;
        }

        var headerRow = currentRow;
        var columnIndex = 1;
        foreach (var column in report.Columns)
        {
            worksheet.Cell(headerRow, columnIndex).Value = column.Header;
            columnIndex++;
        }

        ApplyTableHeader(
            worksheet.Range(headerRow, 1, headerRow, report.Columns.Count),
            report.Theme);

        currentRow++;
        var itemIndex = 0;
        foreach (var item in report.Items)
        {
            columnIndex = 1;
            foreach (var column in report.Columns)
            {
                SetCellValue(
                    worksheet.Cell(currentRow, columnIndex),
                    column.ValueSelector(item));
                columnIndex++;
            }

            var rowRange = worksheet.Range(currentRow, 1, currentRow, report.Columns.Count);
            if (itemIndex % 2 == 1)
            {
                rowRange.Style.Fill.BackgroundColor =
                    XLColor.FromHtml(report.Theme.PrimaryLight);
            }

            ApplyBorders(rowRange, report.Theme);
            itemIndex++;
            currentRow++;
        }

        worksheet.SheetView.FreezeRows(headerRow);
        worksheet.Columns().AdjustToContents(10, 55);

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return new GeneratedReportDocument(
            stream.ToArray(),
            ContentType,
            ".xlsx",
            report.Metadata.FileName);
    }

    public async Task<GeneratedReportDocument> RenderDetailAsync(
        StyledDetailReport report,
        CancellationToken cancellationToken = default)
    {
        var branding = await _brandingProvider.GetAsync(cancellationToken);
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add(report.Metadata.SheetName);

        RenderHeader(
            worksheet,
            report.Metadata,
            report.Theme,
            branding,
            2);

        var currentRow = 5;
        foreach (var section in report.Sections)
        {
            worksheet.Cell(currentRow, 1).Value = section.Title;
            var sectionRange = worksheet.Range(currentRow, 1, currentRow, 2);
            sectionRange.Merge();
            ApplyTableHeader(sectionRange, report.Theme);
            currentRow++;

            foreach (var field in section.Fields)
            {
                worksheet.Cell(currentRow, 1).Value = field.Label;
                worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                worksheet.Cell(currentRow, 1).Style.Fill.BackgroundColor =
                    XLColor.FromHtml(report.Theme.PrimaryLight);
                SetCellValue(worksheet.Cell(currentRow, 2), field.Value);
                ApplyBorders(worksheet.Range(currentRow, 1, currentRow, 2), report.Theme);
                currentRow++;
            }

            currentRow++;
        }

        worksheet.Column(1).Width = 30;
        worksheet.Column(2).Width = 55;

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return new GeneratedReportDocument(
            stream.ToArray(),
            ContentType,
            ".xlsx",
            report.Metadata.FileName);
    }

    private void RenderHeader(
        IXLWorksheet worksheet,
        StyledReportMetadata metadata,
        StyledReportTheme theme,
        TenantReportBranding branding,
        int columnCount)
    {
        worksheet.Cell(1, 1).Value = branding.DisplayName;
        worksheet.Cell(2, 1).Value = metadata.Title;
        worksheet.Cell(3, 1).Value =
            $"Generado: {_dateTimeProvider.ServerNow.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)} UTC";

        var header = worksheet.Range(1, 1, 1, columnCount);
        header.Merge();
        header.Style.Font.Bold = true;
        header.Style.Font.FontSize = 16;
        header.Style.Font.FontColor = XLColor.White;
        header.Style.Fill.BackgroundColor = XLColor.FromHtml(theme.Primary);

        worksheet.Range(2, 1, 2, columnCount).Merge().Style.Font.Bold = true;
        worksheet.Range(3, 1, 3, columnCount).Merge().Style.Font.Italic = true;

        if (branding.LogoContent is not null)
        {
            worksheet.AddPicture(new MemoryStream(branding.LogoContent))
                .MoveTo(worksheet.Cell(1, columnCount))
                .WithSize(36, 36);
            worksheet.Row(1).Height = 32;
        }
    }

    private static void ApplyTableHeader(IXLRange range, StyledReportTheme theme)
    {
        range.Style.Font.Bold = true;
        range.Style.Font.FontColor = XLColor.White;
        range.Style.Fill.BackgroundColor = XLColor.FromHtml(theme.Primary);
        range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
    }

    private static void ApplyBorders(IXLRange range, StyledReportTheme theme)
    {
        range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        range.Style.Border.OutsideBorderColor = XLColor.FromHtml(theme.BorderSoft);
        range.Style.Border.InsideBorderColor = XLColor.FromHtml(theme.BorderSoft);
    }

    private static void SetCellValue(IXLCell cell, object? value)
    {
        cell.Value = value switch
        {
            null => string.Empty,
            string text => text,
            bool flag => flag,
            DateOnly date => date.ToDateTime(TimeOnly.MinValue),
            DateTime dateTime => dateTime,
            DateTimeOffset dateTimeOffset => dateTimeOffset.DateTime,
            decimal number => number,
            double number => number,
            float number => number,
            int number => number,
            long number => number,
            short number => number,
            byte number => number,
            _ => StyledReportValueFormatter.Format(value)
        };
    }
}
