using Moq;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetHistorialByVacunoId;
using ZooTech.Application.UnitTests.Modules.Module_Sanidad;
using ZooTech.Domain.Module_Sanidad.Interfaces;

namespace ZooTech.Application.UnitTests.Modules.Module_Sanidad.UseCases.GetHistorialByVacunoId;

public class GetHistorialByVacunoIdInteractorTests
{
    [Fact]
    public async Task Handle_PassesFiltersAndMapsItems()
    {
        var repositoryMock = new Mock<ITriajeRepository>();
        repositoryMock.Setup(r => r.GetHistorialByVacunoIdAsync(10, "2026-07-01", "2026-07-31", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { SanidadTestData.CreateHistorialItem(id: 1, tipoPesoCode: "CONTROL", pesoKg: 123m) });
        var interactor = new GetHistorialByVacunoIdInteractor(repositoryMock.Object);

        var result = await interactor.HandleAsync(new GetHistorialByVacunoIdQuery { Vacunoid = 10, FechaDesde = "2026-07-01", FechaHasta = "2026-07-31" });

        var item = Assert.Single(result.Items);
        Assert.Equal(1, item.Id);
        Assert.Equal("CONTROL", item.TipoPesoCode);
        Assert.Equal(123m, item.PesoKg);
    }
}
