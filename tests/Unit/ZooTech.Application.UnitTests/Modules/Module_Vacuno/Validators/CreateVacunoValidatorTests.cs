using ZooTech.Application.Modules.Module_Vacuno.UseCases.CreateVacuno;
using ZooTech.Application.Modules.Module_Vacuno.Validators;

namespace ZooTech.Application.UnitTests.Modules.Module_Vacuno.Validators;

public class CreateVacunoValidatorTests
{
    private static CreateVacunoCommand ValidCommand(
        string codigo = "VAC001",
        string nombre = "Lola",
        string tipoAdquisicionCode = "COMPRA",
        string razaCode = "HOLSTEIN",
        string colorCode = "NEGRO",
        string sexoCode = "H",
        long granjaId = 1,
        string? observaciones = null)
    {
        return new CreateVacunoCommand(
            Codigo: codigo,
            Nombre: nombre,
            FechaNacimiento: new DateOnly(2020, 1, 1),
            TipoAdquisicionCode: tipoAdquisicionCode,
            RazaCode: razaCode,
            ColorCode: colorCode,
            SexoCode: sexoCode,
            PadreId: null,
            MadreId: null,
            GranjaId: granjaId,
            PrecioCompra: 1000m,
            AptoPara: "Carne",
            Observaciones: observaciones);
    }

    [Fact]
    public void Validate_WhenCommandIsValid_HasNoErrors()
    {
        var validator = new CreateVacunoValidator();

        var result = validator.Validate(ValidCommand());

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_WhenCodigoIsEmpty_HasError(string? codigo)
    {
        var validator = new CreateVacunoValidator();

        var result = validator.Validate(ValidCommand(codigo: codigo!));

        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateVacunoCommand.Codigo));
    }

    [Fact]
    public void Validate_WhenCodigoExceedsMaxLength_HasError()
    {
        var validator = new CreateVacunoValidator();

        var result = validator.Validate(ValidCommand(codigo: new string('A', 16)));

        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateVacunoCommand.Codigo));
    }

    [Fact]
    public void Validate_WhenNombreIsEmpty_HasError()
    {
        var validator = new CreateVacunoValidator();

        var result = validator.Validate(ValidCommand(nombre: string.Empty));

        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateVacunoCommand.Nombre));
    }

    [Fact]
    public void Validate_WhenNombreExceedsMaxLength_HasError()
    {
        var validator = new CreateVacunoValidator();

        var result = validator.Validate(ValidCommand(nombre: new string('A', 101)));

        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateVacunoCommand.Nombre));
    }

    [Fact]
    public void Validate_WhenTipoAdquisicionCodeIsEmpty_HasError()
    {
        var validator = new CreateVacunoValidator();

        var result = validator.Validate(ValidCommand(tipoAdquisicionCode: string.Empty));

        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateVacunoCommand.TipoAdquisicionCode));
    }

    [Fact]
    public void Validate_WhenRazaCodeIsEmpty_HasError()
    {
        var validator = new CreateVacunoValidator();

        var result = validator.Validate(ValidCommand(razaCode: string.Empty));

        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateVacunoCommand.RazaCode));
    }

    [Fact]
    public void Validate_WhenColorCodeIsEmpty_HasError()
    {
        var validator = new CreateVacunoValidator();

        var result = validator.Validate(ValidCommand(colorCode: string.Empty));

        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateVacunoCommand.ColorCode));
    }

    [Fact]
    public void Validate_WhenSexoCodeIsEmpty_HasError()
    {
        var validator = new CreateVacunoValidator();

        var result = validator.Validate(ValidCommand(sexoCode: string.Empty));

        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateVacunoCommand.SexoCode));
    }

    [Fact]
    public void Validate_WhenGranjaIdIsZero_HasError()
    {
        var validator = new CreateVacunoValidator();

        var result = validator.Validate(ValidCommand(granjaId: 0));

        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateVacunoCommand.GranjaId));
    }

    [Fact]
    public void Validate_WhenObservacionesExceedsMaxLength_HasError()
    {
        var validator = new CreateVacunoValidator();

        var result = validator.Validate(ValidCommand(observaciones: new string('A', 151)));

        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateVacunoCommand.Observaciones));
    }

    [Fact]
    public void Validate_WhenObservacionesIsNull_HasNoError()
    {
        var validator = new CreateVacunoValidator();

        var result = validator.Validate(ValidCommand(observaciones: null));

        Assert.DoesNotContain(result.Errors, e => e.PropertyName == nameof(CreateVacunoCommand.Observaciones));
    }
}
