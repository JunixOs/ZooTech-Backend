using ZooTech.Domain.Module_ProduccionLeche.Interfaces;
using ZooTech.Domain.Module_Sanidad.Interfaces;

namespace ZooTech.Domain.Shared.Interfaces;

public interface IGanaderiaUnitOfWork
{
    IOrdenioRepository Ordenios { get; }
    ITriajeRepository Triajes { get; }

    Task<T> ExecuteInTransactionAsync<T>(
        Func<CancellationToken, Task<T>> operation,
        CancellationToken cancellationToken = default,
        Func<T, CancellationToken, Task<T>>? afterSave = null);
}
