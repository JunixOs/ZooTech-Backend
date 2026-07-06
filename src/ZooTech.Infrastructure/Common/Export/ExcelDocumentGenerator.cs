using System.Text.Json;
using ClosedXML.Excel;
using ZooTech.Application.Common.Gateway.Export;

namespace ZooTech.Infrastructure.Common.Export;

public class ExcelDocumentGenerator : IExcelDocumentGenerator
{
    private static readonly char[] InvalidSheetNameChars = { ':', '\\', '/', '?', '*', '[', ']' };

    public byte[] Generate(ReportExportRequest request)
    {
        using var workbook = new XLWorkbook();
        var sheetName = BuildSheetName(request.Title);
        var worksheet = workbook.Worksheets.Add(sheetName);

        for (var columnIndex = 0; columnIndex < request.Columns.Count; columnIndex++)
        {
            var cell = worksheet.Cell(1, columnIndex + 1);
            cell.Value = request.Columns[columnIndex].Header;
            cell.Style.Font.Bold = true;
        }

        for (var rowIndex = 0; rowIndex < request.Rows.Count; rowIndex++)
        {
            var row = request.Rows[rowIndex];
            for (var columnIndex = 0; columnIndex < request.Columns.Count; columnIndex++)
            {
                var key = request.Columns[columnIndex].Key;
                var cell = worksheet.Cell(rowIndex + 2, columnIndex + 1);

                if (row.TryGetValue(key, out var value) && value is not null && !IsJsonNull(value))
                {
                    SetCellValue(cell, value);
                }
            }
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private static bool IsJsonNull(object value) =>
        value is JsonElement { ValueKind: JsonValueKind.Null or JsonValueKind.Undefined };

    private static void SetCellValue(IXLCell cell, object value)
    {
        // System.Text.Json binds `object?` values in a request body as JsonElement,
        // not native CLR types, so JsonElement must be unwrapped before the type switch below.
        if (value is JsonElement element)
        {
            SetCellValueFromJsonElement(cell, element);
            return;
        }

        switch (value)
        {
            case string stringValue:
                cell.Value = stringValue;
                break;
            case bool boolValue:
                cell.Value = boolValue;
                break;
            case DateTime dateTimeValue:
                cell.Value = dateTimeValue;
                break;
            case DateTimeOffset dateTimeOffsetValue:
                cell.Value = dateTimeOffsetValue.DateTime;
                break;
            case int or long or short or byte:
                cell.Value = Convert.ToInt64(value);
                break;
            case float or double or decimal:
                cell.Value = Convert.ToDouble(value);
                break;
            default:
                cell.Value = value.ToString();
                break;
        }
    }

    private static void SetCellValueFromJsonElement(IXLCell cell, JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.String:
                var stringValue = element.GetString() ?? string.Empty;
                if (DateTime.TryParse(stringValue, out var dateTimeValue))
                {
                    cell.Value = dateTimeValue;
                }
                else
                {
                    cell.Value = stringValue;
                }
                break;
            case JsonValueKind.Number:
                if (element.TryGetInt64(out var longValue))
                {
                    cell.Value = longValue;
                }
                else
                {
                    cell.Value = element.GetDouble();
                }
                break;
            case JsonValueKind.True:
            case JsonValueKind.False:
                cell.Value = element.GetBoolean();
                break;
            default:
                cell.Value = element.ToString();
                break;
        }
    }

    private static string BuildSheetName(string title)
    {
        var sanitized = new string(title
            .Where(character => !InvalidSheetNameChars.Contains(character))
            .ToArray())
            .Trim();

        if (string.IsNullOrWhiteSpace(sanitized))
        {
            sanitized = "Reporte";
        }

        return sanitized.Length > 31 ? sanitized[..31] : sanitized;
    }
}
