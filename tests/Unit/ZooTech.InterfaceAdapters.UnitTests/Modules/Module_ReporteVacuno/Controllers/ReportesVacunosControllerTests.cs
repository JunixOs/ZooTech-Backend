using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ListarReportesDisponibles;
using ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ListarReporteVacunos;
using ZooTech.InterfaceAdapters.Modules.Module_ReporteVacuno.Controllers;
using ZooTech.InterfaceAdapters.Modules.Module_ReporteVacuno.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_ReporteVacuno.DTOs.Responses;

namespace ZooTech.InterfaceAdapters.UnitTests.Modules.Module_ReporteVacuno.Controllers;

public class ReportesVacunosControllerTests
{
    private readonly Mock<IListarReportesDisponiblesUseCase> _listarDisponiblesMock;
    private readonly Mock<IListarReporteVacunosUseCase> _listarVacunosMock;
    private readonly ReportesVacunosController _controller;

    public ReportesVacunosControllerTests()
    {
        _listarDisponiblesMock = new Mock<IListarReportesDisponiblesUseCase>();
        _listarVacunosMock = new Mock<IListarReporteVacunosUseCase>();
        _controller = new ReportesVacunosController(_listarDisponiblesMock.Object, _listarVacunosMock.Object);
    }

    [Fact]
    public async Task Disponibles_DebeRetornarOkResult_ConDtoMapeado()
    {
        // Arrange
        var queryDto = new ReportesDisponiblesQueryDto { FechaDesde = "2026-05-01", FechaHasta = "2026-05-31", Q = "test" };
        var domainResponse = new ReportesDisponiblesResponse(
            [new ReporteDisponibleItem("tipo1", "Nombre 1", "Desc 1")],
            new ReportesDisponiblesFiltros(new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 31), "test")
        );

        _listarDisponiblesMock.Setup(x => x.HandleAsync(It.IsAny<ListarReportesDisponiblesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(domainResponse);

        // Act
        var result = await _controller.Disponibles(queryDto, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var responseDto = okResult.Value.Should().BeOfType<ReportesDisponiblesResponseDto>().Subject;
        
        responseDto.Data.Should().HaveCount(1);
        responseDto.Filtros.FechaDesde.Should().Be(new DateOnly(2026, 5, 1));
    }

    [Fact]
    public async Task Listado_DebeRetornarOkResult_ConPaginacionYDtos()
    {
        // Arrange
        var queryDto = new ListadoVacunosReporteQueryDto
        {
            FechaDesde = "2026-01-01",
            Estado = "vivo",
            Formato = "json",
            Page = "1",
            Limit = "10"
        };

        var vacunosItems = new List<VacunoListadoItem>
        {
            new VacunoListadoItem(1, "V-001", new DateOnly(2026, 1, 10), "Vacuno 1", "Holstein", "propio", "vivo")
        };
        var domainResponse = new ListadoVacunosReporteResponse(
            vacunosItems, 
            new ReporteVacunoResumen(100), 
            new ReporteVacunoFiltros(new DateOnly(2026, 1, 1), new DateOnly(2026, 1, 31), null, null, null, "vivo", null, "json"), 
            null);

        _listarVacunosMock.Setup(x => x.HandleAsync(It.IsAny<ListarReporteVacunosQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(domainResponse);

        // Act
        var result = await _controller.Listado(queryDto, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var responseDto = okResult.Value.Should().BeOfType<ListadoVacunosReporteResponseDto>().Subject;
        
        responseDto.Data.Should().HaveCount(1);
        responseDto.Resumen.TotalVacunos.Should().Be(100);
        responseDto.Data.GetEnumerator().MoveNext();
        var primerElemento = responseDto.Data.GetEnumerator().Current;
    }
}
