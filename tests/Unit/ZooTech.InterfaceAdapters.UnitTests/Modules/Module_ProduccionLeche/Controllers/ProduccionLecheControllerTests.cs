#if false
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Common;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.CreateOrdenio;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.DeleteOrdenio;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosExcel;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosPdf;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GetOrdenioById;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.ListOrdenios;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.UpdateOrdenio;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunos;
using ZooTech.Domain.Module_Vacuno.Entities.ListarVacuno;
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
        var listInputPort = new FakeListOrdeniosInputPort();
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
        var createInputPort = new FakeCreateOrdenioInputPort();
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

    private static ProduccionLecheController CreateController(
        IListarVacunosInputPort? listarVacunosInputPort = null,
        ICreateOrdenioInputPort? createInputPort = null,
        IGetOrdenioByIdInputPort? getByIdInputPort = null,
        IGetOrdeniosPdfInputPort? getOrdeniosPdfInputPort = null,
        IGetOrdeniosExcelInputPort? getOrdeniosExcelInputPort = null,
        IListOrdeniosInputPort? listInputPort = null,
        IUpdateOrdenioInputPort? updateInputPort = null,
        IDeleteOrdenioInputPort? deleteInputPort = null)
        => new(
            listarVacunosInputPort ?? new FakeListarVacunosInputPort(),
            createInputPort ?? new FakeCreateOrdenioInputPort(),
            getByIdInputPort ?? new FakeGetOrdenioByIdInputPort(),
            getOrdeniosPdfInputPort ?? new FakeGetOrdeniosPdfInputPort(),
            getOrdeniosExcelInputPort ?? new FakeGetOrdeniosExcelInputPort(),
            listInputPort ?? new FakeListOrdeniosInputPort(),
            updateInputPort ?? new FakeUpdateOrdenioInputPort(),
            deleteInputPort ?? new FakeDeleteOrdenioInputPort());

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

    private sealed class FakeListarVacunosInputPort : IListarVacunosInputPort
    {
        public Task<ListarVacunosOutput> HandleAsync(ListarVacunosCommand command, CancellationToken cancellationToken = default)
            => Task.FromResult(new ListarVacunosOutput(Array.Empty<VacunoListItem>(), TotalCount: 0));
    }

    private sealed class FakeCreateOrdenioInputPort : ICreateOrdenioInputPort
    {
        public CreateOrdenioCommand? CapturedCommand { get; private set; }

        public Task<CreateOrdenioOutput> HandleAsync(CreateOrdenioCommand command, CancellationToken cancellationToken)
        {
            CapturedCommand = command;
            return Task.FromResult(new CreateOrdenioOutput(CreateOrdenioOutput()));
        }
    }

    private sealed class FakeGetOrdenioByIdInputPort : IGetOrdenioByIdInputPort
    {
        public Task<GetOrdenioByIdOutput> HandleAsync(long id, CancellationToken cancellationToken)
            => Task.FromResult(new GetOrdenioByIdOutput(CreateOrdenioOutput()));
    }

    private sealed class FakeGetOrdeniosPdfInputPort : IGetOrdeniosPdfInputPort
    {
        public Task<GenerateOrdeniosPdfOutput> HandleAsync(GenerateOrdeniosComparationPdfQuery query, CancellationToken cancellationToken)
            => Task.FromResult(new GenerateOrdeniosPdfOutput(new byte[] { 1 }, "application/pdf", "ordenios.pdf"));
    }

    private sealed class FakeGetOrdeniosExcelInputPort : IGetOrdeniosExcelInputPort
    {
        public Task<GenerateOrdeniosExcelOutput> HandleAsync(GenerateOrdeniosComparationExcelQuery query, CancellationToken cancellationToken)
            => Task.FromResult(new GenerateOrdeniosExcelOutput(new byte[] { 1 }, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ordenios.xlsx"));
    }

    private sealed class FakeListOrdeniosInputPort : IListOrdeniosInputPort
    {
        public ListOrdeniosQuery? CapturedQuery { get; private set; }

        public Task<ListOrdeniosOutput> HandleAsync(ListOrdeniosQuery query, CancellationToken cancellationToken)
        {
            CapturedQuery = query;
            return Task.FromResult(new ListOrdeniosOutput(new[] { CreateOrdenioListOutput() }, TotalCount: 1));
        }
    }

    private sealed class FakeUpdateOrdenioInputPort : IUpdateOrdenioInputPort
    {
        public Task<UpdateOrdenioOutput> HandleAsync(long id, UpdateOrdenioCommand command, CancellationToken cancellationToken)
            => Task.FromResult(new UpdateOrdenioOutput(CreateOrdenioOutput()));
    }

    private sealed class FakeDeleteOrdenioInputPort : IDeleteOrdenioInputPort
    {
        public Task HandleAsync(long id, DeleteOrdenioCommand command, CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}
#endif

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
using ZooTech.Domain.Module_Vacuno.Entities.ListarVacuno;
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
        var listFactory = new ListOrdeniosFactory(query =>
            Task.FromResult(new ListOrdeniosOutput(new[] { CreateOrdenioListOutput() }, TotalCount: 1)));
        var controller = CreateController(listFactory: listFactory);

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
        Assert.Equal(10, listFactory.CapturedQuery?.VacunoId);
        Assert.Equal("ACTIVO", listFactory.CapturedQuery?.EstadoOrdenioCode);
        Assert.Equal(1, listFactory.CapturedQuery?.Page);
        Assert.Equal(20, listFactory.CapturedQuery?.PageSize);
        Assert.Single(response.Data);
        Assert.Equal(1, response.Pagination.Page);
        Assert.Equal(20, response.Pagination.Limit);
    }

    [Fact]
    public async Task Create_MapsRequestToCommandAndReturnsCreated()
    {
        var createFactory = new CreateOrdenioFactory(command =>
            Task.FromResult(new CreateOrdenioOutput(CreateOrdenioOutput())));
        var controller = CreateController(createFactory: createFactory);
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
        Assert.Equal(request.Codigo, createFactory.CapturedCommand?.Codigo);
        Assert.Equal(request.FechaHora, createFactory.CapturedCommand?.FechaHora);
        Assert.Equal(request.VacunoId, createFactory.CapturedCommand?.VacunoId);
        Assert.Equal(request.EncargadoUsuarioId, createFactory.CapturedCommand?.EncargadoUsuarioId);
        Assert.Equal(request.Litros, createFactory.CapturedCommand?.Litros);
        Assert.Equal(request.EstadoOrdenioCode, createFactory.CapturedCommand?.EstadoOrdenioCode);
        Assert.Equal(request.Observaciones, createFactory.CapturedCommand?.Observaciones);
        Assert.Equal(1, response.Id);
    }

    private static ProduccionLecheController CreateController(
        IListarVacunosBehaviorPipelineFactory? listarVacunosFactory = null,
        ICreateOrdenioBehaviorPipelineFactory? createFactory = null,
        IGetOrdenioByIdBehaviorPipelineFactory? getByIdFactory = null,
        IGenerateOrdeniosPdfBehaviorPipelineFactory? pdfFactory = null,
        IGenerateOrdeniosExcelBehaviorPipelineFactory? excelFactory = null,
        IListOrdeniosBehaviorPipelineFactory? listFactory = null,
        IUpdateOrdenioBehaviorPipelineFactory? updateFactory = null,
        IDeleteOrdenioBehaviorPipelineFactory? deleteFactory = null)
        => new(
            listarVacunosFactory ?? new ListarVacunosFactory(_ => Task.FromResult(new ListarVacunosOutput(Array.Empty<VacunoListItem>(), TotalCount: 0))),
            createFactory ?? new CreateOrdenioFactory(_ => Task.FromResult(new CreateOrdenioOutput(CreateOrdenioOutput()))),
            getByIdFactory ?? new GetOrdenioByIdFactory(_ => Task.FromResult(new GetOrdenioByIdOutput(CreateOrdenioOutput()))),
            pdfFactory ?? new GenerateOrdeniosPdfFactory(_ => Task.FromResult(new GenerateOrdeniosPdfOutput(new byte[] { 1 }, "application/pdf", "ordenios.pdf"))),
            excelFactory ?? new GenerateOrdeniosExcelFactory(_ => Task.FromResult(new GenerateOrdeniosExcelOutput(new byte[] { 1 }, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ordenios.xlsx"))),
            listFactory ?? new ListOrdeniosFactory(_ => Task.FromResult(new ListOrdeniosOutput(new[] { CreateOrdenioListOutput() }, TotalCount: 1))),
            updateFactory ?? new UpdateOrdenioFactory(_ => Task.FromResult(new UpdateOrdenioOutput(CreateOrdenioOutput()))),
            deleteFactory ?? new DeleteOrdenioFactory(_ => Task.FromResult(EmptyOutput.Value)));

    private static T AssertGeneralResponseData<T>(object? value)
    {
        Assert.NotNull(value);
        var success = value.GetType().GetProperty("Success")?.GetValue(value);
        var data = value.GetType().GetProperty("Data")?.GetValue(value);

        Assert.Equal(true, success);
        return Assert.IsType<T>(data);
    }

    private static BehaviorPipeline<TRequest, TResponse> Pipeline<TRequest, TResponse>(Func<TRequest, Task<TResponse>> handler)
        => new(Array.Empty<IBehavior<TRequest, TResponse>>(), (request, _) => handler(request));

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

    private sealed class ListarVacunosFactory(Func<ListarVacunosCommand, Task<ListarVacunosOutput>> handler) : IListarVacunosBehaviorPipelineFactory
    {
        public BehaviorPipeline<ListarVacunosCommand, ListarVacunosOutput> Create() => Pipeline(handler);
    }

    private sealed class CreateOrdenioFactory(Func<CreateOrdenioCommand, Task<CreateOrdenioOutput>> handler) : ICreateOrdenioBehaviorPipelineFactory
    {
        public CreateOrdenioCommand? CapturedCommand { get; private set; }

        public BehaviorPipeline<CreateOrdenioCommand, CreateOrdenioOutput> Create()
            => Pipeline<CreateOrdenioCommand, CreateOrdenioOutput>(command =>
            {
                CapturedCommand = command;
                return handler(command);
            });
    }

    private sealed class GetOrdenioByIdFactory(Func<GetOrdenioByIdCommand, Task<GetOrdenioByIdOutput>> handler) : IGetOrdenioByIdBehaviorPipelineFactory
    {
        public BehaviorPipeline<GetOrdenioByIdCommand, GetOrdenioByIdOutput> Create() => Pipeline(handler);
    }

    private sealed class GenerateOrdeniosPdfFactory(Func<GenerateOrdeniosComparationPdfQuery, Task<GenerateOrdeniosPdfOutput>> handler) : IGenerateOrdeniosPdfBehaviorPipelineFactory
    {
        public BehaviorPipeline<GenerateOrdeniosComparationPdfQuery, GenerateOrdeniosPdfOutput> Create() => Pipeline(handler);
    }

    private sealed class GenerateOrdeniosExcelFactory(Func<GenerateOrdeniosComparationExcelQuery, Task<GenerateOrdeniosExcelOutput>> handler) : IGenerateOrdeniosExcelBehaviorPipelineFactory
    {
        public BehaviorPipeline<GenerateOrdeniosComparationExcelQuery, GenerateOrdeniosExcelOutput> Create() => Pipeline(handler);
    }

    private sealed class ListOrdeniosFactory(Func<ListOrdeniosQuery, Task<ListOrdeniosOutput>> handler) : IListOrdeniosBehaviorPipelineFactory
    {
        public ListOrdeniosQuery? CapturedQuery { get; private set; }

        public BehaviorPipeline<ListOrdeniosQuery, ListOrdeniosOutput> Create()
            => Pipeline<ListOrdeniosQuery, ListOrdeniosOutput>(query =>
            {
                CapturedQuery = query;
                return handler(query);
            });
    }

    private sealed class UpdateOrdenioFactory(Func<UpdateOrdenioCommand, Task<UpdateOrdenioOutput>> handler) : IUpdateOrdenioBehaviorPipelineFactory
    {
        public BehaviorPipeline<UpdateOrdenioCommand, UpdateOrdenioOutput> Create() => Pipeline(handler);
    }

    private sealed class DeleteOrdenioFactory(Func<DeleteOrdenioCommand, Task<EmptyOutput>> handler) : IDeleteOrdenioBehaviorPipelineFactory
    {
        public BehaviorPipeline<DeleteOrdenioCommand, EmptyOutput> Create() => Pipeline(handler);
    }
}
