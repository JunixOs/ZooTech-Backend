using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ObtenerRegistroVacunoReporte;
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

    public void Dispose()
    {
        if (Directory.Exists(_testRoot))
        {
            Directory.Delete(_testRoot, true);
        }
    }

    private VacunoReportPhotoLoader CreateSut(string? mediaRoot = null)
        => new(
            Options.Create(new ReportStorageOptions
            {
                VacunoMediaRoot = mediaRoot ?? _testRoot
            }),
            NullLogger<VacunoReportPhotoLoader>.Instance);

    private static RegistroVacunoDetalle CreateVacuno(string? photoPath)
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
            FotoExtension: ".png",
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
