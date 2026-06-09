using System;
using FluentAssertions;
using Xunit;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Modules.Module_ReporteVacuno.Common;

namespace ZooTech.Application.UnitTests.Modules.Module_ReporteVacuno.Common;

public class ReporteVacunoDateRangeResolverTests
{
    private readonly DateOnly _serverToday = new DateOnly(2026, 6, 8);
    private readonly int _defaultDays = 30;
    private readonly string _dateFormat = "yyyy-MM-dd";

    [Fact]
    public void Resolve_CuandoSeEnvianAmbasFechasValidas_DebeRetornarRangoExacto()
    {
        // Arrange
        var fechaDesdeStr = "2026-05-01";
        var fechaHastaStr = "2026-05-31";

        // Act
        var result = ReporteVacunoDateRangeResolver.Resolve(
            fechaDesdeStr,
            fechaHastaStr,
            _serverToday,
            _defaultDays,
            _dateFormat);

        // Assert
        result.FechaDesde.Should().Be(new DateOnly(2026, 5, 1));
        result.FechaHasta.Should().Be(new DateOnly(2026, 5, 31));
    }

    [Fact]
    public void Resolve_CuandoFaltaFechaDesde_DebeCalcularRestandoDefaultDaysAFechaHasta()
    {
        // Arrange
        string? fechaDesdeStr = null;
        var fechaHastaStr = "2026-05-31";

        // Act
        var result = ReporteVacunoDateRangeResolver.Resolve(
            fechaDesdeStr,
            fechaHastaStr,
            _serverToday,
            _defaultDays,
            _dateFormat);

        // Assert
        // 2026-05-31 minus 30 days is 2026-05-01
        result.FechaDesde.Should().Be(new DateOnly(2026, 5, 1));
        result.FechaHasta.Should().Be(new DateOnly(2026, 5, 31));
    }

    [Fact]
    public void Resolve_CuandoFaltaFechaHasta_DebeUsarFechaDesdeComoLimiteInferiorYActualComoSuperior()
    {
        // Arrange
        var fechaDesdeStr = "2026-06-01";
        string? fechaHastaStr = null;

        // Act
        var result = ReporteVacunoDateRangeResolver.Resolve(
            fechaDesdeStr,
            fechaHastaStr,
            _serverToday,
            _defaultDays,
            _dateFormat);

        // Assert
        result.FechaDesde.Should().Be(new DateOnly(2026, 6, 1));
        result.FechaHasta.Should().Be(_serverToday);
    }

    [Fact]
    public void Resolve_CuandoAmbasFaltan_DebeRetornarDefaultDaysDesdeFechaActual()
    {
        // Arrange
        string? fechaDesdeStr = null;
        string? fechaHastaStr = null;

        // Act
        var result = ReporteVacunoDateRangeResolver.Resolve(
            fechaDesdeStr,
            fechaHastaStr,
            _serverToday,
            _defaultDays,
            _dateFormat);

        // Assert
        // 2026-06-08 minus 30 days is 2026-05-09
        result.FechaDesde.Should().Be(new DateOnly(2026, 5, 9));
        result.FechaHasta.Should().Be(_serverToday);
    }

    [Fact]
    public void Resolve_CuandoFormatosSonInvalidos_DebeLanzarExcepcion()
    {
        // Arrange
        var fechaDesdeStr = "01-05-2026"; // Invalido segun formato yyyy-MM-dd
        var fechaHastaStr = "31-05-2026";

        // Act
        Action act = () => ReporteVacunoDateRangeResolver.Resolve(
            fechaDesdeStr,
            fechaHastaStr,
            _serverToday,
            _defaultDays,
            _dateFormat);

        // Assert
        act.Should().Throw<ApplicationRuleException>()
            .WithMessage("Los datos enviados no son validos.")
            .And.Details.Should().Contain(d => d.Field == "fechaHasta" && d.Message.Contains("YYYY-MM-DD"));
    }

    [Fact]
    public void Resolve_CuandoFechaDesdeEsMayorAFechaHasta_DebeLanzarExcepcion()
    {
        // Arrange
        var fechaDesdeStr = "2026-06-01";
        var fechaHastaStr = "2026-05-01";

        // Act
        Action act = () => ReporteVacunoDateRangeResolver.Resolve(
            fechaDesdeStr,
            fechaHastaStr,
            _serverToday,
            _defaultDays,
            _dateFormat);

        // Assert
        act.Should().Throw<ApplicationRuleException>()
            .WithMessage("Los datos enviados no son validos.")
            .And.Details.Should().Contain(d => d.Field == "fechaDesde" && d.Message.Contains("no puede ser mayor"));
    }
}
