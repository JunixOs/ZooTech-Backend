using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Common.Behaviors;
using ZooTech.Application.Common.Behaviors.Module_Sanidad.CreateTriaje;
using ZooTech.Application.Common.Behaviors.Module_Sanidad.DeleteTriaje;
using ZooTech.Application.Common.Behaviors.Module_Sanidad.GenerateTriajesExcel;
using ZooTech.Application.Common.Behaviors.Module_Sanidad.GenerateTriajesPdf;
using ZooTech.Application.Common.Behaviors.Module_Sanidad.GetAllTipoPesos;
using ZooTech.Application.Common.Behaviors.Module_Sanidad.GetAllTriajes;
using ZooTech.Application.Common.Behaviors.Module_Sanidad.GetAllVacunosSanidad;
using ZooTech.Application.Common.Behaviors.Module_Sanidad.GetHistorialByVacunoId;
using ZooTech.Application.Common.Behaviors.Module_Sanidad.GetHistorialGeneral;
using ZooTech.Application.Common.Behaviors.Module_Sanidad.GetTriajeById;
using ZooTech.Application.Common.Behaviors.Module_Sanidad.UpdateTriaje;
using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.Common;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.CreateTriaje;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.DeleteTriaje;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GenerateTriajesExcel;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GenerateTriajesPdf;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllTipoPesos;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllTriajes;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllVacunosSanidad;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetHistorialByVacunoId;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetHistorialGeneral;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetTriajeById;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.UpdateTriaje;
using ZooTech.InterfaceAdapters.DTOs;
using ZooTech.InterfaceAdapters.Module_Sanidad.Controllers;
using ZooTech.InterfaceAdapters.Modules.Module_Sanidad.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Sanidad.DTOs.Responses;

namespace ZooTech.InterfaceAdapters.UnitTests.Modules.Module_Sanidad.Controllers;

public class TriajeControllerTests
{
    private static readonly DateTime Now = new(2026, 7, 22, 10, 0, 0);

    [Fact]
    public async Task GetAll_MapsQueryParamsAndReturnsOk()
    {
        GetAllTriajesQuery? captured = null;
        var controller = CreateController(getAllFactory: new GetAllFactory(query =>
        {
            captured = query;
            return Task.FromResult(new GetAllTriajesOutput(new[] { CreateTriajeOutput() }, 1));
        }));

        var result = await controller.GetAll(0, 200, "2026-07-22", "2026-07-01", "2026-07-31", "TRI", "Luna", "CONTROL", "120", 5, true);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = AssertGeneralResponseData<PagedTriajeResponse>(okResult.Value);
        Assert.NotNull(captured);
        Assert.Equal(0, captured.Pagina);
        Assert.Equal(200, captured.Tamano);
        Assert.Equal("2026-07-22", captured.Fecha);
        Assert.Equal("2026-07-01", captured.FechaDesde);
        Assert.Equal("2026-07-31", captured.FechaHasta);
        Assert.Equal("TRI", captured.Codigo);
        Assert.Equal("Luna", captured.Nombre);
        Assert.Equal("CONTROL", captured.TipoPeso);
        Assert.Equal("120", captured.PesoKg);
        Assert.Equal(5, captured.VacunoId);
        Assert.True(captured.UniqueVacuno);
        Assert.Equal(1, response.Pagination.Page);
        Assert.Equal(100, response.Pagination.Limit);
    }

    [Fact]
    public async Task Create_MapsRequestAndReturnsCreatedAtAction()
    {
        CreateTriajeCommand? captured = null;
        var controller = CreateController(createFactory: new CreateFactory(command =>
        {
            captured = command;
            return Task.FromResult(new CreateTriajeOutput(9, "TRI009", Now, command.VacunoId, command.TipoPesoCode!, command.PesoKg, command.Observaciones, "ACTIVO", command.EncargadoUsuarioId, Now));
        }));
        var request = new TriajeRequest(5, "CONTROL", 120m, "Obs", 10, Now.AddDays(-1));

        var result = await controller.Create(request);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var response = AssertGeneralResponseData<TriajeResponse>(createdResult.Value);
        Assert.Equal(nameof(TriajeController.GetById), createdResult.ActionName);
        Assert.Equal(9L, createdResult.RouteValues?["id"]);
        Assert.Equal(5, captured?.VacunoId);
        Assert.Equal("CONTROL", captured?.TipoPesoCode);
        Assert.Equal(120m, captured?.PesoKg);
        Assert.Equal("Obs", captured?.Observaciones);
        Assert.Equal(10, captured?.EncargadoUsuarioId);
        Assert.Equal(Now.AddDays(-1), captured?.FechaHora);
        Assert.Equal(9, response.Id);
    }

    [Fact]
    public async Task Delete_MapsRequestAndReturnsNoContent()
    {
        DeleteTriajeCommand? captured = null;
        var controller = CreateController(deleteFactory: new DeleteFactory(command =>
        {
            captured = command;
            return Task.FromResult(EmptyOutput.Value);
        }));

        var result = await controller.Delete(8, new DeleteTriajeRequest("Duplicado"));

        Assert.IsType<NoContentResult>(result);
        Assert.Equal(8, captured?.Id);
        Assert.Equal("Duplicado", captured?.MotivoEliminacion);
    }

    [Fact]
    public async Task GeneratePdf_MapsFiltersAndReturnsFile()
    {
        GenerateTriajesPdfQuery? captured = null;
        var controller = CreateController(pdfFactory: new PdfFactory(query =>
        {
            captured = query;
            return Task.FromResult(new GenerateTriajesPdfOutput(new byte[] { 1 }, "application/pdf", "triajes.pdf"));
        }));

        var result = await controller.GeneratePdf("2026-07-22", "2026-07-01", "2026-07-31", "TRI", "Luna", "CONTROL", "120", 5, CancellationToken.None);

        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal("application/pdf", fileResult.ContentType);
        Assert.Equal("triajes.pdf", fileResult.FileDownloadName);
        Assert.Equal("TRI", captured?.Codigo);
        Assert.Equal("Luna", captured?.Nombre);
        Assert.Equal(5, captured?.VacunoId);
    }

    [Fact]
    public async Task GenerateExcel_MapsFiltersAndReturnsFile()
    {
        GenerateTriajesExcelQuery? captured = null;
        var controller = CreateController(excelFactory: new ExcelFactory(query =>
        {
            captured = query;
            return Task.FromResult(new GenerateTriajesExcelOutput(new byte[] { 1 }, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "triajes.xlsx"));
        }));

        var result = await controller.GenerateExcel("2026-07-22", "2026-07-01", "2026-07-31", "TRI", "Luna", "CONTROL", "120", 5, CancellationToken.None);

        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileResult.ContentType);
        Assert.Equal("triajes.xlsx", fileResult.FileDownloadName);
        Assert.Equal("CONTROL", captured?.TipoPeso);
        Assert.Equal("120", captured?.PesoKg);
    }

    [Fact]
    public async Task GetHistorial_MapsVacunoIdDatesAndReturnsOk()
    {
        GetHistorialByVacunoIdCommand? captured = null;
        var controller = CreateController(historialFactory: new HistorialFactory(command =>
        {
            captured = command;
            return Task.FromResult(new GetHistorialByVacunoIdOutput(new[] { new HistorialTriajeItemOutput(1, Now, "CONTROL", 120m) }));
        }));

        var result = await controller.GetHistorial(5, "2026-07-01", "2026-07-31");

        Assert.IsType<OkObjectResult>(result);
        Assert.Equal(5, captured?.Vacunoid);
        Assert.Equal("2026-07-01", captured?.FechaDesde);
        Assert.Equal("2026-07-31", captured?.FechaHasta);
    }

    [Fact]
    public async Task GetHistorialGeneral_MapsDatesAndReturnsOk()
    {
        GetHistorialGeneralCommand? captured = null;
        var controller = CreateController(historialGeneralFactory: new HistorialGeneralFactory(command =>
        {
            captured = command;
            return Task.FromResult(new GetHistorialGeneralOutput(new[] { new HistorialTriajeItemOutput(1, Now, "CONTROL", 120m) }));
        }));

        var result = await controller.GetHistorialGeneral("2026-07-01", "2026-07-31");

        Assert.IsType<OkObjectResult>(result);
        Assert.Equal("2026-07-01", captured?.FechaDesde);
        Assert.Equal("2026-07-31", captured?.FechaHasta);
    }

    [Fact]
    public async Task GetTiposPeso_ReturnsOk()
    {
        var controller = CreateController(tipoPesoFactory: new TipoPesoFactory(_ =>
            Task.FromResult(new GetAllTipoPesosOutput(new[] { new TipoPesoItemOutput("CONTROL", "Peso Control") }))));

        var result = await controller.GetTiposPeso();

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetVacunos_ReturnsOk()
    {
        var controller = CreateController(vacunosFactory: new VacunosFactory(_ =>
            Task.FromResult(new GetAllVacunosSanidadOutput(new[] { new VacunoSanidadItemOutput(1, "VAC001", "Luna") }))));

        var result = await controller.GetVacunos();

        Assert.IsType<OkObjectResult>(result);
    }

    private static TriajeController CreateController(
        IGetAllTriajesBehaviorPipelineFactory? getAllFactory = null,
        IGetTriajeByIdBehaviorPipelineFactory? getByIdFactory = null,
        ICreateTriajeBehaviorPipelineFactory? createFactory = null,
        IUpdateTriajeBehaviorPipelineFactory? updateFactory = null,
        IDeleteTriajeBehaviorPipelineFactory? deleteFactory = null,
        IGetAllTipoPesosBehaviorPipelineFactory? tipoPesoFactory = null,
        IGetAllVacunosSanidadBehaviorPipelineFactory? vacunosFactory = null,
        IGetHistorialByVacunoIdBehaviorPipelineFactory? historialFactory = null,
        IGetHistorialGeneralBehaviorPipelineFactory? historialGeneralFactory = null,
        IGenerateTriajesPdfBehaviorPipelineFactory? pdfFactory = null,
        IGenerateTriajesExcelBehaviorPipelineFactory? excelFactory = null)
        => new(
            getAllFactory ?? new GetAllFactory(_ => Task.FromResult(new GetAllTriajesOutput(Array.Empty<TriajeOutput>(), 0))),
            getByIdFactory ?? new GetByIdFactory(_ => Task.FromResult(new GetTriajeByIdOutput(1, "TRI001", Now, 1, "Luna", "CONTROL", 120m, null, "ACTIVO", 10, Now))),
            createFactory ?? new CreateFactory(_ => Task.FromResult(new CreateTriajeOutput(1, "TRI001", Now, 1, "CONTROL", 120m, null, "ACTIVO", 10, Now))),
            updateFactory ?? new UpdateFactory(_ => Task.FromResult(new UpdateTriajeOutput(1, "TRI001", Now, 1, "Luna", "CONTROL", 120m, null, "ACTIVO", 10, Now, Now))),
            deleteFactory ?? new DeleteFactory(_ => Task.FromResult(EmptyOutput.Value)),
            tipoPesoFactory ?? new TipoPesoFactory(_ => Task.FromResult(new GetAllTipoPesosOutput(Array.Empty<TipoPesoItemOutput>()))),
            vacunosFactory ?? new VacunosFactory(_ => Task.FromResult(new GetAllVacunosSanidadOutput(Array.Empty<VacunoSanidadItemOutput>()))),
            historialFactory ?? new HistorialFactory(_ => Task.FromResult(new GetHistorialByVacunoIdOutput(Array.Empty<HistorialTriajeItemOutput>()))),
            historialGeneralFactory ?? new HistorialGeneralFactory(_ => Task.FromResult(new GetHistorialGeneralOutput(Array.Empty<HistorialTriajeItemOutput>()))),
            pdfFactory ?? new PdfFactory(_ => Task.FromResult(new GenerateTriajesPdfOutput(Array.Empty<byte>(), "application/pdf", "triajes.pdf"))),
            excelFactory ?? new ExcelFactory(_ => Task.FromResult(new GenerateTriajesExcelOutput(Array.Empty<byte>(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "triajes.xlsx"))));

    private static BehaviorPipeline<TRequest, TResponse> Pipeline<TRequest, TResponse>(Func<TRequest, Task<TResponse>> handler)
        => new(Array.Empty<IBehavior<TRequest, TResponse>>(), (request, _) => handler(request));

    private static TriajeOutput CreateTriajeOutput()
        => new(1, "TRI001", Now, 1, "Luna", "CONTROL", 120m, null, "ACTIVO", 10, Now);

    private static T AssertGeneralResponseData<T>(object? value)
    {
        var response = Assert.IsType<GeneralResponseDTO<T>>(value);
        Assert.True(response.Success);
        return Assert.IsType<T>(response.Data);
    }

    private sealed class GetAllFactory(Func<GetAllTriajesQuery, Task<GetAllTriajesOutput>> handler) : IGetAllTriajesBehaviorPipelineFactory
    {
        public BehaviorPipeline<GetAllTriajesQuery, GetAllTriajesOutput> Create() => Pipeline(handler);
    }

    private sealed class GetByIdFactory(Func<GetTriajeByIdCommand, Task<GetTriajeByIdOutput>> handler) : IGetTriajeByIdBehaviorPipelineFactory
    {
        public BehaviorPipeline<GetTriajeByIdCommand, GetTriajeByIdOutput> Create() => Pipeline(handler);
    }

    private sealed class CreateFactory(Func<CreateTriajeCommand, Task<CreateTriajeOutput>> handler) : ICreateTriajeBehaviorPipelineFactory
    {
        public BehaviorPipeline<CreateTriajeCommand, CreateTriajeOutput> Create() => Pipeline(handler);
    }

    private sealed class UpdateFactory(Func<UpdateTriajeCommand, Task<UpdateTriajeOutput>> handler) : IUpdateTriajeBehaviorPipelineFactory
    {
        public BehaviorPipeline<UpdateTriajeCommand, UpdateTriajeOutput> Create() => Pipeline(handler);
    }

    private sealed class DeleteFactory(Func<DeleteTriajeCommand, Task<EmptyOutput>> handler) : IDeleteTriajeBehaviorPipelineFactory
    {
        public BehaviorPipeline<DeleteTriajeCommand, EmptyOutput> Create() => Pipeline(handler);
    }

    private sealed class TipoPesoFactory(Func<EmptyCommand, Task<GetAllTipoPesosOutput>> handler) : IGetAllTipoPesosBehaviorPipelineFactory
    {
        public BehaviorPipeline<EmptyCommand, GetAllTipoPesosOutput> Create() => Pipeline(handler);
    }

    private sealed class VacunosFactory(Func<EmptyCommand, Task<GetAllVacunosSanidadOutput>> handler) : IGetAllVacunosSanidadBehaviorPipelineFactory
    {
        public BehaviorPipeline<EmptyCommand, GetAllVacunosSanidadOutput> Create() => Pipeline(handler);
    }

    private sealed class HistorialFactory(Func<GetHistorialByVacunoIdCommand, Task<GetHistorialByVacunoIdOutput>> handler) : IGetHistorialByVacunoIdBehaviorPipelineFactory
    {
        public BehaviorPipeline<GetHistorialByVacunoIdCommand, GetHistorialByVacunoIdOutput> Create() => Pipeline(handler);
    }

    private sealed class HistorialGeneralFactory(Func<GetHistorialGeneralCommand, Task<GetHistorialGeneralOutput>> handler) : IGetHistorialGeneralBehaviorPipelineFactory
    {
        public BehaviorPipeline<GetHistorialGeneralCommand, GetHistorialGeneralOutput> Create() => Pipeline(handler);
    }

    private sealed class PdfFactory(Func<GenerateTriajesPdfQuery, Task<GenerateTriajesPdfOutput>> handler) : IGenerateTriajesPdfBehaviorPipelineFactory
    {
        public BehaviorPipeline<GenerateTriajesPdfQuery, GenerateTriajesPdfOutput> Create() => Pipeline(handler);
    }

    private sealed class ExcelFactory(Func<GenerateTriajesExcelQuery, Task<GenerateTriajesExcelOutput>> handler) : IGenerateTriajesExcelBehaviorPipelineFactory
    {
        public BehaviorPipeline<GenerateTriajesExcelQuery, GenerateTriajesExcelOutput> Create() => Pipeline(handler);
    }
}
