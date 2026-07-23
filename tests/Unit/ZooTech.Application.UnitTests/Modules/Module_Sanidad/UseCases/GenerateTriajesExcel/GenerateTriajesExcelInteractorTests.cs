using Moq;
using ZooTech.Application.Common.Gateway.Services;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosExcel;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GenerateTriajesExcel;
using ZooTech.Application.UnitTests.Modules.Module_Sanidad;
using ZooTech.Domain.Module_Sanidad.Interfaces;

namespace ZooTech.Application.UnitTests.Modules.Module_Sanidad.UseCases.GenerateTriajesExcel;

public class GenerateTriajesExcelInteractorTests
{
    [Fact]
    public async Task HandleAsync_PassesFiltersAndReturnsExcelOutput()
    {
        var generatedAt = new DateTime(2026, 7, 22, 10, 30, 0);
        var repositoryMock = new Mock<ITriajeRepository>();
        var excelGeneratorMock = new Mock<IExcelGeneratorService>();
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
                120m,
                5,
                null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((new[] { SanidadTestData.CreateListadoItem() }, 1));
        excelGeneratorMock.Setup(g => g.GenerateTriajesReport(It.IsAny<GenerateTriajesExcelDocument>())).Returns(new byte[] { 4, 5, 6 });
        var interactor = new GenerateTriajesExcelInteractor(repositoryMock.Object, excelGeneratorMock.Object, dateTimeProviderMock.Object);

        var result = await interactor.HandleAsync(
            new GenerateTriajesExcelQuery("2026-07-10", "2026-07-01", "2026-07-31", "TRI", "Luna", "CONTROL", 120m, 5),
            CancellationToken.None);

        Assert.Equal(new byte[] { 4, 5, 6 }, result.Content);
        Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", result.ContentType);
        Assert.EndsWith(".xlsx", result.FileName);
        excelGeneratorMock.Verify(g => g.GenerateTriajesReport(It.Is<GenerateTriajesExcelDocument>(d =>
            d.Items.Count == 1 &&
            d.Codigo == "TRI" &&
            d.Nombre == "Luna" &&
            d.GeneratedAtUtc == generatedAt)), Times.Once);
    }
}
