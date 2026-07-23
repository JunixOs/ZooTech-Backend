using Microsoft.Extensions.DependencyInjection;
using ZooTech.Application;
using ZooTech.Application.Common.Validator;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.UpdateOrdenio;

namespace ZooTech.Application.UnitTests.Modules.Module_ProduccionLeche.Validators;

public class UpdateOrdenioValidatorTests
{
    [Fact]
    public void Validate_WhenEncargadoUsuarioIdIsZero_ShouldFail()
    {
        var validator = GetValidator();
        var command = BuildValidCommand();
        command.EncargadoUsuarioId = 0;

        var errors = validator.Validate(command);

        Assert.Contains("PRODUCCION_LECHE-ORDENIO-UPDATE-ENCARGADO_USUARIO_ID-INVALID", errors);
    }

    [Fact]
    public void Validate_WhenLitrosIsZero_ShouldFail()
    {
        var validator = GetValidator();
        var command = BuildValidCommand();
        command.Litros = 0;

        var errors = validator.Validate(command);

        Assert.Contains("PRODUCCION_LECHE-ORDENIO-UPDATE-LITROS-INVALID", errors);
    }

    [Fact]
    public void Validate_WhenEstadoOrdenioCodeIsEmpty_ShouldFail()
    {
        var validator = GetValidator();
        var command = BuildValidCommand();
        command.EstadoOrdenioCode = "";

        var errors = validator.Validate(command);

        Assert.Contains("PRODUCCION_LECHE-ORDENIO-UPDATE-ESTADO_ORDENIO_CODE-NULL", errors);
    }

    [Fact]
    public void Validate_WhenObservacionesExceeds150Characters_ShouldFail()
    {
        var validator = GetValidator();
        var command = BuildValidCommand();
        command.Observaciones = new string('a', 151);

        var errors = validator.Validate(command);

        Assert.Contains("PRODUCCION_LECHE-ORDENIO-UPDATE-OBSERVACIONES-INVALID", errors);
    }

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldPass()
    {
        var validator = GetValidator();
        var command = BuildValidCommand();

        var errors = validator.Validate(command);

        Assert.Empty(errors);
    }

    private static ICommandValidator<UpdateOrdenioCommand> GetValidator()
    {
        var services = new ServiceCollection();
        services.AddApplication();

        var provider = services.BuildServiceProvider();
        return provider.GetRequiredService<ICommandValidator<UpdateOrdenioCommand>>();
    }

    private static UpdateOrdenioCommand BuildValidCommand()
        => new()
        {
            Id = 1,
            FechaHora = DateTime.UtcNow,
            EncargadoUsuarioId = 2,
            Litros = 10,
            EstadoOrdenioCode = "ACTIVO",
            Observaciones = "Observacion valida"
        };
}
