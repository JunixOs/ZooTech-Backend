using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Common.Behaviors;
using ZooTech.Application.Common.Behaviors.Module_Celo.CreateCelo;
using ZooTech.Application.Common.Behaviors.Module_Celo.DeleteCelo;
using ZooTech.Application.Common.Behaviors.Module_Celo.GetCelos;
using ZooTech.Application.Common.Behaviors.Module_Celo.GetComparacionCelosRealVsEstandar;
using ZooTech.Application.Common.Behaviors.Module_Celo.GetComparacionCelosRealVsEstandarPorVacuno;
using ZooTech.Application.Common.Behaviors.Module_Celo.GetHistorialCeloPorVacuno;
using ZooTech.Application.Common.Behaviors.Module_Celo.GetReporteCelos;
using ZooTech.Application.Common.Behaviors.Module_Celo.GetVacasEnCelo;
using ZooTech.Application.Common.Behaviors.Module_Celo.ListCelos;
using ZooTech.Application.Common.Behaviors.Module_Celo.ListReporteCeloGeneral;
using ZooTech.Application.Common.Behaviors.Module_Celo.UpdateCelo;
using ZooTech.Application.Common.Gateway.Identity;
using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_Celo.UseCases.CreateCelo;
using ZooTech.Application.Modules.Module_Celo.UseCases.DeleteCelo;
using ZooTech.Application.Modules.Module_Celo.UseCases.GetCelos;
using ZooTech.Application.Modules.Module_Celo.UseCases.GetComparacionCelosRealVsEstandar;
using ZooTech.Application.Modules.Module_Celo.UseCases.GetComparacionCelosRealVsEstandarPorVacuno;
using ZooTech.Application.Modules.Module_Celo.UseCases.GetHistorialCeloPorVacuno;
using ZooTech.Application.Modules.Module_Celo.UseCases.GetReporteCelos;
using ZooTech.Application.Modules.Module_Celo.UseCases.GetVacasEnCelo;
using ZooTech.Application.Modules.Module_Celo.UseCases.ListCelos;
using ZooTech.Application.Modules.Module_Celo.UseCases.ListReporteCeloGeneral;
using ZooTech.Application.Modules.Module_Celo.UseCases.UpdateCelo;
using ZooTech.InterfaceAdapters.DTOs;
using ZooTech.InterfaceAdapters.Modules.Module_Celo.Controllers;
using ZooTech.InterfaceAdapters.Modules.Module_Celo.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Celo.DTOs.Responses;

namespace ZooTech.InterfaceAdapters.UnitTests.Modules.Module_Celo.Controllers;

public class CeloControllerTests
{
    private static readonly DateTime Now = new(2026, 7, 22, 10, 0, 0);

    [Fact]
    public async Task GetCelos_ReturnsOk()
    {
        var controller = CreateController(listCelosFactory: new ListCelosFactory(_ =>
            Task.FromResult(new ListCelosOutput(
                new ZooTech.Application.Common.Pagination.PagedResult<CeloItemDto>(new[] { CreateCeloItemDto() }, TotalCount: 1, Page: 1, PageSize: 20)))));

        var result = await controller.GetCelos(null, cancellationToken: CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result);
        AssertGeneralResponseData<ListCelosResponse>(okResult.Value);
    }

    [Fact]
    public async Task GetReporteCeloGeneral_ReturnsOk()
    {
        var controller = CreateController(listReporteCeloGeneralFactory: new ListReporteCeloGeneralFactory(_ =>
            Task.FromResult(new ListReporteCeloGeneralOutput(
                new ZooTech.Application.Common.Pagination.PagedResult<CeloReporteItemDto>(new[] { CreateCeloReporteItemDto() }, TotalCount: 1, Page: 1, PageSize: 20)))));

        var result = await controller.GetReporteCeloGeneral(null, cancellationToken: CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result);
        AssertGeneralResponseData<ListReporteCeloGeneralResponse>(okResult.Value);
    }

    [Fact]
    public async Task GetReporteCeloPorVacuno_GroupsAndAggregatesByVacunoAndFiltersByFecha()
    {
        var controller = CreateController(getCelosFactory: new GetCelosFactory(_ =>
            Task.FromResult(new GetCelosOutput(new List<CeloItemDto>
            {
                CreateCeloItemDto(codigoVacuno: "VAC-001", fecha: new DateOnly(2026, 7, 10)),
                CreateCeloItemDto(codigoVacuno: "VAC-001", fecha: new DateOnly(2026, 7, 20)),
                CreateCeloItemDto(codigoVacuno: "VAC-002", fecha: new DateOnly(2026, 6, 1))
            }))));

        var result = await controller.GetReporteCeloPorVacuno(
            new DateTime(2026, 7, 1), new DateTime(2026, 7, 31), CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = AssertGeneralResponseData<List<ReporteCeloPorVacunoResponse>>(okResult.Value);
        var item = Assert.Single(response);
        Assert.Equal("VAC-001", item.CodigoVacuno);
        Assert.Equal(new DateOnly(2026, 7, 20), item.UltimoCelo);
        Assert.Equal(2, item.VecesEnCelo);
    }

    [Fact]
    public async Task GetDetalleCeloPorVacuno_FiltersByCodigoAndOrdersDescendingByFechaThenHora()
    {
        var controller = CreateController(getReporteCelosFactory: new GetReporteCelosFactory(_ =>
            Task.FromResult(new GetReporteCelosOutput(new List<CeloReporteItemDto>
            {
                CreateCeloReporteItemDto(codigoVacuno: "VAC-001", fecha: new DateOnly(2026, 7, 10), hora: new TimeOnly(9, 0)),
                CreateCeloReporteItemDto(codigoVacuno: "VAC-001", fecha: new DateOnly(2026, 7, 10), hora: new TimeOnly(15, 0)),
                CreateCeloReporteItemDto(codigoVacuno: "VAC-002", fecha: new DateOnly(2026, 7, 15), hora: new TimeOnly(9, 0))
            }))));

        var result = await controller.GetDetalleCeloPorVacuno("VAC-001", CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = AssertGeneralResponseData<List<CeloReporteItemResponse>>(okResult.Value);
        Assert.Equal(2, response.Count);
        Assert.All(response, item => Assert.Equal("VAC-001", item.CodigoVacuno));
        Assert.Equal(new TimeOnly(15, 0), response[0].Hora);
        Assert.Equal(new TimeOnly(9, 0), response[1].Hora);
    }

    [Fact]
    public async Task GetHistorialPorVacuno_MapsCodigoVacunoAndRegistroId()
    {
        GetHistorialCeloPorVacunoCommand? captured = null;
        var controller = CreateController(getHistorialCeloPorVacunoFactory: new GetHistorialCeloPorVacunoFactory(command =>
        {
            captured = command;
            return Task.FromResult(new GetHistorialCeloPorVacunoOutput(
                "Juan Perez",
                new List<string>(),
                new List<CeloHistorialCeloPorVacunoDto>(),
                new CeloResumenReproductivoDto(0, 0, 0, 0, 0, 0, null)));
        }));

        var result = await controller.GetHistorialPorVacuno("VAC-001", 7, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result);
        AssertGeneralResponseData<CeloHistorialPorVacunoResponse>(okResult.Value);
        Assert.NotNull(captured);
        Assert.Equal("VAC-001", captured.CodigoVacuno);
        Assert.Equal(7, captured.RegistroId);
    }

    [Fact]
    public async Task GetVacasEnCelo_ReturnsOk()
    {
        var controller = CreateController(getVacasEnCeloFactory: new GetVacasEnCeloFactory(_ =>
            Task.FromResult(new GetVacasEnCeloOutput(new List<VacaEnCeloDto> { CreateVacaEnCeloDto() }))));

        var result = await controller.GetVacasEnCelo(null, null, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result);
        AssertGeneralResponseData<List<VacaEnCeloResponse>>(okResult.Value);
    }

    [Fact]
    public async Task GetComparacionRealVsEstandar_ReturnsOk()
    {
        var controller = CreateController(getComparacionRealVsEstandarFactory: new GetComparacionRealVsEstandarFactory(_ =>
            Task.FromResult(new GetComparacionCelosRealVsEstandarOutput(new List<ComparacionCelosItemDto>
            {
                new() { Fecha = new DateOnly(2026, 7, 22), RegistrosReales = 1, RegistrosEstandar = 2 }
            }))));

        var result = await controller.GetComparacionRealVsEstandar(null, null, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result);
        AssertGeneralResponseData<List<ComparacionCelosResponse>>(okResult.Value);
    }

    [Fact]
    public async Task GetComparacionPorVacuno_ReturnsOk()
    {
        var controller = CreateController(getComparacionPorVacunoFactory: new GetComparacionPorVacunoFactory(_ =>
            Task.FromResult(new GetComparacionCelosRealVsEstandarPorVacunoOutput(new List<ComparacionCelosPorVacunoItemDto>
            {
                new() { Periodo = new DateOnly(2026, 7, 1), RegistrosReales = 1, RegistrosEstandar = 2 }
            }))));

        var result = await controller.GetComparacionPorVacuno("VAC-001", null, null, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result);
        AssertGeneralResponseData<List<ComparacionCelosPorVacunoResponse>>(okResult.Value);
    }

    [Fact]
    public async Task CreateCelo_MapsRequestAndReturnsCreatedAtAction()
    {
        CreateCeloCommand? captured = null;
        var controller = CreateController(createCeloFactory: new CreateCeloFactory(command =>
        {
            captured = command;
            return Task.FromResult(new CreateCeloOutput(1, "CEL-001", Now));
        }));
        var request = new CreateCeloRequest(5, 10, Now, "Obs", new List<string> { "MONTA" });

        var result = await controller.CreateCelo(request, CancellationToken.None);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var response = AssertGeneralResponseData<CreateCeloResponse>(createdResult.Value);
        Assert.Equal(nameof(CeloController.GetCelos), createdResult.ActionName);
        Assert.Equal(5, captured?.VacunoId);
        Assert.Equal(10, captured?.EncargadoUsuarioId);
        Assert.Equal("Obs", captured?.Observaciones);
        Assert.Equal(1, response.Id);
    }

    [Fact]
    public async Task UpdateCelo_UsesCurrentUserAsActorUsuarioId()
    {
        UpdateCeloCommand? captured = null;
        var controller = CreateController(
            updateCeloFactory: new UpdateCeloFactory(command =>
            {
                captured = command;
                return Task.FromResult(new UpdateCeloOutput(3, "Actualizado"));
            }),
            currentUserService: new FakeCurrentUserService { UserId = 42 });
        var request = new UpdateCeloRequest("Actualizado", new List<string> { "MONTA" });

        var result = await controller.UpdateCelo(3, request, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result);
        AssertGeneralResponseData<UpdateCeloResponse>(okResult.Value);
        Assert.Equal(3, captured?.Id);
        Assert.Equal(42, captured?.ActorUsuarioId);
    }

    [Fact]
    public async Task DeleteCelo_MapsRequestAndReturnsOkWithMensaje()
    {
        DeleteCeloCommand? captured = null;
        var controller = CreateController(deleteCeloFactory: new DeleteCeloFactory(command =>
        {
            captured = command;
            return Task.FromResult(EmptyOutput.Value);
        }));
        var request = new DeleteCeloRequest("Duplicado");

        var result = await controller.DeleteCelo(5, request, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<GeneralResponseDTO<object>>(okResult.Value);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        var mensajeProperty = response.Data!.GetType().GetProperty("mensaje");
        Assert.NotNull(mensajeProperty);
        Assert.Equal("Celo eliminado con éxito", mensajeProperty!.GetValue(response.Data));
        Assert.Equal(5, captured?.Id);
        Assert.Equal("Duplicado", captured?.MotivoEliminacion);
    }

    private static CeloController CreateController(
        IGetCelosBehaviorPipelineFactory? getCelosFactory = null,
        ICreateCeloBehaviorPipelineFactory? createCeloFactory = null,
        IUpdateCeloBehaviorPipelineFactory? updateCeloFactory = null,
        IDeleteCeloBehaviorPipelineFactory? deleteCeloFactory = null,
        IGetComparacionCelosRealVsEstandarBehaviorPipelineFactory? getComparacionRealVsEstandarFactory = null,
        IGetComparacionCelosRealVsEstandarPorVacunoBehaviorPipelineFactory? getComparacionPorVacunoFactory = null,
        IGetVacasEnCeloBehaviorPipelineFactory? getVacasEnCeloFactory = null,
        IListCelosBehaviorPipelineFactory? listCelosFactory = null,
        IListReporteCeloGeneralBehaviorPipelineFactory? listReporteCeloGeneralFactory = null,
        IGetReporteCelosBehaviorPipelineFactory? getReporteCelosFactory = null,
        ICurrentUserService? currentUserService = null,
        IGetHistorialCeloPorVacunoBehaviorPipelineFactory? getHistorialCeloPorVacunoFactory = null)
        => new(
            getCelosFactory ?? new GetCelosFactory(_ => Task.FromResult(new GetCelosOutput(new List<CeloItemDto>()))),
            createCeloFactory ?? new CreateCeloFactory(_ => Task.FromResult(new CreateCeloOutput(1, "CEL-001", Now))),
            updateCeloFactory ?? new UpdateCeloFactory(_ => Task.FromResult(new UpdateCeloOutput(1, null))),
            deleteCeloFactory ?? new DeleteCeloFactory(_ => Task.FromResult(EmptyOutput.Value)),
            getComparacionRealVsEstandarFactory ?? new GetComparacionRealVsEstandarFactory(_ =>
                Task.FromResult(new GetComparacionCelosRealVsEstandarOutput(new List<ComparacionCelosItemDto>()))),
            getComparacionPorVacunoFactory ?? new GetComparacionPorVacunoFactory(_ =>
                Task.FromResult(new GetComparacionCelosRealVsEstandarPorVacunoOutput(new List<ComparacionCelosPorVacunoItemDto>()))),
            getVacasEnCeloFactory ?? new GetVacasEnCeloFactory(_ => Task.FromResult(new GetVacasEnCeloOutput(new List<VacaEnCeloDto>()))),
            listCelosFactory ?? new ListCelosFactory(_ => Task.FromResult(new ListCelosOutput(
                new ZooTech.Application.Common.Pagination.PagedResult<CeloItemDto>(Array.Empty<CeloItemDto>(), 0, 1, 20)))),
            listReporteCeloGeneralFactory ?? new ListReporteCeloGeneralFactory(_ => Task.FromResult(new ListReporteCeloGeneralOutput(
                new ZooTech.Application.Common.Pagination.PagedResult<CeloReporteItemDto>(Array.Empty<CeloReporteItemDto>(), 0, 1, 20)))),
            getReporteCelosFactory ?? new GetReporteCelosFactory(_ => Task.FromResult(new GetReporteCelosOutput(new List<CeloReporteItemDto>()))),
            currentUserService ?? new FakeCurrentUserService(),
            getHistorialCeloPorVacunoFactory ?? new GetHistorialCeloPorVacunoFactory(_ => Task.FromResult(new GetHistorialCeloPorVacunoOutput(
                null,
                new List<string>(),
                new List<CeloHistorialCeloPorVacunoDto>(),
                new CeloResumenReproductivoDto(0, 0, 0, 0, 0, 0, null)))));

    private static BehaviorPipeline<TRequest, TResponse> Pipeline<TRequest, TResponse>(Func<TRequest, Task<TResponse>> handler)
        => new(Array.Empty<IBehavior<TRequest, TResponse>>(), (request, _) => handler(request));

    private static CeloItemDto CreateCeloItemDto(string codigoVacuno = "VAC-001", DateOnly? fecha = null)
        => new()
        {
            Id = 1,
            CodigoRegistro = "CEL-001",
            Fecha = fecha ?? new DateOnly(2026, 7, 22),
            Hora = new TimeOnly(10, 0),
            CodigoVacuno = codigoVacuno,
            NombreVacuno = "Luna",
            VecesEnCelo = 1,
            Observaciones = null,
            CaracteristicaCodes = new List<string>()
        };

    private static CeloReporteItemDto CreateCeloReporteItemDto(string codigoVacuno = "VAC-001", DateOnly? fecha = null, TimeOnly? hora = null)
        => new()
        {
            CodigoRegistro = "CEL-001",
            Fecha = fecha ?? new DateOnly(2026, 7, 22),
            Hora = hora ?? new TimeOnly(10, 0),
            CodigoVacuno = codigoVacuno,
            NombreVacuno = "Luna",
            VecesEnCelo = 1,
            Caracteristicas = 0,
            ListaCaracteristicas = new List<string>(),
            Observaciones = string.Empty,
            Crias = 0
        };

    private static VacaEnCeloDto CreateVacaEnCeloDto()
        => new()
        {
            VacunoId = 1,
            Codigo = "VAC-001",
            Nombre = "Luna",
            Fecha = new DateOnly(2026, 7, 22),
            DiasRestante = 5,
            Estado = "EN_CELO",
            VecesEnCelo = 1,
            Crias = 0
        };

    private static T AssertGeneralResponseData<T>(object? value)
    {
        var response = Assert.IsType<GeneralResponseDTO<T>>(value);
        Assert.True(response.Success);
        return Assert.IsType<T>(response.Data);
    }

    private sealed class FakeCurrentUserService : ICurrentUserService
    {
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public bool IsAuthenticated { get; set; }
    }

    private sealed class GetCelosFactory(Func<EmptyCommand, Task<GetCelosOutput>> handler) : IGetCelosBehaviorPipelineFactory
    {
        public BehaviorPipeline<EmptyCommand, GetCelosOutput> Create() => Pipeline(handler);
    }

    private sealed class CreateCeloFactory(Func<CreateCeloCommand, Task<CreateCeloOutput>> handler) : ICreateCeloBehaviorPipelineFactory
    {
        public BehaviorPipeline<CreateCeloCommand, CreateCeloOutput> Create() => Pipeline(handler);
    }

    private sealed class UpdateCeloFactory(Func<UpdateCeloCommand, Task<UpdateCeloOutput>> handler) : IUpdateCeloBehaviorPipelineFactory
    {
        public BehaviorPipeline<UpdateCeloCommand, UpdateCeloOutput> Create() => Pipeline(handler);
    }

    private sealed class DeleteCeloFactory(Func<DeleteCeloCommand, Task<EmptyOutput>> handler) : IDeleteCeloBehaviorPipelineFactory
    {
        public BehaviorPipeline<DeleteCeloCommand, EmptyOutput> Create() => Pipeline(handler);
    }

    private sealed class GetComparacionRealVsEstandarFactory(
        Func<GetComparacionCelosRealVsEstandarCommand, Task<GetComparacionCelosRealVsEstandarOutput>> handler)
        : IGetComparacionCelosRealVsEstandarBehaviorPipelineFactory
    {
        public BehaviorPipeline<GetComparacionCelosRealVsEstandarCommand, GetComparacionCelosRealVsEstandarOutput> Create() => Pipeline(handler);
    }

    private sealed class GetComparacionPorVacunoFactory(
        Func<GetComparacionCelosRealVsEstandarPorVacunoCommand, Task<GetComparacionCelosRealVsEstandarPorVacunoOutput>> handler)
        : IGetComparacionCelosRealVsEstandarPorVacunoBehaviorPipelineFactory
    {
        public BehaviorPipeline<GetComparacionCelosRealVsEstandarPorVacunoCommand, GetComparacionCelosRealVsEstandarPorVacunoOutput> Create() => Pipeline(handler);
    }

    private sealed class GetVacasEnCeloFactory(Func<GetVacasEnCeloCommand, Task<GetVacasEnCeloOutput>> handler) : IGetVacasEnCeloBehaviorPipelineFactory
    {
        public BehaviorPipeline<GetVacasEnCeloCommand, GetVacasEnCeloOutput> Create() => Pipeline(handler);
    }

    private sealed class ListCelosFactory(Func<ListCelosCommand, Task<ListCelosOutput>> handler) : IListCelosBehaviorPipelineFactory
    {
        public BehaviorPipeline<ListCelosCommand, ListCelosOutput> Create() => Pipeline(handler);
    }

    private sealed class ListReporteCeloGeneralFactory(Func<ListReporteCeloGeneralCommand, Task<ListReporteCeloGeneralOutput>> handler)
        : IListReporteCeloGeneralBehaviorPipelineFactory
    {
        public BehaviorPipeline<ListReporteCeloGeneralCommand, ListReporteCeloGeneralOutput> Create() => Pipeline(handler);
    }

    private sealed class GetReporteCelosFactory(Func<EmptyCommand, Task<GetReporteCelosOutput>> handler) : IGetReporteCelosBehaviorPipelineFactory
    {
        public BehaviorPipeline<EmptyCommand, GetReporteCelosOutput> Create() => Pipeline(handler);
    }

    private sealed class GetHistorialCeloPorVacunoFactory(
        Func<GetHistorialCeloPorVacunoCommand, Task<GetHistorialCeloPorVacunoOutput>> handler)
        : IGetHistorialCeloPorVacunoBehaviorPipelineFactory
    {
        public BehaviorPipeline<GetHistorialCeloPorVacunoCommand, GetHistorialCeloPorVacunoOutput> Create() => Pipeline(handler);
    }
}
