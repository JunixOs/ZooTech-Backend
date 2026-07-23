using Moq;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.UpdateTriaje;
using ZooTech.Application.UnitTests.Modules.Module_Sanidad;
using ZooTech.Domain.Module_Sanidad.Entities;
using ZooTech.Domain.Module_Sanidad.Interfaces;

namespace ZooTech.Application.UnitTests.Modules.Module_Sanidad.UseCases.UpdateTriaje;

public class UpdateTriajeInteractorTests
{
    private readonly Mock<ITriajeRepository> _repositoryMock = new();
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock = new();

    [Fact]
    public async Task Handle_WhenTriajeExists_UpdatesAndReturnsOutput()
    {
        var now = DateTime.UtcNow;
        var triaje = SanidadTestData.CreateTriaje(id: 3);
        _dateTimeProviderMock.Setup(p => p.ServerNow).Returns(now);
        _repositoryMock.Setup(r => r.GetByIdAsync(3, It.IsAny<CancellationToken>())).ReturnsAsync(triaje);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Triaje>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Triaje t, CancellationToken _) => t);
        var interactor = new UpdateTriajeInteractor(_repositoryMock.Object, _dateTimeProviderMock.Object);

        var result = await interactor.Handle(new UpdateTriajeCommand
        {
            Id = 3,
            TipoPesoCode = "FINAL",
            PesoKg = 150.25m,
            Observaciones = "Actualizado",
            EncargadoUsuarioId = 20,
        });

        Assert.Equal("FINAL", result.TipoPesoCode);
        Assert.Equal(150.25m, result.PesoKg);
        Assert.Equal("Actualizado", result.Observaciones);
        Assert.Equal(20, result.EncargadoUsuarioId);
        Assert.Equal(now, result.UpdatedAt);
        _repositoryMock.Verify(r => r.UpdateAsync(It.Is<Triaje>(t => t.Id == 3), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenTriajeDoesNotExist_ThrowsNotFoundException()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(3, It.IsAny<CancellationToken>())).ReturnsAsync((Triaje?)null);
        var interactor = new UpdateTriajeInteractor(_repositoryMock.Object, _dateTimeProviderMock.Object);

        await Assert.ThrowsAsync<NotFoundException>(() => interactor.Handle(new UpdateTriajeCommand { Id = 3, TipoPesoCode = "FINAL", PesoKg = 150 }));
    }
}
