using FluentAssertions;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ListarVacunosReporte;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ObtenerRegistroVacunoReporte;

namespace ZooTech.Application.UnitTests.Modules.Module_Vacuno.UseCases.ReporteVacuno;

public sealed class ReporteVacunoQueryValidatorTests
{
    [Fact]
    public void Listado_InvalidFormatAndDateRange_ReturnsValidationErrors()
    {
        var query = new ListarVacunosReporteQuery(
            "2026-07-22", "2026-07-01", null, null, null, null, null, null,
            null, null, null, null, "csv", null, null, null);

        var errors = new ListarVacunosReporteQueryValidator().Validate(query);

        errors.Should().Contain(error => error.Contains("formato"));
        errors.Should().Contain(error => error.Contains("fecha desde"));
    }

    [Fact]
    public void Registro_InvalidIdAndFormat_ReturnsValidationErrors()
    {
        var errors = new ObtenerRegistroVacunoReporteQueryValidator()
            .Validate(new ObtenerRegistroVacunoReporteQuery(0, "csv"));

        errors.Should().HaveCount(2);
    }
}
