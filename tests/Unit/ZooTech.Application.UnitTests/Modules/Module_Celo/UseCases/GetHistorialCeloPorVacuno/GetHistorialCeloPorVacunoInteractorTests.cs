using NSubstitute;
using ZooTech.Application.Modules.Module_Celo.UseCases.GetHistorialCeloPorVacuno;
using ZooTech.Domain.Module_Celo.Entities;
using ZooTech.Domain.Module_Celo.Interfaces;

namespace ZooTech.Application.UnitTests.Modules.Module_Celo.UseCases.GetHistorialCeloPorVacuno;

public sealed class GetHistorialCeloPorVacunoInteractorTests
{
    [Fact]
    public async Task HandleAsync_ReturnsHistoryAndReproductiveSummaryForTheRequestedCow()
    {
        var repository = Substitute.For<ICeloDetalleRepository>();
        var fechaHora = new DateTime(2026, 7, 24, 10, 30, 0, DateTimeKind.Utc);
        var detalle = new CeloDetallePorVacuno(
            "Ana",
            [
                new CeloHistorialResumenItem(3, fechaHora, true),
                new CeloHistorialResumenItem(2, fechaHora.AddDays(-21), false),
                new CeloHistorialResumenItem(1, fechaHora.AddDays(-42), null),
            ],
            new CeloResumenReproductivo(3, 1, 2, 1, 1, 1, new DateOnly(2026, 3, 15)));
        repository.GetDetallePorVacunoAsync("VAC-001", 306, Arg.Any<CancellationToken>())
            .Returns(detalle);
        var interactor = new GetHistorialCeloPorVacunoInteractor(repository);

        var output = await interactor.HandleAsync(new GetHistorialCeloPorVacunoCommand("VAC-001", 306));

        Assert.Equal(3, output.Historial.Count);
        Assert.Equal("Ana", output.Encargado);
        Assert.True(output.Historial[0].Resultado);
        Assert.False(output.Historial[1].Resultado);
        Assert.Null(output.Historial[2].Resultado);
        Assert.Equal(1, output.Resumen.Embarazos);
        Assert.Equal(new DateOnly(2026, 3, 15), output.Resumen.UltimoParto);
        await repository.Received(1).GetDetallePorVacunoAsync("VAC-001", 306, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ReturnsEmptyHistoryWhenTheCowHasNoActiveRecords()
    {
        var repository = Substitute.For<ICeloDetalleRepository>();
        repository.GetDetallePorVacunoAsync("VAC-404", 404, Arg.Any<CancellationToken>())
            .Returns(new CeloDetallePorVacuno(null, [], CeloResumenReproductivo.Empty));
        var interactor = new GetHistorialCeloPorVacunoInteractor(repository);

        var output = await interactor.HandleAsync(new GetHistorialCeloPorVacunoCommand("VAC-404", 404));

        Assert.Empty(output.Historial);
        Assert.Equal(0, output.Resumen.Celos);
    }
}
