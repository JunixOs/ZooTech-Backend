namespace ZooTech.Domain.Module_ProduccionLeche.Interfaces;

public interface IOrdenioUnitOfWork
{
    IOrdenioRepository Repository { get; }

    Task<T> ExecuteInTransactionAsync<T>(
        Func<CancellationToken, Task<T>> operation,
        CancellationToken cancellationToken = default,
        Func<T, CancellationToken, Task<T>>? afterSave = null);
}
