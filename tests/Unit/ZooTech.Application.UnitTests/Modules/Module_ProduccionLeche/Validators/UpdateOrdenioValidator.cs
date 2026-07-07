using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ZooTech.Application;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.UpdateOrdenio;

namespace ZooTech.Application.UnitTests.Modules.Module_ProduccionLeche.Validators;

public class UpdateOrdenioValidatorTests
{
    [Fact]
    public void Validate_WhenEncargadoUsuarioIdIsZero_ShouldFail()
    {
        var validator = GetValidator();
        var command = BuildValidCommand() with { EncargadoUsuarioId = 0 };

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateOrdenioCommand.EncargadoUsuarioId));
    }

    [Fact]
    public void Validate_WhenLitrosIsZero_ShouldFail()
    {
        var validator = GetValidator();
        var command = BuildValidCommand() with { Litros = 0 };

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateOrdenioCommand.Litros));
    }

    [Fact]
    public void Validate_WhenEstadoOrdenioCodeIsEmpty_ShouldFail()
    {
        var validator = GetValidator();
        var command = BuildValidCommand() with { EstadoOrdenioCode = "" };

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateOrdenioCommand.EstadoOrdenioCode));
    }

    [Fact]
    public void Validate_WhenObservacionesExceeds150Characters_ShouldFail()
    {
        var validator = GetValidator();
        var command = BuildValidCommand() with { Observaciones = new string('a', 151) };

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateOrdenioCommand.Observaciones));
    }

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldPass()
    {
        var validator = GetValidator();
        var command = BuildValidCommand();

        var result = validator.Validate(command);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    private static IValidator<UpdateOrdenioCommand> GetValidator()
    {
        var services = new ServiceCollection();
        services.AddApplication();

        var provider = services.BuildServiceProvider();
        return provider.GetRequiredService<IValidator<UpdateOrdenioCommand>>();
    }

    private static UpdateOrdenioCommand BuildValidCommand()
        => new(
            FechaHora: DateTime.UtcNow,
            EncargadoUsuarioId: 2,
            Litros: 10,
            EstadoOrdenioCode: "ACTIVO",
            Observaciones: "Observacion valida");
}
