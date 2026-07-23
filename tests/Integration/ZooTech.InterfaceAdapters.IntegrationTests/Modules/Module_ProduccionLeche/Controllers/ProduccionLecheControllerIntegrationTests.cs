using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
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
using ZooTech.InterfaceAdapters.DTOs;
using ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.Controllers;
using ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.DTOs.Responses;

namespace ZooTech.InterfaceAdapters.IntegrationTests.Modules.Module_ProduccionLeche.Controllers;

public class ProduccionLecheControllerIntegrationTests
{
    [Fact]
    public async Task Health_ReturnsOkFromConfiguredRoute()
    {
        var client = CreateClient(new OrdenioPipelineSpy());

        var response = await client.GetAsync("/api/v1/produccion-leche/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<HealthResponse>();
        Assert.Equal("modulo produccion leche", payload?.Model);
    }

    [Fact]
    public async Task Create_BindsJsonRequestAndReturnsCreatedResponse()
    {
        var spy = new OrdenioPipelineSpy();
        var client = CreateClient(spy);
        var request = new CreateOrdenioRequest(
            "ORD-001",
            new DateTime(2026, 7, 7, 10, 0, 0),
            10,
            20,
            12.5m,
            "ACTIVO",
            "Sin observaciones");

        var response = await client.PostAsJsonAsync("/api/v1/produccion-leche", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("/api/v1/produccion-leche/1", response.Headers.Location?.ToString());
        Assert.Equal(request.Codigo, spy.CreateCommand?.Codigo);
        Assert.Equal(request.FechaHora, spy.CreateCommand?.FechaHora);
        Assert.Equal(request.VacunoId, spy.CreateCommand?.VacunoId);
        Assert.Equal(request.EncargadoUsuarioId, spy.CreateCommand?.EncargadoUsuarioId);
        Assert.Equal(request.Litros, spy.CreateCommand?.Litros);
        Assert.Equal(request.EstadoOrdenioCode, spy.CreateCommand?.EstadoOrdenioCode);
        Assert.Equal(request.Observaciones, spy.CreateCommand?.Observaciones);

        var payload = await response.Content.ReadFromJsonAsync<GeneralResponseDTO<OrdenioResponse>>();
        Assert.True(payload?.Success);
        Assert.Equal(1, payload?.Data?.Id);
        Assert.Equal("ORD-001", payload?.Data?.Codigo);
    }

    [Fact]
    public async Task List_BindsQueryStringAndReturnsPagedResponse()
    {
        var spy = new OrdenioPipelineSpy();
        var client = CreateClient(spy);

        var response = await client.GetAsync("/api/v1/produccion-leche?vacunoId=10&estadoOrdenioCode=ACTIVO&page=2&pageSize=5");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(10, spy.ListQuery?.VacunoId);
        Assert.Equal("ACTIVO", spy.ListQuery?.EstadoOrdenioCode);
        Assert.Equal(2, spy.ListQuery?.Page);
        Assert.Equal(5, spy.ListQuery?.PageSize);

        var payload = await response.Content.ReadFromJsonAsync<GeneralResponseDTO<ListOrdeniosResponse>>();
        Assert.True(payload?.Success);
        Assert.Single(payload!.Data!.Data);
        Assert.Equal(2, payload.Data.Pagination.Page);
        Assert.Equal(5, payload.Data.Pagination.Limit);
        Assert.Equal(12, payload.Data.Pagination.Total);
        Assert.Equal(3, payload.Data.Pagination.TotalPages);
    }

    [Fact]
    public async Task GetById_BindsRouteIdAndReturnsOkResponse()
    {
        var spy = new OrdenioPipelineSpy();
        var client = CreateClient(spy);

        var response = await client.GetAsync("/api/v1/produccion-leche/25");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(25, spy.GetByIdCommand?.Id);

        var payload = await response.Content.ReadFromJsonAsync<GeneralResponseDTO<OrdenioResponse>>();
        Assert.True(payload?.Success);
        Assert.Equal(25, payload?.Data?.Id);
    }

    [Fact]
    public async Task Update_BindsRouteAndJsonBodyAndReturnsOkResponse()
    {
        var spy = new OrdenioPipelineSpy();
        var client = CreateClient(spy);
        var request = new UpdateOrdenioRequest(
            new DateTime(2026, 7, 7, 11, 0, 0),
            30,
            14.25m,
            "FINALIZADO",
            "Actualizado");

        var response = await client.PatchAsJsonAsync("/api/v1/produccion-leche/30", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(30, spy.UpdateCommand?.Id);
        Assert.Equal(request.FechaHora, spy.UpdateCommand?.FechaHora);
        Assert.Equal(request.EncargadoUsuarioId, spy.UpdateCommand?.EncargadoUsuarioId);
        Assert.Equal(request.Litros, spy.UpdateCommand?.Litros);
        Assert.Equal(request.EstadoOrdenioCode, spy.UpdateCommand?.EstadoOrdenioCode);
        Assert.Equal(request.Observaciones, spy.UpdateCommand?.Observaciones);

        var payload = await response.Content.ReadFromJsonAsync<GeneralResponseDTO<OrdenioResponse>>();
        Assert.True(payload?.Success);
        Assert.Equal(30, payload?.Data?.Id);
    }

    [Fact]
    public async Task Delete_BindsRouteAndJsonBodyAndReturnsNoContent()
    {
        var spy = new OrdenioPipelineSpy();
        var client = CreateClient(spy);
        using var request = new HttpRequestMessage(HttpMethod.Delete, "/api/v1/produccion-leche/40")
        {
            Content = JsonContent.Create(new DeleteOrdenioRequest("Registro duplicado"))
        };

        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(40, spy.DeleteCommand?.Id);
        Assert.Equal("Registro duplicado", spy.DeleteCommand?.MotivoEliminacion);
    }

    [Fact]
    public async Task GenerateExcel_BindsQueryStringAndReturnsFileResponse()
    {
        var spy = new OrdenioPipelineSpy();
        var client = CreateClient(spy);

        var response = await client.GetAsync("/api/v1/produccion-leche/reporte/excel?vacunoId=10&estadoOrdenioCode=ACTIVO&comparativo=true");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(10, spy.ExcelQuery?.VacunoId);
        Assert.Equal("ACTIVO", spy.ExcelQuery?.EstadoOrdenioCode);
        Assert.True(spy.ExcelQuery?.Comparativo);
        Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", response.Content.Headers.ContentType?.MediaType);
        Assert.Equal("ordenios.xlsx", response.Content.Headers.ContentDisposition?.FileNameStar ?? response.Content.Headers.ContentDisposition?.FileName?.Trim('"'));
    }

    private static HttpClient CreateClient(OrdenioPipelineSpy spy)
    {
        var server = new TestServer(new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddSingleton(spy);
                services.AddSingleton<IListarVacunosBehaviorPipelineFactory, FakeListarVacunosBehaviorPipelineFactory>();
                services.AddSingleton<ICreateOrdenioBehaviorPipelineFactory, FakeCreateOrdenioBehaviorPipelineFactory>();
                services.AddSingleton<IGetOrdenioByIdBehaviorPipelineFactory, FakeGetOrdenioByIdBehaviorPipelineFactory>();
                services.AddSingleton<IGenerateOrdeniosPdfBehaviorPipelineFactory, FakeGenerateOrdeniosPdfBehaviorPipelineFactory>();
                services.AddSingleton<IGenerateOrdeniosExcelBehaviorPipelineFactory, FakeGenerateOrdeniosExcelBehaviorPipelineFactory>();
                services.AddSingleton<IListOrdeniosBehaviorPipelineFactory, FakeListOrdeniosBehaviorPipelineFactory>();
                services.AddSingleton<IUpdateOrdenioBehaviorPipelineFactory, FakeUpdateOrdenioBehaviorPipelineFactory>();
                services.AddSingleton<IDeleteOrdenioBehaviorPipelineFactory, FakeDeleteOrdenioBehaviorPipelineFactory>();

                services
                    .AddControllers()
                    .AddApplicationPart(typeof(ProduccionLecheController).Assembly);
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(endpoints => endpoints.MapControllers());
            }));

        return server.CreateClient();
    }

    private sealed record HealthResponse(string Status, string Model);

    private sealed class OrdenioPipelineSpy
    {
        public CreateOrdenioCommand? CreateCommand { get; set; }
        public GetOrdenioByIdCommand? GetByIdCommand { get; set; }
        public ListOrdeniosQuery? ListQuery { get; set; }
        public UpdateOrdenioCommand? UpdateCommand { get; set; }
        public DeleteOrdenioCommand? DeleteCommand { get; set; }
        public GenerateOrdeniosComparationExcelQuery? ExcelQuery { get; set; }
    }

    private sealed class FakeListarVacunosBehaviorPipelineFactory : IListarVacunosBehaviorPipelineFactory
    {
        public BehaviorPipeline<ListarVacunosCommand, ListarVacunosOutput> Create()
            => new(
                Array.Empty<IBehavior<ListarVacunosCommand, ListarVacunosOutput>>(),
                (_, _) => Task.FromResult(new ListarVacunosOutput(Array.Empty<VacunoListItem>(), 0)));
    }

    private sealed class FakeCreateOrdenioBehaviorPipelineFactory(OrdenioPipelineSpy spy) : ICreateOrdenioBehaviorPipelineFactory
    {
        public BehaviorPipeline<CreateOrdenioCommand, CreateOrdenioOutput> Create()
            => new(
                Array.Empty<IBehavior<CreateOrdenioCommand, CreateOrdenioOutput>>(),
                (command, _) =>
                {
                    spy.CreateCommand = command;
                    return Task.FromResult(new CreateOrdenioOutput(CreateOrdenioOutput(1, command.Codigo ?? string.Empty)));
                });
    }

    private sealed class FakeGetOrdenioByIdBehaviorPipelineFactory(OrdenioPipelineSpy spy) : IGetOrdenioByIdBehaviorPipelineFactory
    {
        public BehaviorPipeline<GetOrdenioByIdCommand, GetOrdenioByIdOutput> Create()
            => new(
                Array.Empty<IBehavior<GetOrdenioByIdCommand, GetOrdenioByIdOutput>>(),
                (command, _) =>
                {
                    spy.GetByIdCommand = command;
                    return Task.FromResult(new GetOrdenioByIdOutput(CreateOrdenioOutput(command.Id, "ORD-025")));
                });
    }

    private sealed class FakeGenerateOrdeniosPdfBehaviorPipelineFactory : IGenerateOrdeniosPdfBehaviorPipelineFactory
    {
        public BehaviorPipeline<GenerateOrdeniosComparationPdfQuery, GenerateOrdeniosPdfOutput> Create()
            => new(
                Array.Empty<IBehavior<GenerateOrdeniosComparationPdfQuery, GenerateOrdeniosPdfOutput>>(),
                (_, _) => Task.FromResult(new GenerateOrdeniosPdfOutput(new byte[] { 1 }, "application/pdf", "ordenios.pdf")));
    }

    private sealed class FakeGenerateOrdeniosExcelBehaviorPipelineFactory(OrdenioPipelineSpy spy) : IGenerateOrdeniosExcelBehaviorPipelineFactory
    {
        public BehaviorPipeline<GenerateOrdeniosComparationExcelQuery, GenerateOrdeniosExcelOutput> Create()
            => new(
                Array.Empty<IBehavior<GenerateOrdeniosComparationExcelQuery, GenerateOrdeniosExcelOutput>>(),
                (query, _) =>
                {
                    spy.ExcelQuery = query;
                    return Task.FromResult(new GenerateOrdeniosExcelOutput(new byte[] { 1 }, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ordenios.xlsx"));
                });
    }

    private sealed class FakeListOrdeniosBehaviorPipelineFactory(OrdenioPipelineSpy spy) : IListOrdeniosBehaviorPipelineFactory
    {
        public BehaviorPipeline<ListOrdeniosQuery, ListOrdeniosOutput> Create()
            => new(
                Array.Empty<IBehavior<ListOrdeniosQuery, ListOrdeniosOutput>>(),
                (query, _) =>
                {
                    spy.ListQuery = query;
                    return Task.FromResult(new ListOrdeniosOutput(new[] { CreateOrdenioListOutput() }, 12));
                });
    }

    private sealed class FakeUpdateOrdenioBehaviorPipelineFactory(OrdenioPipelineSpy spy) : IUpdateOrdenioBehaviorPipelineFactory
    {
        public BehaviorPipeline<UpdateOrdenioCommand, UpdateOrdenioOutput> Create()
            => new(
                Array.Empty<IBehavior<UpdateOrdenioCommand, UpdateOrdenioOutput>>(),
                (command, _) =>
                {
                    spy.UpdateCommand = command;
                    return Task.FromResult(new UpdateOrdenioOutput(CreateOrdenioOutput(command.Id, "ORD-030")));
                });
    }

    private sealed class FakeDeleteOrdenioBehaviorPipelineFactory(OrdenioPipelineSpy spy) : IDeleteOrdenioBehaviorPipelineFactory
    {
        public BehaviorPipeline<DeleteOrdenioCommand, EmptyOutput> Create()
            => new(
                Array.Empty<IBehavior<DeleteOrdenioCommand, EmptyOutput>>(),
                (command, _) =>
                {
                    spy.DeleteCommand = command;
                    return Task.FromResult(EmptyOutput.Value);
                });
    }

    private static OrdenioOutput CreateOrdenioOutput(long id, string codigo)
    {
        var now = new DateTime(2026, 7, 7, 10, 0, 0);

        return new OrdenioOutput(
            id,
            codigo,
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
}
