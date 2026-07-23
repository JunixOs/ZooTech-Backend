using FluentAssertions;
using Moq;
using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_Fecundacion.Exceptions;
using ZooTech.Application.UnitTests.Modules.Module_Fecundacion.Support;
using ZooTech.Domain.Ganaderia.Module_Fecundacion.Interfaces;
using ZooTech.Tests.Shared.Factories;

namespace ZooTech.Application.UnitTests.Modules.Module_Fecundacion.UseCases.DeleteFecundacion;

public sealed class DeleteFecundacionInteractorTests
{
    private readonly DeleteFecundacionTestContext _context = new();

    [Fact]
    public async Task HandleAsync_WhenFecundacionCanBeDeleted_DeletesInsideTransaction()
    {
        var command = FecundacionTestDataFactory.DeleteCommand();
        _context.Repository.Setup(x => x.GetForEditAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(FecundacionTestDataFactory.EditData(command.Id));
        _context.Repository.Setup(x => x.HasCriaAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _context.CreateInteractor().HandleAsync(command, CancellationToken.None);

        result.Should().Be(EmptyOutput.Value);
        _context.UnitOfWork.Verify(x => x.ExecuteInTransactionAsync(
            It.IsAny<Func<CancellationToken, Task<EmptyOutput>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<Func<EmptyOutput, CancellationToken, Task<EmptyOutput>>?>()), Times.Once);
        _context.Repository.Verify(x => x.DeleteAsync(
            command.Id,
            command.Razon,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WhenFecundacionDoesNotExist_DoesNotDelete()
    {
        var command = FecundacionTestDataFactory.DeleteCommand(id: 99);
        _context.Repository.Setup(x => x.GetForEditAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((FecundacionEditData?)null);

        var act = () => _context.CreateInteractor().HandleAsync(command, CancellationToken.None);

        await act.Should().ThrowAsync<FecundacionNotFoundException>();
        _context.Repository.Verify(x => x.DeleteAsync(
            It.IsAny<long>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_WhenFecundacionHasCria_DoesNotDelete()
    {
        var command = FecundacionTestDataFactory.DeleteCommand();
        _context.Repository.Setup(x => x.GetForEditAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(FecundacionTestDataFactory.EditData(command.Id));
        _context.Repository.Setup(x => x.HasCriaAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var act = () => _context.CreateInteractor().HandleAsync(command, CancellationToken.None);

        await act.Should().ThrowAsync<FecundacionHasDependenciesException>();
        _context.Repository.Verify(x => x.DeleteAsync(
            It.IsAny<long>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_WhenTransactionFails_ThrowsException()
    {
        var command = FecundacionTestDataFactory.DeleteCommand();
        _context.UnitOfWork
            .Setup(x => x.ExecuteInTransactionAsync(
                It.IsAny<Func<CancellationToken, Task<EmptyOutput>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<Func<EmptyOutput, CancellationToken, Task<EmptyOutput>>?>()))
            .ThrowsAsync(new InvalidOperationException("Transaction failed"));

        var act = () => _context.CreateInteractor().HandleAsync(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
