using FluentAssertions;
using ZooTech.Application.Common.Validator;
using ZooTech.Application.Modules.Module_Fecundacion.Validators;
using ZooTech.Tests.Shared.Factories;

namespace ZooTech.Application.UnitTests.Modules.Module_Fecundacion.Validators;

public sealed class DeleteFecundacionValidatorTests
{
    private readonly DeleteFecundacionValidator _validator = new();

    [Fact]
    public void Validate_WhenCommandIsValid_ReturnsNoErrors()
    {
        _validator.Validate(FecundacionTestDataFactory.DeleteCommand()).Should().BeEmpty();
    }

    [Theory]
    [InlineData(0, "Motivo valido", "FECUNDACION-DELETE-ID-INVALID", "id")]
    [InlineData(10, " ", "FECUNDACION-DELETE-RAZON-REQUIRED", "razon")]
    public void Validate_WhenCommandIsInvalid_ReturnsStructuredFieldError(
        long id,
        string razon,
        string expectedCode,
        string expectedField)
    {
        var errors = _validator.Validate(FecundacionTestDataFactory.DeleteCommand(id, razon));
        var details = ((IValidationErrorDetailsProvider)_validator).GetFieldErrors(errors);

        errors.Should().ContainSingle().Which.Should().Be(expectedCode);
        details.Should().ContainSingle(error =>
            error.Field == expectedField && error.Code == expectedCode);
    }
}
