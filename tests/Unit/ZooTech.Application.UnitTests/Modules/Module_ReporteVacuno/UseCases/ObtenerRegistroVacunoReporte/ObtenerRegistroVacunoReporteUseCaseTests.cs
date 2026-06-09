using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Gateway.Configuration;
using ZooTech.Application.Common.Gateway.Context;
using ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ObtenerRegistroVacunoReporte;

namespace ZooTech.Application.UnitTests.Modules.Module_ReporteVacuno.UseCases.ObtenerRegistroVacunoReporte;

public class ObtenerRegistroVacunoReporteUseCaseTests
{
    private readonly Mock<IRegistroVacunoReadRepository> _repositoryMock;
    private readonly Mock<IRegistroVacunoExcelReportService> _excelReportServiceMock;
    private readonly Mock<IRegistroVacunoPdfReportService> _pdfReportServiceMock;
    private readonly Mock<ISettingProvider> _settingProviderMock;
    private readonly Mock<ITenantContext> _tenantContextMock;
    private readonly ObtenerRegistroVacunoReporteUseCase _useCase;

    public ObtenerRegistroVacunoReporteUseCaseTests()
    {
        _repositoryMock = new Mock<IRegistroVacunoReadRepository>();
        _excelReportServiceMock = new Mock<IRegistroVacunoExcelReportService>();
        _pdfReportServiceMock = new Mock<IRegistroVacunoPdfReportService>();
        _settingProviderMock = new Mock<ISettingProvider>();
        _tenantContextMock = new Mock<ITenantContext>();

        _tenantContextMock.Setup(x => x.TenantId).Returns(1L);
        _settingProviderMock.Setup(x => x.GetSettingAsync<string[]>("REPORTS_ALLOWED_FORMATS", 1L))
            .ReturnsAsync(["json", "pdf", "excel"]);

        _useCase = new ObtenerRegistroVacunoReporteUseCase(
            _repositoryMock.Object,
            _excelReportServiceMock.Object,
            _pdfReportServiceMock.Object,
            _settingProviderMock.Object,
            _tenantContextMock.Object);
    }

    [Fact]
    public async Task HandleAsync_CuandoVacunoIdEsInvalido_DebeLanzarAppException()
    {
        // Arrange
        var query = new ObtenerRegistroVacunoReporteQuery(0, "json");

        // Act
        Func<Task> act = async () => await _useCase.HandleAsync(query, CancellationToken.None);

        // Assert
        var exception = await act.Should().ThrowAsync<ApplicationRuleException>()
            .WithMessage("Los datos enviados no son validos.");
        exception.And.Details.Should().Contain(d => d.Field == "vacunoId");
    }

    [Fact]
    public async Task HandleAsync_CuandoFormatoEsInvalido_DebeLanzarAppException()
    {
        // Arrange
        var query = new ObtenerRegistroVacunoReporteQuery(1, "xml");

        // Act
        Func<Task> act = async () => await _useCase.HandleAsync(query, CancellationToken.None);

        // Assert
        var exception = await act.Should().ThrowAsync<ApplicationRuleException>();
        exception.And.Code.Should().Be("INVALID_REPORT_FORMAT");
        exception.And.Details.Should().Contain(d => d.Field == "formato");
    }

    [Fact]
    public async Task HandleAsync_CuandoVacunoNoExiste_DebeLanzarNotFoundException()
    {
        // Arrange
        var query = new ObtenerRegistroVacunoReporteQuery(99, "json");
        _repositoryMock.Setup(x => x.ObtenerRegistroAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((RegistroVacunoDetalle?)null);

        // Act
        Func<Task> act = async () => await _useCase.HandleAsync(query, CancellationToken.None);

        // Assert
        var exception = await act.Should().ThrowAsync<ApplicationRuleException>()
            .WithMessage("No existe un vacuno con el ID enviado.");
        exception.And.Code.Should().Be("VACUNO_NOT_FOUND");
    }

    [Fact]
    public async Task HandleAsync_CuandoFormatoEsJson_DebeRetornarDatosSinUrl()
    {
        // Arrange
        var query = new ObtenerRegistroVacunoReporteQuery(1, "json");
        var detalle = new RegistroVacunoDetalle(1, "V001", "Vaca Test", new DateOnly(2026, 1, 1), null, null, "Raza", null, "H", null, null, null, null, "Granja", null, null, null, "Procedencia", "Produccion", null, null, null, null, null, null, null, null, null, null, null, "Vivo", null, null, new DateOnly(2026, 1, 1), null, null, DateTime.UtcNow, null, DateTime.UtcNow);

        _repositoryMock.Setup(x => x.ObtenerRegistroAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(detalle);

        // Act
        var result = await _useCase.HandleAsync(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Vacuno.Should().BeEquivalentTo(detalle);
        result.DownloadUrl.Should().BeNull();
        
        _pdfReportServiceMock.Verify(x => x.GenerateAsync(It.IsAny<RegistroVacunoDetalle>(), It.IsAny<CancellationToken>()), Times.Never);
        _excelReportServiceMock.Verify(x => x.GenerateAsync(It.IsAny<RegistroVacunoDetalle>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_CuandoFormatoEsPdf_DebeInvocarPdfServiceYRetornarUrl()
    {
        // Arrange
        var query = new ObtenerRegistroVacunoReporteQuery(1, "pdf");
        var detalle = new RegistroVacunoDetalle(1, "V001", "Vaca Test", new DateOnly(2026, 1, 1), null, null, "Raza", null, "H", null, null, null, null, "Granja", null, null, null, "Procedencia", "Produccion", null, null, null, null, null, null, null, null, null, null, null, "Vivo", null, null, new DateOnly(2026, 1, 1), null, null, DateTime.UtcNow, null, DateTime.UtcNow);

        _repositoryMock.Setup(x => x.ObtenerRegistroAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(detalle);

        _pdfReportServiceMock.Setup(x => x.GenerateAsync(detalle, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RegistroVacunoPdfReportResult("file.pdf", "http://url.pdf"));

        // Act
        var result = await _useCase.HandleAsync(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.DownloadUrl.Should().Be("http://url.pdf");
        _pdfReportServiceMock.Verify(x => x.GenerateAsync(detalle, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_CuandoFormatoEsExcel_DebeInvocarExcelServiceYRetornarUrl()
    {
        // Arrange
        var query = new ObtenerRegistroVacunoReporteQuery(1, "excel");
        var detalle = new RegistroVacunoDetalle(1, "V001", "Vaca Test", new DateOnly(2026, 1, 1), null, null, "Raza", null, "H", null, null, null, null, "Granja", null, null, null, "Procedencia", "Produccion", null, null, null, null, null, null, null, null, null, null, null, "Vivo", null, null, new DateOnly(2026, 1, 1), null, null, DateTime.UtcNow, null, DateTime.UtcNow);

        _repositoryMock.Setup(x => x.ObtenerRegistroAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(detalle);

        _excelReportServiceMock.Setup(x => x.GenerateAsync(detalle, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RegistroVacunoExcelReportResult("file.xlsx", "http://url.excel"));

        // Act
        var result = await _useCase.HandleAsync(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.DownloadUrl.Should().Be("http://url.excel");
        _excelReportServiceMock.Verify(x => x.GenerateAsync(detalle, It.IsAny<CancellationToken>()), Times.Once);
    }
}
