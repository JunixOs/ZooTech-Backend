using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using ZooTech.Application.Common.Gateway.Parametrization;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ObtenerRegistroVacunoReporte;
using ZooTech.Domain.Configuration;
using ZooTech.Infrastructure.Reports.Vacunos;
using ZooTech.Infrastructure.Storage;

namespace ZooTech.Infrastructure.UnitTests.Reports.Vacunos;

public sealed class VacunoReportPhotoLoaderTests : IDisposable
{
    private static readonly byte[] ValidPng = Convert.FromBase64String(
        "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNk+A8AAQUBAScY42YAAAAASUVORK5CYII=");
    private readonly string _testRoot =
        Path.Combine(Path.GetTempPath(), $"zootech-report-photo-{Guid.NewGuid():N}");

    [Fact]
    public async Task LoadAsync_WhenImageIsInsideConfiguredRoot_ReturnsContent()
    {
        Directory.CreateDirectory(_testRoot);
        var imagePath = Path.Combine(_testRoot, "vacuno.png");
        await File.WriteAllBytesAsync(imagePath, ValidPng);
        var sut = CreateSut();

        var result = await sut.LoadAsync(CreateVacuno("vacuno.png"));

        result.Should().Equal(ValidPng);
    }

    [Fact]
    public async Task LoadAsync_WhenRelativePathEscapesRoot_ReturnsNull()
    {
        var allowedRoot = Path.Combine(_testRoot, "allowed");
        Directory.CreateDirectory(allowedRoot);
        await File.WriteAllBytesAsync(Path.Combine(_testRoot, "outside.png"), ValidPng);
        var sut = CreateSut(allowedRoot);

        var result = await sut.LoadAsync(CreateVacuno("../outside.png"));

        result.Should().BeNull();
    }

    [Fact]
    public async Task LoadAsync_WhenAbsoluteOrFileUriIsOutsideRoot_ReturnsNull()
    {
        var allowedRoot = Path.Combine(_testRoot, "allowed");
        Directory.CreateDirectory(allowedRoot);
        var outsidePath = Path.Combine(_testRoot, "outside.png");
        await File.WriteAllBytesAsync(outsidePath, ValidPng);
        var sut = CreateSut(allowedRoot);

        var absoluteResult = await sut.LoadAsync(CreateVacuno(outsidePath));
        var fileUriResult = await sut.LoadAsync(CreateVacuno(new Uri(outsidePath).AbsoluteUri));

        absoluteResult.Should().BeNull();
        fileUriResult.Should().BeNull();
    }

    [Fact]
    public async Task LoadAsync_WhenImageSignatureIsInvalid_ReturnsNull()
    {
        Directory.CreateDirectory(_testRoot);
        await File.WriteAllTextAsync(Path.Combine(_testRoot, "not-image.png"), "not an image");
        var sut = CreateSut();

        var result = await sut.LoadAsync(CreateVacuno("not-image.png"));

        result.Should().BeNull();
    }

    [Fact]
    public async Task LoadAsync_WhenImageExceedsLimit_ReturnsNull()
    {
        Directory.CreateDirectory(_testRoot);
        var imagePath = Path.Combine(_testRoot, "large.png");
        await using (var stream = File.Create(imagePath))
        {
            stream.SetLength((5L * 1024 * 1024) + 1);
        }
        var sut = CreateSut();

        var result = await sut.LoadAsync(CreateVacuno("large.png"));

        result.Should().BeNull();
    }

    [Fact]
    public async Task LoadAsync_WhenTenantAllowsPng_ReturnsPngContent()
    {
        Directory.CreateDirectory(_testRoot);
        await File.WriteAllBytesAsync(Path.Combine(_testRoot, "vacuno.png"), ValidPng);
        var sut = CreateSut(configuredFormats: ".png");

        var result = await sut.LoadAsync(CreateVacuno("vacuno.png"));

        result.Should().Equal(ValidPng);
    }

    [Fact]
    public async Task LoadAsync_WhenTenantDoesNotAllowPng_ReturnsNull()
    {
        Directory.CreateDirectory(_testRoot);
        await File.WriteAllBytesAsync(Path.Combine(_testRoot, "vacuno.png"), ValidPng);
        var sut = CreateSut(configuredFormats: ".jpg");

        var result = await sut.LoadAsync(CreateVacuno("vacuno.png"));

        result.Should().BeNull();
    }

    [Theory]
    [InlineData(".jpg")]
    [InlineData(".jpeg")]
    public async Task LoadAsync_WhenTenantAllowsJpegAlias_ReturnsJpegContent(string configuredFormat)
    {
        byte[] validJpeg = [0xFF, 0xD8, 0xFF, 0xD9];
        Directory.CreateDirectory(_testRoot);
        await File.WriteAllBytesAsync(Path.Combine(_testRoot, "vacuno.jpg"), validJpeg);
        var sut = CreateSut(configuredFormats: configuredFormat);

        var result = await sut.LoadAsync(CreateVacuno("vacuno.jpg", ".jpg"));

        result.Should().Equal(validJpeg);
    }

    [Fact]
    public async Task LoadAsync_WhenExtensionDoesNotMatchSignature_ReturnsNull()
    {
        byte[] jpegContent = [0xFF, 0xD8, 0xFF, 0xD9];
        Directory.CreateDirectory(_testRoot);
        await File.WriteAllBytesAsync(Path.Combine(_testRoot, "vacuno.png"), jpegContent);
        var sut = CreateSut(configuredFormats: ".png,.jpg");

        var result = await sut.LoadAsync(CreateVacuno("vacuno.png"));

        result.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData(".gif")]
    public async Task LoadAsync_WhenTenantConfigurationIsInvalid_ReturnsNull(string configuredFormats)
    {
        Directory.CreateDirectory(_testRoot);
        await File.WriteAllBytesAsync(Path.Combine(_testRoot, "vacuno.png"), ValidPng);
        var sut = CreateSut(configuredFormats: configuredFormats);

        var result = await sut.LoadAsync(CreateVacuno("vacuno.png"));

        result.Should().BeNull();
    }

    public void Dispose()
    {
        if (Directory.Exists(_testRoot))
        {
            Directory.Delete(_testRoot, true);
        }
    }

    private VacunoReportPhotoLoader CreateSut(
        string? mediaRoot = null,
        string configuredFormats = ".png,.jpg")
    {
        var configurationProvider = new Mock<ITenantConfigurationProvider>();
        configurationProvider
            .Setup(provider => provider.GetSettingAsync(
                Settings.Vacunos.VacunosFotoFormatosPermitidos))
            .ReturnsAsync(configuredFormats);

        return new(
            Options.Create(new ReportStorageOptions
            {
                VacunoMediaRoot = mediaRoot ?? _testRoot
            }),
            configurationProvider.Object,
            NullLogger<VacunoReportPhotoLoader>.Instance);
    }

    private static RegistroVacunoDetalle CreateVacuno(
        string? photoPath,
        string? photoExtension = ".png")
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
            FotoId: 1,
            FotoNombreOriginal: null,
            FotoNombreAlmacenado: null,
            FotoRuta: photoPath,
            FotoUrl: null,
            FotoExtension: photoExtension,
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
}
