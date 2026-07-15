using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Common.Gateway.Export;
using ZooTech.InterfaceAdapters.DTOs;

namespace ZooTech.InterfaceAdapters.Modules.Shared.Controllers;

[ApiController]
[Route("api/v1/export")]
[ApiExplorerSettings(GroupName = "public")]
public class ExportController : ControllerBase
{
    private const int MaxRows = 50_000;

    private readonly IExcelDocumentGenerator _excelDocumentGenerator;
    private readonly IPdfDocumentGenerator _pdfDocumentGenerator;

    public ExportController(
        IExcelDocumentGenerator excelDocumentGenerator,
        IPdfDocumentGenerator pdfDocumentGenerator)
    {
        _excelDocumentGenerator = excelDocumentGenerator;
        _pdfDocumentGenerator = pdfDocumentGenerator;
    }

    [HttpPost("excel")]
    [Tags("Export")]
    public IActionResult Excel([FromBody] ExportDocumentRequest request)
    {
        if (request.Rows.Count > MaxRows)
        {
            return BadRequest($"El reporte supera el límite máximo de {MaxRows} filas.");
        }

        var bytes = _excelDocumentGenerator.Generate(ToReportExportRequest(request));
        var fileName = $"{Slugify(request.Title)}.xlsx";

        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    [HttpPost("pdf")]
    [Tags("Export")]
    public IActionResult Pdf([FromBody] ExportDocumentRequest request)
    {
        if (request.Rows.Count > MaxRows)
        {
            return BadRequest($"El reporte supera el límite máximo de {MaxRows} filas.");
        }

        var bytes = _pdfDocumentGenerator.Generate(ToReportExportRequest(request));
        var fileName = $"{Slugify(request.Title)}.pdf";

        return File(bytes, "application/pdf", fileName);
    }

    private static ReportExportRequest ToReportExportRequest(ExportDocumentRequest request)
    {
        var columns = request.Columns
            .Select(column => new ExportColumn(column.Header, column.Key))
            .ToList();

        var rows = request.Rows
            .Select(row => (IReadOnlyDictionary<string, object?>)row)
            .ToList();

        return new ReportExportRequest(request.Title, columns, rows);
    }

    private static string Slugify(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "reporte";
        }

        var normalized = value.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder();

        foreach (var character in normalized)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(character);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(character);
            }
        }

        var withoutDiacritics = builder.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant();

        var slugBuilder = new StringBuilder();
        var previousWasDash = false;

        foreach (var character in withoutDiacritics)
        {
            if (char.IsLetterOrDigit(character))
            {
                slugBuilder.Append(character);
                previousWasDash = false;
            }
            else if (!previousWasDash && slugBuilder.Length > 0)
            {
                slugBuilder.Append('-');
                previousWasDash = true;
            }
        }

        var slug = slugBuilder.ToString().Trim('-');
        return string.IsNullOrWhiteSpace(slug) ? "reporte" : slug;
    }
}
