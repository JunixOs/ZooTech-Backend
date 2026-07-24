using System.Text;
using ClosedXML.Excel;
using FluentAssertions;
using Moq;
using QuestPDF.Infrastructure;
using ZooTech.Application.Common.Gateway.Reports;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ListarVacunosReporte;
using ZooTech.Infrastructure.Common.Export;
using ZooTech.Infrastructure.Reports.Vacunos;

namespace ZooTech.Infrastructure.UnitTests.Reports.Vacunos;

public sealed class ListadoVacunosReportServiceTests
{
    [Fact]
    public async Task Excel_GeneratesOpenableWorkbookWithExpectedHeaders()
    {
        var dependencies = CreateDependencies();
        var sut = new ListadoVacunosExcelReportService(
            new StyledExcelReportRenderer(
                dependencies.Branding.Object,
                dependencies.Time.Object));

        var document = await sut.GenerateAsync(
            new ListadoVacunosReportModel([CreateItem()], CreateFilters()));

        document.Content.Should().NotBeNullOrEmpty();
        document.ContentType.Should().Be(
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        document.Extension.Should().Be(".xlsx");
        using var workbook = new XLWorkbook(new MemoryStream(document.Content));
        var worksheet = workbook.Worksheet("Listado de vacunos");
        worksheet.Cell(1, 1).GetString().Should().Be("Zootecnia UNAS");
        worksheet.Cell(7, 1).GetString().Should().Be("Código");
        worksheet.Cell(8, 1).GetString().Should().Be("VAC-001");
    }

    [Fact]
    public async Task Pdf_GeneratesNonEmptyPdfContent()
    {
        QuestPDF.Settings.License = LicenseType.Community;
        var dependencies = CreateDependencies();
        var sut = new ListadoVacunosPdfReportService(
            new StyledPdfReportRenderer(
                dependencies.Branding.Object,
                dependencies.Time.Object));

        var document = await sut.GenerateAsync(
            new ListadoVacunosReportModel([CreateItem()], CreateFilters()));

        document.Content.Should().NotBeNullOrEmpty();
        document.ContentType.Should().Be("application/pdf");
        document.Extension.Should().Be(".pdf");
        Encoding.ASCII.GetString(document.Content, 0, 4).Should().Be("%PDF");
    }

    private static ReportDependencies CreateDependencies()
    {
        var branding = new Mock<ITenantReportBrandingProvider>();
        branding.Setup(provider => provider.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TenantReportBranding("Zootecnia UNAS", null));

        var time = new Mock<IDateTimeProvider>();
        time.SetupGet(provider => provider.ServerNow)
            .Returns(new DateTime(2026, 7, 22, 10, 0, 0, DateTimeKind.Utc));

        return new ReportDependencies(branding, time);
    }

    private static VacunoListadoReporteItem CreateItem()
        => new(
            1,
            "VAC-001",
            new DateOnly(2020, 1, 1),
            new DateOnly(2026, 1, 1),
            "Luna",
            "Compra",
            "Holstein",
            "Negro",
            "Hembra",
            "Granja UNAS",
            "Tingo María",
            "vivo",
            "activo");

    private static ReporteVacunoListadoFiltros CreateFilters()
        => new(
            null, null, null, null, null, null, null, null,
            null, null, null, "excel");

    private sealed record ReportDependencies(
        Mock<ITenantReportBrandingProvider> Branding,
        Mock<IDateTimeProvider> Time);
}
