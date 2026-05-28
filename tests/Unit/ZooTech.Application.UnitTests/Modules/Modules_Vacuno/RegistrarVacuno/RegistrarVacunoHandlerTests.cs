using FluentAssertions;
using Moq;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Repositories;
using ZooTech.Application.Common.Features;     // IArchivoService
using ZooTech.Application.Common.Time;          // ITimeProvider
using ZooTech.Application.Modules.Module_Vacuno.UseCases.RegistrarVacuno;
using ZooTech.Domain.Module_Vacuno.Entities;
using Xunit;

namespace ZooTech.Application.UnitTests.Modules.Module_Vacuno.RegistrarVacuno;

public class RegistrarVacunoHandlerTests
{
    // ─────────────────────────────────────────────────────────
    // Dependencias mockeadas
    // ─────────────────────────────────────────────────────────

    private readonly Mock<IVacunoRepository> _repoMock;
    private readonly Mock<IArchivoService> _archivoMock;
    private readonly Mock<ITimeProvider> _timeMock;
    private readonly RegistrarVacunoHandler _sut;

    public RegistrarVacunoHandlerTests()
    {
        _repoMock = new Mock<IVacunoRepository>();
        _archivoMock = new Mock<IArchivoService>();
        _timeMock = new Mock<ITimeProvider>();

        _timeMock.Setup(t => t.UtcNow).Returns(DateTime.UtcNow);

        _sut = new RegistrarVacunoHandler(
            _repoMock.Object,
            _archivoMock.Object,
            _timeMock.Object);
    }

    // ─────────────────────────────────────────────────────────
    // Helper
    // ─────────────────────────────────────────────────────────

    private static RegistrarVacunoCommand ComandoValido() => new()
    {
        Codigo = "VACA001",
        Nombre = "Lola",
        FechaNacimiento = DateOnly.FromDateTime(DateTime.Today.AddYears(-2)),
        AdquisicionPor = "monta",
        PrecioCompra = null,
        Raza = "Angus",
        Color = "Negro",
        Sexo = "hembra",
        CodigoPadre = "TORO001",
        CodigoMadre = "VACA002",
        Granja = "Granja Norte",
        Distrito = "Tocache",
        Departamento = "San Martín",
        Provincia = "Tocache",
        AptoPara = "produccion_leche",
        FechaEspecificacion = DateOnly.FromDateTime(DateTime.Today),
        Observaciones = null,
        Foto = null,
    };

    // ─────────────────────────────────────────────────────────
    // TK 09-A  Detectar duplicado por código
    // ─────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_CuandoCodigoYaExiste_DebeLanzarVacunoYaExisteException()
    {
        // Arrange
        var cmd = ComandoValido();
        _repoMock
            .Setup(r => r.ExisteConCodigoAsync(cmd.Codigo, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var act = async () => await _sut.Handle(cmd, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<VacunoYaExisteException>()
            .WithMessage("*VACA001*");
    }

    // ─────────────────────────────────────────────────────────
    // TK 09-B  No debe bloquear cuando el código es único
    // ─────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_CuandoCodigoNoExiste_DebeGuardarCorrectamente()
    {
        // Arrange
        var cmd = ComandoValido();

        _repoMock
            .Setup(r => r.ExisteConCodigoAsync(cmd.Codigo, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _repoMock
            .Setup(r => r.AgregarAsync(It.IsAny<Vacuno>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1); // ID generado

        // Act
        var resultado = await _sut.Handle(cmd, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(1);
        resultado.Codigo.Should().Be("VACA001");

        _repoMock.Verify(
            r => r.AgregarAsync(It.IsAny<Vacuno>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    // ─────────────────────────────────────────────────────────
    // TK 09-C  Código duplicado con distinta capitalización
    //          (el sistema normaliza a mayúsculas antes de buscar)
    // ─────────────────────────────────────────────────────────

    [Theory]
    [InlineData("VACA001")]
    [InlineData("vaca001")]
    [InlineData("Vaca001")]
    public async Task Handle_CuandoCodigoExisteConDistintaCapitalizacion_DebeLanzarExcepcion(
        string codigoDuplicado)
    {
        // Arrange
        var cmd = ComandoValido();
        cmd.Codigo = codigoDuplicado;

        // El repositorio recibe siempre el código en mayúsculas (lo normaliza el handler)
        _repoMock
            .Setup(r => r.ExisteConCodigoAsync(
                It.Is<string>(c => c == codigoDuplicado.ToUpper()),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var act = async () => await _sut.Handle(cmd, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<VacunoYaExisteException>();
    }

    // ─────────────────────────────────────────────────────────
    // TK 09-D  No se llama a AgregarAsync si hay duplicado
    // ─────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_CuandoExisteDuplicado_NoDebeGuardar()
    {
        // Arrange
        var cmd = ComandoValido();
        _repoMock
            .Setup(r => r.ExisteConCodigoAsync(cmd.Codigo, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        try { await _sut.Handle(cmd, CancellationToken.None); } catch { /* esperado */ }

        // Assert
        _repoMock.Verify(
            r => r.AgregarAsync(It.IsAny<Vacuno>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    // ─────────────────────────────────────────────────────────
    // TK 09-E  No sube foto si hay duplicado
    // ─────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_CuandoExisteDuplicado_NoDebeSubirFoto()
    {
        // Arrange
        var cmd = ComandoValido();
        cmd.Foto = CrearFotoFake("foto.jpg");

        _repoMock
            .Setup(r => r.ExisteConCodigoAsync(cmd.Codigo, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        try { await _sut.Handle(cmd, CancellationToken.None); } catch { }

        // Assert
        _archivoMock.Verify(
            a => a.GuardarAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    // ─────────────────────────────────────────────────────────
    // TK 09-F  Registro exitoso devuelve los campos del contrato
    // ─────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_RegistroExitoso_DebeRetornarIdCodigoNombreYFechas()
    {
        // Arrange
        var cmd = ComandoValido();

        _repoMock
            .Setup(r => r.ExisteConCodigoAsync(cmd.Codigo, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _repoMock
            .Setup(r => r.AgregarAsync(It.IsAny<Vacuno>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(42);

        // Act
        var resultado = await _sut.Handle(cmd, CancellationToken.None);

        // Assert — campos definidos en el Response del contrato API (POST /v1/vacunos)
        resultado.Id.Should().Be(42);
        resultado.Codigo.Should().Be("VACA001");
        resultado.Nombre.Should().Be("Lola");
        resultado.AdquisicionPor.Should().Be("monta");
        resultado.PrecioCompra.Should().BeNull();
        resultado.CreadoEn.Should().NotBe(default);
    }

    // ─────────────────────────────────────────────────────────
    // Utilidad interna
    // ─────────────────────────────────────────────────────────

    private static Microsoft.AspNetCore.Http.IFormFile CrearFotoFake(string nombre)
    {
        var stream = new MemoryStream(new byte[] { 0xFF, 0xD8 });
        return new Microsoft.AspNetCore.Http.FormFile(
            stream, 0, stream.Length, "foto", nombre)
        {
            Headers = new Microsoft.AspNetCore.Http.HeaderDictionary(),
            ContentType = "image/jpeg",
        };
    }
}