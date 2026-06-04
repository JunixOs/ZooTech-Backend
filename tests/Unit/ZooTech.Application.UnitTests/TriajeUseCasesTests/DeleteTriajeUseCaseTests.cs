using Moq;
using System.Threading.Tasks;
using ZooTech.Application.Modules.Module_Sanidad.UseCases;
using ZooTech.Domain.Module_Sanidad.Entities;
using ZooTech.Domain.Module_Sanidad.Interfaces;
using Xunit;

namespace ZooTech.Application.UnitTests;

public class DeleteTriajeUseCaseTests
{
    private readonly Mock<ITriajeRepository> _repositoryMock;
    private readonly DeleteTriajeUseCase _useCase;

    public DeleteTriajeUseCaseTests()
    {
        _repositoryMock = new Mock<ITriajeRepository>();
        _useCase = new DeleteTriajeUseCase(_repositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_TriajeExistente_DebeEliminarYRetornarTrue()
    {
        // Arrange
        var id = 1L;
        var existingTriaje = new Triaje { Id = id };
        
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existingTriaje);

        // Act
        var result = await _useCase.ExecuteAsync(id);

        // Assert
        Assert.True(result);
        _repositoryMock.Verify(r => r.DeleteAsync(id), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_TriajeNoExistente_DebeRetornarFalse()
    {
        // Arrange
        var id = 99L;
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Triaje?)null);

        // Act
        var result = await _useCase.ExecuteAsync(id);

        // Assert
        Assert.False(result);
        _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<long>()), Times.Never);
    }
}