using ZooTech.Application.Modules.Module_Fecundacion.UseCases.UpdateFecundacion;
using ZooTech.Application.Modules.Module_Fecundacion.Validators;

namespace ZooTech.Application.UnitTests.Modules.Module_Fecundacion.Validators;

public sealed class UpdateFecundacionValidatorTests
{
    private readonly UpdateFecundacionValidator _validator = new();

    [Fact]
    public void Validate_ShouldFail_WhenFechaProcedimientoIsFuture()
    {
        var command = BuildCommand(fechaProcedimiento: DateOnly.FromDateTime(DateTime.Today.AddDays(1)));

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateFecundacionCommand.FechaProcedimiento));
    }

    [Fact]
    public void Validate_ShouldFail_WhenIaDoesNotHaveCodigoSemen()
    {
        var command = BuildCommand(tipoFecundacionCode: "IA", codigoSemen: null);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateFecundacionCommand.CodigoSemen));
    }

    [Fact]
    public void Validate_ShouldFail_WhenTeDoesNotHaveCodigoEmbrion()
    {
        var command = BuildCommand(tipoFecundacionCode: "TE", codigoEmbrion: null);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateFecundacionCommand.CodigoEmbrion));
    }

    [Fact]
    public void Validate_ShouldPass_WhenMontaNaturalHasInternalDonor()
    {
        var command = BuildCommand(tipoFecundacionCode: "MN");

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }

    private static UpdateFecundacionCommand BuildCommand(
        string tipoFecundacionCode = "MN",
        DateOnly? fechaProcedimiento = null,
        string? codigoSemen = "SEM-001",
        string? codigoEmbrion = "EMB-001")
        => new(
            Id: 1,
            TipoFecundacionCode: tipoFecundacionCode,
            VacunoReceptorId: 2,
            TipoDonante: "INTERNO",
            VacunoDonanteId: 3,
            ExternoDonanteNombre: null,
            FechaProcedimiento: fechaProcedimiento ?? DateOnly.FromDateTime(DateTime.Today),
            ResponsableNombre: "Tec. Ruiz",
            ResultadoCode: "PENDIENTE",
            EstadoFecundacionCode: "PENDIENTE",
            ObservacionesVeterinarias: null,
            CodigoSemen: codigoSemen,
            CodigoEmbrion: codigoEmbrion);
}
