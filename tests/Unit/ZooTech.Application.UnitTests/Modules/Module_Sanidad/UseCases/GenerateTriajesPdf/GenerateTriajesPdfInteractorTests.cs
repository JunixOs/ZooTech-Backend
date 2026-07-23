using Moq;
using ZooTech.Application.Common.Gateway.Services;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosPdf;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GenerateTriajesPdf;
using ZooTech.Application.UnitTests.Modules.Module_Sanidad;
using ZooTech.Domain.Module_Sanidad.Interfaces;

namespace ZooTech.Application.UnitTests.Modules.Module_Sanidad.UseCases.GenerateTriajesPdf;

public class GenerateTriajesPdfInteractorTests
{
    [Fact]
    public async Task HandleAsync_PassesFiltersAndReturnsPdfOutput()
    {
        var generatedAt = new DateTime(2026, 7, 22, 10, 30, 0);
        var repositoryMock = new Mock<ITriajeRepository>();
        var pdfGeneratorMock = new Mock<IPdfGeneratorService>();
        var dateTimeProviderMock = new Mock<IDateTimeProvider>();
        dateTimeProviderMock.Setup(p => p.ServerNow).Returns(generatedAt);
        repositoryMock.Setup(r => r.GetAllAsync(
                1,
                int.MaxValue,
                "2026-07-10",
                "2026-07-01",
                "2026-07-31",
                "TRI",
                "Luna",
                "CONTROL",
                "120",
                5,
                null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((new[] { SanidadTestData.CreateListadoItem() }, 1));
        pdfGeneratorMock.Setup(g => g.GenerateTriajesReport(It.IsAny<GenerateTriajesPdfDocument>())).Returns(new byte[] { 1, 2, 3 });
        var interactor = new GenerateTriajesPdfInteractor(repositoryMock.Object, pdfGeneratorMock.Object, dateTimeProviderMock.Object);

        var result = await interactor.HandleAsync(
            new GenerateTriajesPdfQuery("2026-07-10", "2026-07-01", "2026-07-31", "TRI", "Luna", "CONTROL", "120", 5),
            CancellationToken.None);

        Assert.Equal(new byte[] { 1, 2, 3 }, result.Content);
        Assert.Equal("application/pdf", result.ContentType);
        Assert.EndsWith(".pdf", result.FileName);
        pdfGeneratorMock.Verify(g => g.GenerateTriajesReport(It.Is<GenerateTriajesPdfDocument>(d =>
            d.Items.Count == 1 &&
            d.Codigo == "TRI" &&
            d.Nombre == "Luna" &&
            d.GeneratedAtUtc == generatedAt)), Times.Once);
    }
}
