using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Domain.Ganaderia.Module_Fecundacion.Interfaces;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Interfaces;
using ZooTech.Domain.Module_Celo.Interfaces;
using ZooTech.Domain.Module_ProduccionLeche.Interfaces;
using ZooTech.Domain.Module_Sanidad.Interfaces;
using ZooTech.Domain.Shared.Interfaces;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Modules.Module_Celo.Repositories;
using ZooTech.Infrastructure.Persistence.Modules.Module_Fecundacion.Repositories;
using ZooTech.Infrastructure.Persistence.Modules.Module_ProduccionLeche.Repositories;
using ZooTech.Infrastructure.Persistence.Modules.Module_Sanidad.Repositories;
using ZooTech.Infrastructure.Persistence.Modules.Module_Vacuno.Repositories;

namespace ZooTech.Infrastructure.Persistence;

public sealed class GanaderiaUnitOfWork : IGanaderiaUnitOfWork, IAsyncDisposable
{
    private readonly GanaderiaDbContext _context;

    public GanaderiaUnitOfWork(
        IGanaderiaDbContextFactory ganaderiaDbContextFactory,
        IDateTimeProvider dateTimeProvider)
    {
        _context = ganaderiaDbContextFactory.CreateDbContextByTenantContext();
        Vacunos = new VacunoRepository(_context);
        Fecundaciones = new FecundacionRepository(_context);
        Ordenios = new OrdenioRepository(_context);
        Triajes = new TriajeRepository(_context, dateTimeProvider);
        Celos = new CeloRepository(_context);
    }

    public IVacunoRepository Vacunos { get; }
    public IFecundacionRepository Fecundaciones { get; }
    public IOrdenioRepository Ordenios { get; }
    public ITriajeRepository Triajes { get; }
    public ICeloRepository Celos { get; }

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
