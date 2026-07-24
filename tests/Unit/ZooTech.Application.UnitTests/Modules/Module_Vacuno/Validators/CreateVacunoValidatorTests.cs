using FluentAssertions;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.CreateVacuno;
using ZooTech.Application.Modules.Module_Vacuno.Validators;
using ZooTech.Tests.Shared.Factories;

namespace ZooTech.Application.UnitTests.Modules.Module_Vacuno.Validators;

public sealed class CreateVacunoValidatorTests
{
    private readonly CreateVacunoValidator _validator = new();

    public static TheoryData<CreateVacunoCommand, string> RequiredFieldCases => new()
    {
        { VacunoTestDataFactory.CreateCommand() with { Codigo = string.Empty }, "VACUNO-VACUNO-CREATE-CODIGO-NULL" },
        { VacunoTestDataFactory.CreateCommand() with { Nombre = string.Empty }, "VACUNO-VACUNO-CREATE-NOMBRE-NULL" },
        { VacunoTestDataFactory.CreateCommand() with { FechaNacimiento = default }, "VACUNO-VACUNO-CREATE-FECHA_NACIMIENTO-NULL" },
        { VacunoTestDataFactory.CreateCommand() with { TipoAdquisicionCode = string.Empty }, "VACUNO-VACUNO-CREATE-TIPO_ADQUISICION_CODE-NULL" },
        { VacunoTestDataFactory.CreateCommand() with { RazaCode = string.Empty }, "VACUNO-VACUNO-CREATE-RAZA_CODE-NULL" },
        { VacunoTestDataFactory.CreateCommand() with { ColorCode = string.Empty }, "VACUNO-VACUNO-CREATE-COLOR_CODE-NULL" },
        { VacunoTestDataFactory.CreateCommand() with { SexoCode = string.Empty }, "VACUNO-VACUNO-CREATE-SEXO_CODE-NULL" },
        { VacunoTestDataFactory.CreateCommand() with { GranjaId = null }, "VACUNO-VACUNO-CREATE-GRANJA_ID-INVALID" },
        { VacunoTestDataFactory.CreateCommand() with { TipoAdquisicionCode = "COMPRA", PrecioCompra = null }, "VACUNO-VACUNO-CREATE-PRECIO_COMPRA-NULL" }
    };

    [Fact]
    public void Validate_WhenCommandIsValid_ReturnsNoErrors()
    {
        _validator.Validate(VacunoTestDataFactory.CreateCommand()).Should().BeEmpty();
    }

    [Theory]
    [MemberData(nameof(RequiredFieldCases))]
    public void Validate_WhenRequiredFieldIsMissing_ReturnsExpectedCode(
        CreateVacunoCommand command,
        string expectedCode)
    {
        _validator.Validate(command).Should().Contain(expectedCode);
    }

    [Fact]
    public void Validate_WhenNewGranjaHasNameAndDistrict_DoesNotRequireExistingGranja()
    {
        var command = VacunoTestDataFactory.CreateCommand() with
        {
            GranjaId = null,
            Granja = "Granja Nueva",
            CodigoDistrito = "010101"
        };

        _validator.Validate(command)
            .Should().NotContain("VACUNO-VACUNO-CREATE-GRANJA_ID-INVALID");
    }

    [Fact]
    public void Validate_WhenTenantManagedLengthIsExceeded_LeavesRuleToTenantValidation()
    {
        var command = VacunoTestDataFactory.CreateCommand() with
        {
            Codigo = new string('A', 21),
            Nombre = new string('B', 101),
            Observaciones = new string('C', 151)
        };

        _validator.Validate(command).Should().BeEmpty();
    }
}
