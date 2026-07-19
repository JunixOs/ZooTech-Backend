using FluentAssertions;
using Moq;
using ZooTech.Application.Common.Gateway.Caching;
using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_Fecundacion.Common;
using ZooTech.Application.Modules.Module_Fecundacion.Exceptions;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.DeleteFecundacion;
using ZooTech.Domain.Ganaderia.Module_Fecundacion.Interfaces;
using ZooTech.Domain.Shared.Interfaces;

namespace ZooTech.Application.UnitTests.Modules.Module_Fecundacion.UseCases.DeleteFecundacion;

public sealed class DeleteFecundacionInteractorTests
{
    private readonly Mock<IGanaderiaUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IFecundacionRepository> _repositoryMock;
    private readonly Mock<IAppCacheService> _cacheMock;
    private readonly DeleteFecundacionInteractor _interactor;

    public DeleteFecundacionInteractorTests()
    {
        _unitOfWorkMock = new Mock<IGanaderiaUnitOfWork>();
        _repositoryMock = new Mock<IFecundacionRepository>();
        _cacheMock = new Mock<IAppCacheService>();

        _unitOfWorkMock
            .Setup(u => u.Fecundaciones)
            .Returns(_repositoryMock.Object);

        _unitOfWorkMock
            .Setup(u => u.ExecuteInTransactionAsync(
                It.IsAny<Func<CancellationToken, Task<EmptyOutput>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<Func<EmptyOutput, CancellationToken, Task<EmptyOutput>>?>()))
            .Returns<Func<CancellationToken, Task<EmptyOutput>>, CancellationToken, Func<EmptyOutput, CancellationToken, Task<EmptyOutput>>?>(
                async (operation, cancellationToken, _) => await operation(cancellationToken));

        _interactor = new DeleteFecundacionInteractor(_unitOfWorkMock.Object, _cacheMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WhenFecundacionExists_ShouldDeleteInsideUnitOfWorkAndClearCache()
    {
        // Arrange
        var command = new DeleteFecundacionCommand(10, "Registro duplicado");

        _repositoryMock
            .Setup(r => r.GetForEditAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateEditData(command.Id));

        _repositoryMock
            .Setup(r => r.HasCriaAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _interactor.HandleAsync(command, CancellationToken.None);

        // Assert
        result.Should().Be(EmptyOutput.Value);

        _unitOfWorkMock.Verify(u => u.ExecuteInTransactionAsync(
            It.IsAny<Func<CancellationToken, Task<EmptyOutput>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<Func<EmptyOutput, CancellationToken, Task<EmptyOutput>>?>()), Times.Once);

        _repositoryMock.Verify(r => r.DeleteAsync(command.Id, command.Razon, It.IsAny<CancellationToken>()), Times.Once);
        _cacheMock.Verify(c => c.RemoveByPrefixAsync(FecundacionCacheKeys.ListarPrefix), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WhenFecundacionDoesNotExist_ShouldThrowNotFoundAndNotDelete()
    {
        // Arrange
        var command = new DeleteFecundacionCommand(99, "Registro incorrecto");

        _repositoryMock
            .Setup(r => r.GetForEditAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((FecundacionEditData?)null);

        // Act
        var act = async () => await _interactor.HandleAsync(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<FecundacionNotFoundException>();
        _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.ExecuteInTransactionAsync(
            It.IsAny<Func<CancellationToken, Task<EmptyOutput>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<Func<EmptyOutput, CancellationToken, Task<EmptyOutput>>?>()), Times.Once);
        _cacheMock.Verify(c => c.RemoveByPrefixAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_WhenFecundacionHasCria_ShouldThrowDependenciesAndNotDelete()
    {
        // Arrange
        var command = new DeleteFecundacionCommand(10, "Registro con dependencias");

        _repositoryMock
            .Setup(r => r.GetForEditAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateEditData(command.Id));

        _repositoryMock
            .Setup(r => r.HasCriaAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var act = async () => await _interactor.HandleAsync(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<FecundacionHasDependenciesException>();
        _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.ExecuteInTransactionAsync(
            It.IsAny<Func<CancellationToken, Task<EmptyOutput>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<Func<EmptyOutput, CancellationToken, Task<EmptyOutput>>?>()), Times.Once);
        _cacheMock.Verify(c => c.RemoveByPrefixAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_WhenTransactionFails_ShouldNotClearCache()
    {
        var command = new DeleteFecundacionCommand(10, "Error transaccional");
        _unitOfWorkMock
            .Setup(u => u.ExecuteInTransactionAsync(
                It.IsAny<Func<CancellationToken, Task<EmptyOutput>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<Func<EmptyOutput, CancellationToken, Task<EmptyOutput>>?>()))
            .ThrowsAsync(new InvalidOperationException("Transaction failed"));

        var act = async () => await _interactor.HandleAsync(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
        _cacheMock.Verify(c => c.RemoveByPrefixAsync(It.IsAny<string>()), Times.Never);
    }

    private static FecundacionEditData CreateEditData(long id)
    {
        return new FecundacionEditData(
            id,
            "FEC001",
            "INSEMINACION_ARTIFICIAL",
            1,
            "VAC001",
            "Estrella",
            "INTERNO",
            2,
            "VAC002",
            "Toro Rey",
            null,
            null,
            new DateOnly(2026, 1, 11),
            "Dr. Prueba",
            "EXITOSO",
            "CONFIRMADA",
            "Procedimiento sin complicaciones",
            "SEM-001",
            null,
            DateTime.UtcNow.AddDays(-1),
            DateTime.UtcNow);
    }
}
