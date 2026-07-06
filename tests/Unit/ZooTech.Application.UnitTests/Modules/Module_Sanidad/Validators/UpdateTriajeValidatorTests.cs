using ZooTech.Application.Modules.Module_Sanidad.UseCases.UpdateTriaje;
using ZooTech.Application.Modules.Module_Sanidad.Validators;

namespace ZooTech.Application.UnitTests.Modules.Module_Sanidad.Validators;

public class UpdateTriajeValidatorTests
{
    private static UpdateTriajeCommand ValidCommand(
        long vacunoId = 1,
        string tipoPesoCode = "NACIMIENTO",
        decimal pesoKg = 10,
        string? observaciones = null,
        string estadoRegistroCode = "ACTIVO",
        long? encargadoUsuarioId = null)
    {
        return new UpdateTriajeCommand(
            VacunoId: vacunoId,
            TipoPesoCode: tipoPesoCode,
            PesoKg: pesoKg,
            Observaciones: observaciones,
            EstadoRegistroCode: estadoRegistroCode,
            EncargadoUsuarioId: encargadoUsuarioId);
    }

    [Fact]
    public void Validate_WhenCommandIsValid_HasNoErrors()
    {
        var validator = new UpdateTriajeValidator();

        var result = validator.Validate(ValidCommand());

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WhenVacunoIdIsZero_HasError()
    {
        var validator = new UpdateTriajeValidator();

        var result = validator.Validate(ValidCommand(vacunoId: 0));

        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateTriajeCommand.VacunoId));
    }

    [Fact]
    public void Validate_WhenTipoPesoCodeIsEmpty_HasError()
    {
        var validator = new UpdateTriajeValidator();

        var result = validator.Validate(ValidCommand(tipoPesoCode: string.Empty));

        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateTriajeCommand.TipoPesoCode));
    }

    [Fact]
    public void Validate_WhenPesoKgIsZero_HasError()
    {
        var validator = new UpdateTriajeValidator();

        var result = validator.Validate(ValidCommand(pesoKg: 0));

        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateTriajeCommand.PesoKg));
    }

    [Fact]
    public void Validate_WhenEstadoRegistroCodeIsEmpty_HasError()
    {
        var validator = new UpdateTriajeValidator();

        var result = validator.Validate(ValidCommand(estadoRegistroCode: string.Empty));

        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateTriajeCommand.EstadoRegistroCode));
    }

    [Fact]
    public void Validate_WhenObservacionesExceedsMaxLength_HasError()
    {
        var validator = new UpdateTriajeValidator();

        var result = validator.Validate(ValidCommand(observaciones: new string('A', 501)));

        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateTriajeCommand.Observaciones));
    }

    [Fact]
    public void Validate_WhenObservacionesIsNull_HasNoError()
    {
        var validator = new UpdateTriajeValidator();

        var result = validator.Validate(ValidCommand(observaciones: null));

        Assert.DoesNotContain(result.Errors, e => e.PropertyName == nameof(UpdateTriajeCommand.Observaciones));
    }
}
