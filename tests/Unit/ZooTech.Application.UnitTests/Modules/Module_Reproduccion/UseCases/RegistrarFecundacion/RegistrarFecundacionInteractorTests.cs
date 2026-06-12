using Moq;
using FluentValidation;
using FluentValidation.Results;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Application.Modules.Module_Reproduccion.UseCases.RegistrarFecundacion;
using ZooTech.Domain.Module_Reproduccion.Entities;
using ZooTech.Domain.Module_Reproduccion.Interfaces;
using Xunit;

namespace ZooTech.Application.UnitTests.Modules.Module_Reproduccion.UseCases.RegistrarFecundacion;

public class RegistrarFecundacionInteractorTests
{
    private readonly Mock<IFecundacionRepository> _fecundacionRepoMock;
    private readonly Mock<IVacunoReproduccionRepository> _vacunoRepoMock;
    private readonly Mock<IValidator<RegistrarFecundacionCommand>> _validatorMock;
    private readonly RegistrarFecundacionInteractor _interactor;

    public RegistrarFecundacionInteractorTests()
    {
        _fecundacionRepoMock = new Mock<IFecundacionRepository>();
        _vacunoRepoMock = new Mock<IVacunoReproduccionRepository>();
        _validatorMock = new Mock<IValidator<RegistrarFecundacionCommand>>();

        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<RegistrarFecundacionCommand>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _interactor = new RegistrarFecundacionInteractor(
            _fecundacionRepoMock.Object,
            _vacunoRepoMock.Object,
            _validatorMock.Object
        );
    }

    [Fact]
    public async Task HandleAsync_WhenValidCommand_ReturnsOutput()
    {
        // Arrange
        var command = new RegistrarFecundacionCommand(
            Fecundacion.TipoMontaNatural, 1, 2, null, DateOnly.FromDateTime(DateTime.UtcNow), 1, null, null, null, 1);

        _vacunoRepoMock.Setup(r => r.ExistsAsync(1, default)).ReturnsAsync(true);
        _vacunoRepoMock.Setup(r => r.IsHembraAsync(1, default)).ReturnsAsync(true);
        _vacunoRepoMock.Setup(r => r.IsVivoAsync(1, default)).ReturnsAsync(true);
        _fecundacionRepoMock.Setup(r => r.HasPendingFecundacionAsync(1, default)).ReturnsAsync(false);
        _vacunoRepoMock.Setup(r => r.ExistsAsync(2, default)).ReturnsAsync(true);
        _fecundacionRepoMock.Setup(r => r.GenerateNextCodeAsync(default)).ReturnsAsync("FEC-000001");

        // Act
        var result = await _interactor.HandleAsync(command);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("FEC-000001", result.Codigo);
        Assert.Equal(Fecundacion.ResultadoPendiente, result.ResultadoCode);
        _fecundacionRepoMock.Verify(r => r.AddAsync(It.IsAny<Fecundacion>(), default), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WhenVacunoNotHembra_ThrowsException()
    {
        // Arrange
        var command = new RegistrarFecundacionCommand(
            Fecundacion.TipoMontaNatural, 1, 2, null, DateOnly.FromDateTime(DateTime.UtcNow), 1, null, null, null, 1);

        _vacunoRepoMock.Setup(r => r.ExistsAsync(1, default)).ReturnsAsync(true);
        _vacunoRepoMock.Setup(r => r.IsHembraAsync(1, default)).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<ConflictException>(() => _interactor.HandleAsync(command));
    }

    [Fact]
    public async Task HandleAsync_WhenVacunoNotVivo_ThrowsException()
    {
        // Arrange
        var command = new RegistrarFecundacionCommand(
            Fecundacion.TipoMontaNatural, 1, 2, null, DateOnly.FromDateTime(DateTime.UtcNow), 1, null, null, null, 1);

        _vacunoRepoMock.Setup(r => r.ExistsAsync(1, default)).ReturnsAsync(true);
        _vacunoRepoMock.Setup(r => r.IsHembraAsync(1, default)).ReturnsAsync(true);
        _vacunoRepoMock.Setup(r => r.IsVivoAsync(1, default)).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<ConflictException>(() => _interactor.HandleAsync(command));
    }

    [Fact]
    public async Task HandleAsync_WhenHasPendingFecundacion_ThrowsConflictException()
    {
        // Arrange
        var command = new RegistrarFecundacionCommand(
            Fecundacion.TipoMontaNatural, 1, 2, null, DateOnly.FromDateTime(DateTime.UtcNow), 1, null, null, null, 1);

        _vacunoRepoMock.Setup(r => r.ExistsAsync(1, default)).ReturnsAsync(true);
        _vacunoRepoMock.Setup(r => r.IsHembraAsync(1, default)).ReturnsAsync(true);
        _vacunoRepoMock.Setup(r => r.IsVivoAsync(1, default)).ReturnsAsync(true);
        _fecundacionRepoMock.Setup(r => r.HasPendingFecundacionAsync(1, default)).ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<ConflictException>(() => _interactor.HandleAsync(command));
    }
}
