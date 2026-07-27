using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Common.Behaviors;
using ZooTech.Application.Common.Behaviors.Module_ProduccionLeche.Ordenios.CreateOrdenio;
using ZooTech.Application.Common.Behaviors.Module_ProduccionLeche.Ordenios.DeleteOrdenio;
using ZooTech.Application.Common.Behaviors.Module_ProduccionLeche.Ordenios.GenerateOrdeniosExcel;
using ZooTech.Application.Common.Behaviors.Module_ProduccionLeche.Ordenios.GenerateOrdeniosPdf;
using ZooTech.Application.Common.Behaviors.Module_ProduccionLeche.Ordenios.GetOrdenioById;
using ZooTech.Application.Common.Behaviors.Module_ProduccionLeche.Ordenios.ListOrdenios;
using ZooTech.Application.Common.Behaviors.Module_ProduccionLeche.Ordenios.UpdateOrdenio;
using ZooTech.Application.Common.Behaviors.Module_Vacuno.ListarVacunos;
using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Common;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.CreateOrdenio;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.DeleteOrdenio;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosExcel;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosPdf;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GetOrdenioById;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.ListOrdenios;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.UpdateOrdenio;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunos;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Entities.ListarVacuno;
using ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.Controllers;
using ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.DTOs.Responses;

namespace ZooTech.InterfaceAdapters.UnitTests.Modules.Module_ProduccionLeche.Controllers;

public class ProduccionLecheControllerTests
{
    [Fact]
    public void Health_ReturnsOkResponse()
    {
        var controller = CreateController();

        var result = controller.Health();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task List_WhenPagingIsNull_UsesDefaultPagingAndReturnsOk()
    {
        var listInputPort = new FakeListOrdeniosBehaviorPipelineFactory();
        var controller = CreateController(listInputPort: listInputPort);

        var result = await controller.List(
            vacunoId: 10,
            estadoOrdenioCode: "ACTIVO",
            fechaDesde: null,
            fechaHasta: null,
            page: null,
            pageSize: null,
            CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = AssertGeneralResponseData<ListOrdeniosResponse>(okResult.Value);
        Assert.Equal(10, listInputPort.CapturedQuery?.VacunoId);
        Assert.Equal("ACTIVO", listInputPort.CapturedQuery?.EstadoOrdenioCode);
        Assert.Equal(1, listInputPort.CapturedQuery?.Page);
        Assert.Equal(20, listInputPort.CapturedQuery?.PageSize);
        Assert.Single(response.Data);
        Assert.Equal(1, response.Pagination.Page);
        Assert.Equal(20, response.Pagination.Limit);
    }

    [Fact]
    public async Task Create_MapsRequestToCommandAndReturnsCreated()
    {
        var createInputPort = new FakeCreateOrdenioBehaviorPipelineFactory();
        var controller = CreateController(createInputPort: createInputPort);
        var request = new CreateOrdenioRequest(
            "ORD-001",
            new DateTime(2026, 7, 7, 10, 0, 0),
            10,
            20,
            12.5m,
            "ACTIVO",
            "Sin observaciones");

        var result = await controller.Create(request, CancellationToken.None);

        var createdResult = Assert.IsType<CreatedResult>(result);
        var response = AssertGeneralResponseData<OrdenioResponse>(createdResult.Value);
        Assert.Equal("/api/v1/produccion-leche/1", createdResult.Location);
        Assert.Equal(request.Codigo, createInputPort.CapturedCommand?.Codigo);
        Assert.Equal(request.FechaHora, createInputPort.CapturedCommand?.FechaHora);
        Assert.Equal(request.VacunoId, createInputPort.CapturedCommand?.VacunoId);
        Assert.Equal(request.EncargadoUsuarioId, createInputPort.CapturedCommand?.EncargadoUsuarioId);
        Assert.Equal(request.Litros, createInputPort.CapturedCommand?.Litros);
        Assert.Equal(request.EstadoOrdenioCode, createInputPort.CapturedCommand?.EstadoOrdenioCode);
        Assert.Equal(request.Observaciones, createInputPort.CapturedCommand?.Observaciones);
        Assert.Equal(1, response.Id);
    }

    [Fact]
    public async Task GenerateExcel_MapsComparativoQueryAndReturnsFile()
    {
        var excelInputPort = new FakeGetOrdeniosExcelBehaviorPipelineFactory();
        var controller = CreateController(getOrdeniosExcelInputPort: excelInputPort);
        var fechaDesde = new DateTime(2026, 7, 1, 8, 0, 0);
        var fechaHasta = new DateTime(2026, 7, 2, 8, 0, 0);

        var result = await controller.GenerateExcel(
            vacunoId: 10,
            estadoOrdenioCode: "ACTIVO",
            fechaDesde: fechaDesde,
            fechaHasta: fechaHasta,
            comparativo: true,
            CancellationToken.None);

        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileResult.ContentType);
        Assert.Equal("ordenios.xlsx", fileResult.FileDownloadName);
        Assert.Equal(10, excelInputPort.CapturedQuery?.VacunoId);
        Assert.Equal("ACTIVO", excelInputPort.CapturedQuery?.EstadoOrdenioCode);
        Assert.Equal(fechaDesde, excelInputPort.CapturedQuery?.FechaDesde);
        Assert.Equal(fechaHasta, excelInputPort.CapturedQuery?.FechaHasta);
        Assert.True(excelInputPort.CapturedQuery?.Comparativo);
    }

    private static ProduccionLecheController CreateController(
        IListarVacunosBehaviorPipelineFactory? listarVacunosInputPort = null,
        ICreateOrdenioBehaviorPipelineFactory? createInputPort = null,
        IGetOrdenioByIdBehaviorPipelineFactory? getByIdInputPort = null,
        IGenerateOrdeniosPdfBehaviorPipelineFactory? getOrdeniosPdfInputPort = null,
        IGenerateOrdeniosExcelBehaviorPipelineFactory? getOrdeniosExcelInputPort = null,
        IListOrdeniosBehaviorPipelineFactory? listInputPort = null,
        IUpdateOrdenioBehaviorPipelineFactory? updateInputPort = null,
        IDeleteOrdenioBehaviorPipelineFactory? deleteInputPort = null)
        => new(
            listarVacunosInputPort ?? new FakeListarVacunosBehaviorPipelineFactory(),
            createInputPort ?? new FakeCreateOrdenioBehaviorPipelineFactory(),
            getByIdInputPort ?? new FakeGetOrdenioByIdBehaviorPipelineFactory(),
            getOrdeniosPdfInputPort ?? new FakeGetOrdeniosPdfBehaviorPipelineFactory(),
            getOrdeniosExcelInputPort ?? new FakeGetOrdeniosExcelBehaviorPipelineFactory(),
            listInputPort ?? new FakeListOrdeniosBehaviorPipelineFactory(),
            updateInputPort ?? new FakeUpdateOrdenioBehaviorPipelineFactory(),
            deleteInputPort ?? new FakeDeleteOrdenioBehaviorPipelineFactory());

    private static T AssertGeneralResponseData<T>(object? value)
    {
        Assert.NotNull(value);
        var success = value.GetType().GetProperty("Success")?.GetValue(value);
        var data = value.GetType().GetProperty("Data")?.GetValue(value);

        Assert.Equal(true, success);
        return Assert.IsType<T>(data);
    }

    private static OrdenioOutput CreateOrdenioOutput()
    {
        var now = new DateTime(2026, 7, 7, 10, 0, 0);

        return new OrdenioOutput(
            1,
            "ORD-001",
            now,
            10,
            "Luna",
            20,
            "Juan Perez",
            12.5m,
            "ACTIVO",
            "Sin observaciones",
            now,
            now);
    }

    private static OrdenioListOutput CreateOrdenioListOutput()
    {
        var now = new DateTime(2026, 7, 7, 10, 0, 0);

        return new OrdenioListOutput(
            1,
            "ORD-001",
            now,
            10,
            "Luna",
            "VAC-001",
            20,
            "Juan Perez",
            12.5m,
            "ACTIVO",
            "Sin observaciones",
            now,
            now);
    }

    private sealed class FakeListarVacunosBehaviorPipelineFactory : IListarVacunosBehaviorPipelineFactory
    {
        public BehaviorPipeline<ListarVacunosQuery, ListarVacunosOutput> Create()
            => new(
                Array.Empty<IBehavior<ListarVacunosQuery, ListarVacunosOutput>>(),
                (_, _) => Task.FromResult(new ListarVacunosOutput(Array.Empty<VacunoListItem>(), TotalCount: 0)));
    }

    private sealed class FakeCreateOrdenioBehaviorPipelineFactory : ICreateOrdenioBehaviorPipelineFactory
    {
        public CreateOrdenioCommand? CapturedCommand { get; private set; }

        public BehaviorPipeline<CreateOrdenioCommand, CreateOrdenioOutput> Create()
            => new(
                Array.Empty<IBehavior<CreateOrdenioCommand, CreateOrdenioOutput>>(),
                (command, _) =>
                {
                    CapturedCommand = command;
                    return Task.FromResult(new CreateOrdenioOutput(CreateOrdenioOutput()));
                });
    }

    private sealed class FakeGetOrdenioByIdBehaviorPipelineFactory : IGetOrdenioByIdBehaviorPipelineFactory
    {
        public BehaviorPipeline<GetOrdenioByIdQuery, GetOrdenioByIdOutput> Create()
            => new(
                Array.Empty<IBehavior<GetOrdenioByIdQuery, GetOrdenioByIdOutput>>(),
                (_, _) => Task.FromResult(new GetOrdenioByIdOutput(CreateOrdenioOutput())));
    }

    private sealed class FakeGetOrdeniosPdfBehaviorPipelineFactory : IGenerateOrdeniosPdfBehaviorPipelineFactory
    {
        public BehaviorPipeline<GenerateOrdeniosComparationPdfQuery, GenerateOrdeniosPdfOutput> Create()
            => new(
                Array.Empty<IBehavior<GenerateOrdeniosComparationPdfQuery, GenerateOrdeniosPdfOutput>>(),
                (_, _) => Task.FromResult(new GenerateOrdeniosPdfOutput(new byte[] { 1 }, "application/pdf", "ordenios.pdf")));
    }

    private sealed class FakeGetOrdeniosExcelBehaviorPipelineFactory : IGenerateOrdeniosExcelBehaviorPipelineFactory
    {
        public GenerateOrdeniosComparationExcelQuery? CapturedQuery { get; private set; }

        public BehaviorPipeline<GenerateOrdeniosComparationExcelQuery, GenerateOrdeniosExcelOutput> Create()
            => new(
                Array.Empty<IBehavior<GenerateOrdeniosComparationExcelQuery, GenerateOrdeniosExcelOutput>>(),
                (query, _) =>
                {
                    CapturedQuery = query;
                    return Task.FromResult(new GenerateOrdeniosExcelOutput(new byte[] { 1 }, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ordenios.xlsx"));
                });
    }

    private sealed class FakeListOrdeniosBehaviorPipelineFactory : IListOrdeniosBehaviorPipelineFactory
    {
        public ListOrdeniosQuery? CapturedQuery { get; private set; }

        public BehaviorPipeline<ListOrdeniosQuery, ListOrdeniosOutput> Create()
            => new(
                Array.Empty<IBehavior<ListOrdeniosQuery, ListOrdeniosOutput>>(),
                (query, _) =>
                {
                    CapturedQuery = query;
                    return Task.FromResult(new ListOrdeniosOutput(new[] { CreateOrdenioListOutput() }, TotalCount: 1));
                });
    }

    private sealed class FakeUpdateOrdenioBehaviorPipelineFactory : IUpdateOrdenioBehaviorPipelineFactory
    {
        public BehaviorPipeline<UpdateOrdenioCommand, UpdateOrdenioOutput> Create()
            => new(
                Array.Empty<IBehavior<UpdateOrdenioCommand, UpdateOrdenioOutput>>(),
                (_, _) => Task.FromResult(new UpdateOrdenioOutput(CreateOrdenioOutput())));
    }

    private sealed class FakeDeleteOrdenioBehaviorPipelineFactory : IDeleteOrdenioBehaviorPipelineFactory
    {
        public BehaviorPipeline<DeleteOrdenioCommand, EmptyOutput> Create()
            => new(
                Array.Empty<IBehavior<DeleteOrdenioCommand, EmptyOutput>>(),
                (_, _) => Task.FromResult(EmptyOutput.Value));
    }
}
