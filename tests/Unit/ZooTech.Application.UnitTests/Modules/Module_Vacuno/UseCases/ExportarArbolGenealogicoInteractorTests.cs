using FluentAssertions;
using NSubstitute;
using Xunit;
using ZooTech.Application.Common.Gateway.Services;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ExportarArbolGenealogico;
using ZooTech.Domain.Module_Vacuno.Entities;
using ZooTech.Domain.Module_Vacuno.Interfaces;
using ZooTech.Domain.Module_Vacuno.ReadModels.GetArbolGenealogico;
using ZooTech.Application.Common.Exceptions;

namespace ZooTech.Application.UnitTests.Modules.Module_Vacuno.UseCases;

public class ExportarArbolGenealogicoInteractorTests
{
    private readonly IVacunoRepository _vacunoRepositoryMock;
    private readonly IArbolGenealogicoExportService _exportServiceMock;
    private readonly ExportarArbolGenealogicoInteractor _interactor;

    public ExportarArbolGenealogicoInteractorTests()
    {
        _vacunoRepositoryMock = Substitute.For<IVacunoRepository>();
        _exportServiceMock = Substitute.For<IArbolGenealogicoExportService>();
        _interactor = new ExportarArbolGenealogicoInteractor(_vacunoRepositoryMock, _exportServiceMock);
    }

    [Fact]
    public async Task HandleAsync_WhenVacunoExists_ShouldReturnBytes()
    {
        // Arrange
        var vacunoId = 1L;
        var command = new ExportarArbolGenealogicoCommand(4);
        var raiz = Vacuno.Rehydrate(vacunoId, "V1", "Estrella", new DateOnly(2020, 1, 1), "COMPRA", "HOLSTEIN", "BLANCO_NEGRO", "HEMBRA", null, null, 1, null, new DateOnly(2020, 1, 1), DateTime.UtcNow, DateTime.UtcNow, null, null, null, null, null);
        var arbol = new List<VacunoGenealogiaNode> { new VacunoGenealogiaNode(raiz, 1, null) };
        var expectedBytes = new byte[] { 0x01, 0x02 };

        _vacunoRepositoryMock.GetByIdAsync(vacunoId, Arg.Any<CancellationToken>()).Returns(raiz);
        _vacunoRepositoryMock.GetArbolGenealogicoAsync(vacunoId, command.Niveles, Arg.Any<CancellationToken>()).Returns(arbol);
        _exportServiceMock.GenerateExcelAsync(arbol, raiz, Arg.Any<CancellationToken>()).Returns(expectedBytes);

        // Act
        var result = await _interactor.HandleAsync(vacunoId, command);

        // Assert
        result.Should().BeEquivalentTo(expectedBytes);
        await _vacunoRepositoryMock.Received(1).GetArbolGenealogicoAsync(vacunoId, command.Niveles, Arg.Any<CancellationToken>());
        await _exportServiceMock.Received(1).GenerateExcelAsync(arbol, raiz, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenVacunoDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        var vacunoId = 99L;
        var command = new ExportarArbolGenealogicoCommand(4);

        _vacunoRepositoryMock.GetByIdAsync(vacunoId, Arg.Any<CancellationToken>()).Returns((Vacuno?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _interactor.HandleAsync(vacunoId, command));
        exception.Message.Should().Contain(vacunoId.ToString());

        await _vacunoRepositoryMock.DidNotReceive().GetArbolGenealogicoAsync(Arg.Any<long>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
        await _exportServiceMock.DidNotReceive().GenerateExcelAsync(Arg.Any<List<VacunoGenealogiaNode>>(), Arg.Any<Vacuno>(), Arg.Any<CancellationToken>());
    }
}
