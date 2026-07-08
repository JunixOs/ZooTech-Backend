namespace ZooTech.Domain.Module_Vacuno.Interfaces;

public interface IVacunoMutationUnitOfWork
{
    Task<T> ExecuteInTransactionAsync<T>(
        Func<CancellationToken, Task<T>> operation,
        CancellationToken cancellationToken = default);

    Task<long> EnsureGranjaAsync(
        string nombre,
        string codigoDistrito,
        CancellationToken cancellationToken = default);
}
