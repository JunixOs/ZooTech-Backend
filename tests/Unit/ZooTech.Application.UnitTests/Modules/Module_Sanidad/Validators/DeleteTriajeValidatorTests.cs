using ZooTech.Application.Modules.Module_Sanidad.UseCases.DeleteTriaje;
using ZooTech.Application.Modules.Module_Sanidad.Validators;

namespace ZooTech.Application.UnitTests.Modules.Module_Sanidad.Validators;

public class DeleteTriajeValidatorTests
{
    [Fact]
    public void Validate_WhenCommandIsValid_HasNoErrors()
    {
        var validator = new DeleteTriajeValidator();

        var result = validator.Validate(new DeleteTriajeCommand { Id = 1, MotivoEliminacion = "Duplicado" });

        Assert.Empty(result);
    }

    [Fact]
    public void Validate_WhenMotivoEliminacionIsEmpty_HasError()
    {
        var validator = new DeleteTriajeValidator();

        var result = validator.Validate(new DeleteTriajeCommand { Id = 1, MotivoEliminacion = string.Empty });

        Assert.NotEmpty(result);
    }

    [Fact]
    public void Validate_WhenMotivoEliminacionExceedsMaxLength_HasError()
    {
        var validator = new DeleteTriajeValidator();

        var result = validator.Validate(new DeleteTriajeCommand { Id = 1, MotivoEliminacion = new string('A', 501) });

        Assert.NotEmpty(result);
    }
}
