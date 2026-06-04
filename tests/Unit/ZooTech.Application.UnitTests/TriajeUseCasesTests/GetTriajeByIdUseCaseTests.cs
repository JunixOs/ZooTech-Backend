using Moq;
using System.Threading.Tasks;
using ZooTech.Application.Modules.Module_Sanidad.UseCases;
using ZooTech.Domain.Module_Sanidad.Entities;
using ZooTech.Domain.Module_Sanidad.Interfaces;
using Xunit;

namespace ZooTech.Application.UnitTests;

public class GetTriajeByIdUseCaseTests
{
    private readonly Mock<ITriajeRepository> _repositoryMock;
    private readonly GetTriajeByIdUseCase _useCase;

    public GetTriajeByIdUseCaseTests()
    {
        _repositoryMock = new Mock<ITriajeRepository>();
        _useCase = new GetTriajeByIdUseCase(_repositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_TriajeExistente_DebeRetornarTriaje()
    {
        // Arrange
        var id = 1L;
        var existingTriaje = new Triaje 
        { 
            Id = id, 
            Codigo = "TRI123",
            VacunoId = 1,
            TipoPesoCode = "CONTROL",
            PesoKg = 80.0m
        };
        
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existingTriaje);

        // Act
        var result = await _useCase.ExecuteAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(existingTriaje.Id, result.Id);
        Assert.Equal(existingTriaje.Codigo, result.Codigo);
        Assert.Equal(existingTriaje.VacunoId, result.VacunoId);
        Assert.Equal(existingTriaje.PesoKg, result.PesoKg);
    }

    [Fact]
    public async Task ExecuteAsync_TriajeNoExistente_DebeRetornarNull()
    {
        // Arrange
        var id = 99L;
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Triaje?)null);

        // Act
        var result = await _useCase.ExecuteAsync(id);

        // Assert
        Assert.Null(result);
    }
}