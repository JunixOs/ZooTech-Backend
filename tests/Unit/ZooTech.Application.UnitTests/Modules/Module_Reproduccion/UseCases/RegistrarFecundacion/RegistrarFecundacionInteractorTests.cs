using Moq;
using FluentValidation;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Modules.Module_Reproduccion.UseCases.RegistrarFecundacion;
using ZooTech.Domain.Module_Reproduccion.Entities;
using ZooTech.Domain.Module_Reproduccion.Interfaces;
using Xunit;

namespace ZooTech.Application.UnitTests.Modules.Module_Reproduccion.UseCases.RegistrarFecundacion;

public class RegistrarFecundacionInteractorTests
{
    private readonly Mock<IFecundacionRepository> _repositoryMock;
    private readonly Mock<IVacunoReproduccionRepository> _vacunoRepositoryMock;
    private readonly Mock<IValidator<RegistrarFecundacionCommand>> _validatorMock;
    private readonly RegistrarFecundacionInteractor _interactor;

    public RegistrarFecundacionInteractorTests()
    {
        _repositoryMock = new Mock<IFecundacionRepository>();
        _vacunoRepositoryMock = new Mock<IVacunoReproduccionRepository>();
        _validatorMock = new Mock<IValidator<RegistrarFecundacionCommand>>();

        // Setup base validation (always passes by default for these tests)
        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<RegistrarFecundacionCommand>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _interactor = new RegistrarFecundacionInteractor(
            _repositoryMock.Object,
            _vacunoRepositoryMock.Object,
            _validatorMock.Object);
    }

    private RegistrarFecundacionCommand CreateValidCommand()
    {
        return new RegistrarFecundacionCommand(
            TipoFecundacionCode: "MONTA_NATURAL",
            VacunoReceptorId: 10,
            VacunoDonanteId: null,
            NombreMachoExterno: "Toro Externo",
            FechaProcedimiento: DateOnly.FromDateTime(DateTime.Now),
            ResponsableId: 1,
            CodigoSemen: null,
            CodigoEmbrion: null,
            Observaciones: null,
            CurrentUserId: 1
        );
    }

    [Fact]
    public async Task HandleAsync_ValidCommand_ReturnsOutputAndCallsAdd()
    {
        // Arrange
        var command = CreateValidCommand();
        _vacunoRepositoryMock.Setup(r => r.ExistsAsync(command.VacunoReceptorId, default)).ReturnsAsync(true);
        _vacunoRepositoryMock.Setup(r => r.IsHembraAsync(command.VacunoReceptorId, default)).ReturnsAsync(true);
        _vacunoRepositoryMock.Setup(r => r.IsVivoAsync(command.VacunoReceptorId, default)).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.HasPendingFecundacionAsync(command.VacunoReceptorId, default)).ReturnsAsync(false);
        _repositoryMock.Setup(r => r.GenerateNextCodeAsync(default)).ReturnsAsync("FEC-2026-0001");

        // Act
        var result = await _interactor.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("FEC-2026-0001", result.Codigo);
        Assert.Equal(command.VacunoReceptorId, result.VacunoReceptorId);
        
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Fecundacion>(), default), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_VacunoReceptorNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var command = CreateValidCommand();
        _vacunoRepositoryMock.Setup(r => r.ExistsAsync(command.VacunoReceptorId, default)).ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _interactor.HandleAsync(command, CancellationToken.None));
        Assert.Contains(command.VacunoReceptorId.ToString(), exception.Message);
        
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Fecundacion>(), default), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_VacunoReceptorNotFemale_ThrowsConflictException()
    {
        // Arrange
        var command = CreateValidCommand();
        _vacunoRepositoryMock.Setup(r => r.ExistsAsync(command.VacunoReceptorId, default)).ReturnsAsync(true);
        _vacunoRepositoryMock.Setup(r => r.IsHembraAsync(command.VacunoReceptorId, default)).ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ConflictException>(() => _interactor.HandleAsync(command, CancellationToken.None));
        Assert.Contains("hembra", exception.Message);
    }

    [Fact]
    public async Task HandleAsync_VacunoReceptorNotAlive_ThrowsConflictException()
    {
        // Arrange
        var command = CreateValidCommand();
        _vacunoRepositoryMock.Setup(r => r.ExistsAsync(command.VacunoReceptorId, default)).ReturnsAsync(true);
        _vacunoRepositoryMock.Setup(r => r.IsHembraAsync(command.VacunoReceptorId, default)).ReturnsAsync(true);
        _vacunoRepositoryMock.Setup(r => r.IsVivoAsync(command.VacunoReceptorId, default)).ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ConflictException>(() => _interactor.HandleAsync(command, CancellationToken.None));
        Assert.Contains("VIVO", exception.Message);
    }

    [Fact]
    public async Task HandleAsync_PendingFecundacionExists_ThrowsConflictException()
    {
        // Arrange
        var command = CreateValidCommand();
        _vacunoRepositoryMock.Setup(r => r.ExistsAsync(command.VacunoReceptorId, default)).ReturnsAsync(true);
        _vacunoRepositoryMock.Setup(r => r.IsHembraAsync(command.VacunoReceptorId, default)).ReturnsAsync(true);
        _vacunoRepositoryMock.Setup(r => r.IsVivoAsync(command.VacunoReceptorId, default)).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.HasPendingFecundacionAsync(command.VacunoReceptorId, default)).ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ConflictException>(() => _interactor.HandleAsync(command, CancellationToken.None));
        Assert.Contains("PENDIENTE", exception.Message);
    }

    [Fact]
    public async Task HandleAsync_VacunoDonanteNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var command = new RegistrarFecundacionCommand(
            TipoFecundacionCode: "INSEMINACION_ARTIFICIAL",
            VacunoReceptorId: 10,
            VacunoDonanteId: 99, // Internal donante
            NombreMachoExterno: null,
            FechaProcedimiento: DateOnly.FromDateTime(DateTime.Now),
            ResponsableId: 1,
            CodigoSemen: "SEM-001",
            CodigoEmbrion: null,
            Observaciones: null,
            CurrentUserId: 1
        );

        _vacunoRepositoryMock.Setup(r => r.ExistsAsync(command.VacunoReceptorId, default)).ReturnsAsync(true);
        _vacunoRepositoryMock.Setup(r => r.IsHembraAsync(command.VacunoReceptorId, default)).ReturnsAsync(true);
        _vacunoRepositoryMock.Setup(r => r.IsVivoAsync(command.VacunoReceptorId, default)).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.HasPendingFecundacionAsync(command.VacunoReceptorId, default)).ReturnsAsync(false);
        
        // Donante does not exist
        _vacunoRepositoryMock.Setup(r => r.ExistsAsync(command.VacunoDonanteId!.Value, default)).ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _interactor.HandleAsync(command, CancellationToken.None));
        Assert.Contains(command.VacunoDonanteId.ToString(), exception.Message);
    }
}
