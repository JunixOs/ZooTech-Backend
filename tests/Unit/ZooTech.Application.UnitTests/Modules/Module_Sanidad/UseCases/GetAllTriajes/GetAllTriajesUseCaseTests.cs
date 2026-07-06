using Moq;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllTriajes;
using ZooTech.Domain.Module_Sanidad.Entities;
using ZooTech.Domain.Module_Sanidad.Interfaces;

namespace ZooTech.Application.UnitTests.Modules.Module_Sanidad.UseCases.GetAllTriajes;

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
            CreateTriaje(1, "TRI001", 1, "Estrella", "CONTROL", 100),
            CreateTriaje(2, "TRI002", 2, "Luna", "FINAL", 200)
        };
        _repositoryMock.Setup(r => r.GetAllAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<decimal?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((triajes, triajes.Count));
        var useCase = new GetAllTriajesInteractor(_repositoryMock.Object);

        // Act
        var result = await useCase.HandleAsync(new GetAllTriajesQuery(1, 10));

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Data.Count);
        Assert.Equal("TRI001", result.Data.First().Codigo);
        Assert.Equal("Estrella", result.Data.First().VacunoNombre);
    }

    [Fact]
    public async Task ExecuteAsync_CuandoNoHayTriajes_DebeRetornarListaVacia()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetAllAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<decimal?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Triaje>(), 0));
        var useCase = new GetAllTriajesInteractor(_repositoryMock.Object);

        // Act
        var result = await useCase.HandleAsync(new GetAllTriajesQuery(1, 10));

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Data);
    }

    private static Triaje CreateTriaje(long id, string codigo, long vacunoId, string vacunoNombre, string tipoPesoCode, decimal pesoKg)
    {
        var now = DateTime.UtcNow;

        return Triaje.Rehydrate(
            id: id,
            codigo: codigo,
            fechaHora: now,
            vacunoId: vacunoId,
            vacunoNombre: vacunoNombre,
            tipoPesoCode: tipoPesoCode,
            pesoKg: pesoKg,
            observaciones: null,
            estadoRegistroCode: "ACTIVO",
            encargadoUsuarioId: null,
            createdBy: null,
            updatedBy: null,
            deletedBy: null,
            createdAt: now,
            updatedAt: now,
            deletedAt: null,
            motivoEliminacion: null);
    }
}
