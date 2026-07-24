using NSubstitute;
using FluentAssertions;
using Xunit;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Gateway.Caching;
using ZooTech.Application.Modules.Module_Fecundacion.Exceptions;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.CreateFecundacion;
using ZooTech.Domain.Ganaderia.Module_Fecundacion.Entities;
using ZooTech.Domain.Ganaderia.Module_Fecundacion.Interfaces;
using ZooTech.Domain.Shared.Interfaces;

namespace ZooTech.Application.UnitTests.Modules.Module_Fecundacion.UseCases.CreateFecundacion;

public sealed class CreateFecundacionInteractorTests
{
    private readonly IFecundacionRepository _repositoryMock;
    private readonly IAppCacheService _cacheMock;
    private readonly IGanaderiaUnitOfWork _unitOfWorkMock;
    private readonly CreateFecundacionInteractor _interactor;
    private bool _insideTransaction;

    public CreateFecundacionInteractorTests()
    {
        _repositoryMock = Substitute.For<IFecundacionRepository>();
        _cacheMock = Substitute.For<IAppCacheService>();
        _unitOfWorkMock = Substitute.For<IGanaderiaUnitOfWork>();

        _unitOfWorkMock.Fecundaciones.Returns(_repositoryMock);

        _unitOfWorkMock.ExecuteInTransactionAsync(
                Arg.Any<Func<CancellationToken, Task<Fecundacion>>>(),
                Arg.Any<CancellationToken>(),
                Arg.Any<Func<Fecundacion, CancellationToken, Task<Fecundacion>>>())
            .Returns(async callInfo =>
            {
                var operation = callInfo.Arg<Func<CancellationToken, Task<Fecundacion>>>();
                var afterSave = callInfo.Arg<Func<Fecundacion, CancellationToken, Task<Fecundacion>>>();

                _insideTransaction = true;
                try
                {
                    var operationResult = await operation(CancellationToken.None);
                    return afterSave == null
                        ? operationResult
                        : await afterSave(operationResult, CancellationToken.None);
                }
                finally
                {
                    _insideTransaction = false;
                }
            });

        _interactor = new CreateFecundacionInteractor(_unitOfWorkMock, _cacheMock);
    }

    [Fact]
    public async Task HandleAsync_DebeRegistrarExitosamente_CuandoDatosSonValidos()
    {
        // Arrange
        var command = new CreateFecundacionCommand(
            TipoFecundacionCode: "MN",
            VacunoReceptorId: 1,
            CeloRegistroId: null,
            FechaProcedimiento: DateTime.UtcNow,
            ResponsableName: "Juan Perez",
            ResultadoCode: "pendiente",
            ObservacionesVeterinarias: "Ninguna",
            MachoExterno: false,
            MachoExternoNombre: null,
            VacunoDonanteId: 2,
            CreatedById: 1
        );

        _repositoryMock.ExistsVacunoAsync(command.VacunoReceptorId, Arg.Any<CancellationToken>())
            .Returns(true);
        _repositoryMock.ExistsVacunoAsync(command.VacunoDonanteId!.Value, Arg.Any<CancellationToken>())
            .Returns(true);
        _repositoryMock.GetOrCreateResponsableByNameAsync(command.ResponsableName, Arg.Any<CancellationToken>())
            .Returns(_ =>
            {
                _insideTransaction.Should().BeTrue();
                return 10;
            });
        _repositoryMock.ExistsCodigoAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(false);

        var domainFecundacion = Fecundacion.CreateNew(
            codigo: "FEC-123456",
            tipoFecundacionCode: command.TipoFecundacionCode,
            vacunoReceptorId: command.VacunoReceptorId,
            celoRegistroId: command.CeloRegistroId,
            fechaProcedimiento: command.FechaProcedimiento,
            responsableId: 10,
            resultadoCode: command.ResultadoCode,
            observacionesVeterinarias: command.ObservacionesVeterinarias,
            actorUsuarioId: command.CreatedById,
            utcNow: DateTime.UtcNow,
            machoExterno: command.MachoExterno,
            machoExternoNombre: command.MachoExternoNombre,
            vacunoDonanteId: command.VacunoDonanteId
        );

        var domainFecundacionWithId = new Fecundacion(
            id: 500L,
            codigo: "FEC-123456",
            tipoFecundacionCode: command.TipoFecundacionCode,
            vacunoReceptorId: command.VacunoReceptorId,
            celoRegistroId: command.CeloRegistroId,
            fechaProcedimiento: command.FechaProcedimiento,
            responsableId: 10,
            resultadoCode: command.ResultadoCode,
            observacionesVeterinarias: command.ObservacionesVeterinarias,
            actorUsuarioId: command.CreatedById,
            machoExterno: command.MachoExterno,
            machoExternoNombre: command.MachoExternoNombre,
            vacunoDonanteId: command.VacunoDonanteId
        );

        _repositoryMock.AddAsync(Arg.Any<Fecundacion>(), Arg.Any<CancellationToken>())
            .Returns(_ =>
            {
                _insideTransaction.Should().BeTrue();
                return domainFecundacion;
            });

        // Mock para el nuevo método GetByCodigoAsync usado en afterSave
        _repositoryMock.GetByCodigoAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(domainFecundacionWithId);

        // Act
        var result = await _interactor.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(500L); // Validamos que toma el ID del GetByCodigoAsync
        result.Codigo.Should().Be("FEC-123456");

        await _repositoryMock.Received(1).AddAsync(Arg.Any<Fecundacion>(), Arg.Any<CancellationToken>());
        await _repositoryMock.Received(1).GetOrCreateResponsableByNameAsync(
            command.ResponsableName,
            Arg.Any<CancellationToken>());
        await _repositoryMock.Received(1).GetByCodigoAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _cacheMock.Received(1).RemoveByPrefixAsync("fecundacion:listar");
    }

    [Fact]
    public async Task HandleAsync_NoDebeInvalidarCache_CuandoFallaLaPersistencia()
    {
        var command = CreateValidCommand();
        ConfigureValidReferences(command);
        _repositoryMock.GetOrCreateResponsableByNameAsync(
                command.ResponsableName,
                Arg.Any<CancellationToken>())
            .Returns(10);
        _repositoryMock.AddAsync(
                Arg.Any<Fecundacion>(),
                Arg.Any<CancellationToken>())
            .Returns(_ => Task.FromException<Fecundacion>(
                new InvalidOperationException("Fallo de persistencia")));

        var action = () => _interactor.HandleAsync(command);

        await action.Should().ThrowAsync<InvalidOperationException>();
        await _repositoryMock.DidNotReceive().GetByCodigoAsync(
            Arg.Any<string>(),
            Arg.Any<CancellationToken>());
        await _cacheMock.DidNotReceive().RemoveByPrefixAsync(Arg.Any<string>());
    }

    [Fact]
    public async Task HandleAsync_DebePropagarConflictException_CuandoNoRecuperaLaFecundacion()
    {
        var command = CreateValidCommand();
        ConfigureValidReferences(command);
        _repositoryMock.GetOrCreateResponsableByNameAsync(
                command.ResponsableName,
                Arg.Any<CancellationToken>())
            .Returns(10);
        _repositoryMock.AddAsync(
                Arg.Any<Fecundacion>(),
                Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Fecundacion>());
        _repositoryMock.GetByCodigoAsync(
                Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns((Fecundacion?)null);

        var action = () => _interactor.HandleAsync(command);

        await action.Should().ThrowAsync<ConflictException>();
        await _cacheMock.DidNotReceive().RemoveByPrefixAsync(Arg.Any<string>());
    }

    [Fact]
    public async Task HandleAsync_DebeLanzarConflictException_CuandoReceptorNoExiste()
    {
        // Arrange
        var command = new CreateFecundacionCommand(
            TipoFecundacionCode: "MN",
            VacunoReceptorId: 999,
            CeloRegistroId: null,
            FechaProcedimiento: DateTime.UtcNow,
            ResponsableName: "Juan Perez",
            ResultadoCode: "pendiente",
            ObservacionesVeterinarias: "Ninguna",
            MachoExterno: false,
            MachoExternoNombre: null,
            VacunoDonanteId: 2,
            CreatedById: 1
        );

        _repositoryMock.ExistsVacunoAsync(command.VacunoReceptorId, Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var action = async () => await _interactor.HandleAsync(command);

        // Assert
        await action.Should().ThrowAsync<FecundacionVacunoNotFoundException>();
    }

    private void ConfigureValidReferences(CreateFecundacionCommand command)
    {
        _repositoryMock.ExistsVacunoAsync(
                command.VacunoReceptorId,
                Arg.Any<CancellationToken>())
            .Returns(true);
        _repositoryMock.ExistsVacunoAsync(
                command.VacunoDonanteId!.Value,
                Arg.Any<CancellationToken>())
            .Returns(true);
        _repositoryMock.ExistsCodigoAsync(
                Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns(false);
    }

    private static CreateFecundacionCommand CreateValidCommand()
        => new(
            TipoFecundacionCode: "MN",
            VacunoReceptorId: 1,
            CeloRegistroId: null,
            FechaProcedimiento: DateTime.UtcNow,
            ResponsableName: "Juan Perez",
            ResultadoCode: "pendiente",
            ObservacionesVeterinarias: "Ninguna",
            MachoExterno: false,
            MachoExternoNombre: null,
            VacunoDonanteId: 2,
            CreatedById: 1);
}
