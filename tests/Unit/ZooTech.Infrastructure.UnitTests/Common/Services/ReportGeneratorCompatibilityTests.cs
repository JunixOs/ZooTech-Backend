using System.Text;
using ClosedXML.Excel;
using QuestPDF.Infrastructure;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Common;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosExcel;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosPdf;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.Common;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GenerateTriajesExcel;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GenerateTriajesPdf;
using ZooTech.Infrastructure.Common.Services.ExcelGenerator;
using ZooTech.Infrastructure.Common.Services.PdfGenerator;

namespace ZooTech.Infrastructure.UnitTests.Common.Services;

public sealed class ReportGeneratorCompatibilityTests
{
    private static readonly DateTime GeneratedAt =
        new(2026, 7, 23, 10, 30, 0, DateTimeKind.Utc);

    [Fact]
    public void ProduccionLeche_ExcelGenerator_RemainsCompatible()
    {
        var document = new GenerateOrdeniosExcelDocument(
            [CreateOrdenio()],
            VacunoId: 1,
            EstadoOrdenioCode: "ACTIVO",
            FechaDesde: GeneratedAt.AddDays(-1),
            FechaHasta: GeneratedAt,
            GeneratedAtUtc: GeneratedAt);

        var content = new ExcelGeneratorService().GenerateOrdeniosReport(document);

        AssertExcel(content, "Reporte de Ordenios", "Código", "ORD-001");
    }

    [Fact]
    public void ProduccionLeche_PdfGenerator_RemainsCompatible()
    {
        QuestPDF.Settings.License = LicenseType.Community;
        var document = new GenerateOrdeniosPdfDocument(
            [CreateOrdenio()],
            VacunoId: 1,
            EstadoOrdenioCode: "ACTIVO",
            FechaDesde: GeneratedAt.AddDays(-1),
            FechaHasta: GeneratedAt,
            GeneratedAtUtc: GeneratedAt);

        var content = new PdfGeneratorService().GenerateOrdeniosReport(document);

        AssertPdf(content);
    }

    [Fact]
    public void Sanidad_ExcelGenerator_RemainsCompatible()
    {
        var document = new GenerateTriajesExcelDocument(
            [CreateTriaje()],
            Fecha: null,
            Codigo: "TRI-001",
            Nombre: "Luna",
            TipoPeso: "CONTROL",
            PesoKg: 450,
            GeneratedAtUtc: GeneratedAt);

        var content = new ExcelGeneratorService().GenerateTriajesReport(document);

        AssertExcel(content, "Reporte de Triajes", "C.Registro", "TRI-001");
    }

    [Fact]
    public void Sanidad_PdfGenerator_RemainsCompatible()
    {
        QuestPDF.Settings.License = LicenseType.Community;
        var document = new GenerateTriajesPdfDocument(
            [CreateTriaje()],
            Fecha: null,
            Codigo: "TRI-001",
            Nombre: "Luna",
            TipoPeso: "CONTROL",
            PesoKg: 450,
            GeneratedAtUtc: GeneratedAt);

        var content = new PdfGeneratorService().GenerateTriajesReport(document);

        AssertPdf(content);
    }

    private static OrdenioOutput CreateOrdenio()
        => new(
            1,
            "ORD-001",
            GeneratedAt,
            1,
            "Luna",
            2,
            "Juan Pérez",
            12.5m,
            "ACTIVO",
            null,
            GeneratedAt,
            GeneratedAt);

    private static TriajeOutput CreateTriaje()
        => new(
            1,
            "TRI-001",
            GeneratedAt,
            1,
            "Luna",
            "CONTROL",
            450,
            null,
            "ACTIVO",
            2,
            GeneratedAt);

    private static void AssertExcel(
        byte[] content,
        string sheetName,
        string header,
        string expectedValue)
    {
        Assert.NotEmpty(content);
        using var workbook = new XLWorkbook(new MemoryStream(content));
        var worksheet = workbook.Worksheet(sheetName);
        Assert.Equal(header, worksheet.Cell(5, 1).GetString());
        Assert.Equal(expectedValue, worksheet.Cell(6, 1).GetString());
    }

    private static void AssertPdf(byte[] content)
    {
        Assert.NotEmpty(content);
        Assert.Equal("%PDF", Encoding.ASCII.GetString(content, 0, 4));
    }
}
