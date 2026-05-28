using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Modules.Module_ReporteVacuno.Common;

namespace ZooTech.Application.UnitTests.ReporteVacuno;

[TestClass]
public sealed class ReporteVacunoDateRangeResolverTests
{
    private static readonly DateOnly ServerToday = new(2026, 5, 28);

    [TestMethod]
    public void Resolve_WhenNoDates_ShouldApplyLastThirtyDays()
    {
        var result = ReporteVacunoDateRangeResolver.Resolve(null, null, ServerToday);

        Assert.AreEqual(new DateOnly(2026, 4, 28), result.FechaDesde);
        Assert.AreEqual(ServerToday, result.FechaHasta);
        Assert.AreEqual(30, result.FechaHasta.DayNumber - result.FechaDesde.DayNumber);
    }

    [TestMethod]
    public void Resolve_WhenOnlyFechaHasta_ShouldCalculateFechaDesdeThirtyDaysBefore()
    {
        var result = ReporteVacunoDateRangeResolver.Resolve(null, "2026-05-20", ServerToday);

        Assert.AreEqual(new DateOnly(2026, 4, 20), result.FechaDesde);
        Assert.AreEqual(new DateOnly(2026, 5, 20), result.FechaHasta);
    }

    [TestMethod]
    public void Resolve_WhenOnlyFechaDesde_ShouldUseServerTodayAsFechaHasta()
    {
        var result = ReporteVacunoDateRangeResolver.Resolve("2026-05-01", null, ServerToday);

        Assert.AreEqual(new DateOnly(2026, 5, 1), result.FechaDesde);
        Assert.AreEqual(ServerToday, result.FechaHasta);
    }

    [TestMethod]
    public void Resolve_WhenFechaDesdeGreaterThanFechaHasta_ShouldThrowValidationError()
    {
        var ex = Assert.ThrowsException<ApplicationRuleException>(() =>
            ReporteVacunoDateRangeResolver.Resolve("2026-05-21", "2026-05-20", ServerToday));

        Assert.AreEqual("VALIDATION_ERROR", ex.Code);
        Assert.AreEqual(400, ex.StatusCode);
        Assert.IsTrue(ex.Details.Any(x => x.Field == "fechaDesde"));
    }

    [DataTestMethod]
    [DataRow("20-05-2026", null, "fechaDesde")]
    [DataRow(null, "2026/05/20", "fechaHasta")]
    public void Resolve_WhenDateFormatIsInvalid_ShouldThrowValidationError(string? fechaDesde, string? fechaHasta, string expectedField)
    {
        var ex = Assert.ThrowsException<ApplicationRuleException>(() =>
            ReporteVacunoDateRangeResolver.Resolve(fechaDesde, fechaHasta, ServerToday));

        Assert.AreEqual("VALIDATION_ERROR", ex.Code);
        Assert.AreEqual(400, ex.StatusCode);
        Assert.IsTrue(ex.Details.Any(x => x.Field == expectedField));
    }
}
