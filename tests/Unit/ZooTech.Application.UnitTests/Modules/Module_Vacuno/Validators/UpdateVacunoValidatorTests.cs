using ZooTech.Application.Modules.Module_Vacuno.UseCases.UpdateVacuno;
using ZooTech.Application.Modules.Module_Vacuno.Validators;

namespace ZooTech.Application.UnitTests.Modules.Module_Vacuno.Validators;

public class UpdateVacunoValidatorTests
{
    private static UpdateVacunoCommand ValidCommand(
        long id = 1,
        string codigo = "VAC001",
        string nombre = "Lola",
        string tipoAdquisicionCode = "COMPRA",
        string razaCode = "HOLSTEIN",
        string colorCode = "NEGRO",
        string sexoCode = "H",
        long? granjaId = 1,
        string? observaciones = null)
    {
        return new UpdateVacunoCommand(
            Id: id,
            Nombre: nombre,
            FechaNacimiento: new DateOnly(2020, 1, 1),
            TipoAdquisicionCode: tipoAdquisicionCode,
            RazaCode: razaCode,
            ColorCode: colorCode,
            SexoCode: sexoCode,
            CodigoPadre: null,
            CodigoMadre: null,
            GranjaId: granjaId,
            Granja: null,
            CodigoDistrito: null,
            PrecioCompra: 1000m,
            AptoPara: "Carne",
            Observaciones: observaciones);
    }

    [Fact]
    public void Validate_WhenCommandIsValid_HasNoErrors()
    {
        var validator = new UpdateVacunoValidator();

        var result = validator.Validate(ValidCommand());

        Assert.Empty(result);
    }

    [Fact]
    public void Validate_WhenNombreIsEmpty_HasError()
    {
        var validator = new UpdateVacunoValidator();

        var result = validator.Validate(ValidCommand(nombre: string.Empty));

        Assert.NotEmpty(result);
    }

    [Fact]
    public void Validate_WhenNombreExceedsTenantLimit_HasNoBasicValidatorError()
    {
        var validator = new UpdateVacunoValidator();

        var result = validator.Validate(ValidCommand(nombre: new string('A', 101)));

        Assert.Empty(result);
    }

    [Fact]
    public void Validate_WhenTipoAdquisicionCodeIsEmpty_HasError()
    {
        var validator = new UpdateVacunoValidator();

        var result = validator.Validate(ValidCommand(tipoAdquisicionCode: string.Empty));

        Assert.NotEmpty(result);
    }

    [Fact]
    public void Validate_WhenRazaCodeIsEmpty_HasError()
    {
        var validator = new UpdateVacunoValidator();

        var result = validator.Validate(ValidCommand(razaCode: string.Empty));

        Assert.NotEmpty(result);
    }

    [Fact]
    public void Validate_WhenColorCodeIsEmpty_HasError()
    {
        var validator = new UpdateVacunoValidator();

        var result = validator.Validate(ValidCommand(colorCode: string.Empty));

        Assert.NotEmpty(result);
    }

    [Fact]
    public void Validate_WhenSexoCodeIsEmpty_HasError()
    {
        var validator = new UpdateVacunoValidator();

        var result = validator.Validate(ValidCommand(sexoCode: string.Empty));

        Assert.NotEmpty(result);
    }

    [Fact]
    public void Validate_WhenGranjaIdIsZero_HasError()
    {
        var validator = new UpdateVacunoValidator();

        var result = validator.Validate(ValidCommand(granjaId: 0));

        Assert.NotEmpty(result);
    }

    [Fact]
    public void Validate_WhenNewGranjaDataIsProvided_HasNoGranjaError()
    {
        var validator = new UpdateVacunoValidator();
        var command = ValidCommand(granjaId: null) with
        {
            Granja = "Granja Nueva",
            CodigoDistrito = "010101"
        };

        var result = validator.Validate(command);

        Assert.DoesNotContain("VACUNO-VACUNO-UPDATE-GRANJA_ID-INVALID", result);
    }

    [Fact]
    public void Validate_WhenObservacionesExceedsTenantLimit_HasNoBasicValidatorError()
    {
        var validator = new UpdateVacunoValidator();

        var result = validator.Validate(ValidCommand(observaciones: new string('A', 151)));

        Assert.Empty(result);
    }

    [Fact]
    public void Validate_WhenObservacionesIsNull_HasNoError()
    {
        var validator = new UpdateVacunoValidator();

        var result = validator.Validate(ValidCommand(observaciones: null));

        Assert.Empty(result);
    }
}
