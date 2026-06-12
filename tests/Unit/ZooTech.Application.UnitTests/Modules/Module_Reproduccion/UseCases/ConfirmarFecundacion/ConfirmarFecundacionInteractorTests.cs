using Moq;
using FluentValidation;
using FluentValidation.Results;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Modules.Module_Reproduccion.UseCases.ConfirmarFecundacion;
using ZooTech.Domain.Module_Reproduccion.Entities;
using ZooTech.Domain.Module_Reproduccion.Interfaces;
using Xunit;

namespace ZooTech.Application.UnitTests.Modules.Module_Reproduccion.UseCases.ConfirmarFecundacion;

public class ConfirmarFecundacionInteractorTests
{
    private readonly Mock<IFecundacionRepository> _fecundacionRepoMock;
    private readonly Mock<IValidator<ConfirmarFecundacionCommand>> _validatorMock;
    private readonly ConfirmarFecundacionInteractor _interactor;

    public ConfirmarFecundacionInteractorTests()
    {
        _fecundacionRepoMock = new Mock<IFecundacionRepository>();
        _validatorMock = new Mock<IValidator<ConfirmarFecundacionCommand>>();

        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<ConfirmarFecundacionCommand>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _interactor = new ConfirmarFecundacionInteractor(
            _fecundacionRepoMock.Object,
            _validatorMock.Object
        );
    }

    [Fact]
    public async Task HandleAsync_WhenValidCommand_ReturnsOutput()
    {
        // Arrange
        var command = new ConfirmarFecundacionCommand(1, Fecundacion.ResultadoExitosa, 1);
        var fecundacion = Fecundacion.CreateNew("FEC-001", Fecundacion.TipoMontaNatural, 1, 2, null, null, DateOnly.FromDateTime(DateTime.UtcNow), 1, null, null, null, 1);

        _fecundacionRepoMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(fecundacion);

        // Act
        var result = await _interactor.HandleAsync(command);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(Fecundacion.ResultadoExitosa, result.NuevoResultadoCode);
        _fecundacionRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Fecundacion>(), default), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WhenNotFound_ThrowsException()
    {
        // Arrange
        var command = new ConfirmarFecundacionCommand(1, Fecundacion.ResultadoExitosa, 1);
        _fecundacionRepoMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync((Fecundacion?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _interactor.HandleAsync(command));
    }
}
