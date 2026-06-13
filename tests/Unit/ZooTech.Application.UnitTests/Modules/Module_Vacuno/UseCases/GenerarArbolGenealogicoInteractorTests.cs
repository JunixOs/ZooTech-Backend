using Moq;
using FluentAssertions;
using Xunit;
using ZooTech.Application.Common.Configuration;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GenerarArbolGenealogico;
using ZooTech.Domain.Module_Vacuno.Entities;
using ZooTech.Domain.Module_Vacuno.Interfaces;


namespace ZooTech.Application.UnitTests.Modules.Module_Vacuno.UseCases;

public class GenerarArbolGenealogicoInteractorTests
{
    private readonly Mock<IVacunoRepository> _vacunoRepositoryMock;
    private readonly Mock<IVacunosConfiguration> _settingsMock;
    private readonly GenerarArbolGenealogicoInteractor _interactor;

    public GenerarArbolGenealogicoInteractorTests()
    {
        _vacunoRepositoryMock = new Mock<IVacunoRepository>();
        _settingsMock = new Mock<IVacunosConfiguration>();
        
        _settingsMock.Setup(x => x.ArbolMinNiveles).Returns(1);
        _settingsMock.Setup(x => x.ArbolMaxNiveles).Returns(4);

        _interactor = new GenerarArbolGenealogicoInteractor(_vacunoRepositoryMock.Object, _settingsMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WhenVacunoDoesNotExist_ThrowsArgumentException()
    {
        // Arrange
        _vacunoRepositoryMock.Setup(x => x.ExistsAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new GenerarArbolGenealogicoCommand(4);

        // Act
        var act = async () => await _interactor.HandleAsync(999, command);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("No se encontró el vacuno con ID 999");
    }

    [Fact]
    public async Task HandleAsync_WhenVacunoExists_ReturnsTreeSuccessfully()
    {
        // Arrange
        _vacunoRepositoryMock.Setup(x => x.ExistsAsync(1L, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var fakeVacunos = new List<Vacuno>
        {
            Vacuno.Rehydrate(1L, "V-001", "Lola", DateOnly.Parse("2020-01-01"), "C01", "R01", "C01", "HEMBRA", null, null, 1L, null, DateOnly.FromDateTime(DateTime.UtcNow), DateTime.UtcNow, DateTime.UtcNow, null, null, null, null, null)
        };

        _vacunoRepositoryMock.Setup(x => x.GetArbolGenealogicoAsync(1L, 4, It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeVacunos);

        var command = new GenerarArbolGenealogicoCommand(4);

        // Act
        var result = await _interactor.HandleAsync(1L, command);

        // Assert
        result.Should().NotBeNull();
        result.Arbol.Should().HaveCount(1);
        result.Arbol.First().Codigo.Should().Be("V-001");
    }

    [Fact]
    public async Task HandleAsync_WhenNivelesIsLessThanMin_UsesMinNiveles()
    {
        // Arrange
        _vacunoRepositoryMock.Setup(x => x.ExistsAsync(1L, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _vacunoRepositoryMock.Setup(x => x.GetArbolGenealogicoAsync(1L, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Vacuno>());

        var command = new GenerarArbolGenealogicoCommand(0); // below min

        // Act
        await _interactor.HandleAsync(1L, command);

        // Assert
        _vacunoRepositoryMock.Verify(x => x.GetArbolGenealogicoAsync(1L, 1, It.IsAny<CancellationToken>()), Times.Once);
    }
}
