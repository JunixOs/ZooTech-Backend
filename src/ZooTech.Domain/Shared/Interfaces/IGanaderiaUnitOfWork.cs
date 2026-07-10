using ZooTech.Domain.Module_ProduccionLeche.Interfaces;

namespace ZooTech.Domain.Shared.Interfaces;

public interface IGanaderiaUnitOfWork
{
    IOrdenioRepository Ordenios { get; }

    Task<T> ExecuteInTransactionAsync<T>(
        Func<CancellationToken, Task<T>> operation,
        CancellationToken cancellationToken = default,
        Func<T, CancellationToken, Task<T>>? afterSave = null);
}
