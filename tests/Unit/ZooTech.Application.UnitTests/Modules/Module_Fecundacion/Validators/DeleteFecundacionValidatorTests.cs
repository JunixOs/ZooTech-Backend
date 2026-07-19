using FluentAssertions;
using ZooTech.Application.Common.Validator;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.DeleteFecundacion;
using ZooTech.Application.Modules.Module_Fecundacion.Validators;

namespace ZooTech.Application.UnitTests.Modules.Module_Fecundacion.Validators;

public sealed class DeleteFecundacionValidatorTests
{
    private readonly DeleteFecundacionValidator _validator = new();

    [Fact]
    public void Validate_WhenRequestIsValid_ShouldReturnNoErrors()
    {
        var errors = _validator.Validate(
            new DeleteFecundacionCommand(10, "Registro duplicado"));

        errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_WhenReasonIsEmpty_ShouldReturnStructuredFieldError()
    {
        var errors = _validator.Validate(new DeleteFecundacionCommand(10, " "));
        var details = ((IValidationErrorDetailsProvider)_validator)
            .GetFieldErrors(errors);

        errors.Should().ContainSingle()
            .Which.Should().Be("FECUNDACION-DELETE-RAZON-REQUIRED");
        details.Should().ContainSingle(error =>
            error.Field == "razon" &&
            error.Code == errors[0] &&
            error.Message == "Debe indicar la razon de eliminacion.");
    }

    [Fact]
    public void Validate_WhenIdIsInvalid_ShouldIdentifyIdField()
    {
        var errors = _validator.Validate(
            new DeleteFecundacionCommand(0, "Registro incorrecto"));
        var details = ((IValidationErrorDetailsProvider)_validator)
            .GetFieldErrors(errors);

        errors.Should().ContainSingle()
            .Which.Should().Be("FECUNDACION-DELETE-ID-INVALID");
        details.Should().ContainSingle(error => error.Field == "id");
    }
}
