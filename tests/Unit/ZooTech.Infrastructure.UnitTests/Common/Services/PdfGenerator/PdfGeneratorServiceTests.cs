using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Common;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosPdf;
using ZooTech.Infrastructure.Common.Services.PdfGenerator;

namespace ZooTech.Infrastructure.UnitTests.Common.Services.PdfGenerator;

public class PdfGeneratorServiceTests
{
    [Fact]
    public void GenerateOrdeniosReport_WithEmptyItems_ReturnsPdfBytes()
    {
        var service = new PdfGeneratorService();
        var document = new GenerateOrdeniosPdfDocument(
            Array.Empty<OrdenioListOutput>(),
            VacunoId: null,
            EstadoOrdenioCode: null,
            FechaDesde: null,
            FechaHasta: null,
            GeneratedAtUtc: new DateTime(2026, 7, 2, 10, 0, 0, DateTimeKind.Utc));

        var result = service.GenerateOrdeniosReport(document);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public void GenerateComparationReport_WithEmptyItems_ReturnsPdfBytes()
    {
        var service = new PdfGenerateComparationService();
        var document = new GenerateOrdeniosPdfDocument(
            Array.Empty<OrdenioListOutput>(),
            VacunoId: null,
            EstadoOrdenioCode: null,
            FechaDesde: null,
            FechaHasta: null,
            GeneratedAtUtc: new DateTime(2026, 7, 2, 10, 0, 0, DateTimeKind.Utc));

        var result = service.GenerateOrdeniosReport(document);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public void GenerateComparationReport_WithoutVacunoId_ComparesVacunosAndReturnsPdfBytes()
    {
        var service = new PdfGenerateComparationService();
        var generatedAt = new DateTime(2026, 7, 2, 10, 0, 0, DateTimeKind.Utc);
        var document = new GenerateOrdeniosPdfDocument(
            new[]
            {
                new OrdenioListOutput(1, "ORD-001", generatedAt.AddDays(-2), 1, "Luna", "VAC-001", 2, "Juan Perez", 10, "ACTIVO", null, generatedAt, generatedAt),
                new OrdenioListOutput(2, "ORD-002", generatedAt.AddDays(-1), 1, "Luna", "VAC-001", 2, "Juan Perez", 8, "ACTIVO", null, generatedAt, generatedAt),
                new OrdenioListOutput(3, "ORD-003", generatedAt.AddDays(-1), 2, "Estrella", "VAC-002", 3, "Maria Lopez", 14, "ACTIVO", null, generatedAt, generatedAt)
            },
            VacunoId: null,
            EstadoOrdenioCode: null,
            FechaDesde: null,
            FechaHasta: null,
            GeneratedAtUtc: generatedAt);

        var result = service.GenerateOrdeniosReport(document);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public void GenerateComparationReport_WithVacunoId_UsesVacunoHistoryAndReturnsPdfBytes()
    {
        var service = new PdfGenerateComparationService();
        var generatedAt = new DateTime(2026, 7, 2, 10, 0, 0, DateTimeKind.Utc);
        var document = new GenerateOrdeniosPdfDocument(
            new[]
            {
                new OrdenioListOutput(1, "ORD-001", generatedAt.AddDays(-2), 1, "Luna", "VAC-001", 2, "Juan Perez", 10, "ACTIVO", null, generatedAt, generatedAt),
                new OrdenioListOutput(2, "ORD-002", generatedAt.AddDays(-1), 1, "Luna", "VAC-001", 2, "Juan Perez", 8, "ACTIVO", null, generatedAt, generatedAt),
                new OrdenioListOutput(3, "ORD-003", generatedAt, 1, "Luna", "VAC-001", 2, "Juan Perez", 14, "ACTIVO", null, generatedAt, generatedAt)
            },
            VacunoId: 1,
            EstadoOrdenioCode: null,
            FechaDesde: null,
            FechaHasta: null,
            GeneratedAtUtc: generatedAt);

        var result = service.GenerateOrdeniosReport(document);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }
}
