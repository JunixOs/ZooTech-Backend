using Microsoft.AspNetCore.Mvc;

using ZooTech.Application.Common.Behaviors;
using ZooTech.Application.Common.Gateway.Identity;
using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_Celo.Models;
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
    private static readonly DateTime Now =
        new(2026, 7, 22, 10, 0, 0);

    [Fact]
    public async Task GetCelos_ReturnsOk()
    {
        var dispatcher = new TestBehaviorDispatcher();

        dispatcher.Setup<ListCelosQuery, ListCelosOutput>((_, _) =>
            Task.FromResult(
                new ListCelosOutput(
                    new ZooTech.Application.Common.Pagination.PagedResult<CeloItemDto>(
                        new[] { CreateCeloItemDto() },
                        TotalCount: 1,
                        Page: 1,
                        PageSize: 20))));

        var controller = CreateController(dispatcher);

        var result = await controller.GetCelos(
            null,
            cancellationToken: CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result);

        AssertGeneralResponseData<ListCelosResponse>(okResult.Value);
    }

    [Fact]
    public async Task GetReporteCeloGeneral_ReturnsOk()
    {
        var dispatcher = new TestBehaviorDispatcher();

        dispatcher.Setup<ListReporteCeloGeneralQuery, ListReporteCeloGeneralOutput>((_, _) =>
            Task.FromResult(
                new ListReporteCeloGeneralOutput(
                    new ZooTech.Application.Common.Pagination.PagedResult<CeloReporteItemDto>(
                        new[] { CreateCeloReporteItemDto() },
                        TotalCount: 1,
                        Page: 1,
                        PageSize: 20))));

        var controller = CreateController(dispatcher);

        var result = await controller.GetReporteCeloGeneral(
            null,
            cancellationToken: CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result);

        AssertGeneralResponseData<ListReporteCeloGeneralResponse>(
            okResult.Value);
    }

    [Fact]
    public async Task GetReporteCeloPorVacuno_GroupsAndAggregatesByVacunoAndFiltersByFecha()
    {
        var dispatcher = new TestBehaviorDispatcher();

        dispatcher.Setup<EmptyCommandQuery, GetCelosOutput>((_, _) =>
            Task.FromResult(
                new GetCelosOutput(
                    new List<CeloItemDto>
                    {
                        CreateCeloItemDto(
                            codigoVacuno: "VAC-001",
                            fecha: new DateOnly(2026, 7, 10)),

                        CreateCeloItemDto(
                            codigoVacuno: "VAC-001",
                            fecha: new DateOnly(2026, 7, 20)),

                        CreateCeloItemDto(
                            codigoVacuno: "VAC-002",
                            fecha: new DateOnly(2026, 6, 1))
                    })));

        var controller = CreateController(dispatcher);

        var result = await controller.GetReporteCeloPorVacuno(
            new DateTime(2026, 7, 1),
            new DateTime(2026, 7, 31),
            CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result);

        var response =
            AssertGeneralResponseData<List<ReporteCeloPorVacunoResponse>>(
                okResult.Value);

        var item = Assert.Single(response);

        Assert.Equal("VAC-001", item.CodigoVacuno);
        Assert.Equal(new DateOnly(2026, 7, 20), item.UltimoCelo);
        Assert.Equal(2, item.VecesEnCelo);
    }

    [Fact]
    public async Task GetDetalleCeloPorVacuno_FiltersByCodigoAndOrdersDescendingByFechaThenHora()
    {
        var dispatcher = new TestBehaviorDispatcher();

        dispatcher.Setup<EmptyCommandQuery, GetReporteCelosOutput>((_, _) =>
            Task.FromResult(
                new GetReporteCelosOutput(
                    new List<CeloReporteItemDto>
                    {
                        CreateCeloReporteItemDto(
                            codigoVacuno: "VAC-001",
                            fecha: new DateOnly(2026, 7, 10),
                            hora: new TimeOnly(9, 0)),

                        CreateCeloReporteItemDto(
                            codigoVacuno: "VAC-001",
                            fecha: new DateOnly(2026, 7, 10),
                            hora: new TimeOnly(15, 0)),

                        CreateCeloReporteItemDto(
                            codigoVacuno: "VAC-002",
                            fecha: new DateOnly(2026, 7, 15),
                            hora: new TimeOnly(9, 0))
                    })));

        var controller = CreateController(dispatcher);

        var result = await controller.GetDetalleCeloPorVacuno(
            "VAC-001",
            CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result);

        var response =
            AssertGeneralResponseData<List<CeloReporteItemResponse>>(
                okResult.Value);

        Assert.Equal(2, response.Count);
        Assert.All(
            response,
            item => Assert.Equal("VAC-001", item.CodigoVacuno));

        Assert.Equal(new TimeOnly(15, 0), response[0].Hora);
        Assert.Equal(new TimeOnly(9, 0), response[1].Hora);
    }

    [Fact]
    public async Task GetHistorialPorVacuno_MapsCodigoVacunoAndRegistroId()
    {
        GetHistorialCeloPorVacunoCommand? captured = null;

        var dispatcher = new TestBehaviorDispatcher();

        dispatcher.Setup<
            GetHistorialCeloPorVacunoCommand,
            GetHistorialCeloPorVacunoOutput>((command, _) =>
        {
            captured = command;

            return Task.FromResult(
                new GetHistorialCeloPorVacunoOutput(
                    "Juan Perez",
                    new List<string>(),
                    new List<CeloHistorialCeloPorVacunoDto>(),
                    new CeloResumenReproductivoDto(
                        0, 0, 0, 0, 0, 0, null)));
        });

        var controller = CreateController(dispatcher);

        var result = await controller.GetHistorialPorVacuno(
            "VAC-001",
            7,
            CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result);

        AssertGeneralResponseData<CeloHistorialPorVacunoResponse>(
            okResult.Value);

        Assert.NotNull(captured);
        Assert.Equal("VAC-001", captured.CodigoVacuno);
        Assert.Equal(7, captured.RegistroId);
    }

    [Fact]
    public async Task GetVacasEnCelo_ReturnsOk()
    {
        var dispatcher = new TestBehaviorDispatcher();

        dispatcher.Setup<GetVacasEnCeloQuery, GetVacasEnCeloOutput>((_, _) =>
            Task.FromResult(
                new GetVacasEnCeloOutput(
                    new List<VacaEnCeloDto>
                    {
                        CreateVacaEnCeloDto()
                    })));

        var controller = CreateController(dispatcher);

        var result = await controller.GetVacasEnCelo(
            null,
            null,
            CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result);

        AssertGeneralResponseData<List<VacaEnCeloResponse>>(
            okResult.Value);
    }

    [Fact]
    public async Task GetComparacionRealVsEstandar_ReturnsOk()
    {
        var dispatcher = new TestBehaviorDispatcher();

        dispatcher.Setup<
            GetComparacionCelosRealVsEstandarQuery,
            GetComparacionCelosRealVsEstandarOutput>((_, _) =>
        {
            return Task.FromResult(
                new GetComparacionCelosRealVsEstandarOutput(
                    new List<ComparacionCelosItemDto>
                    {
                        new()
                        {
                            Fecha = new DateOnly(2026, 7, 22),
                            RegistrosReales = 1,
                            RegistrosEstandar = 2
                        }
                    }));
        });

        var controller = CreateController(dispatcher);

        var result = await controller.GetComparacionRealVsEstandar(
            null,
            null,
            CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result);

        AssertGeneralResponseData<List<ComparacionCelosResponse>>(
            okResult.Value);
    }

    [Fact]
    public async Task GetComparacionPorVacuno_ReturnsOk()
    {
        var dispatcher = new TestBehaviorDispatcher();

        dispatcher.Setup<
            GetComparacionCelosRealVsEstandarPorVacunoQuery,
            GetComparacionCelosRealVsEstandarPorVacunoOutput>((_, _) =>
        {
            return Task.FromResult(
                new GetComparacionCelosRealVsEstandarPorVacunoOutput(
                    new List<ComparacionCelosPorVacunoItemDto>
                    {
                        new()
                        {
                            Periodo = new DateOnly(2026, 7, 1),
                            RegistrosReales = 1,
                            RegistrosEstandar = 2
                        }
                    }));
        });

        var controller = CreateController(dispatcher);

        var result = await controller.GetComparacionPorVacuno(
            "VAC-001",
            null,
            null,
            CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result);

        AssertGeneralResponseData<List<ComparacionCelosPorVacunoResponse>>(
            okResult.Value);
    }

    [Fact]
    public async Task CreateCelo_MapsRequestAndReturnsCreatedAtAction()
    {
        CreateCeloCommand? captured = null;

        var dispatcher = new TestBehaviorDispatcher();

        dispatcher.Setup<CreateCeloCommand, CreateCeloOutput>((command, _) =>
        {
            captured = command;

            return Task.FromResult(
                new CreateCeloOutput(
                    1,
                    "CEL-001",
                    Now));
        });

        var controller = CreateController(dispatcher);

        var request = new CreateCeloRequest(
            5,
            10,
            Now,
            "Obs",
            new List<string> { "MONTA" });

        var result = await controller.CreateCelo(
            request,
            CancellationToken.None);

        var createdResult =
            Assert.IsType<CreatedAtActionResult>(result);

        var response =
            AssertGeneralResponseData<CreateCeloResponse>(
                createdResult.Value);

        Assert.Equal(
            nameof(CeloController.GetCelos),
            createdResult.ActionName);

        Assert.Equal(5, captured?.VacunoId);
        Assert.Equal(10, captured?.EncargadoUsuarioId);
        Assert.Equal("Obs", captured?.Observaciones);
        Assert.Equal(1, response.Id);
    }

    [Fact]
    public async Task UpdateCelo_UsesCurrentUserAsActorUsuarioId()
    {
        UpdateCeloCommand? captured = null;

        var dispatcher = new TestBehaviorDispatcher();

        dispatcher.Setup<UpdateCeloCommand, UpdateCeloOutput>((command, _) =>
        {
            captured = command;

            return Task.FromResult(
                new UpdateCeloOutput(
                    3,
                    "Actualizado"));
        });

        var controller = CreateController(dispatcher);

        var request = new UpdateCeloRequest(
            "Actualizado",
            new List<string> { "MONTA" });

        var result = await controller.UpdateCelo(
            3,
            request,
            CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result);

        AssertGeneralResponseData<UpdateCeloResponse>(
            okResult.Value);

        Assert.Equal(3, captured?.Id);
        Assert.Equal(42, captured?.ActorUsuarioId);
    }

    [Fact]
    public async Task DeleteCelo_MapsRequestAndReturnsOkWithMensaje()
    {
        DeleteCeloCommand? captured = null;

        var dispatcher = new TestBehaviorDispatcher();

        dispatcher.Setup<DeleteCeloCommand, EmptyOutput>((command, _) =>
        {
            captured = command;

            return Task.FromResult(
                EmptyOutput.Value);
        });

        var controller = CreateController(dispatcher);

        var request = new DeleteCeloRequest("Duplicado");

        var result = await controller.DeleteCelo(
            5,
            request,
            CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result);

        var response =
            Assert.IsType<GeneralResponseDTO<object>>(
                okResult.Value);

        Assert.True(response.Success);
        Assert.NotNull(response.Data);

        var mensajeProperty =
            response.Data!
                .GetType()
                .GetProperty("mensaje");

        Assert.NotNull(mensajeProperty);

        Assert.Equal(
            "Celo eliminado con éxito",
            mensajeProperty!.GetValue(response.Data));

        Assert.Equal(5, captured?.Id);
        Assert.Equal(
            "Duplicado",
            captured?.MotivoEliminacion);
    }

    private static CeloController CreateController(
        IBehaviorDispatcher dispatcher)
        => new(
            new TestCurrentUserService(),
            dispatcher);

    private sealed class TestCurrentUserService : ICurrentUserService
    {
        public int? UserId => 42;
        public string? UserName => "test-user";
        public string? Email => "test@example.com";
        public string? Role => "REGULAR";
        public bool IsAuthenticated => true;
    }

    private sealed class TestBehaviorDispatcher : IBehaviorDispatcher
    {
        private readonly Dictionary<
            (Type Request, Type Response),
            Func<object, CancellationToken, Task<object>>> _handlers = new();

        public void Setup<TRequest, TResponse>(
            Func<TRequest, CancellationToken, Task<TResponse>> handler)
        {
            _handlers[
                (typeof(TRequest), typeof(TResponse))] =
                async (request, cancellationToken) =>
                    await handler(
                        (TRequest)request,
                        cancellationToken);
        }

        public async Task<TResponse> Send<TRequest, TResponse>(
            TRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!_handlers.TryGetValue(
                    (typeof(TRequest), typeof(TResponse)),
                    out var handler))
            {
                throw new InvalidOperationException(
                    $"No test handler configured for " +
                    $"{typeof(TRequest).Name} -> " +
                    $"{typeof(TResponse).Name}");
            }

            var response = await handler(
                request!,
                cancellationToken);

            return (TResponse)response;
        }
    }

    private static T AssertGeneralResponseData<T>(
        object? value)
    {
        Assert.NotNull(value);

        var success =
            value.GetType()
                .GetProperty("Success")
                ?.GetValue(value);

        var data =
            value.GetType()
                .GetProperty("Data")
                ?.GetValue(value);

        Assert.Equal(true, success);

        return Assert.IsType<T>(data);
    }

    private static CeloItemDto CreateCeloItemDto(
        string codigoVacuno = "VAC-001",
        DateOnly? fecha = null)
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

    private static CeloReporteItemDto CreateCeloReporteItemDto(
        string codigoVacuno = "VAC-001",
        DateOnly? fecha = null,
        TimeOnly? hora = null)
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
}
