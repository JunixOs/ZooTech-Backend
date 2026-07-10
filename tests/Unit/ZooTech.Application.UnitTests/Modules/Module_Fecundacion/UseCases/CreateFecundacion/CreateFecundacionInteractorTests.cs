using Moq;
using Xunit;
using ZooTech.Application.Modules.Module_Fecundacion.Exceptions;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.CreateFecundacion;
using ZooTech.Domain.Module_Fecundacion.Entities;
using ZooTech.Domain.Module_Fecundacion.Interfaces;

namespace ZooTech.Application.UnitTests.Modules.Module_Fecundacion.UseCases.CreateFecundacion;

public sealed class CreateFecundacionInteractorTests
{
    private readonly Mock<IFecundacionRepository> _repositoryMock;
    private readonly CreateFecundacionInteractor _interactor;

    public CreateFecundacionInteractorTests()
    {
        _repositoryMock = new Mock<IFecundacionRepository>();
        _interactor = new CreateFecundacionInteractor(_repositoryMock.Object);
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

        _repositoryMock.Setup(r => r.ExistsVacunoAsync(command.VacunoReceptorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _repositoryMock.Setup(r => r.ExistsVacunoAsync(command.VacunoDonanteId!.Value, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _repositoryMock.Setup(r => r.GetOrCreateResponsableByNameAsync(command.ResponsableName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(10);
        _repositoryMock.Setup(r => r.ExistsCodigoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

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

        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Fecundacion>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(domainFecundacion);

        // Act
        var result = await _interactor.HandleAsync(command);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("FEC-123456", result.Codigo);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Fecundacion>(), It.IsAny<CancellationToken>()), Times.Once);
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

        _repositoryMock.Setup(r => r.ExistsVacunoAsync(command.VacunoReceptorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<FecundacionVacunoNotFoundException>(() => _interactor.HandleAsync(command));
    }
}
