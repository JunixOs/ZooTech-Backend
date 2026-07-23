using ClosedXML.Excel;
using FluentAssertions;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Common;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosExcel;
using ZooTech.Infrastructure.Common.Services.ExcelGenerator;

namespace ZooTech.Infrastructure.UnitTests.Common.Services.ExcelGenerator;

public class ExcelGenerateComparationServiceTests
{
    [Fact]
    public void GenerateOrdeniosReport_WithoutVacunoId_ReturnsWorkbookWithComparisonTables()
    {
        var service = new ExcelGenerateComparationService();
        var generatedAt = new DateTime(2026, 7, 2, 10, 0, 0, DateTimeKind.Utc);
        var document = new GenerateOrdeniosExcelDocument(
            new[]
            {
                new OrdenioListOutput(1, "ORD-001", generatedAt.AddDays(-2), 1, "Luna", "VAC-001", 2, "Juan Perez", 10, "ACTIVO", null, generatedAt, generatedAt),
                new OrdenioListOutput(2, "ORD-002", generatedAt.AddDays(-2), 2, "Estrella", "VAC-002", 3, "Maria Lopez", 14, "ACTIVO", null, generatedAt, generatedAt),
                new OrdenioListOutput(3, "ORD-003", generatedAt.AddDays(-1), 1, "Luna", "VAC-001", 2, "Juan Perez", 8, "ACTIVO", null, generatedAt, generatedAt)
            },
            VacunoId: null,
            EstadoOrdenioCode: null,
            FechaDesde: null,
            FechaHasta: null,
            GeneratedAtUtc: generatedAt);

        var result = service.GenerateOrdeniosReport(document);

        result.Should().NotBeNullOrEmpty();
        using var workbook = new XLWorkbook(new MemoryStream(result));
        var worksheet = workbook.Worksheet("Comparativo");
        worksheet.Cell(1, 1).GetString().Should().Be("Comparativo de produccion por vacunos");
        worksheet.Cell(5, 1).GetString().Should().Be("Produccion individual");
        worksheet.Cell(5, 5).GetString().Should().Be("Promedio diario");
        worksheet.Cell(8, 2).GetString().Should().Be("Luna");
        worksheet.Cell(8, 3).GetDouble().Should().Be(10);
        worksheet.Cell(8, 6).GetDouble().Should().Be(12);
    }

    [Fact]
    public void GenerateOrdeniosReport_WithVacunoId_ReturnsWorkbookWithTrendTable()
    {
        var service = new ExcelGenerateComparationService();
        var generatedAt = new DateTime(2026, 7, 2, 10, 0, 0, DateTimeKind.Utc);
        var document = new GenerateOrdeniosExcelDocument(
            new[]
            {
                new OrdenioListOutput(1, "ORD-001", generatedAt.AddDays(-2), 1, "Luna", "VAC-001", 2, "Juan Perez", 10, "ACTIVO", null, generatedAt, generatedAt),
                new OrdenioListOutput(2, "ORD-002", generatedAt.AddDays(-1), 1, "Luna", "VAC-001", 2, "Juan Perez", 14, "ACTIVO", null, generatedAt, generatedAt)
            },
            VacunoId: 1,
            EstadoOrdenioCode: null,
            FechaDesde: null,
            FechaHasta: null,
            GeneratedAtUtc: generatedAt);

        var result = service.GenerateOrdeniosReport(document);

        result.Should().NotBeNullOrEmpty();
        using var workbook = new XLWorkbook(new MemoryStream(result));
        var worksheet = workbook.Worksheet("Comparativo");
        worksheet.Cell(1, 1).GetString().Should().Be("Comparativo de produccion de Luna");
        worksheet.Cell(5, 1).GetString().Should().Be("Historial de produccion");
        worksheet.Cell(7, 3).GetString().Should().Be("Tendencia");
        worksheet.Cell(8, 2).GetDouble().Should().Be(10);
        worksheet.Cell(9, 3).GetDouble().Should().Be(12);
    }

    [Fact]
    public void ComputeActualSeries_MultipleRecords_ReturnsIndividualPointsNotAggregated()
    {
        var date = new DateTime(2026, 7, 1, 8, 0, 0, DateTimeKind.Utc);
        var items = new[]
        {
            new OrdenioListOutput(1, "ORD-001", date, 1, "Luna", "VAC-001", 2, "Juan", 10m, "ACTIVO", null, date, date),
            new OrdenioListOutput(2, "ORD-002", date, 2, "Estrella", "VAC-002", 2, "Maria", 6m, "ACTIVO", null, date, date)
        };

        var result = ExcelGenerateComparationService.ComputeActualSeries(items);

        result.Should().HaveCount(2);
        result[0].Value.Should().Be(10f);
        result[1].Value.Should().Be(6f);
    }

    [Fact]
    public void ComputeReferenceSeries_MultipleVacunosSameDate_ReturnsDailyAverage()
    {
        var date = new DateTime(2026, 7, 1, 8, 0, 0, DateTimeKind.Utc);
        var items = new[]
        {
            new OrdenioListOutput(1, "ORD-001", date, 1, "Luna", "VAC-001", 2, "Juan", 10m, "ACTIVO", null, date, date),
            new OrdenioListOutput(2, "ORD-002", date, 2, "Estrella", "VAC-002", 2, "Maria", 6m, "ACTIVO", null, date, date)
        };

        var result = ExcelGenerateComparationService.ComputeReferenceSeries(items);

        result.Should().HaveCount(1);
        result[0].Value.Should().Be(8f);
        result[0].Label.Should().Be("01/07/2026");
    }
}
