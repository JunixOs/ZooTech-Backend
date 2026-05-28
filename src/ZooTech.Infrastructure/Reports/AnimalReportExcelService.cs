using ClosedXML.Excel;
using ZooTech.Application.Modules.Animals.UseCases.ReportAnimalList;

namespace ZooTech.Infrastructure.Reports;

public sealed class AnimalReportExcelService : IAnimalReportExcelService
{
    public byte[] GenerateAnimalListExcel(ReportAnimalListOutput output)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Vacunos");

        worksheet.Cell(1, 1).Value = "Reporte listado de vacunos";
        worksheet.Range(1, 1, 1, 7).Merge();
        worksheet.Cell(2, 1).Value = $"Fecha inicio: {output.FechaInicio:yyyy-MM-dd}";
        worksheet.Cell(2, 3).Value = $"Fecha fin: {output.FechaFin:yyyy-MM-dd}";
        worksheet.Cell(2, 5).Value = $"Keyword: {output.Keyword ?? "Todos"}";

        var headerRow = 4;
        worksheet.Cell(headerRow, 1).Value = "Codigo";
        worksheet.Cell(headerRow, 2).Value = "Nombre";
        worksheet.Cell(headerRow, 3).Value = "Raza";
        worksheet.Cell(headerRow, 4).Value = "Sexo";
        worksheet.Cell(headerRow, 5).Value = "Procedencia";
        worksheet.Cell(headerRow, 6).Value = "Estado";
        worksheet.Cell(headerRow, 7).Value = "Fecha registro";

        var row = headerRow + 1;
        foreach (var item in output.Items)
        {
            worksheet.Cell(row, 1).Value = item.Codigo;
            worksheet.Cell(row, 2).Value = item.Nombre;
            worksheet.Cell(row, 3).Value = item.Raza;
            worksheet.Cell(row, 4).Value = item.Sexo;
            worksheet.Cell(row, 5).Value = item.Procedencia;
            worksheet.Cell(row, 6).Value = item.Estado;
            worksheet.Cell(row, 7).Value = item.FechaRegistro.ToDateTime(TimeOnly.MinValue);
            row++;
        }

        worksheet.Range(1, 1, 1, 7).Style.Font.Bold = true;
        worksheet.Range(1, 1, 1, 7).Style.Font.FontSize = 14;
        worksheet.Range(headerRow, 1, headerRow, 7).Style.Font.Bold = true;
        worksheet.Range(headerRow, 1, headerRow, 7).Style.Fill.BackgroundColor = XLColor.FromHtml("#E7EEF8");
        worksheet.Range(headerRow, 1, Math.Max(headerRow, row - 1), 7).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        worksheet.Range(headerRow, 1, Math.Max(headerRow, row - 1), 7).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        worksheet.Column(7).Style.DateFormat.Format = "yyyy-mm-dd";
        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
