using ZooTech.Application.Modules.Module_Sanidad.UseCases.CreateTriaje;
using ZooTech.Application.Modules.Module_Sanidad.Validators;

namespace ZooTech.Application.UnitTests.Modules.Module_Sanidad.Validators;

public class CreateTriajeValidatorTests
{
    private static CreateTriajeCommand ValidCommand(
        long vacunoId = 1,
        string tipoPesoCode = "NACIMIENTO",
        decimal pesoKg = 10,
        string? observaciones = null,
        long? encargadoUsuarioId = null,
        DateTime? fechaHora = null)
    {
        return new CreateTriajeCommand(
            VacunoId: vacunoId,
            TipoPesoCode: tipoPesoCode,
            PesoKg: pesoKg,
            Observaciones: observaciones,
            EncargadoUsuarioId: encargadoUsuarioId,
            FechaHora: fechaHora ?? DateTime.UtcNow);
    }

    [Fact]
    public void Validate_WhenCommandIsValid_HasNoErrors()
    {
        var validator = new CreateTriajeValidator();

        var result = validator.Validate(ValidCommand());

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WhenVacunoIdIsZero_HasError()
    {
        var validator = new CreateTriajeValidator();

        var result = validator.Validate(ValidCommand(vacunoId: 0));

        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateTriajeCommand.VacunoId));
    }

    [Fact]
    public void Validate_WhenTipoPesoCodeIsEmpty_HasError()
    {
        var validator = new CreateTriajeValidator();

        var result = validator.Validate(ValidCommand(tipoPesoCode: string.Empty));

        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateTriajeCommand.TipoPesoCode));
    }

    [Fact]
    public void Validate_WhenPesoKgIsZero_HasError()
    {
        var validator = new CreateTriajeValidator();

        var result = validator.Validate(ValidCommand(pesoKg: 0));

        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateTriajeCommand.PesoKg));
    }

    [Fact]
    public void Validate_WhenObservacionesExceedsMaxLength_HasError()
    {
        var validator = new CreateTriajeValidator();

        var result = validator.Validate(ValidCommand(observaciones: new string('A', 151)));

        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateTriajeCommand.Observaciones));
    }

    [Fact]
    public void Validate_WhenObservacionesIsNull_HasNoError()
    {
        var validator = new CreateTriajeValidator();

        var result = validator.Validate(ValidCommand(observaciones: null));

        Assert.DoesNotContain(result.Errors, e => e.PropertyName == nameof(CreateTriajeCommand.Observaciones));
    }

    [Fact]
    public void Validate_WhenFechaHoraIsFuture_HasError()
    {
        var validator = new CreateTriajeValidator();

        var result = validator.Validate(ValidCommand(fechaHora: DateTime.UtcNow.AddDays(1)));

        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateTriajeCommand.FechaHora));
    }

}
