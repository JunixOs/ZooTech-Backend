using Moq;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetDetalleTriajeByVacunoId;
using ZooTech.Domain.Module_Sanidad.Entities;
using ZooTech.Domain.Module_Sanidad.Interfaces;

namespace ZooTech.Application.UnitTests.Modules.Module_Sanidad.UseCases.GetDetallesTriajeByVacunoId;

public class GetDetallesTriajeByVacunoIdInteractorTests
{
    [Fact]
    public async Task HandleAsync_PassesVacunoIdAndReturnsItems()
    {
        var repositoryMock = new Mock<ITriajeRepository>();
        var items = new[]
        {
            new TriajeDetallePorVacunoItem
            {
                CodigoRegistro = "TRI001",
                FechaHora = DateTime.UtcNow,
                TipoPesoMedido = "Peso Control",
                PesoKg = 120m,
                Observaciones = "Sin observaciones",
            },
        };
        repositoryMock.Setup(r => r.GetDetallesByVacunoIdAsync(15, It.IsAny<CancellationToken>())).ReturnsAsync(items);
        var interactor = new GetDetallesTriajeByVacunoIdInteractor(repositoryMock.Object);

        var result = await interactor.HandleAsync(new GetDetallesTriajeByVacunoIdQuery { VacunoId = 15 });

        var item = Assert.Single(result.Items);
        Assert.Equal("TRI001", item.CodigoRegistro);
        Assert.Equal("Peso Control", item.TipoPesoMedido);
    }
}
