using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using ZooTech.Application.Common.Gateway.Configuration;
using ZooTech.Application.Common.Gateway.Context;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ListarReportesDisponibles;

namespace ZooTech.Application.UnitTests.Modules.Module_ReporteVacuno.UseCases.ListarReportesDisponibles;

public class ListarReportesDisponiblesUseCaseTests
{
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;
    private readonly Mock<ISettingProvider> _settingProviderMock;
    private readonly Mock<ITenantContext> _tenantContextMock;
    private readonly Mock<ILogger<ListarReportesDisponiblesUseCase>> _loggerMock;
    private readonly ListarReportesDisponiblesUseCase _useCase;

    public ListarReportesDisponiblesUseCaseTests()
    {
        _dateTimeProviderMock = new Mock<IDateTimeProvider>();
        _settingProviderMock = new Mock<ISettingProvider>();
        _tenantContextMock = new Mock<ITenantContext>();
        _loggerMock = new Mock<ILogger<ListarReportesDisponiblesUseCase>>();

        _tenantContextMock.Setup(x => x.TenantId).Returns(1L);
        _dateTimeProviderMock.Setup(x => x.Today).Returns(new DateOnly(2026, 6, 8));

        // Default configurations
        _settingProviderMock.Setup(x => x.GetSettingAsync<int>("REPORTS_DEFAULT_DAYS", 1L))
            .ReturnsAsync(30);
        _settingProviderMock.Setup(x => x.GetSettingAsync<string>("REPORTS_DATE_FORMAT", 1L))
            .ReturnsAsync("yyyy-MM-dd");

        _useCase = new ListarReportesDisponiblesUseCase(
            _dateTimeProviderMock.Object,
            _settingProviderMock.Object,
            _tenantContextMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task HandleAsync_CuandoCatalogoEstaVacio_DebeRetornarListaVacia()
    {
        // Arrange
        var query = new ListarReportesDisponiblesQuery(null, null, "  ");
        _settingProviderMock.Setup(x => x.GetSettingAsync<string>("REPORTS_AVAILABLE_CATALOG", 1L))
            .ReturnsAsync(string.Empty);

        // Act
        var result = await _useCase.HandleAsync(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().BeEmpty();
        result.Filtros.Q.Should().BeNull();
    }

    [Fact]
    public async Task HandleAsync_CuandoExistenReportesEnConfiguracion_DebeRetornarListaDeserializada()
    {
        // Arrange
        var query = new ListarReportesDisponiblesQuery("2026-05-01", "2026-05-31", " test query ");
        var jsonCatalog = "[{\"Tipo\":\"rpt1\", \"Nombre\":\"Reporte 1\"}, {\"Tipo\":\"rpt2\", \"Nombre\":\"Reporte 2\"}]";

        _settingProviderMock.Setup(x => x.GetSettingAsync<string>("REPORTS_AVAILABLE_CATALOG", 1L))
            .ReturnsAsync(jsonCatalog);

        // Act
        var result = await _useCase.HandleAsync(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        
        // Verifica filtros normalizados
        result.Filtros.FechaDesde.Should().Be(new DateOnly(2026, 5, 1));
        result.Filtros.FechaHasta.Should().Be(new DateOnly(2026, 5, 31));
        result.Filtros.Q.Should().Be("test query"); // trim
    }
}
