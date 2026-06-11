using Moq;
using ZooTech.Application.Modules.Module_Sanidad.UseCases;
using ZooTech.Domain.Module_Sanidad.Entities;
using ZooTech.Domain.Module_Sanidad.Interfaces;

namespace ZooTech.Application.UnitTests.Modules.Module_Sanidad.UseCases.GetAllVacunosSanidad;

public class GetAllVacunosSanidadUseCaseTests
{
    private readonly Mock<ITriajeRepository> _repositoryMock;

    public GetAllVacunosSanidadUseCaseTests()
    {
        _repositoryMock = new Mock<ITriajeRepository>();
    }

    [Fact]
    public async Task ExecuteAsync_DebeRetornarListaDeVacunos()
    {
        // Arrange
        var vacunos = new List<VacunoOption>
        {
            new() { Id = 1, Codigo = "VAC001", Nombre = "Estrella" },
            new() { Id = 2, Codigo = "VAC002", Nombre = "Luna" },
            new() { Id = 3, Codigo = "VAC003", Nombre = "Toro Rey" }
        };
        _repositoryMock.Setup(r => r.GetAllVacunosAsync()).ReturnsAsync(vacunos);
        var useCase = new GetAllVacunosUseCase(_repositoryMock.Object);

        // Act
        var result = await useCase.ExecuteAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
        Assert.Equal("VAC001", result.First().Codigo);
        Assert.Equal("Estrella", result.First().Nombre);
    }

    [Fact]
    public async Task ExecuteAsync_CuandoNoHayVacunos_DebeRetornarListaVacia()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetAllVacunosAsync()).ReturnsAsync(new List<VacunoOption>());
        var useCase = new GetAllVacunosUseCase(_repositoryMock.Object);

        // Act
        var result = await useCase.ExecuteAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
