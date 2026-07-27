using FluentAssertions;
using NSubstitute;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Gateway.Parametrization;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GetArbolGenealogico;
using ZooTech.Domain.Configuration;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Entities;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Entities.GetArbolGenealogico;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Interfaces;

namespace ZooTech.Application.UnitTests.Modules.Module_Vacuno.UseCases;

public class GetArbolGenealogicoInteractorTests
{
    private readonly IVacunoRepository _vacunoRepositoryMock;
    private readonly ITenantConfigurationProvider _tenantConfigurationProviderMock;
    private readonly GetArbolGenealogicoInteractor _interactor;

    public GetArbolGenealogicoInteractorTests()
    {
        _vacunoRepositoryMock = Substitute.For<IVacunoRepository>();
        _tenantConfigurationProviderMock = Substitute.For<ITenantConfigurationProvider>();
        _interactor = new GetArbolGenealogicoInteractor(_vacunoRepositoryMock, _tenantConfigurationProviderMock);
    }

    [Fact]
    public async Task HandleAsync_WhenVacunoExists_ShouldClampNivelesAndReturnNodos()
    {
        // Arrange
        var vacunoId = 1L;
        var command = new GetArbolGenealogicoQuery(vacunoId, 5); // 5 exceeds max allowed (4)
        var raiz = Vacuno.Rehydrate(vacunoId, "V1", "Estrella", new DateOnly(2020, 1, 1), "COMPRA", "HOLSTEIN", "BLANCO_NEGRO", "HEMBRA", null, null, 1, null, new DateOnly(2020, 1, 1), DateTime.UtcNow, DateTime.UtcNow, null, null, null, null, null);
        var arbol = new List<VacunoGenealogiaNode> { new VacunoGenealogiaNode(raiz, 1, null) };

        _vacunoRepositoryMock.GetByIdAsync(vacunoId, Arg.Any<CancellationToken>()).Returns(raiz);
        _tenantConfigurationProviderMock.GetSettingAsync(Settings.Vacunos.VacunosArbolMinNiveles).Returns(1);
        _tenantConfigurationProviderMock.GetSettingAsync(Settings.Vacunos.VacunosArbolMaxNiveles).Returns(4);
        
        // Repository should receive 4 (clamped), not 5
        _vacunoRepositoryMock.GetArbolGenealogicoAsync(vacunoId, 4, Arg.Any<CancellationToken>()).Returns(arbol);

        // Act
        var result = await _interactor.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.Arbol.Should().HaveCount(1);
        result.Arbol[0].Id.Should().Be(vacunoId);
        result.Arbol[0].Codigo.Should().Be("V1");

        await _vacunoRepositoryMock.Received(1).GetByIdAsync(vacunoId, Arg.Any<CancellationToken>());
        await _tenantConfigurationProviderMock.Received(1).GetSettingAsync(Settings.Vacunos.VacunosArbolMinNiveles);
        await _tenantConfigurationProviderMock.Received(1).GetSettingAsync(Settings.Vacunos.VacunosArbolMaxNiveles);
        await _vacunoRepositoryMock.Received(1).GetArbolGenealogicoAsync(vacunoId, 4, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenVacunoDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        var vacunoId = 99L;
        var command = new GetArbolGenealogicoQuery(vacunoId, 4);

        _vacunoRepositoryMock.GetByIdAsync(vacunoId, Arg.Any<CancellationToken>()).Returns((Vacuno?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _interactor.HandleAsync(command));
        exception.Message.Should().Contain(vacunoId.ToString());

        await _vacunoRepositoryMock.Received(1).GetByIdAsync(vacunoId, Arg.Any<CancellationToken>());
        await _tenantConfigurationProviderMock.DidNotReceive().GetSettingAsync(Arg.Any<SettingDefinition<int>>());
        await _vacunoRepositoryMock.DidNotReceive().GetArbolGenealogicoAsync(Arg.Any<long>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
    }
}
