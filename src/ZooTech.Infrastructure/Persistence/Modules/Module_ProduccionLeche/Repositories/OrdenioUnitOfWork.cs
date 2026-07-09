using ZooTech.Domain.Module_ProduccionLeche.Interfaces;
using ZooTech.Infrastructure.Persistence.Context;

namespace ZooTech.Infrastructure.Persistence.Modules.Module_ProduccionLeche.Repositories;

public sealed class OrdenioUnitOfWork : IOrdenioUnitOfWork, IAsyncDisposable
{
    private readonly GanaderiaDbContext _context;

    public OrdenioUnitOfWork(IGanaderiaDbContextFactory ganaderiaDbContextFactory)
    {
        _context = ganaderiaDbContextFactory.CreateDbContextByTenantContext();
        Repository = new OrdenioRepository(_context);
    }

    public IOrdenioRepository Repository { get; }

    public async Task<T> ExecuteInTransactionAsync<T>(
        Func<CancellationToken, Task<T>> operation,
        CancellationToken cancellationToken = default,
        Func<T, CancellationToken, Task<T>>? afterSave = null)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var result = await operation(cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            if (afterSave is not null)
            {
                result = await afterSave(result, cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);
            return result;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public ValueTask DisposeAsync()
        => _context.DisposeAsync();
}
