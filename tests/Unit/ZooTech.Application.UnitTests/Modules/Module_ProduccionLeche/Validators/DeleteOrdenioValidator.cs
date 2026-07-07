using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ZooTech.Application;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.DeleteOrdenio;

namespace ZooTech.Application.UnitTests.Modules.Module_ProduccionLeche.Validators;

public class DeleteOrdenioValidatorTests
{
    [Fact]
    public void Validate_WhenMotivoIsEmpty_ShouldFail()
    {
        var validator = GetValidator();
        var command = new DeleteOrdenioCommand("");

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(DeleteOrdenioCommand.MotivoEliminacion));
    }

    [Fact]
    public void Validate_WhenMotivoExceeds500Characters_ShouldFail()
    {
        var validator = GetValidator();
        var command = new DeleteOrdenioCommand(new string('a', 501));

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(DeleteOrdenioCommand.MotivoEliminacion));
    }

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldPass()
    {
        var validator = GetValidator();
        var command = new DeleteOrdenioCommand("Registro duplicado");

        var result = validator.Validate(command);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    private static IValidator<DeleteOrdenioCommand> GetValidator()
    {
        var services = new ServiceCollection();
        services.AddApplication();

        var provider = services.BuildServiceProvider();
        return provider.GetRequiredService<IValidator<DeleteOrdenioCommand>>();
    }
}
