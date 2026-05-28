using System.IO.Compression;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ObtenerRegistroVacunoReporte;
using ZooTech.Infrastructure.Reports;

namespace ZooTech.Infrastructure.UnitTests.ReporteVacuno;

[TestClass]
public sealed class RegistroVacunoReportFileServiceTests
{
    [TestMethod]
    public async Task ExcelGenerateAsync_WithCompleteData_ShouldCreateXlsxWithExpectedNameAndSections()
    {
        var service = new RegistroVacunoExcelReportService();
        var vacuno = CreateDetalleCompleto();

        var result = await service.GenerateAsync(vacuno);
        var path = ResolveGeneratedFilePath(result.FileName);

        Assert.IsTrue(Regex.IsMatch(result.FileName, @"^reporte_VACA001_\d{8}\.xlsx$"), result.FileName);
        Assert.AreEqual($"/reportes/vacunos/{Uri.EscapeDataString(result.FileName)}", result.DownloadUrl);
        Assert.IsTrue(File.Exists(path), path);
        Assert.AreEqual(".xlsx", Path.GetExtension(path));

        using var archive = ZipFile.OpenRead(path);
        Assert.IsNotNull(archive.GetEntry("xl/workbook.xml"));
        var workbookText = ReadAllZipText(archive);
        StringAssert.Contains(workbookText, "Datos de Identificación");
        StringAssert.Contains(workbookText, "Trazabilidad");
        StringAssert.Contains(workbookText, "Especialización");
        StringAssert.Contains(workbookText, "Observaciones");
        StringAssert.Contains(workbookText, "VACA001");
    }

    [TestMethod]
    public async Task ExcelGenerateAsync_WithMinimumData_ShouldCreateXlsxWithoutNullErrors()
    {
        var service = new RegistroVacunoExcelReportService();
        var vacuno = CreateDetalleMinimo();

        var result = await service.GenerateAsync(vacuno);
        var path = ResolveGeneratedFilePath(result.FileName);

        Assert.IsTrue(File.Exists(path), path);
        using var archive = ZipFile.OpenRead(path);
        var workbookText = ReadAllZipText(archive);
        StringAssert.Contains(workbookText, "Datos de Identificación");
        StringAssert.Contains(workbookText, "Observaciones");
    }

    [TestMethod]
    public async Task PdfGenerateAsync_WithMinimumData_ShouldCreateReadablePdfWithExpectedNameAndSections()
    {
        var service = new RegistroVacunoPdfReportService();
        var vacuno = CreateDetalleMinimo();

        var result = await service.GenerateAsync(vacuno);
        var path = ResolveGeneratedFilePath(result.FileName);
        var content = File.ReadAllText(path);

        Assert.IsTrue(Regex.IsMatch(result.FileName, @"^reporte_VACA001_\d{8}\.pdf$"), result.FileName);
        Assert.AreEqual($"/reportes/vacunos/{Uri.EscapeDataString(result.FileName)}", result.DownloadUrl);
        Assert.IsTrue(File.Exists(path), path);
        Assert.AreEqual(".pdf", Path.GetExtension(path));
        Assert.IsTrue(content.StartsWith("%PDF-1.4"));
        StringAssert.Contains(content, "ZooTech | Modulo Vacuno");
        StringAssert.Contains(content, "Datos de Identificacion");
        StringAssert.Contains(content, "Trazabilidad");
        StringAssert.Contains(content, "Especializacion");
        StringAssert.Contains(content, "Observaciones");
        StringAssert.Contains(content, "Sin foto disponible");
    }

    [TestMethod]
    public async Task PdfGenerateAsync_WithLocalJpegPhoto_ShouldIncludePhotoReferenceWithoutFailing()
    {
        var photoDirectory = Path.Combine(AppContext.BaseDirectory, "wwwroot", "uploads", "vacunos");
        Directory.CreateDirectory(photoDirectory);
        var photoPath = Path.Combine(photoDirectory, "vaca001.jpg");
        await File.WriteAllBytesAsync(photoPath, MinimalJpegBytes);

        var service = new RegistroVacunoPdfReportService();
        var vacuno = CreateDetalleCompleto(fotoUrl: photoPath);

        var result = await service.GenerateAsync(vacuno);
        var path = ResolveGeneratedFilePath(result.FileName);
        var content = File.ReadAllText(path);

        Assert.IsTrue(File.Exists(path), path);
        StringAssert.Contains(content, "/Subtype /Image");
        StringAssert.Contains(content, "Incluida en el reporte");
    }

    private static string ResolveGeneratedFilePath(string fileName)
    {
        return Path.Combine(AppContext.BaseDirectory, "wwwroot", "reportes", "vacunos", fileName);
    }

    private static string ReadAllZipText(ZipArchive archive)
    {
        var parts = archive.Entries
            .Where(entry => entry.FullName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
            .Select(entry =>
            {
                using var reader = new StreamReader(entry.Open());
                return reader.ReadToEnd();
            });

        return WebUtility.HtmlDecode(string.Join(Environment.NewLine, parts));
    }

    private static RegistroVacunoDetalle CreateDetalleCompleto(string? fotoUrl = null)
    {
        return new RegistroVacunoDetalle(
            1,
            "VACA001",
            "Luna",
            new DateOnly(2022, 6, 10),
            "compra",
            1500.50m,
            "Angus",
            "Negro",
            "hembra",
            "TORO001",
            "VACA000",
            "ABU001",
            "ABU002",
            "Granja Norte",
            "Rupa-Rupa",
            "Leoncio Prado",
            "Huanuco",
            "Proveedor X",
            "produccion_leche",
            new DateOnly(2024, 3, 15),
            "Registro inicial",
            10,
            "vaca001.jpg",
            "vaca001.jpg",
            fotoUrl,
            fotoUrl,
            ".jpg",
            632,
            "VIVO",
            "Vivo",
            "vivo",
            new DateOnly(2024, 3, 15),
            "Alta inicial",
            new DateOnly(2024, 3, 15),
            new DateOnly(2024, 3, 15),
            "admin",
            new DateTime(2024, 3, 15, 8, 30, 0),
            "admin",
            new DateTime(2024, 3, 16, 9, 0, 0));
    }

    private static RegistroVacunoDetalle CreateDetalleMinimo()
    {
        return new RegistroVacunoDetalle(
            1,
            "VACA001",
            "Luna",
            new DateOnly(2022, 6, 10),
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            new DateOnly(2024, 3, 15),
            null,
            null,
            new DateTime(2024, 3, 15, 8, 30, 0),
            null,
            new DateTime(2024, 3, 15, 8, 30, 0));
    }

    private static readonly byte[] MinimalJpegBytes = Convert.FromBase64String(
        "/9j/4AAQSkZJRgABAQAAAQABAAD/2wBDAAgGBgcGBQgHBwcJCQgKDBQNDAsLDBkSEw8UHRofHh0aHBwgJC4nICIsIxwcKDcpLDAxNDQ0Hyc5PTgyPC4zNDL/2wBDAQkJCQwLDBgNDRgyIRwhMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjL/wAARCAABAAEDASIAAhEBAxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/8QAtRAAAgEDAwIEAwUFBAQAAAF9AQIDAAQRBRIhMUEGE1FhByJxFDKBkaEII0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4+Tl5ufo6erx8vP09fb3+Pn6/8QAHwEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL/8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQdhcRMiMoEIFEKRobHBCSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6goOEhYaHiImKkpOUlZaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3+Pn6/9oADAMBAAIRAxEAPwDi6KKK+ZP3E//Z");
}
