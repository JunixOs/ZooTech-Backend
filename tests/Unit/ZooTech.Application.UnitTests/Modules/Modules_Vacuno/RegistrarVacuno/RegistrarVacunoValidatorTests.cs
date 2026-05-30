using ZooTech.Application.Modules.Module_Vacuno.UseCases.RegistrarVacuno;

namespace ZooTech.Application.UnitTests.Modules.Module_Vacuno.RegistrarVacuno;

public sealed class RegistrarVacunoValidatorTests
{
    private readonly RegistrarVacunoValidator _validator = new();

    [Fact]
    public void ComandoValido_NoTieneErrores()
    {
        var result = _validator.Validate(ComandoValido());

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("", "Codigo")]
    [InlineData("VACUNO12345", "Codigo")]
    [InlineData("vaca001", "Codigo")]
    public void Codigo_Invalido_DebeFallar(string codigo, string property)
    {
        var command = ComandoValido() with { Codigo = codigo };

        Assert.Contains(_validator.Validate(command).Errors, e => e.PropertyName == property);
    }

    [Fact]
    public void CamposObligatorios_DebenFallar()
    {
        var command = ComandoValido() with
        {
            Nombre = "",
            IdRaza = 0,
            IdColor = 0,
            IdSexo = 0,
            IdDistrito = 0,
            IdDepartamento = 0,
            IdProvincia = 0,
            IdTipoUtilizacion = 0
        };

        var errors = _validator.Validate(command).Errors.Select(e => e.PropertyName).ToHashSet();

        Assert.Contains("Nombre", errors);
        Assert.Contains("IdRaza", errors);
        Assert.Contains("IdColor", errors);
        Assert.Contains("IdSexo", errors);
        Assert.Contains("IdDistrito", errors);
        Assert.Contains("IdDepartamento", errors);
        Assert.Contains("IdProvincia", errors);
        Assert.Contains("IdTipoUtilizacion", errors);
    }

    [Fact]
    public void Compra_RequierePrecioMayorACero()
    {
        var sinPrecio = ComandoValido() with { IdTipoAdquisicion = 2, PrecioCompra = null };
        var precioNegativo = ComandoValido() with { IdTipoAdquisicion = 2, PrecioCompra = -1 };

        Assert.Contains(_validator.Validate(sinPrecio).Errors, e => e.PropertyName == "PrecioCompra");
        Assert.Contains(_validator.Validate(precioNegativo).Errors, e => e.PropertyName == "PrecioCompra");
    }

    [Fact]
    public void Monta_NoPermitePrecioCompra()
    {
        var command = ComandoValido() with { IdTipoAdquisicion = 1, PrecioCompra = 100 };

        Assert.Contains(_validator.Validate(command).Errors, e => e.PropertyName == "PrecioCompra");
    }

    [Fact]
    public void Observaciones_RespetaLimites()
    {
        var muchosCaracteres = ComandoValido() with { Observaciones = new string('a', 151) };
        var muchasPalabras = ComandoValido() with
        {
            Observaciones = string.Join(" ", Enumerable.Range(1, 31).Select(i => $"palabra{i}"))
        };

        Assert.Contains(_validator.Validate(muchosCaracteres).Errors, e => e.PropertyName == "Observaciones");
        Assert.Contains(_validator.Validate(muchasPalabras).Errors, e => e.PropertyName == "Observaciones");
    }

    [Theory]
    [InlineData("foto.png", true)]
    [InlineData("foto.jpg", true)]
    [InlineData("foto.jpeg", true)]
    [InlineData("foto.gif", false)]
    [InlineData("foto.pdf", false)]
    public void Foto_ValidaExtensionPermitida(string fileName, bool valido)
    {
        var command = ComandoValido() with
        {
            FotoStream = new MemoryStream([1, 2, 3]),
            FotoNombreOriginal = fileName
        };

        Assert.Equal(valido, _validator.Validate(command).IsValid);
    }

    public static RegistrarVacunoCommand ComandoValido() => new()
    {
        Codigo = "VACA001",
        Nombre = "Lola",
        FechaNacimiento = DateOnly.FromDateTime(DateTime.Today.AddYears(-2)),
        IdTipoAdquisicion = 1,
        PrecioCompra = null,
        IdRaza = 3,
        Raza = "Angus",
        IdColor = 1,
        Color = "Negro",
        IdSexo = 2,
        Sexo = "hembra",
        CodigoPadre = "TORO001",
        CodigoMadre = "VACA002",
        NombreGranja = "Granja Norte",
        IdDistrito = 1,
        Distrito = "Tocache",
        IdDepartamento = 1,
        Departamento = "San Martin",
        IdProvincia = 1,
        Provincia = "Tocache",
        IdTipoUtilizacion = 1,
        AptoPara = "produccion_leche",
        FechaEspecificacion = DateOnly.FromDateTime(DateTime.Today),
        Observaciones = null
    };
}
