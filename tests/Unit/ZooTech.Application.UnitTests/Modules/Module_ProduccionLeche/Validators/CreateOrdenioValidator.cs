using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ZooTech.Application;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.CreateOrdenio;

namespace ZooTech.Application.UnitTests.Modules.Module_ProduccionLeche.Validators;

public class CreateOrdenioValidatorTests
{
    [Fact]
    public void Validate_WhenCodigoIsEmpty_ShouldFail()
    {
        var validator = GetValidator();
        var command = BuildValidCommand() with { Codigo = "" };

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateOrdenioCommand.Codigo));
    }

    [Fact]
    public void Validate_WhenVacunoIdIsZero_ShouldFail()
    {
        var validator = GetValidator();
        var command = BuildValidCommand() with { VacunoId = 0 };

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateOrdenioCommand.VacunoId));
    }

    [Fact]
    public void Validate_WhenEncargadoUsuarioIdIsZero_ShouldFail()
    {
        var validator = GetValidator();
        var command = BuildValidCommand() with { EncargadoUsuarioId = 0 };

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateOrdenioCommand.EncargadoUsuarioId));
    }

    [Fact]
    public void Validate_WhenLitrosIsZero_ShouldFail()
    {
        var validator = GetValidator();
        var command = BuildValidCommand() with { Litros = 0 };

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateOrdenioCommand.Litros));
    }

    [Fact]
    public void Validate_WhenEstadoOrdenioCodeIsEmpty_ShouldFail()
    {
        var validator = GetValidator();
        var command = BuildValidCommand() with { EstadoOrdenioCode = "" };

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateOrdenioCommand.EstadoOrdenioCode));
    }

    [Fact]
    public void Validate_WhenObservacionesExceeds150Characters_ShouldFail()
    {
        var validator = GetValidator();
        var command = BuildValidCommand() with { Observaciones = new string('a', 151) };

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateOrdenioCommand.Observaciones));
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

    private static IValidator<CreateOrdenioCommand> GetValidator()
    {
        var services = new ServiceCollection();
        services.AddApplication();

        var provider = services.BuildServiceProvider();
        return provider.GetRequiredService<IValidator<CreateOrdenioCommand>>();
    }

    private static CreateOrdenioCommand BuildValidCommand()
        => new(
            Codigo: "ORD-001",
            FechaHora: DateTime.UtcNow,
            VacunoId: 1,
            EncargadoUsuarioId: 2,
            Litros: 10,
            EstadoOrdenioCode: "ACTIVO",
            Observaciones: "Observacion valida");
}
