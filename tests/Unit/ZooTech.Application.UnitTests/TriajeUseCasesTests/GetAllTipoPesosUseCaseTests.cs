using Moq;
using ZooTech.Application.Modules.Module_Sanidad.UseCases;
using ZooTech.Domain.Module_Sanidad.Entities;
using ZooTech.Domain.Module_Sanidad.Interfaces;

namespace ZooTech.Application.UnitTests;

public class GetAllTipoPesosUseCaseTests
{
    private readonly Mock<ITriajeRepository> _repositoryMock;

    public GetAllTipoPesosUseCaseTests()
    {
        _repositoryMock = new Mock<ITriajeRepository>();
    }

    [Fact]
    public async Task ExecuteAsync_DebeRetornarListaDeTiposPeso()
    {
        // Arrange
        var tipos = new List<TipoPeso>
        {
            new() { Code = "CONTROL", Nombre = "Peso de Control" },
            new() { Code = "FINAL", Nombre = "Peso Final" },
            new() { Code = "INICIAL", Nombre = "Peso Inicial" }
        };
        _repositoryMock.Setup(r => r.GetAllTipoPesosAsync()).ReturnsAsync(tipos);
        var useCase = new GetAllTipoPesosUseCase(_repositoryMock.Object);

        // Act
        var result = await useCase.ExecuteAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
        Assert.Equal("CONTROL", result.First().Code);
        Assert.Equal("Peso de Control", result.First().Nombre);
    }

    [Fact]
    public async Task ExecuteAsync_CuandoNoHayTipos_DebeRetornarListaVacia()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetAllTipoPesosAsync()).ReturnsAsync(new List<TipoPeso>());
        var useCase = new GetAllTipoPesosUseCase(_repositoryMock.Object);

        // Act
        var result = await useCase.ExecuteAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}