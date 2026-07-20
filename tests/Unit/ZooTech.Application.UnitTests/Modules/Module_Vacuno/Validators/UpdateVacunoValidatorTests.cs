using FluentAssertions;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.UpdateVacuno;
using ZooTech.Application.Modules.Module_Vacuno.Validators;
using ZooTech.Tests.Shared.Factories;

namespace ZooTech.Application.UnitTests.Modules.Module_Vacuno.Validators;

public sealed class UpdateVacunoValidatorTests
{
    private readonly UpdateVacunoValidator _validator = new();

    public static TheoryData<UpdateVacunoCommand, string> RequiredFieldCases => new()
    {
        { VacunoTestDataFactory.UpdateCommand(0), "VACUNO-VACUNO-UPDATE-ID-INVALID" },
        { VacunoTestDataFactory.UpdateCommand() with { Nombre = string.Empty }, "VACUNO-VACUNO-UPDATE-NOMBRE-NULL" },
        { VacunoTestDataFactory.UpdateCommand() with { FechaNacimiento = default }, "VACUNO-VACUNO-UPDATE-FECHA_NACIMIENTO-NULL" },
        { VacunoTestDataFactory.UpdateCommand() with { TipoAdquisicionCode = string.Empty }, "VACUNO-VACUNO-UPDATE-TIPO_ADQUISICION_CODE-NULL" },
        { VacunoTestDataFactory.UpdateCommand() with { RazaCode = string.Empty }, "VACUNO-VACUNO-UPDATE-RAZA_CODE-NULL" },
        { VacunoTestDataFactory.UpdateCommand() with { ColorCode = string.Empty }, "VACUNO-VACUNO-UPDATE-COLOR_CODE-NULL" },
        { VacunoTestDataFactory.UpdateCommand() with { SexoCode = string.Empty }, "VACUNO-VACUNO-UPDATE-SEXO_CODE-NULL" },
        { VacunoTestDataFactory.UpdateCommand() with { GranjaId = null }, "VACUNO-VACUNO-UPDATE-GRANJA_ID-INVALID" },
        { VacunoTestDataFactory.UpdateCommand() with { TipoAdquisicionCode = "COMPRA", PrecioCompra = null }, "VACUNO-VACUNO-UPDATE-PRECIO_COMPRA-NULL" }
    };

    [Fact]
    public void Validate_WhenCommandIsValid_ReturnsNoErrors()
    {
        _validator.Validate(VacunoTestDataFactory.UpdateCommand()).Should().BeEmpty();
    }

    [Theory]
    [MemberData(nameof(RequiredFieldCases))]
    public void Validate_WhenRequiredFieldIsMissing_ReturnsExpectedCode(
        UpdateVacunoCommand command,
        string expectedCode)
    {
        _validator.Validate(command).Should().Contain(expectedCode);
    }

    [Fact]
    public void Validate_WhenNewGranjaHasNameAndDistrict_DoesNotRequireExistingGranja()
    {
        var command = VacunoTestDataFactory.UpdateCommand() with
        {
            GranjaId = null,
            Granja = "Granja Nueva",
            CodigoDistrito = "010101"
        };

        _validator.Validate(command)
            .Should().NotContain("VACUNO-VACUNO-UPDATE-GRANJA_ID-INVALID");
    }

    [Fact]
    public void Validate_WhenTenantManagedLengthIsExceeded_LeavesRuleToTenantValidation()
    {
        var command = VacunoTestDataFactory.UpdateCommand() with
        {
            Nombre = new string('B', 101),
            Observaciones = new string('C', 151)
        };

        _validator.Validate(command).Should().BeEmpty();
    }
}
