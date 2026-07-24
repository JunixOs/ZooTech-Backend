using ZooTech.Domain.Module_ProduccionLeche.Interfaces;
using ZooTech.Domain.Module_Sanidad.Interfaces;
using ZooTech.Domain.Shared.Interfaces;

namespace ZooTech.Application.UnitTests.Modules.Module_Sanidad;

internal sealed class FakeGanaderiaUnitOfWork : IGanaderiaUnitOfWork
{
    public FakeGanaderiaUnitOfWork(ITriajeRepository triajes)
    {
        Triajes = triajes;
    }

    public IOrdenioRepository Ordenios => throw new NotSupportedException();
    public ITriajeRepository Triajes { get; }

    public Task<T> ExecuteInTransactionAsync<T>(
        Func<CancellationToken, Task<T>> operation,
        CancellationToken cancellationToken = default,
        Func<T, CancellationToken, Task<T>>? afterSave = null)
        => ExecuteAsync(operation, cancellationToken, afterSave);

    private static async Task<T> ExecuteAsync<T>(
        Func<CancellationToken, Task<T>> operation,
        CancellationToken cancellationToken,
        Func<T, CancellationToken, Task<T>>? afterSave)
    {
        var result = await operation(cancellationToken);
        return afterSave is null ? result : await afterSave(result, cancellationToken);
    }
}
