using ZooTech.Application.Modules.Module_Sanidad.UseCases.UpdateTriaje;
using ZooTech.Application.Modules.Module_Sanidad.Validators;

namespace ZooTech.Application.UnitTests.Modules.Module_Sanidad.Validators;

public class UpdateTriajeValidatorTests
{
    private static UpdateTriajeCommand ValidCommand(
        long id = 1,
        string tipoPesoCode = "NACIMIENTO",
        decimal pesoKg = 10,
        string? observaciones = null,
        long? encargadoUsuarioId = null)
    {
        return new UpdateTriajeCommand
        {
            Id = id,
            TipoPesoCode = tipoPesoCode,
            PesoKg = pesoKg,
            Observaciones = observaciones,
            EncargadoUsuarioId = encargadoUsuarioId,
        };
    }

    [Fact]
    public void Validate_WhenCommandIsValid_HasNoErrors()
    {
        var validator = new UpdateTriajeValidator();

        var result = validator.Validate(ValidCommand());

        Assert.Empty(result);
    }

    [Fact]
    public void Validate_WhenTipoPesoCodeIsEmpty_HasError()
    {
        var validator = new UpdateTriajeValidator();

        var result = validator.Validate(ValidCommand(tipoPesoCode: string.Empty));

        Assert.NotEmpty(result);
    }

    [Fact]
    public void Validate_WhenPesoKgIsZero_HasError()
    {
        var validator = new UpdateTriajeValidator();

        var result = validator.Validate(ValidCommand(pesoKg: 0));

        Assert.NotEmpty(result);
    }

    [Fact]
    public void Validate_WhenObservacionesExceedsMaxLength_HasError()
    {
        var validator = new UpdateTriajeValidator();

        var result = validator.Validate(ValidCommand(observaciones: new string('A', 501)));

        Assert.NotEmpty(result);
    }

    [Fact]
    public void Validate_WhenObservacionesIsNull_HasNoError()
    {
        var validator = new UpdateTriajeValidator();

        var result = validator.Validate(ValidCommand(observaciones: null));

        Assert.Empty(result);
    }

    [Fact]
    public void Validate_WhenIdIsInvalid_HasError()
    {
        var validator = new UpdateTriajeValidator();

        var result = validator.Validate(ValidCommand(id: 0));

        Assert.NotEmpty(result);
    }

    [Fact]
    public void Validate_WhenEncargadoUsuarioIdIsInvalid_HasError()
    {
        var validator = new UpdateTriajeValidator();

        var result = validator.Validate(ValidCommand(encargadoUsuarioId: 0));

        Assert.NotEmpty(result);
    }
}
