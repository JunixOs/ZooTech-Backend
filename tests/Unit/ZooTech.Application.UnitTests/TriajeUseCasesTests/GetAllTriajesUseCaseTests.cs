using Moq;
using ZooTech.Application.Modules.Module_Sanidad.UseCases;
using ZooTech.Domain.Module_Sanidad.Entities;
using ZooTech.Domain.Module_Sanidad.Interfaces;

namespace ZooTech.Application.UnitTests;

public class GetAllTriajesUseCaseTests
{
    private readonly Mock<ITriajeRepository> _repositoryMock;

    public GetAllTriajesUseCaseTests()
    {
        _repositoryMock = new Mock<ITriajeRepository>();
    }

    [Fact]
    public async Task ExecuteAsync_DebeRetornarListaDeTriajes()
    {
        // Arrange
        var triajes = new List<Triaje>
        {
            new() { Id = 1, Codigo = "TRI001", VacunoId = 1, VacunoNombre = "Estrella", TipoPesoCode = "CONTROL", PesoKg = 100, FechaHora = DateTime.Now, EstadoRegistroCode = "ACTIVO", CreatedAt = DateTime.Now },
            new() { Id = 2, Codigo = "TRI002", VacunoId = 2, VacunoNombre = "Luna", TipoPesoCode = "FINAL", PesoKg = 200, FechaHora = DateTime.Now, EstadoRegistroCode = "ACTIVO", CreatedAt = DateTime.Now }
        };
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(triajes);
        var useCase = new GetAllTriajesUseCase(_repositoryMock.Object);

        // Act
        var result = await useCase.ExecuteAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Equal("TRI001", result.First().Codigo);
        Assert.Equal("Estrella", result.First().VacunoNombre);
    }

    [Fact]
    public async Task ExecuteAsync_CuandoNoHayTriajes_DebeRetornarListaVacia()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Triaje>());
        var useCase = new GetAllTriajesUseCase(_repositoryMock.Object);

        // Act
        var result = await useCase.ExecuteAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}