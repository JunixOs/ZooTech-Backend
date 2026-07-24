using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Xunit;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Gateway.Parametrization;
using ZooTech.Application.Common.Gateway.Reports;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.Common;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ExportarArbolGenealogico;
using ZooTech.Domain.Configuration;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Entities;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Entities.GetArbolGenealogico;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Interfaces;

namespace ZooTech.Application.UnitTests.Modules.Module_Vacuno.UseCases;

public class ExportarArbolGenealogicoInteractorTests
{
    private readonly IVacunoRepository _vacunoRepositoryMock;
    private readonly IReportStrategyResolver<GenealogiaVacunoReportModel> _resolverMock;
    private readonly IReportStrategy<GenealogiaVacunoReportModel> _strategyMock;
    private readonly IVacunoReportFormatPolicy _formatPolicyMock;
    private readonly ITenantConfigurationProvider _tenantConfigurationProviderMock;
    private readonly ExportarArbolGenealogicoInteractor _interactor;

    public ExportarArbolGenealogicoInteractorTests()
    {
        _vacunoRepositoryMock = Substitute.For<IVacunoRepository>();
        _resolverMock = Substitute.For<IReportStrategyResolver<GenealogiaVacunoReportModel>>();
        _strategyMock = Substitute.For<IReportStrategy<GenealogiaVacunoReportModel>>();
        _formatPolicyMock = Substitute.For<IVacunoReportFormatPolicy>();
        _tenantConfigurationProviderMock = Substitute.For<ITenantConfigurationProvider>();
        _interactor = new ExportarArbolGenealogicoInteractor(
            _vacunoRepositoryMock,
            _resolverMock,
            _formatPolicyMock,
            _tenantConfigurationProviderMock);
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
        _formatPolicyMock.EnsureAllowedAsync("excel").Returns(ReportFileFormat.Excel);
        _resolverMock.Resolve(ReportFileFormat.Excel).Returns(_strategyMock);
        _strategyMock.GenerateAsync(
                Arg.Any<GenealogiaVacunoReportModel>(),
                Arg.Any<CancellationToken>())
            .Returns(new GeneratedReportDocument(
                expectedBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "xlsx",
                "Genealogia_1.xlsx"));

        // Act
        var result = await _interactor.HandleAsync(command);

        // Assert
        result.Bytes.Should().BeEquivalentTo(expectedBytes);
        result.ContentType.Should().Be("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        result.FileName.Should().Contain(".xlsx");

        await _vacunoRepositoryMock.Received(1).GetArbolGenealogicoAsync(vacunoId, 4, Arg.Any<CancellationToken>());
        await _strategyMock.Received(1).GenerateAsync(
            Arg.Is<GenealogiaVacunoReportModel>(model =>
                ReferenceEquals(model.Root, raiz) && model.Nodes.Count == 1),
            Arg.Any<CancellationToken>());
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
        _formatPolicyMock.EnsureAllowedAsync("pdf").Returns(ReportFileFormat.Pdf);
        _resolverMock.Resolve(ReportFileFormat.Pdf).Returns(_strategyMock);
        _strategyMock.GenerateAsync(
                Arg.Any<GenealogiaVacunoReportModel>(),
                Arg.Any<CancellationToken>())
            .Returns(new GeneratedReportDocument(
                expectedBytes,
                "application/pdf",
                "pdf",
                "Genealogia_2.pdf"));

        // Act
        var result = await _interactor.HandleAsync(command);

        // Assert
        result.Bytes.Should().BeEquivalentTo(expectedBytes);
        result.ContentType.Should().Be("application/pdf");
        result.FileName.Should().Contain(".pdf");

        _resolverMock.Received(1).Resolve(ReportFileFormat.Pdf);
    }

    [Fact]
    public async Task HandleAsync_WhenNivelesAreBelowTenantMinimum_ShouldUseMinimum()
    {
        var vacunoId = 3L;
        var command = new ExportarArbolGenealogicoCommand(vacunoId, 1, "excel");
        var raiz = Vacuno.Rehydrate(
            vacunoId,
            "V3",
            "Sol",
            new DateOnly(2022, 1, 1),
            "COMPRA",
            "HOLSTEIN",
            "BLANCO_NEGRO",
            "HEMBRA",
            null,
            null,
            1,
            null,
            new DateOnly(2022, 1, 1),
            DateTime.UtcNow,
            DateTime.UtcNow,
            null,
            null,
            null,
            null,
            null);
        var arbol = new List<VacunoGenealogiaNode>
        {
            new(raiz, 1, null)
        };

        _vacunoRepositoryMock.GetByIdAsync(vacunoId, Arg.Any<CancellationToken>())
            .Returns(raiz);
        _tenantConfigurationProviderMock
            .GetSettingAsync(Settings.Vacunos.VacunosArbolMinNiveles)
            .Returns(2);
        _tenantConfigurationProviderMock
            .GetSettingAsync(Settings.Vacunos.VacunosArbolMaxNiveles)
            .Returns(4);
        _vacunoRepositoryMock
            .GetArbolGenealogicoAsync(vacunoId, 2, Arg.Any<CancellationToken>())
            .Returns(arbol);
        _formatPolicyMock.EnsureAllowedAsync("excel").Returns(ReportFileFormat.Excel);
        _resolverMock.Resolve(ReportFileFormat.Excel).Returns(_strategyMock);
        _strategyMock.GenerateAsync(
                Arg.Any<GenealogiaVacunoReportModel>(),
                Arg.Any<CancellationToken>())
            .Returns(new GeneratedReportDocument(
                [1],
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".xlsx",
                "Genealogia_3.xlsx"));

        await _interactor.HandleAsync(command);

        await _vacunoRepositoryMock.Received(1)
            .GetArbolGenealogicoAsync(vacunoId, 2, Arg.Any<CancellationToken>());
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
        await _strategyMock.DidNotReceiveWithAnyArgs().GenerateAsync(default!, default);
    }
}
