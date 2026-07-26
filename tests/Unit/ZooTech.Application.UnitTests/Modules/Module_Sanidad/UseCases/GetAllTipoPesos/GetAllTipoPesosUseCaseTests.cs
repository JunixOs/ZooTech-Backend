using Moq;
using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllTipoPesos;
using ZooTech.Domain.Module_Sanidad.Entities;
using ZooTech.Domain.Module_Sanidad.Interfaces;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.UnitTests.Modules.Module_Sanidad.UseCases.GetAllTipoPesos;

public class GetAllTipoPesosUseCaseTests
{
    private readonly Mock<ITipoPesoRepository> _repositoryMock;

    public GetAllTipoPesosUseCaseTests()
    {
        _repositoryMock = new Mock<ITipoPesoRepository>();
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
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(tipos);
        var useCase = new GetAllTipoPesosInteractor(_repositoryMock.Object);

        // Act
        var result = await useCase.Handle(EmptyCommandQuery.Value(AuditEventType.Read, "Get all tipo pesos"));

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Items.Count);
        Assert.Equal("CONTROL", result.Items.First().Code);
        Assert.Equal("Peso de Control", result.Items.First().Nombre);
    }

    [Fact]
    public async Task ExecuteAsync_CuandoNoHayTipos_DebeRetornarListaVacia()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<TipoPeso>());
        var useCase = new GetAllTipoPesosInteractor(_repositoryMock.Object);

        // Act
        var result = await useCase.Handle(EmptyCommandQuery.Value(AuditEventType.Read, "Get all tipo pesos"));

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Items);
    }
}
