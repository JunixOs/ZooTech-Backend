using Moq;
using Xunit;
using ZooTech.Application.Common.Gateway.Context;
using ZooTech.Application.Common.Gateway.Features;
using ZooTech.Application.Modules.Module_Vacuno.Common;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GenerarArbolGenealogico;

namespace ZooTech.Application.UnitTests.Modules.Module_Vacuno.UseCases;

public class GenerarArbolGenealogicoInteractorTests
{
    private readonly Mock<IVacunoQueryRepository> _repositoryMock;
    private readonly Mock<IGenerarArbolGenealogicoOutputPort> _outputMock;
    private readonly Mock<IFeatureService> _featuresMock;
    private readonly Mock<ITenantContext> _tenantMock;
    private readonly GenerarArbolGenealogicoInteractor _interactor;

    public GenerarArbolGenealogicoInteractorTests()
    {
        _repositoryMock = new Mock<IVacunoQueryRepository>();
        _outputMock = new Mock<IGenerarArbolGenealogicoOutputPort>();
        _featuresMock = new Mock<IFeatureService>();
        _tenantMock = new Mock<ITenantContext>();

        _interactor = new GenerarArbolGenealogicoInteractor(
            _repositoryMock.Object,
            _outputMock.Object,
            _featuresMock.Object,
            _tenantMock.Object);
    }

    [Fact]
    public async Task Handle_DebeRetornarOk_CuandoVacunoExisteYFeatureEstaHabilitado()
    {
        // Arrange
        var command = new GenerarArbolGenealogicoCommand { VacunoId = 1, Niveles = 3 };
        var arbolDto = new VacunoNodoDto { Id = 1, Nombre = "Test" };

        _featuresMock.Setup(f => f.IsEnabledAsync("module.vacunos")).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.GetArbolGenealogicoAsync(command.VacunoId, command.Niveles))
            .ReturnsAsync(arbolDto);

        // Act
        await _interactor.Handle(command);

        // Assert
        _outputMock.Verify(o => o.Ok(It.Is<GenerarArbolGenealogicoOutput>(outp => outp.Data == arbolDto)), Times.Once);
        _outputMock.Verify(o => o.NotFound(It.IsAny<string>()), Times.Never);
        _outputMock.Verify(o => o.Error(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Theory]
    [InlineData(10, 4)]  // Nivel mayor a 4 debe ser corregido a 4
    [InlineData(-5, 1)]  // Nivel menor a 1 debe ser corregido a 1
    [InlineData(0, 1)]   // Nivel 0 debe ser corregido a 1
    public async Task Handle_DebeHacerClampDeNiveles_CuandoSeEnvianNivelesFueraDeRango(int nivelesEnviados, int nivelEsperadoEnRepo)
    {
        // Arrange
        var command = new GenerarArbolGenealogicoCommand { VacunoId = 1, Niveles = nivelesEnviados };
        var arbolDto = new VacunoNodoDto { Id = 1, Nombre = "Test" };

        _featuresMock.Setup(f => f.IsEnabledAsync("module.vacunos")).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.GetArbolGenealogicoAsync(command.VacunoId, nivelEsperadoEnRepo))
            .ReturnsAsync(arbolDto);

        // Act
        await _interactor.Handle(command);

        // Assert
        // Verificamos que al repositorio se le pasó el valor corregido mediante It.Is
        _repositoryMock.Verify(r => r.GetArbolGenealogicoAsync(command.VacunoId, It.Is<int>(n => n == nivelEsperadoEnRepo)), Times.Once);
        
        // Verificamos que culminó en un Ok
        _outputMock.Verify(o => o.Ok(It.IsAny<GenerarArbolGenealogicoOutput>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DebeRetornarNotFound_CuandoRepositorioRetornaNull()
    {
        // Arrange
        var command = new GenerarArbolGenealogicoCommand { VacunoId = 999, Niveles = 4 };

        _featuresMock.Setup(f => f.IsEnabledAsync("module.vacunos")).ReturnsAsync(true);
        // El repositorio devuelve null porque el vacuno no existe o fue eliminado
        _repositoryMock.Setup(r => r.GetArbolGenealogicoAsync(command.VacunoId, command.Niveles))
            .ReturnsAsync((VacunoNodoDto?)null);

        // Act
        await _interactor.Handle(command);

        // Assert
        _outputMock.Verify(o => o.NotFound(It.Is<string>(msg => msg.Contains("999"))), Times.Once);
        _outputMock.Verify(o => o.Ok(It.IsAny<GenerarArbolGenealogicoOutput>()), Times.Never);
    }

    [Fact]
    public async Task Handle_DebeRetornarError_CuandoFeatureFlagEstaDeshabilitado()
    {
        // Arrange
        var command = new GenerarArbolGenealogicoCommand { VacunoId = 1, Niveles = 4 };

        // El feature flag está apagado para este tenant
        _featuresMock.Setup(f => f.IsEnabledAsync("module.vacunos")).ReturnsAsync(false);

        // Act
        await _interactor.Handle(command);

        // Assert
        // Verificamos que el error se disparó por FEATURE_DISABLED
        _outputMock.Verify(o => o.Error("FEATURE_DISABLED", It.IsAny<string>()), Times.Once);
        
        // Verificamos que NUNCA se llamó al repositorio para evitar consultas inútiles a base de datos
        _repositoryMock.Verify(r => r.GetArbolGenealogicoAsync(It.IsAny<long>(), It.IsAny<int>()), Times.Never);
        
        // Verificamos que no se llamó a Ok ni a NotFound
        _outputMock.Verify(o => o.Ok(It.IsAny<GenerarArbolGenealogicoOutput>()), Times.Never);
        _outputMock.Verify(o => o.NotFound(It.IsAny<string>()), Times.Never);
    }
}
