using ZooTech.Application.Modules.Module_Fecundacion.UseCases.CreateFecundacion;
using ZooTech.Application.Modules.Module_Fecundacion.Validators;

namespace ZooTech.Application.UnitTests.Modules.Module_Fecundacion.Validators;

public sealed class CreateFecundacionValidatorTests
{
    private readonly CreateFecundacionValidator _validator = new();

    [Fact]
    public void Validate_ShouldFail_WhenIaDoesNotHaveCodigoSemen()
    {
        var command = BuildCommand(tipoFecundacionCode: "IA", codigoSemen: null);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateFecundacionCommand.CodigoSemen));
    }

    [Fact]
    public void Validate_ShouldFail_WhenTeDoesNotHaveCodigoEmbrion()
    {
        var command = BuildCommand(tipoFecundacionCode: "TE", codigoEmbrion: null);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateFecundacionCommand.CodigoEmbrion));
    }

    [Fact]
    public void Validate_ShouldPass_WhenMontaNaturalDoesNotHaveTecnicaCode()
    {
        var command = BuildCommand(tipoFecundacionCode: "MN", codigoSemen: null, codigoEmbrion: null);

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }

    private static CreateFecundacionCommand BuildCommand(
        string tipoFecundacionCode = "MN",
        string? codigoSemen = "SEM-001",
        string? codigoEmbrion = "EMB-001")
        => new(
            TipoFecundacionCode: tipoFecundacionCode,
            VacunoReceptorId: 2,
            CeloRegistroId: null,
            FechaProcedimiento: DateTime.Today,
            ResponsableName: "Tec. Ruiz",
            ResultadoCode: "PENDIENTE",
            ObservacionesVeterinarias: null,
            MachoExterno: false,
            MachoExternoNombre: null,
            VacunoDonanteId: 3,
            CreatedById: 1,
            CodigoSemen: codigoSemen,
            CodigoEmbrion: codigoEmbrion);
}
