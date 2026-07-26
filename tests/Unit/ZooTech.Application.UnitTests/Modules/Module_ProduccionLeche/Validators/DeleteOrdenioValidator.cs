using Microsoft.Extensions.DependencyInjection;
using ZooTech.Application;
using ZooTech.Application.Common.Validator;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.DeleteOrdenio;

namespace ZooTech.Application.UnitTests.Modules.Module_ProduccionLeche.Validators;

public class DeleteOrdenioValidatorTests
{
    [Fact]
    public void Validate_WhenMotivoIsEmpty_ShouldFail()
    {
        var validator = GetValidator();
        var command = new DeleteOrdenioCommand { MotivoEliminacion = "" };

        var errors = validator.Validate(command);

        Assert.Contains("PRODUCCION_LECHE-ORDENIO-DELETE-MOTIVO_ELIMINACION-NULL", errors);
    }

    [Fact]
    public void Validate_WhenMotivoExceeds500Characters_ShouldFail()
    {
        var validator = GetValidator();
        var command = new DeleteOrdenioCommand { MotivoEliminacion = new string('a', 501) };

        var errors = validator.Validate(command);

        Assert.Contains("PRODUCCION_LECHE-ORDENIO-DELETE-MOTIVO_ELIMINACION-INVALID", errors);
    }

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldPass()
    {
        var validator = GetValidator();
        var command = new DeleteOrdenioCommand { MotivoEliminacion = "Registro duplicado" };

        var errors = validator.Validate(command);

        Assert.Empty(errors);
    }

    private static ICommandQueryValidator<DeleteOrdenioCommand> GetValidator()
    {
        var services = new ServiceCollection();
        services.AddApplication();

        var provider = services.BuildServiceProvider();
        return provider.GetRequiredService<ICommandQueryValidator<DeleteOrdenioCommand>>();
    }
}
