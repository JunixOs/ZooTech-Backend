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
        worksheet.Range(1, 1, 1, 10).Merge();
        worksheet.Cell(2, 1).Value = $"Fecha inicio: {output.FechaInicio:yyyy-MM-dd}";
        worksheet.Cell(2, 3).Value = $"Fecha fin: {output.FechaFin:yyyy-MM-dd}";
        worksheet.Cell(2, 5).Value = $"Keyword: {output.Keyword ?? "Todos"}";

        var headerRow = 4;
        worksheet.Cell(headerRow, 1).Value = "Codigo";
        worksheet.Cell(headerRow, 2).Value = "Nombre";
        worksheet.Cell(headerRow, 3).Value = "Fecha nacimiento";
        worksheet.Cell(headerRow, 4).Value = "Tipo adquisicion";
        worksheet.Cell(headerRow, 5).Value = "Raza";
        worksheet.Cell(headerRow, 6).Value = "Color";
        worksheet.Cell(headerRow, 7).Value = "Sexo";
        worksheet.Cell(headerRow, 8).Value = "Granja";
        worksheet.Cell(headerRow, 9).Value = "Estado";
        worksheet.Cell(headerRow, 10).Value = "Fecha registro";

        var row = headerRow + 1;
        foreach (var item in output.Items)
        {
            worksheet.Cell(row, 1).Value = item.Codigo;
            worksheet.Cell(row, 2).Value = item.Nombre;
            worksheet.Cell(row, 3).Value = item.FechaNacimiento.ToDateTime(TimeOnly.MinValue);
            worksheet.Cell(row, 4).Value = item.TipoAdquisicion;
            worksheet.Cell(row, 5).Value = item.Raza;
            worksheet.Cell(row, 6).Value = item.Color;
            worksheet.Cell(row, 7).Value = item.Sexo;
            worksheet.Cell(row, 8).Value = item.Granja;
            worksheet.Cell(row, 9).Value = item.Estado;
            worksheet.Cell(row, 10).Value = item.FechaRegistro.ToDateTime(TimeOnly.MinValue);
            row++;
        }

        worksheet.Range(1, 1, 1, 10).Style.Font.Bold = true;
        worksheet.Range(1, 1, 1, 10).Style.Font.FontSize = 14;
        worksheet.Range(headerRow, 1, headerRow, 10).Style.Font.Bold = true;
        worksheet.Range(headerRow, 1, headerRow, 10).Style.Fill.BackgroundColor = XLColor.FromHtml("#E7EEF8");
        worksheet.Range(headerRow, 1, Math.Max(headerRow, row - 1), 10).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        worksheet.Range(headerRow, 1, Math.Max(headerRow, row - 1), 10).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        worksheet.Column(3).Style.DateFormat.Format = "yyyy-mm-dd";
        worksheet.Column(10).Style.DateFormat.Format = "yyyy-mm-dd";
        worksheet.Column(1).Width = 14;
        worksheet.Column(2).Width = 24;
        worksheet.Column(3).Width = 18;
        worksheet.Column(4).Width = 22;
        worksheet.Column(5).Width = 18;
        worksheet.Column(6).Width = 16;
        worksheet.Column(7).Width = 14;
        worksheet.Column(8).Width = 22;
        worksheet.Column(9).Width = 16;
        worksheet.Column(10).Width = 18;

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
