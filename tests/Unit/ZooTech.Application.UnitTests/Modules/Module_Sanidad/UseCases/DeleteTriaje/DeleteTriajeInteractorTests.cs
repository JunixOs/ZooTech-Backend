using Moq;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.DeleteTriaje;
using ZooTech.Application.UnitTests.Modules.Module_Sanidad;
using ZooTech.Domain.Common.Interfaces;
using ZooTech.Domain.Module_Sanidad.Entities;
using ZooTech.Domain.Module_Sanidad.Interfaces;

namespace ZooTech.Application.UnitTests.Modules.Module_Sanidad.UseCases.DeleteTriaje;

public class DeleteTriajeInteractorTests
{
    private readonly Mock<ITriajeRepository> _repositoryMock = new();
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock = new();
    private readonly Mock<IEstadoRegistroRepository> _estadoRegistroRepositoryMock = new();

    [Fact]
    public async Task Handle_WhenTriajeExists_SoftDeletesAndUpdates()
    {
        var now = DateTime.UtcNow;
        var triaje = SanidadTestData.CreateTriaje(id: 7);
        _dateTimeProviderMock.Setup(p => p.ServerNow).Returns(now);
        _estadoRegistroRepositoryMock.Setup(r => r.GetDeletedCodeAsync(It.IsAny<CancellationToken>())).ReturnsAsync("ELIMINADO");
        _repositoryMock.Setup(r => r.GetByIdAsync(7, It.IsAny<CancellationToken>())).ReturnsAsync(triaje);
        var interactor = new DeleteTriajeInteractor(new FakeGanaderiaUnitOfWork(_repositoryMock.Object), _dateTimeProviderMock.Object, _estadoRegistroRepositoryMock.Object);

        await interactor.Handle(new DeleteTriajeCommand { Id = 7, MotivoEliminacion = "Duplicado" }, CancellationToken.None);

        _repositoryMock.Verify(r => r.UpdateAsync(
            It.Is<Triaje>(t => t.IsDeleted && t.MotivoEliminacion == "Duplicado" && t.DeletedAt == now && t.EstadoRegistroCode == "ELIMINADO"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenTriajeDoesNotExist_ThrowsNotFoundException()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(7, It.IsAny<CancellationToken>())).ReturnsAsync((Triaje?)null);
        var interactor = new DeleteTriajeInteractor(new FakeGanaderiaUnitOfWork(_repositoryMock.Object), _dateTimeProviderMock.Object, _estadoRegistroRepositoryMock.Object);

        await Assert.ThrowsAsync<NotFoundException>(() => interactor.Handle(new DeleteTriajeCommand { Id = 7, MotivoEliminacion = "Duplicado" }, CancellationToken.None));
    }
}
