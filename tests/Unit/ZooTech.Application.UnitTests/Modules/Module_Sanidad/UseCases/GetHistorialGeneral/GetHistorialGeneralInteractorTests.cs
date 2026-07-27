using Moq;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetHistorialGeneral;
using ZooTech.Application.UnitTests.Modules.Module_Sanidad;
using ZooTech.Domain.Module_Sanidad.Interfaces;

namespace ZooTech.Application.UnitTests.Modules.Module_Sanidad.UseCases.GetHistorialGeneral;

public class GetHistorialGeneralInteractorTests
{
    [Fact]
    public async Task HandleAsync_PassesFiltersAndMapsItems()
    {
        var repositoryMock = new Mock<ITriajeRepository>();
        repositoryMock.Setup(r => r.GetHistorialGeneralAsync("2026-07-01", "2026-07-31", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { SanidadTestData.CreateHistorialItem(id: 2, tipoPesoCode: "FINAL", pesoKg: 140m) });
        var interactor = new GetHistorialGeneralInteractor(repositoryMock.Object);

        var result = await interactor.HandleAsync(new GetHistorialGeneralQuery { FechaDesde = "2026-07-01", FechaHasta = "2026-07-31" });

        var item = Assert.Single(result.Items);
        Assert.Equal(2, item.Id);
        Assert.Equal("FINAL", item.TipoPesoCode);
        Assert.Equal(140m, item.PesoKg);
    }
}
