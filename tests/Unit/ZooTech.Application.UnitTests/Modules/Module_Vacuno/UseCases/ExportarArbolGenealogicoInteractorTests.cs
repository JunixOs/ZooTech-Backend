using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Xunit;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Gateway.Parametrization;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ExportarArbolGenealogico;
using ZooTech.Domain.Configuration;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Entities;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Entities.GetArbolGenealogico;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Interfaces;
using ZooTech.Application.Common.Gateway.Services;

namespace ZooTech.Application.UnitTests.Modules.Module_Vacuno.UseCases;

public class ExportarArbolGenealogicoInteractorTests
{
    private readonly IVacunoRepository _vacunoRepositoryMock;
    private readonly IArbolGenealogicoExportService _exportServiceMock;
    private readonly ITenantConfigurationProvider _tenantConfigurationProviderMock;
    private readonly ExportarArbolGenealogicoInteractor _interactor;

    public ExportarArbolGenealogicoInteractorTests()
    {
        _vacunoRepositoryMock = Substitute.For<IVacunoRepository>();
        _exportServiceMock = Substitute.For<IArbolGenealogicoExportService>();
        _tenantConfigurationProviderMock = Substitute.For<ITenantConfigurationProvider>();
        _interactor = new ExportarArbolGenealogicoInteractor(_vacunoRepositoryMock, _exportServiceMock, _tenantConfigurationProviderMock);
    }

    [Fact]
    public async Task HandleAsync_WhenFormatoIsExcel_ShouldClampNivelesAndReturnExcelBytes()
    {
        // Arrange
        var vacunoId = 1L;
        var command = new ExportarArbolGenealogicoCommand(vacunoId, 5, "excel"); // 5 > 4 (max)
        var raiz = Vacuno.Rehydrate(vacunoId, "V1", "Estrella", new DateOnly(2020, 1, 1), "COMPRA", "HOLSTEIN", "BLANCO_NEGRO", "HEMBRA", null, null, 1, null, new DateOnly(2020, 1, 1), DateTime.UtcNow, DateTime.UtcNow, null, null, null, null, null);
        var arbol = new List<VacunoGenealogiaNode> { new VacunoGenealogiaNode(raiz, 1, null) };
        var expectedBytes = new byte[] { 0x01, 0x02 };

        _vacunoRepositoryMock.GetByIdAsync(vacunoId, Arg.Any<CancellationToken>()).Returns(raiz);
        _tenantConfigurationProviderMock.GetSettingAsync(Settings.Vacunos.VacunosArbolMinNiveles).Returns(1);
        _tenantConfigurationProviderMock.GetSettingAsync(Settings.Vacunos.VacunosArbolMaxNiveles).Returns(4);

        _vacunoRepositoryMock.GetArbolGenealogicoAsync(vacunoId, 4, Arg.Any<CancellationToken>()).Returns(arbol);
        _exportServiceMock.GenerateExcelAsync(arbol, raiz, Arg.Any<CancellationToken>()).Returns(expectedBytes);

        // Act
        var result = await _interactor.HandleAsync(command);

        // Assert
        result.Bytes.Should().BeEquivalentTo(expectedBytes);
        result.ContentType.Should().Be("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        result.FileName.Should().Contain(".xlsx");

        await _vacunoRepositoryMock.Received(1).GetArbolGenealogicoAsync(vacunoId, 4, Arg.Any<CancellationToken>());
        await _exportServiceMock.Received(1).GenerateExcelAsync(arbol, raiz, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenFormatoIsPdf_ShouldClampNivelesAndReturnPdfBytes()
    {
        // Arrange
        var vacunoId = 2L;
        var command = new ExportarArbolGenealogicoCommand(vacunoId, 4, "pdf");
        var raiz = Vacuno.Rehydrate(vacunoId, "V2", "Luna", new DateOnly(2021, 1, 1), "COMPRA", "HOLSTEIN", "BLANCO_NEGRO", "HEMBRA", null, null, 1, null, new DateOnly(2021, 1, 1), DateTime.UtcNow, DateTime.UtcNow, null, null, null, null, null);
        var arbol = new List<VacunoGenealogiaNode> { new VacunoGenealogiaNode(raiz, 1, null) };
        var expectedBytes = new byte[] { 0x03, 0x04 };

        _vacunoRepositoryMock.GetByIdAsync(vacunoId, Arg.Any<CancellationToken>()).Returns(raiz);
        _tenantConfigurationProviderMock.GetSettingAsync(Settings.Vacunos.VacunosArbolMinNiveles).Returns(1);
        _tenantConfigurationProviderMock.GetSettingAsync(Settings.Vacunos.VacunosArbolMaxNiveles).Returns(4);

        _vacunoRepositoryMock.GetArbolGenealogicoAsync(vacunoId, 4, Arg.Any<CancellationToken>()).Returns(arbol);
        _exportServiceMock.GeneratePdfAsync(arbol, raiz, Arg.Any<CancellationToken>()).Returns(expectedBytes);

        // Act
        var result = await _interactor.HandleAsync(command);

        // Assert
        result.Bytes.Should().BeEquivalentTo(expectedBytes);
        result.ContentType.Should().Be("application/pdf");
        result.FileName.Should().Contain(".pdf");

        await _exportServiceMock.Received(1).GeneratePdfAsync(arbol, raiz, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenVacunoDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        var vacunoId = 99L;
        var command = new ExportarArbolGenealogicoCommand(vacunoId, 4);

        _vacunoRepositoryMock.GetByIdAsync(vacunoId, Arg.Any<CancellationToken>()).Returns((Vacuno?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _interactor.HandleAsync(command));
        exception.Message.Should().Contain(vacunoId.ToString());

        await _vacunoRepositoryMock.DidNotReceive().GetArbolGenealogicoAsync(Arg.Any<long>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
        await _exportServiceMock.DidNotReceive().GenerateExcelAsync(Arg.Any<List<VacunoGenealogiaNode>>(), Arg.Any<Vacuno>(), Arg.Any<CancellationToken>());
    }
}
