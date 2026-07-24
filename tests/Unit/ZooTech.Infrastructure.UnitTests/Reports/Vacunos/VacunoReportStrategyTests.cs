using System.Text;
using ClosedXML.Excel;
using FluentAssertions;
using Moq;
using QuestPDF.Infrastructure;
using ZooTech.Application.Common.Gateway.Reports;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ExportarActividadVacunos;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ExportarArbolGenealogico;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GetActivityStats;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ObtenerRegistroVacunoReporte;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Entities;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Entities.GetArbolGenealogico;
using ZooTech.Infrastructure.Common.Export;
using ZooTech.Infrastructure.Reports.Vacunos;

namespace ZooTech.Infrastructure.UnitTests.Reports.Vacunos;

public sealed class VacunoReportStrategyTests
{
    [Fact]
    public async Task ActividadExcelAndPdf_GenerateValidDocuments()
    {
        var renderers = CreateRenderers();
        var model = new ActividadVacunosReportModel(
            new GetActivityStatsOutput(
                "2026-07-01",
                "2026-07-02",
                [new("2026-07-01", 10), new("2026-07-02", 12)],
                12,
                10));

        var excel = await new ActividadVacunosExcelReportStrategy(renderers.Excel)
            .GenerateAsync(model);
        var pdf = await new ActividadVacunosPdfReportStrategy(renderers.Pdf)
            .GenerateAsync(model);

        AssertExcel(excel, "Actividad de vacunos");
        AssertPdf(pdf);
    }

    [Fact]
    public async Task GenealogiaExcelAndPdf_GenerateValidDocuments()
    {
        var renderers = CreateRenderers();
        var root = CreateDomainVacuno();
        var model = new GenealogiaVacunoReportModel(
            [new VacunoGenealogiaNode(root, 1, "UNAS")],
            root);

        var excel = await new GenealogiaVacunoExcelReportStrategy(renderers.Excel)
            .GenerateAsync(model);
        var pdf = await new GenealogiaVacunoPdfReportStrategy(renderers.Pdf)
            .GenerateAsync(model);

        AssertExcel(excel, "Árbol genealógico");
        AssertPdf(pdf);
    }

    [Fact]
    public async Task RegistroExcelAndPdf_GenerateValidDocuments()
    {
        var renderers = CreateRenderers();
        var model = CreateRegistroVacuno();
        var photoLoader = new Mock<IVacunoReportPhotoLoader>();
        photoLoader
            .Setup(loader => loader.LoadAsync(model, It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);

        var excel = await new RegistroVacunoExcelReportService(renderers.Excel)
            .GenerateAsync(model);
        var pdf = await new RegistroVacunoPdfReportService(renderers.Pdf, photoLoader.Object)
            .GenerateAsync(model);

        AssertExcel(excel, "Registro vacuno");
        AssertPdf(pdf);
        photoLoader.Verify(
            loader => loader.LoadAsync(model, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task RegistroPdf_WhenPhotoIsValid_GeneratesDocumentWithImage()
    {
        var renderers = CreateRenderers();
        var model = CreateRegistroVacuno();
        var photo = Convert.FromBase64String(
            "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNk+A8AAQUBAScY42YAAAAASUVORK5CYII=");
        var photoLoader = new Mock<IVacunoReportPhotoLoader>();
        photoLoader
            .Setup(loader => loader.LoadAsync(model, It.IsAny<CancellationToken>()))
            .ReturnsAsync(photo);

        var document = await new RegistroVacunoPdfReportService(
                renderers.Pdf,
                photoLoader.Object)
            .GenerateAsync(model);

        AssertPdf(document);
    }

    private static void AssertExcel(GeneratedReportDocument document, string sheetName)
    {
        document.Extension.Should().Be(".xlsx");
        document.Content.Should().NotBeEmpty();
        using var workbook = new XLWorkbook(new MemoryStream(document.Content));
        workbook.Worksheets.Contains(sheetName).Should().BeTrue();
    }

    private static void AssertPdf(GeneratedReportDocument document)
    {
        document.Extension.Should().Be(".pdf");
        document.Content.Should().NotBeEmpty();
        Encoding.ASCII.GetString(document.Content, 0, 4).Should().Be("%PDF");
    }

    private static ReportRenderers CreateRenderers()
    {
        QuestPDF.Settings.License = LicenseType.Community;
        var branding = new Mock<ITenantReportBrandingProvider>();
        branding
            .Setup(provider => provider.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TenantReportBranding("Zootecnia UNAS", null));
        var time = new Mock<IDateTimeProvider>();
        time.SetupGet(provider => provider.ServerNow)
            .Returns(new DateTime(2026, 7, 22, 10, 0, 0, DateTimeKind.Utc));

        return new ReportRenderers(
            new StyledExcelReportRenderer(branding.Object, time.Object),
            new StyledPdfReportRenderer(branding.Object, time.Object));
    }

    private static Vacuno CreateDomainVacuno()
        => Vacuno.Rehydrate(
            1,
            "VAC-001",
            "Luna",
            new DateOnly(2020, 1, 1),
            "COMPRA",
            "HOLSTEIN",
            "NEGRO",
            "HEMBRA",
            null,
            null,
            1,
            null,
            new DateOnly(2026, 1, 1),
            new DateTime(2026, 1, 1),
            new DateTime(2026, 1, 1),
            null,
            null,
            1,
            1,
            null);

    private static RegistroVacunoDetalle CreateRegistroVacuno()
        => new(
            Id: 1,
            Codigo: "VAC-001",
            Nombre: "Luna",
            FechaNacimiento: new DateOnly(2020, 1, 1),
            AdquisicionPor: "Compra",
            PrecioCompra: null,
            Raza: "Holstein",
            Color: "Negro",
            Sexo: "Hembra",
            CodigoPadre: null,
            CodigoMadre: null,
            CodigoAbuelo: null,
            CodigoAbuela: null,
            Granja: "UNAS",
            Distrito: "Rupa-Rupa",
            Departamento: "Huánuco",
            Provincia: "Leoncio Prado",
            Procedencia: "Tingo María",
            AptoPara: "Leche",
            FechaEspecificacion: null,
            Observaciones: null,
            FotoId: null,
            FotoNombreOriginal: null,
            FotoNombreAlmacenado: null,
            FotoRuta: null,
            FotoUrl: null,
            FotoExtension: null,
            FotoTamanoBytes: null,
            EstadoActualCode: "SANO",
            EstadoActualNombre: "Sano",
            Estado: "vivo",
            FechaEstado: null,
            MotivoEstado: null,
            FechaRegistro: new DateOnly(2026, 1, 1),
            DiasRegistrado: 1,
            FechaAdquisicion: null,
            CreadoPor: "admin",
            CreadoEn: new DateTime(2026, 1, 1),
            ActualizadoPor: "admin",
            ActualizadoEn: new DateTime(2026, 1, 1));

    private sealed record ReportRenderers(
        StyledExcelReportRenderer Excel,
        StyledPdfReportRenderer Pdf);
}
