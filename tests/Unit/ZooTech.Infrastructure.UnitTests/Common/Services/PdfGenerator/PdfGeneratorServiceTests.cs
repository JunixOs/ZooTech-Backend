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
            Array.Empty<OrdenioOutput>(),
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
            Array.Empty<OrdenioOutput>(),
            VacunoId: null,
            EstadoOrdenioCode: null,
            FechaDesde: null,
            FechaHasta: null,
            GeneratedAtUtc: new DateTime(2026, 7, 2, 10, 0, 0, DateTimeKind.Utc));

        var result = service.GenerateOrdeniosReport(document);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }
}
