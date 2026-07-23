using Moq;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetTriajeById;
using ZooTech.Application.UnitTests.Modules.Module_Sanidad;
using ZooTech.Domain.Module_Sanidad.Entities;
using ZooTech.Domain.Module_Sanidad.Interfaces;

namespace ZooTech.Application.UnitTests.Modules.Module_Sanidad.UseCases.GetTriajeById;

public class GetTriajeByIdInteractorTests
{
    private readonly Mock<ITriajeRepository> _repositoryMock = new();

    [Fact]
    public async Task Handle_WhenTriajeExists_ReturnsOutput()
    {
        var triaje = SanidadTestData.CreateTriaje(id: 5, codigo: "TRI005", vacunoNombre: "Estrella");
        _repositoryMock.Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(triaje);
        var interactor = new GetTriajeByIdInteractor(_repositoryMock.Object);

        var result = await interactor.Handle(new GetTriajeByIdCommand { Id = 5 });

        Assert.Equal(5, result.Id);
        Assert.Equal("TRI005", result.Codigo);
        Assert.Equal("Estrella", result.VacunoNombre);
    }

    [Fact]
    public async Task Handle_WhenTriajeDoesNotExist_ThrowsNotFoundException()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((Triaje?)null);
        var interactor = new GetTriajeByIdInteractor(_repositoryMock.Object);

        await Assert.ThrowsAsync<NotFoundException>(() => interactor.Handle(new GetTriajeByIdCommand { Id = 99 }));
    }
}
