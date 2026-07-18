using System.Globalization;
using ClosedXML.Excel;
using Microsoft.Extensions.Options;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ObtenerRegistroVacunoReporte;
using ZooTech.Infrastructure.Storage;

namespace ZooTech.Infrastructure.Reports.Vacunos;

public sealed class RegistroVacunoExcelReportService : IRegistroVacunoExcelReportService
{
    private readonly ReportStorageOptions _storageOptions;

    public RegistroVacunoExcelReportService(
        IOptions<ReportStorageOptions> storageOptions)
    {
        _storageOptions = storageOptions.Value;
    }

    public async Task<RegistroVacunoExcelReportResult> GenerateAsync(
        RegistroVacunoDetalle vacuno,
        CancellationToken cancellationToken = default)
    {
        var culture = CultureInfo.InvariantCulture;
        var namePattern = "reporte_{0}_{1}.xlsx";

        var codigo = SanitizeFileNamePart(vacuno.Codigo);
        var fecha = DateTime.UtcNow.ToString("yyyyMMdd", culture);
        var fileName = string.Format(culture, namePattern, codigo, fecha);

        var basePath = _storageOptions.ReportesBasePath;
        var vacunosPath = _storageOptions.ReportesVacunosPath;
        var urlBase = _storageOptions.ReportesUrlBase;

        var outputDirectory = Path.Combine(
            AppContext.BaseDirectory,
            basePath,
            vacunosPath);

        Directory.CreateDirectory(outputDirectory);

        var filePath = Path.Combine(outputDirectory, fileName);

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Registro Vacuno");

            // Title
            worksheet.Cell("A1").Value = "Reporte de Registro Individual de Vacuno";
            worksheet.Range("A1:B1").Merge();
            var titleStyle = worksheet.Range("A1:B1").Style;
            titleStyle.Font.Bold = true;
            titleStyle.Font.FontSize = 16;
            titleStyle.Font.FontColor = XLColor.White;
            titleStyle.Fill.BackgroundColor = XLColor.FromHtml("#166534"); // Dark green
            titleStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            titleStyle.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Row(1).Height = 30;

            // Generate Date
            worksheet.Cell("A2").Value = "Fecha de Generación:";
            worksheet.Cell("B2").Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", culture);
            worksheet.Range("A2:B2").Style.Font.Italic = true;

            int currentRow = 4;

            void AddSectionHeader(string title)
            {
                worksheet.Cell(currentRow, 1).Value = title;
                worksheet.Range(currentRow, 1, currentRow, 2).Merge();
                var style = worksheet.Range(currentRow, 1, currentRow, 2).Style;
                style.Font.Bold = true;
                style.Font.FontSize = 12;
                style.Font.FontColor = XLColor.White;
                style.Fill.BackgroundColor = XLColor.FromHtml("#22c55e"); // Green
                style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Row(currentRow).Height = 20;
                currentRow++;
            }

            void AddRow(string label, string? value)
            {
                worksheet.Cell(currentRow, 1).Value = label;
                worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                worksheet.Cell(currentRow, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#f0fdf4"); // Light green

                worksheet.Cell(currentRow, 2).Value = value ?? string.Empty;
                worksheet.Cell(currentRow, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                
                // Borders
                worksheet.Range(currentRow, 1, currentRow, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Range(currentRow, 1, currentRow, 2).Style.Border.OutsideBorderColor = XLColor.FromHtml("#bbf7d0");
                worksheet.Range(currentRow, 1, currentRow, 2).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                worksheet.Range(currentRow, 1, currentRow, 2).Style.Border.InsideBorderColor = XLColor.FromHtml("#bbf7d0");

                currentRow++;
            }

            string FormatDate(DateOnly? d) => d?.ToString("yyyy-MM-dd", culture) ?? "";
            string FormatDateTime(DateTime? d) => d?.ToString("yyyy-MM-dd HH:mm:ss", culture) ?? "";
            string FormatDecimal(decimal? v) => v?.ToString("0.##", culture) ?? "";

            // Datos de Identificación
            AddSectionHeader("Datos de Identificación");
            AddRow("ID", vacuno.Id.ToString(culture));
            AddRow("Código", vacuno.Codigo);
            AddRow("Nombre", vacuno.Nombre);
            AddRow("Fecha de Nacimiento", FormatDate(vacuno.FechaNacimiento));
            AddRow("Sexo", vacuno.Sexo);
            AddRow("Raza", vacuno.Raza);
            AddRow("Color", vacuno.Color);
            AddRow("Estado", vacuno.Estado);
            AddRow("Fecha de Registro", FormatDate(vacuno.FechaRegistro));
            currentRow++;

            // Trazabilidad
            AddSectionHeader("Trazabilidad");
            AddRow("Código Padre", vacuno.CodigoPadre);
            AddRow("Código Madre", vacuno.CodigoMadre);
            AddRow("Código Abuelo", vacuno.CodigoAbuelo);
            AddRow("Código Abuela", vacuno.CodigoAbuela);
            AddRow("Granja", vacuno.Granja);
            AddRow("Distrito", vacuno.Distrito);
            AddRow("Provincia", vacuno.Provincia);
            AddRow("Departamento", vacuno.Departamento);
            AddRow("Procedencia", vacuno.Procedencia);
            AddRow("Adquisición por", vacuno.AdquisicionPor);
            AddRow("Precio Compra", FormatDecimal(vacuno.PrecioCompra));
            AddRow("Fecha Adquisición", FormatDate(vacuno.FechaAdquisicion));
            currentRow++;

            // Especialización
            AddSectionHeader("Especialización");
            AddRow("Apto para", vacuno.AptoPara);
            AddRow("Fecha Especificación", FormatDate(vacuno.FechaEspecificacion));
            currentRow++;

            // Observaciones
            AddSectionHeader("Observaciones y Detalles");
            AddRow("Observaciones", vacuno.Observaciones);
            AddRow("Motivo Estado", vacuno.MotivoEstado);
            currentRow++;

            // Auditoría
            AddSectionHeader("Auditoría");
            AddRow("Creado por", vacuno.CreadoPor);
            AddRow("Creado en", FormatDateTime(vacuno.CreadoEn));
            AddRow("Actualizado por", vacuno.ActualizadoPor);
            AddRow("Actualizado en", FormatDateTime(vacuno.ActualizadoEn));

            worksheet.Column(1).Width = 30;
            worksheet.Column(2).Width = 50;

            // Outer border for the whole table content
            var tableRange = worksheet.Range(1, 1, currentRow - 1, 2);
            tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            tableRange.Style.Border.OutsideBorderColor = XLColor.FromHtml("#166534");

            workbook.SaveAs(filePath);
        }

        var downloadUrl = $"{urlBase}{Uri.EscapeDataString(fileName)}";
        return new RegistroVacunoExcelReportResult(fileName, downloadUrl);
    }

    private static string SanitizeFileNamePart(string value)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var safe = new string(value
            .Where(ch => !invalidChars.Contains(ch) && !char.IsWhiteSpace(ch))
            .ToArray());

        return string.IsNullOrWhiteSpace(safe) ? "vacuno" : safe;
    }
}
