using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Modules.Module_ReporteVacuno.Common;

namespace ZooTech.Application.UnitTests.ReporteVacuno;

public sealed class ReporteVacunoDateRangeResolverTests
{
    private static readonly DateOnly ServerToday = new(2026, 5, 20);

    [Fact]
    public void Resolve_WhenDatesAreMissing_AppliesLastThirtyDays()
    {
        var result = ReporteVacunoDateRangeResolver.Resolve(null, null, ServerToday);

        Assert.Equal(new DateOnly(2026, 4, 20), result.FechaDesde);
        Assert.Equal(new DateOnly(2026, 5, 20), result.FechaHasta);
    }

    [Fact]
    public void Resolve_WhenOnlyFechaHastaIsProvided_CalculatesFechaDesdeThirtyDaysBeforeFechaHasta()
    {
        var result = ReporteVacunoDateRangeResolver.Resolve(null, "2026-05-10", ServerToday);

        Assert.Equal(new DateOnly(2026, 4, 10), result.FechaDesde);
        Assert.Equal(new DateOnly(2026, 5, 10), result.FechaHasta);
    }

    [Fact]
    public void Resolve_WhenOnlyFechaDesdeIsProvided_UsesServerTodayAsFechaHasta()
    {
        var result = ReporteVacunoDateRangeResolver.Resolve("2026-05-01", null, ServerToday);

        Assert.Equal(new DateOnly(2026, 5, 1), result.FechaDesde);
        Assert.Equal(new DateOnly(2026, 5, 20), result.FechaHasta);
    }

    [Fact]
    public void Resolve_WhenFechaDesdeIsGreaterThanFechaHasta_ThrowsValidationError()
    {
        var exception = Assert.Throws<ApplicationRuleException>(() =>
            ReporteVacunoDateRangeResolver.Resolve("2026-05-21", "2026-05-20", ServerToday));

        Assert.Equal("VALIDATION_ERROR", exception.Code);
        Assert.Contains(exception.Details, detail => detail.Field == "fechaDesde");
    }

    [Fact]
    public void Resolve_WhenDateFormatIsInvalid_ThrowsValidationError()
    {
        var exception = Assert.Throws<ApplicationRuleException>(() =>
            ReporteVacunoDateRangeResolver.Resolve("20/05/2026", null, ServerToday));

        Assert.Equal("VALIDATION_ERROR", exception.Code);
        Assert.Contains(exception.Details, detail => detail.Field == "fechaDesde");
    }
}
