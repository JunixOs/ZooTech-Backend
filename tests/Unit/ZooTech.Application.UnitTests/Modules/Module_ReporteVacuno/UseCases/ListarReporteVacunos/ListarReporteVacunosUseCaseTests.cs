using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Gateway.Configuration;
using ZooTech.Application.Common.Gateway.Context;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ListarReporteVacunos;

namespace ZooTech.Application.UnitTests.Modules.Module_ReporteVacuno.UseCases.ListarReporteVacunos;

public class ListarReporteVacunosUseCaseTests
{
    private readonly Mock<IReporteVacunoReadRepository> _repositoryMock;
    private readonly Mock<IListadoVacunosReportFileService> _reportFileServiceMock;
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;
    private readonly Mock<ISettingProvider> _settingProviderMock;
    private readonly Mock<ITenantContext> _tenantContextMock;
    private readonly Mock<ILogger<ListarReporteVacunosUseCase>> _loggerMock;
    private readonly ListarReporteVacunosUseCase _useCase;

    public ListarReporteVacunosUseCaseTests()
    {
        _repositoryMock = new Mock<IReporteVacunoReadRepository>();
        _reportFileServiceMock = new Mock<IListadoVacunosReportFileService>();
        _dateTimeProviderMock = new Mock<IDateTimeProvider>();
        _settingProviderMock = new Mock<ISettingProvider>();
        _tenantContextMock = new Mock<ITenantContext>();
        _loggerMock = new Mock<ILogger<ListarReporteVacunosUseCase>>();

        _tenantContextMock.Setup(x => x.TenantId).Returns(1L);
        _dateTimeProviderMock.Setup(x => x.Today).Returns(new DateOnly(2026, 6, 8));

        _settingProviderMock.Setup(x => x.GetSettingAsync<int>("REPORTS_DEFAULT_DAYS", 1L)).ReturnsAsync(30);
        _settingProviderMock.Setup(x => x.GetSettingAsync<string>("REPORTS_DATE_FORMAT", 1L)).ReturnsAsync("yyyy-MM-dd");
        _settingProviderMock.Setup(x => x.GetSettingAsync<string[]>("REPORTS_ALLOWED_FORMATS", 1L)).ReturnsAsync(["json", "pdf", "excel"]);

        _useCase = new ListarReporteVacunosUseCase(
            _repositoryMock.Object,
            _reportFileServiceMock.Object,
            _dateTimeProviderMock.Object,
            _settingProviderMock.Object,
            _tenantContextMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task HandleAsync_CuandoFormatoEsInvalido_DebeLanzarAppException()
    {
        // Arrange
        var query = new ListarReporteVacunosQuery(null, null, null, null, null, null, null, "xml", null, null);

        // Act
        Func<Task> act = async () => await _useCase.HandleAsync(query, CancellationToken.None);

        // Assert
        var exception = await act.Should().ThrowAsync<ApplicationRuleException>()
            .WithMessage("El formato debe ser json, pdf o excel.");
        exception.And.Code.Should().Be("INVALID_REPORT_FORMAT");
        exception.And.Details.Should().Contain(d => d.Field == "formato");
    }

    [Fact]
    public async Task HandleAsync_CuandoEstadoNoEsVivoNiMuerto_DebeLanzarAppException()
    {
        // Arrange
        var query = new ListarReporteVacunosQuery(null, null, null, null, null, "enfermo", null, "json", null, null);

        // Act
        Func<Task> act = async () => await _useCase.HandleAsync(query, CancellationToken.None);

        // Assert
        var exception = await act.Should().ThrowAsync<ApplicationRuleException>()
            .WithMessage("Los datos enviados no son validos.");
        exception.And.Details.Should().Contain(d => d.Field == "estado" && d.Message.Contains("El estado debe ser vivo o muerto"));
    }

    [Fact]
    public async Task HandleAsync_CuandoFiltrosSonValidos_DebeInvocarRepositorioYRetornarData()
    {
        // Arrange
        var query = new ListarReporteVacunosQuery(null, null, null, null, null, "vivo", null, "json", null, null);

        var vacunos = new List<VacunoListadoItem> 
        { 
            new VacunoListadoItem(1, "V001", new DateOnly(2026, 1, 1), "Vaca 1", "Raza", "Procedencia", "Vivo") 
        };
        var pageResult = new ReporteVacunoListadoPage(vacunos, 1);

        _repositoryMock.Setup(x => x.ListarAsync(It.IsAny<ReporteVacunoListadoCriteria>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pageResult);

        // Act
        var result = await _useCase.HandleAsync(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(1);
        result.Resumen.TotalVacunos.Should().Be(1);
        result.Filtros.Estado.Should().Be("vivo");
        result.DownloadUrl.Should().BeNull(); // JSON no genera URL
    }

    [Fact]
    public async Task HandleAsync_CuandoFormatoEsPdf_DebeInvocarGeneradorYRetornarUrl()
    {
        // Arrange
        var query = new ListarReporteVacunosQuery(null, null, null, null, null, null, null, "pdf", null, null);
        var pageResult = new ReporteVacunoListadoPage([], 0);
        
        _repositoryMock.Setup(x => x.ListarAsync(It.IsAny<ReporteVacunoListadoCriteria>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pageResult);

        _reportFileServiceMock.Setup(x => x.GeneratePdfAsync(It.IsAny<IReadOnlyCollection<VacunoListadoItem>>(), It.IsAny<ReporteVacunoResumen>(), It.IsAny<ReporteVacunoFiltros>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ListadoVacunosReportFileResult("test.pdf", "http://url.pdf"));

        // Act
        var result = await _useCase.HandleAsync(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.DownloadUrl.Should().Be("http://url.pdf");
        _reportFileServiceMock.Verify(x => x.GeneratePdfAsync(It.IsAny<IReadOnlyCollection<VacunoListadoItem>>(), It.IsAny<ReporteVacunoResumen>(), It.IsAny<ReporteVacunoFiltros>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
