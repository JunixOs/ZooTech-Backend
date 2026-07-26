using Microsoft.Extensions.DependencyInjection;
using ZooTech.Application;
using ZooTech.Application.Common.Validator;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.CreateOrdenio;

namespace ZooTech.Application.UnitTests.Modules.Module_ProduccionLeche.Validators;

public class CreateOrdenioValidatorTests
{
    [Fact]
    public void Validate_WhenCodigoIsEmpty_ShouldFail()
    {
        var validator = GetValidator();
        var command = BuildValidCommand();
        command.Codigo = "";

        var errors = validator.Validate(command);

        Assert.Contains("PRODUCCION_LECHE-ORDENIO-CREATE-CODIGO-NULL", errors);
    }

    [Fact]
    public void Validate_WhenVacunoIdIsZero_ShouldFail()
    {
        var validator = GetValidator();
        var command = BuildValidCommand();
        command.VacunoId = 0;

        var errors = validator.Validate(command);

        Assert.Contains("PRODUCCION_LECHE-ORDENIO-CREATE-VACUNO_ID-INVALID", errors);
    }

    [Fact]
    public void Validate_WhenEncargadoUsuarioIdIsZero_ShouldFail()
    {
        var validator = GetValidator();
        var command = BuildValidCommand();
        command.EncargadoUsuarioId = 0;

        var errors = validator.Validate(command);

        Assert.Contains("PRODUCCION_LECHE-ORDENIO-CREATE-ENCARGADO_USUARIO_ID-INVALID", errors);
    }

    [Fact]
    public void Validate_WhenLitrosIsZero_ShouldFail()
    {
        var validator = GetValidator();
        var command = BuildValidCommand();
        command.Litros = 0;

        var errors = validator.Validate(command);

        Assert.Contains("PRODUCCION_LECHE-ORDENIO-CREATE-LITROS-INVALID", errors);
    }

    [Fact]
    public void Validate_WhenEstadoOrdenioCodeIsEmpty_ShouldFail()
    {
        var validator = GetValidator();
        var command = BuildValidCommand();
        command.EstadoOrdenioCode = "";

        var errors = validator.Validate(command);

        Assert.Contains("PRODUCCION_LECHE-ORDENIO-CREATE-ESTADO_ORDENIO_CODE-NULL", errors);
    }

    [Fact]
    public void Validate_WhenObservacionesExceeds150Characters_ShouldFail()
    {
        var validator = GetValidator();
        var command = BuildValidCommand();
        command.Observaciones = new string('a', 151);

        var errors = validator.Validate(command);

        Assert.Contains("PRODUCCION_LECHE-ORDENIO-CREATE-OBSERVACIONES-INVALID", errors);
    }

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldPass()
    {
        var validator = GetValidator();
        var command = BuildValidCommand();

        var errors = validator.Validate(command);

        Assert.Empty(errors);
    }

    private static ICommandQueryValidator<CreateOrdenioCommand> GetValidator()
    {
        var services = new ServiceCollection();
        services.AddApplication();

        var provider = services.BuildServiceProvider();
        return provider.GetRequiredService<ICommandQueryValidator<CreateOrdenioCommand>>();
    }

    private static CreateOrdenioCommand BuildValidCommand()
        => new()
        {
            Codigo = "ORD-001",
            FechaHora = DateTime.UtcNow,
            VacunoId = 1,
            EncargadoUsuarioId = 2,
            Litros = 10,
            EstadoOrdenioCode = "ACTIVO",
            Observaciones = "Observacion valida"
        };
}
