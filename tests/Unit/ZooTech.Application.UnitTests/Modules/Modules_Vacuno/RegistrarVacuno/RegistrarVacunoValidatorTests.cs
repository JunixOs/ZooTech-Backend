using FluentValidation.TestHelper;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.RegistrarVacuno;
using Xunit;

namespace ZooTech.Application.UnitTests.Modules.Module_Vacuno.RegistrarVacuno;

public class RegistrarVacunoValidatorTests
{
    private readonly RegistrarVacunoValidator _sut;

    public RegistrarVacunoValidatorTests()
    {
        _sut = new RegistrarVacunoValidator();
    }

    // ─────────────────────────────────────────────────────────
    // Helpers
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
    // CÓDIGO
    // ─────────────────────────────────────────────────────────

    [Fact]
    public void Codigo_Requerido_DebefallarSiEsNulo()
    {
        var cmd = ComandoValido();
        cmd.Codigo = null!;
        _sut.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Codigo);
    }

    [Fact]
    public void Codigo_Requerido_DebefallarSiEsVacio()
    {
        var cmd = ComandoValido();
        cmd.Codigo = "";
        _sut.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Codigo);
    }

    [Fact]
    public void Codigo_MaxLength_DebefallarSiSuperaLos10Caracteres()
    {
        var cmd = ComandoValido();
        cmd.Codigo = "VACA0012345"; // 11 chars
        _sut.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Codigo);
    }

    [Fact]
    public void Codigo_MaxLength_DebePermitir10Caracteres()
    {
        var cmd = ComandoValido();
        cmd.Codigo = "VACA001234"; // 10 chars
        _sut.TestValidate(cmd).ShouldNotHaveValidationErrorFor(x => x.Codigo);
    }

    [Theory]
    [InlineData("vaca001")]
    [InlineData("Vaca001")]
    [InlineData("vACA001")]
    public void Codigo_DebeEstarEnMayusculas(string codigo)
    {
        var cmd = ComandoValido();
        cmd.Codigo = codigo;
        _sut.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Codigo);
    }

    // ─────────────────────────────────────────────────────────
    // NOMBRE
    // ─────────────────────────────────────────────────────────

    [Fact]
    public void Nombre_Requerido_DebefallarSiEsNulo()
    {
        var cmd = ComandoValido();
        cmd.Nombre = null!;
        _sut.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Nombre);
    }

    [Fact]
    public void Nombre_MaxLength_DebefallarSiSuperaLos15Caracteres()
    {
        var cmd = ComandoValido();
        cmd.Nombre = "NombreMuyLargoX"; // 15 chars — probar 16
        cmd.Nombre = "NombreMuyLargoXY"; // 16 chars
        _sut.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Nombre);
    }

    // ─────────────────────────────────────────────────────────
    // FECHA DE NACIMIENTO
    // ─────────────────────────────────────────────────────────

    [Fact]
    public void FechaNacimiento_Requerido_DebefallarSiEsDefault()
    {
        var cmd = ComandoValido();
        cmd.FechaNacimiento = default;
        _sut.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.FechaNacimiento);
    }

    // ─────────────────────────────────────────────────────────
    // ADQUISICIÓN POR
    // ─────────────────────────────────────────────────────────

    [Theory]
    [InlineData("monta")]
    [InlineData("compra")]
    public void AdquisicionPor_EnumValido_NoDebebFallar(string valor)
    {
        var cmd = ComandoValido();
        cmd.AdquisicionPor = valor;
        if (valor == "compra") cmd.PrecioCompra = 500m;
        _sut.TestValidate(cmd).ShouldNotHaveValidationErrorFor(x => x.AdquisicionPor);
    }

    [Fact]
    public void AdquisicionPor_EnumInvalido_DebeFallar()
    {
        var cmd = ComandoValido();
        cmd.AdquisicionPor = "donacion";
        _sut.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.AdquisicionPor);
    }

    // ─────────────────────────────────────────────────────────
    // PRECIO COMPRA — lógica condicional
    // ─────────────────────────────────────────────────────────

    [Fact]
    public void PrecioCompra_EsObligatorioSiAdquisicionEsCompra()
    {
        var cmd = ComandoValido();
        cmd.AdquisicionPor = "compra";
        cmd.PrecioCompra = null; // falta
        _sut.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.PrecioCompra);
    }

    [Fact]
    public void PrecioCompra_NoEsObligatorioSiAdquisicionEsMonta()
    {
        var cmd = ComandoValido();
        cmd.AdquisicionPor = "monta";
        cmd.PrecioCompra = null;
        _sut.TestValidate(cmd).ShouldNotHaveValidationErrorFor(x => x.PrecioCompra);
    }

    [Fact]
    public void PrecioCompra_DebeSerNumericoPositivo()
    {
        var cmd = ComandoValido();
        cmd.AdquisicionPor = "compra";
        cmd.PrecioCompra = -100m;
        _sut.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.PrecioCompra);
    }

    [Fact]
    public void PrecioCompra_ValorPositivo_NoDebebFallar()
    {
        var cmd = ComandoValido();
        cmd.AdquisicionPor = "compra";
        cmd.PrecioCompra = 250.50m;
        _sut.TestValidate(cmd).ShouldNotHaveValidationErrorFor(x => x.PrecioCompra);
    }

    // ─────────────────────────────────────────────────────────
    // RAZA / COLOR
    // ─────────────────────────────────────────────────────────

    [Fact]
    public void Raza_Requerido_DebeFallarSiEsNula()
    {
        var cmd = ComandoValido();
        cmd.Raza = null!;
        _sut.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Raza);
    }

    [Fact]
    public void Color_MaxLength_DebeFallarSiSuperaLos15Caracteres()
    {
        var cmd = ComandoValido();
        cmd.Color = "ColorMuyLargoXYZ"; // 16 chars
        _sut.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Color);
    }

    // ─────────────────────────────────────────────────────────
    // SEXO
    // ─────────────────────────────────────────────────────────

    [Theory]
    [InlineData("macho")]
    [InlineData("hembra")]
    public void Sexo_EnumValido_NoDebebFallar(string sexo)
    {
        var cmd = ComandoValido();
        cmd.Sexo = sexo;
        _sut.TestValidate(cmd).ShouldNotHaveValidationErrorFor(x => x.Sexo);
    }

    [Fact]
    public void Sexo_EnumInvalido_DebeFallar()
    {
        var cmd = ComandoValido();
        cmd.Sexo = "neutro";
        _sut.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Sexo);
    }

    // ─────────────────────────────────────────────────────────
    // CÓDIGOS PADRE / MADRE
    // ─────────────────────────────────────────────────────────

    [Fact]
    public void CodigoPadre_MaxLength_DebeFallarSiSuperaLos10Caracteres()
    {
        var cmd = ComandoValido();
        cmd.CodigoPadre = "TORO0123456"; // 11 chars
        _sut.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.CodigoPadre);
    }

    [Theory]
    [InlineData("toro001")]
    [InlineData("Toro001")]
    public void CodigoPadre_DebeEstarEnMayusculas(string codigo)
    {
        var cmd = ComandoValido();
        cmd.CodigoPadre = codigo;
        _sut.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.CodigoPadre);
    }

    [Theory]
    [InlineData("vaca001")]
    [InlineData("Vaca001")]
    public void CodigoMadre_DebeEstarEnMayusculas(string codigo)
    {
        var cmd = ComandoValido();
        cmd.CodigoMadre = codigo;
        _sut.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.CodigoMadre);
    }

    // ─────────────────────────────────────────────────────────
    // GRANJA
    // ─────────────────────────────────────────────────────────

    [Fact]
    public void Granja_MaxLength_DebeFallarSiSuperaLos15Caracteres()
    {
        var cmd = ComandoValido();
        cmd.Granja = "GranjaMuyLargaXY"; // 16 chars
        _sut.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Granja);
    }

    // ─────────────────────────────────────────────────────────
    // CAMPOS GEOGRÁFICOS
    // ─────────────────────────────────────────────────────────

    [Theory]
    [InlineData(nameof(RegistrarVacunoCommand.Distrito))]
    [InlineData(nameof(RegistrarVacunoCommand.Departamento))]
    [InlineData(nameof(RegistrarVacunoCommand.Provincia))]
    public void CamposGeograficos_SonObligatorios(string campo)
    {
        var cmd = ComandoValido();
        typeof(RegistrarVacunoCommand).GetProperty(campo)!.SetValue(cmd, null);
        var result = _sut.TestValidate(cmd);
        result.ShouldHaveAnyValidationError();
    }

    // ─────────────────────────────────────────────────────────
    // APTO PARA
    // ─────────────────────────────────────────────────────────

    [Theory]
    [InlineData("produccion_leche")]
    [InlineData("carne")]
    [InlineData("reproduccion")]
    public void AptoPara_EnumValido_NoDebebFallar(string valor)
    {
        var cmd = ComandoValido();
        cmd.AptoPara = valor;
        _sut.TestValidate(cmd).ShouldNotHaveValidationErrorFor(x => x.AptoPara);
    }

    [Fact]
    public void AptoPara_EnumInvalido_DebeFallar()
    {
        var cmd = ComandoValido();
        cmd.AptoPara = "trabajo";
        _sut.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.AptoPara);
    }

    // ─────────────────────────────────────────────────────────
    // OBSERVACIONES
    // ─────────────────────────────────────────────────────────

    [Fact]
    public void Observaciones_MaxLength_DebeFallarSiSuperaLos150Caracteres()
    {
        var cmd = ComandoValido();
        cmd.Observaciones = new string('a', 151);
        _sut.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Observaciones);
    }

    [Fact]
    public void Observaciones_MaxPalabras_DebeFallarSiSuperaLas30Palabras()
    {
        var cmd = ComandoValido();
        // 31 palabras
        cmd.Observaciones = string.Join(" ", Enumerable.Range(1, 31).Select(i => $"palabra{i}"));
        _sut.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Observaciones);
    }

    [Fact]
    public void Observaciones_Opcional_NoDebebFallarSiEsNula()
    {
        var cmd = ComandoValido();
        cmd.Observaciones = null;
        _sut.TestValidate(cmd).ShouldNotHaveValidationErrorFor(x => x.Observaciones);
    }

    // ─────────────────────────────────────────────────────────
    // FOTO — formato de imagen
    // ─────────────────────────────────────────────────────────

    [Theory]
    [InlineData("foto.png")]
    [InlineData("foto.jpg")]
    [InlineData("foto.jpeg")]
    public void Foto_FormatoValido_NoDebebFallar(string nombreArchivo)
    {
        var cmd = ComandoValido();
        cmd.Foto = CrearFotoFake(nombreArchivo);
        _sut.TestValidate(cmd).ShouldNotHaveValidationErrorFor(x => x.Foto);
    }

    [Theory]
    [InlineData("foto.gif")]
    [InlineData("foto.bmp")]
    [InlineData("foto.pdf")]
    [InlineData("foto.webp")]
    public void Foto_FormatoInvalido_DebeFallar(string nombreArchivo)
    {
        var cmd = ComandoValido();
        cmd.Foto = CrearFotoFake(nombreArchivo);
        _sut.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Foto);
    }

    [Fact]
    public void Foto_Opcional_NoDebebFallarSiEsNula()
    {
        var cmd = ComandoValido();
        cmd.Foto = null;
        _sut.TestValidate(cmd).ShouldNotHaveValidationErrorFor(x => x.Foto);
    }

    // ─────────────────────────────────────────────────────────
    // COMANDO COMPLETO VÁLIDO
    // ─────────────────────────────────────────────────────────

    [Fact]
    public void ComandoValido_NoDebebTenerErrores()
    {
        _sut.TestValidate(ComandoValido()).ShouldNotHaveAnyValidationErrors();
    }

    // ─────────────────────────────────────────────────────────
    // Utilidad interna
    // ─────────────────────────────────────────────────────────

    /// <summary>
    /// Simula un IFormFile con el nombre dado.
    /// Ajusta según cómo esté tipado Foto en RegistrarVacunoCommand
    /// (IFormFile de ASP.NET o un wrapper propio).
    /// </summary>
    private static Microsoft.AspNetCore.Http.IFormFile CrearFotoFake(string nombre)
    {
        var stream = new MemoryStream(new byte[] { 0xFF, 0xD8 }); // magic bytes JPEG
        var formFile = new Microsoft.AspNetCore.Http.FormFile(
            stream, 0, stream.Length, "foto", nombre)
        {
            Headers = new Microsoft.AspNetCore.Http.HeaderDictionary(),
            ContentType = nombre.EndsWith(".png") ? "image/png" : "image/jpeg",
        };
        return formFile;
    }
}