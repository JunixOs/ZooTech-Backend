using Moq;
using ZooTech.Application.Common.Gateway.Caching;
using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.DeleteFecundacion;
using ZooTech.Domain.Ganaderia.Module_Fecundacion.Interfaces;
using ZooTech.Domain.Shared.Interfaces;

namespace ZooTech.Application.UnitTests.Modules.Module_Fecundacion.Support;

internal sealed class DeleteFecundacionTestContext
{
    public Mock<IGanaderiaUnitOfWork> UnitOfWork { get; } = new();
    public Mock<IFecundacionRepository> Repository { get; } = new();

    public DeleteFecundacionTestContext()
    {
        UnitOfWork.Setup(x => x.Fecundaciones).Returns(Repository.Object);
        UnitOfWork
            .Setup(x => x.ExecuteInTransactionAsync(
                It.IsAny<Func<CancellationToken, Task<EmptyOutput>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<Func<EmptyOutput, CancellationToken, Task<EmptyOutput>>?>()))
            .Returns((Func<CancellationToken, Task<EmptyOutput>> operation, CancellationToken token,
                Func<EmptyOutput, CancellationToken, Task<EmptyOutput>>? _) => operation(token));
    }

    public DeleteFecundacionInteractor CreateInteractor() => new(UnitOfWork.Object);
}
